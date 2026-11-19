using System.Data;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Features.Sales;

namespace Semou.Api.Features.Discounts;

/*
 * سند الخصم (EncodingDiscountDoc, form "discount"): reduces what the student owes.
 *   StudentDiscount + journal: Maden amount → the discount type's account, Daen amount → student, branch cost center.
 *   Discount types (DisCountType):
 *     DiscountType = 1  → linked to a registration form (RegDoc)
 *     DiscountVatType   → percentage: amount = form total × DiscountVat %  (computed on the server)
 * The desktop called AddDay1 with removed parameters (@Id/@Code), so it could never post; this posts correctly.
 */
public sealed class DiscountInput
{
    public Guid StudentGuid { get; set; }
    public Guid BranchGuid { get; set; }
    public Guid TypeGuid { get; set; }
    public Guid? RegDocGuid { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public bool IsUse { get; set; } = true;
    public string? Reason { get; set; }
}

public sealed class DiscountTypeRow
{
    public Guid Guid { get; set; }
    public string Name { get; set; } = "";
    public Guid? AccountGuid { get; set; }
    public decimal Percent { get; set; }
    public bool IsPercent { get; set; }
    public bool ForRegDoc { get; set; }
}

public sealed class DiscountService(Db db, UserActionLog audit)
{
    public const string Form = "discount";

    private const string TypesSql = """
        SELECT Guid, DisName AS Name, AccountGuid, CAST(ISNULL(DiscountVat,0) AS decimal(18,2)) AS [Percent],
               CAST(ISNULL(DiscountVatType,0) AS bit) AS IsPercent, CAST(CASE WHEN ISNULL(DiscountType,0) = 1 THEN 1 ELSE 0 END AS bit) AS ForRegDoc
        FROM dbo.DisCountType
        """;

    public async Task<object> PrepareAsync(Guid studentGuid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var account = await c.ExecuteScalarAsync<Guid?>("SELECT AccountGuid FROM dbo.Student WHERE Guid = @g", new { g = studentGuid }) ?? throw ApiException.NotFound("الطالب غير موجود");
        return new
        {
            types = await c.QueryAsync<DiscountTypeRow>(TypesSql + " WHERE IsUse = 1 ORDER BY TRY_CONVERT(int, Code)"),
            branches = await c.QueryAsync("SELECT Guid AS guid, BrName AS name FROM dbo.Branch WHERE ISNULL(IsUse,1) = 1 ORDER BY TRY_CONVERT(int, Code)"),
            regDocs = await c.QueryAsync("""
                SELECT Guid AS guid, CAST(Code AS nvarchar(50)) AS code, RegDate AS date, CAST(ISNULL(DocSubTotal,0) AS decimal(18,2)) AS total
                FROM dbo.RegDoc WHERE AccountGuid = @account AND ISNULL(IstUse,1) = 1 AND ISNULL(IsReReg,0) = 0 ORDER BY RegDate DESC
                """, new { account }),
            defaultBranch = await c.ExecuteScalarAsync<Guid?>("SELECT BarnchGuid FROM dbo.Student WHERE Guid = @g", new { g = studentGuid }),
        };
    }

    public async Task<object> GetAsync(Guid guid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryFirstOrDefaultAsync("""
            SELECT X.Guid, CAST(X.Code AS nvarchar(50)) AS Code, X.ActionDate AS Date, X.BranchGuid, B.BrName AS BranchName, X.DiscountTypeGuid AS TypeGuid, T.DisName AS TypeName,
                   X.RegDocGuid, CAST(R.Code AS nvarchar(50)) AS RegCode, CAST(ISNULL(R.DocSubTotal,0) AS decimal(18,2)) AS RegTotal,
                   CAST(ISNULL(X.Amount,0) AS decimal(18,2)) AS Amount, X.ActionNotes AS Notes, CAST(ISNULL(X.IsUse,1) AS bit) AS IsUse,
                   S.Guid AS StudentGuid, S.StudentName, S.NationalId, S.StudentTel,
                   (SELECT TOP 1 CAST(D.Code AS nvarchar(50)) FROM dbo.Day1 D WHERE D.DocGuid = X.Guid ORDER BY D.ID DESC) AS JournalCode
            FROM dbo.StudentDiscount X JOIN dbo.Student S ON S.AccountGuid = X.AccountGuid
            LEFT JOIN dbo.Branch B ON B.Guid = X.BranchGuid LEFT JOIN dbo.DisCountType T ON T.Guid = X.DiscountTypeGuid
            LEFT JOIN dbo.RegDoc R ON R.Guid = X.RegDocGuid
            WHERE X.Guid = @guid
            """, new { guid }) ?? throw ApiException.NotFound("سند الخصم غير موجود");
    }

    public Task<object> SaveAsync(Guid? guid, DiscountInput i, CancellationToken ct) => db.InTransactionAsync<object>(async (c, tx) =>
    {
        var st = await c.QueryFirstOrDefaultAsync<(Guid AccountGuid, string StudentName, bool? IsUse, bool? Locked)>(
            "SELECT AccountGuid, StudentName, IsUse, LockProfile FROM dbo.Student WHERE Guid = @g", new { g = i.StudentGuid }, tx);
        if (st.StudentName is null) throw ApiException.NotFound("الطالب غير موجود");
        if (st.Locked == true) throw new ApiException("ملف الطالب مغلق، يرجى التواصل مع الإدارة");
        if (guid is null && st.IsUse == false) throw new ApiException("الطالب موقوف عن التعامل");

        string? oldCode = null;
        if (guid is { } g)
        {
            var o = await c.QueryFirstOrDefaultAsync<(string Code, Guid AccountGuid)>("SELECT CAST(Code AS nvarchar(50)), AccountGuid FROM dbo.StudentDiscount WITH (UPDLOCK) WHERE Guid = @g", new { g }, tx);
            if (o.Code is null) throw ApiException.NotFound("سند الخصم غير موجود");
            if (o.AccountGuid != st.AccountGuid) throw new ApiException("لا يمكن نقل سند الخصم لطالب آخر");
            if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب التعديل");
            oldCode = o.Code;
        }

        /* VALIDATE_DATA */
        var branch = await c.QueryFirstOrDefaultAsync<(Guid Guid, Guid? CostCenterGuid)>("SELECT Guid, CostCenterGuid FROM dbo.Branch WHERE Guid = @b", new { b = i.BranchGuid }, tx);
        if (branch.Guid == Guid.Empty) throw new ApiException("يرجى اختيار الفرع أولاً");
        var t = await c.QueryFirstOrDefaultAsync<DiscountTypeRow>(TypesSql + " WHERE Guid = @g AND IsUse = 1", new { g = i.TypeGuid }, tx)
                ?? throw new ApiException("يرجى اختيار نوع الخصم");
        var discountAccount = t.AccountGuid ?? throw new ApiException("نوع الخصم غير مربوط بحساب، يرجى مراجعة شاشة أنواع الخصم");
        var notes = (i.Notes ?? "").Trim();
        if (notes.Length == 0) throw new ApiException("يرجى إدخال البيان الخاص بعملية الخصم");

        decimal amount = Math.Round(i.Amount, 2);
        Guid regDoc = Guid.Empty;
        if (t.ForRegDoc)
        {
            if (i.RegDocGuid is null) throw new ApiException("يرجى اختيار استمارة التسجيل");
            var r = await c.QueryFirstOrDefaultAsync<(Guid Guid, decimal Total)>(
                "SELECT Guid, CAST(ISNULL(DocSubTotal,0) AS decimal(18,2)) FROM dbo.RegDoc WHERE Guid = @g AND AccountGuid = @a AND ISNULL(IstUse,1) = 1",
                new { g = i.RegDocGuid, a = st.AccountGuid }, tx);
            if (r.Guid == Guid.Empty) throw new ApiException("استمارة التسجيل المختارة لا تخص الطالب");
            regDoc = r.Guid;
            if (t.IsPercent) amount = Math.Round(r.Total * t.Percent / 100, 2);
            if (amount > r.Total) throw new ApiException("قيمة الخصم أكبر من قيمة استمارة التسجيل");
        }
        if (amount <= 0) throw new ApiException("يرجى إدخال قيمة الخصم");

        var cost = branch.CostCenterGuid ?? Guid.Empty;
        List<SalesPoster.JLine> jl = [new(discountAccount, amount, 0, notes, cost), new(st.AccountGuid, 0, amount, notes, cost)];
        var date = i.Date == default ? Clock.Now : i.Date;
        Guid docGuid; string code; string? journal;
        if (guid is null)
        {
            var id = await c.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(Id),0) + 1 FROM dbo.StudentDiscount WITH (UPDLOCK, HOLDLOCK)", transaction: tx);
            code = id.ToString();
            await c.ExecuteAsync("dbo.AddStudentDiscount", new
            {
                Id = id, Code = code, ActionDate = date, BranchGuid = branch.Guid, AccountGuid = st.AccountGuid, DiscountTypeGuid = t.Guid,
                RegDocGuid = regDoc, Amount = (double)amount, ActionNotes = notes, IsUse = true,
            }, tx, commandType: CommandType.StoredProcedure);
            docGuid = await c.ExecuteScalarAsync<Guid>("SELECT Guid FROM dbo.StudentDiscount WHERE Id = @id", new { id }, tx);
            (journal, _) = await SalesPoster.JournalAsync(c, tx, await SalesPoster.TypeAsync(c, tx, Form), docGuid, code, branch.Guid, date, notes, jl);
            await audit.WriteAsync(c, tx, Form, docGuid, UserActionLog.New, code);
        }
        else
        {
            docGuid = guid.Value; code = oldCode!;
            await c.ExecuteAsync("dbo.UpdateStudentDiscount", new
            {
                Guid = docGuid, ActionDate = date, BranchGuid = branch.Guid, AccountGuid = st.AccountGuid, DiscountTypeGuid = t.Guid,
                RegDocGuid = regDoc, Amount = (double)amount, ActionNotes = notes, i.IsUse,
            }, tx, commandType: CommandType.StoredProcedure);
            journal = await SalesPoster.ReplaceJournalAsync(c, tx, docGuid, branch.Guid, date, notes, i.IsUse, jl);
            await audit.WriteAsync(c, tx, Form, docGuid, i.IsUse ? UserActionLog.Update : "إلغاء", code, i.Reason!.Trim());
        }
        return new
        {
            message = guid is null ? $"تم حفظ سند الخصم رقم {code} بنجاح" : i.IsUse ? $"تم تعديل سند الخصم رقم {code} بنجاح" : $"تم إلغاء سند الخصم رقم {code}",
            guid = docGuid, code, journal, amount,
        };
    }, IsolationLevel.Serializable, ct);
}
