using System.Data;
using System.Data.Common;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Security;
using Semou.Api.Features.Sales;

namespace Semou.Api.Features.Returns;

/*
 * المرتجعات:
 *  مرتجع استمارة (Re_Register, form "reregister"):
 *    ReRegDoc + copy of the form lines (RegDocDiplom Type_ 0) + journal
 *      Maden SubTotal → book MadenGuid (or student)   Daen SubTotal → book DaenGuid (or student), book cost center
 *    + ReReDoc: RegDoc.IsReReg = 1 (frees the seat). Cancelling the return sets it back.
 *  مرتجع فاتورة (RePay, form "repay"):
 *    ReBill + returned lines (RegDocDiplom Type_ 1, amount ≤ the original line) + journal
 *      Maden Total (pre-VAT) → MadenGuid/student, Maden Tax → VAT account, Daen SubTotal → DaenGuid/student
 *    + RePayBill: Bill.IsRePay = 1.
 * Notes are mandatory (the desktop rule). Amounts are recomputed on the server.
 */
public sealed class ReturnLineIn { public Guid DiplomGuid { get; set; } public decimal Total { get; set; } }

public sealed class ReturnInput
{
    /// <summary>RegDoc guid (kind reg) or Bill guid (kind bill).</summary>
    public Guid SourceGuid { get; set; }
    public Guid BookGuid { get; set; }
    public Guid? SellerGuid { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    /// <summary>Bill returns only: amount (incl. VAT) returned per line.</summary>
    public List<ReturnLineIn> Lines { get; set; } = [];
    public bool IsUse { get; set; } = true;
    public string? Reason { get; set; }
}

public sealed record ReturnLine(string Code, string Name, Guid DiplomGuid, int DiplomType, decimal Cost, decimal Tax, decimal Total, decimal TaxRate);

public sealed class ReturnsService(Db db, UserActionLog audit, CurrentUser me)
{
    public const string RegForm = "reregister";
    public const string BillForm = "repay";
    private const decimal Eps = 0.01m;

    private static string FormOf(string kind) => kind switch { "reg" => RegForm, "bill" => BillForm, _ => throw new ApiException("نوع المرتجع غير صحيح") };

    /* ---------------- Source documents ---------------- */
    private sealed record Source(Guid Guid, string Code, DateTime Date, Guid AccountGuid, Guid BranchGuid, Guid? SellerGuid, string StudentName, bool Returned, bool Active, decimal SubTotal);

    private static async Task<Source> SourceAsync(DbConnection c, DbTransaction? tx, string kind, Guid guid, bool lockRow = false)
    {
        var hint = lockRow ? "WITH (UPDLOCK)" : "";
        var s = kind == "reg"
            ? await c.QueryFirstOrDefaultAsync<Source>($"""
                SELECT R.Guid, CAST(R.Code AS nvarchar(50)) AS Code, R.RegDate AS Date, R.AccountGuid, R.BranchGuid, R.SellerGuid, S.StudentName,
                       CAST(ISNULL(R.IsReReg,0) AS bit) AS Returned, CAST(ISNULL(R.IstUse,1) AS bit) AS Active, CAST(ISNULL(R.DocSubTotal,0) AS decimal(18,2)) AS SubTotal
                FROM dbo.RegDoc R {hint} JOIN dbo.Student S ON S.AccountGuid = R.AccountGuid WHERE R.Guid = @guid
                """, new { guid }, tx)
            : await c.QueryFirstOrDefaultAsync<Source>($"""
                SELECT B.Guid, CAST(B.Code AS nvarchar(50)) AS Code, B.BillDate AS Date, B.AccountGuid, B.BranchGuid, CAST(NULL AS uniqueidentifier) AS SellerGuid, S.StudentName,
                       CAST(ISNULL(B.IsRePay,0) AS bit) AS Returned, CAST(ISNULL(B.IsUse,1) AS bit) AS Active, CAST(ISNULL(B.BillSubTotal,0) AS decimal(18,2)) AS SubTotal
                FROM dbo.Bill B {hint} JOIN dbo.Student S ON S.AccountGuid = B.AccountGuid WHERE B.Guid = @guid
                """, new { guid }, tx);
        return s ?? throw ApiException.NotFound(kind == "reg" ? "استمارة التسجيل غير موجودة" : "الفاتورة غير موجودة");
    }

    private static async Task<List<ReturnLine>> SourceLinesAsync(DbConnection c, DbTransaction? tx, string kind, Guid guid) =>
        (await c.QueryAsync<ReturnLine>("""
            SELECT D.Code, D.DiplomName AS Name, RD.DiplomGuid, ISNULL(RD.DiplomType, 0) AS DiplomType,
                   CAST(ISNULL(RD.Cost,0) AS decimal(18,2)) AS Cost, CAST(ISNULL(RD.Tax,0) AS decimal(18,2)) AS Tax, CAST(ISNULL(RD.SubTotal,0) AS decimal(18,2)) AS Total,
                   CAST(CASE WHEN ISNULL(RD.Cost,0) = 0 THEN 0 ELSE ROUND(ISNULL(RD.Tax,0) * 100.0 / RD.Cost, 2) END AS decimal(18,2)) AS TaxRate
            FROM dbo.RegDocDiplom RD JOIN dbo.Diplom D ON D.Guid = RD.DiplomGuid
            WHERE RD.RegDocGuid = @guid AND RD.Type_ = @t ORDER BY RD.ID
            """, new { guid, t = kind == "reg" ? 0 : 1 }, tx)).ToList();

    /// <summary>Documents of the student that can still be returned.</summary>
    public async Task<object> CandidatesAsync(Guid account, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var regs = await c.QueryAsync(new CommandDefinition("""
            SELECT R.Guid AS guid, CAST(R.Code AS nvarchar(50)) AS code, R.RegDate AS date, CAST(ISNULL(R.DocSubTotal,0) AS decimal(18,2)) AS total,
                   ISNULL((SELECT TOP 1 D.DiplomName FROM dbo.RegDocDiplom RD JOIN dbo.Diplom D ON D.Guid = RD.DiplomGuid
                                          WHERE RD.RegDocGuid = R.Guid AND RD.DiplomType = '0' AND RD.Type_ = '0' ORDER BY RD.ID), CO.CourseName) AS title,
                   CAST(ISNULL(R.IsReReg,0) AS bit) AS returned
            FROM dbo.RegDoc R LEFT JOIN dbo.Course CO ON CO.Guid = R.CourseGuid
            WHERE R.AccountGuid = @account AND ISNULL(R.IstUse,1) = 1 ORDER BY R.RegDate DESC
            """, new { account }, cancellationToken: ct));
        var bills = await c.QueryAsync(new CommandDefinition("""
            SELECT B.Guid AS guid, CAST(B.Code AS nvarchar(50)) AS code, B.BillDate AS date, CAST(ISNULL(B.BillSubTotal,0) AS decimal(18,2)) AS total,
                   CAST(B.TaxNo AS nvarchar(50)) AS title, CAST(ISNULL(B.IsRePay,0) AS bit) AS returned
            FROM dbo.Bill B WHERE B.AccountGuid = @account AND ISNULL(B.IsUse,1) = 1 ORDER BY B.BillDate DESC
            """, new { account }, cancellationToken: ct));
        return new { regs, bills };
    }

    public async Task<object> PrepareAsync(string kind, Guid source, bool forEdit, CancellationToken ct)
    {
        var form = FormOf(kind);
        await using var c = await db.OpenAsync(ct);
        var s = await SourceAsync(c, null, kind, source);
        if (s.Returned && !forEdit) throw new ApiException(kind == "reg" ? "تم عمل مرتجع لهذه الاستمارة من قبل، ويمكن عرضه من كشف الحساب" : "تم عمل مرتجع لهذه الفاتورة من قبل، ويمكن عرضه من كشف الحساب");
        if (!s.Active) throw new ApiException("المستند ملغى ولا يمكن عمل مرتجع له");
        var books = await DocBooks.AllowedAsync(c, null, me.UserGuid, form, s.BranchGuid);
        return new
        {
            source = new { s.Guid, s.Code, s.Date, s.StudentName, s.SubTotal, s.SellerGuid },
            lines = await SourceLinesAsync(c, null, kind, source),
            books = books.Select(b => new { b.Guid, b.Code, b.DocName, b.BranchName, b.ChkVat }),
            sellers = kind == "reg" ? await c.QueryAsync("dbo.ViewSalesManForReg", new { BranchGuid = s.BranchGuid }, commandType: CommandType.StoredProcedure) : [],
        };
    }

    /* ---------------- Saved returns ---------------- */
    public async Task<object> GetAsync(string kind, Guid guid, CancellationToken ct)
    {
        FormOf(kind);
        await using var c = await db.OpenAsync(ct);
        var h = await c.QueryFirstOrDefaultAsync(kind == "reg" ? """
            SELECT X.Guid, CAST(X.Code AS nvarchar(50)) AS Code, X.ReDate AS Date, X.DocGuid AS BookGuid, R.DocName AS BookName, X.SellerGuid, SM.ManFullName AS SellerName,
                   X.RegDocGuid AS SourceGuid, CAST(SRC.Code AS nvarchar(50)) AS SourceCode, S.StudentName, S.NationalId, S.StudentTel,
                   CAST(ISNULL(X.ReTotal,0) AS decimal(18,2)) AS Total, CAST(ISNULL(X.ReTax,0) AS decimal(18,2)) AS Tax, CAST(ISNULL(X.ReSubTotal,0) AS decimal(18,2)) AS SubTotal,
                   X.ReNotes AS Notes, CAST(ISNULL(X.IsUse,1) AS bit) AS IsUse, B.BrName AS BranchName,
                   (SELECT TOP 1 CAST(D.Code AS nvarchar(50)) FROM dbo.Day1 D WHERE D.DocGuid = X.Guid ORDER BY D.ID DESC) AS JournalCode
            FROM dbo.ReRegDoc X JOIN dbo.RegSetting R ON R.Guid = X.DocGuid JOIN dbo.Student S ON S.AccountGuid = X.AccountGuid
            LEFT JOIN dbo.RegDoc SRC ON SRC.Guid = X.RegDocGuid LEFT JOIN dbo.SalesMan SM ON SM.Guid = X.SellerGuid LEFT JOIN dbo.Branch B ON B.Guid = X.BranchGuid
            WHERE X.Guid = @guid
            """ : """
            SELECT X.Guid, CAST(X.Code AS nvarchar(50)) AS Code, X.ReDate AS Date, X.DocGuid AS BookGuid, R.DocName AS BookName, CAST(NULL AS uniqueidentifier) AS SellerGuid, U.FullName AS SellerName,
                   X.BillGuid AS SourceGuid, CAST(SRC.Code AS nvarchar(50)) AS SourceCode, S.StudentName, S.NationalId, S.StudentTel,
                   CAST(ISNULL(X.ReTotal,0) AS decimal(18,2)) AS Total, CAST(ISNULL(X.ReTax,0) AS decimal(18,2)) AS Tax, CAST(ISNULL(X.ReSubTotal,0) AS decimal(18,2)) AS SubTotal,
                   X.ReNotes AS Notes, CAST(ISNULL(X.IsUse,1) AS bit) AS IsUse, B.BrName AS BranchName,
                   (SELECT TOP 1 CAST(D.Code AS nvarchar(50)) FROM dbo.Day1 D WHERE D.DocGuid = X.Guid ORDER BY D.ID DESC) AS JournalCode
            FROM dbo.ReBill X JOIN dbo.RegSetting R ON R.Guid = X.DocGuid JOIN dbo.Student S ON S.AccountGuid = X.AccountGuid
            LEFT JOIN dbo.Bill SRC ON SRC.Guid = X.BillGuid LEFT JOIN dbo.User_Info U ON U.Guid = X.UserGuid LEFT JOIN dbo.Branch B ON B.Guid = X.BranchGuid
            WHERE X.Guid = @guid
            """, new { guid }) ?? throw ApiException.NotFound("المرتجع غير موجود");
        var src = (Guid)((IDictionary<string, object?>)h)["SourceGuid"]!;
        return new
        {
            head = h,
            lines = await SourceLinesAsync(c, null, kind, guid),          // the return's own lines
            original = await SourceLinesAsync(c, null, kind, src),         // limits for an edit
        };
    }

    /* ---------------- Save ---------------- */
    public Task<object> SaveAsync(string kind, Guid? guid, ReturnInput i, CancellationToken ct)
    {
        var form = FormOf(kind);
        return db.InTransactionAsync<object>(async (c, tx) =>
        {
            Guid? oldSource = null; string? oldCode = null;
            if (guid is { } g)
            {
                var o = await c.QueryFirstOrDefaultAsync<(Guid Source, string Code)>(kind == "reg"
                    ? "SELECT RegDocGuid, CAST(Code AS nvarchar(50)) FROM dbo.ReRegDoc WITH (UPDLOCK) WHERE Guid = @g"
                    : "SELECT BillGuid, CAST(Code AS nvarchar(50)) FROM dbo.ReBill WITH (UPDLOCK) WHERE Guid = @g", new { g }, tx);
                if (o.Code is null) throw ApiException.NotFound("المرتجع غير موجود");
                if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب التعديل");
                oldSource = o.Source; oldCode = o.Code;
            }
            var sourceGuid = oldSource ?? i.SourceGuid;
            var s = await SourceAsync(c, tx, kind, sourceGuid, lockRow: true);
            if (oldSource is null && s.Returned) throw new ApiException(kind == "reg" ? "تم عمل مرتجع لهذه الاستمارة من قبل" : "تم عمل مرتجع لهذه الفاتورة من قبل");
            if (!s.Active) throw new ApiException("المستند ملغى ولا يمكن عمل مرتجع له");
            var st = await c.QueryFirstOrDefaultAsync<(bool? IsUse, bool? Locked)>("SELECT IsUse, LockProfile FROM dbo.Student WHERE AccountGuid = @a", new { a = s.AccountGuid }, tx);
            if (st.Locked == true) throw new ApiException("ملف الطالب مغلق، يرجى التواصل مع الإدارة");

            /* VALIDATE_DATA */
            var b = await DocBooks.PickAsync(c, tx, me.UserGuid, form, s.BranchGuid, i.BookGuid);
            if (kind == "reg" && i.SellerGuid is null) throw new ApiException("يرجى اختيار مندوب البيع");
            var notes = (i.Notes ?? "").Trim();
            if (notes.Length == 0) throw new ApiException(kind == "reg" ? "لا يمكن حفظ مرتجع التسجيل بدون ملاحظات" : "لا يمكن حفظ مرتجع المبيعات بدون ملاحظات");

            var original = await SourceLinesAsync(c, tx, kind, sourceGuid);
            List<ReturnLine> lines;
            if (kind == "reg") lines = original; // the whole form is returned
            else
            {
                lines = [];
                foreach (var l in i.Lines.Where(l => l.Total > 0).GroupBy(l => l.DiplomGuid).Select(x => x.First()))
                {
                    var o = original.FirstOrDefault(x => x.DiplomGuid == l.DiplomGuid) ?? throw new ApiException("أحد البنود غير موجود في الفاتورة الأصلية");
                    if (l.Total > o.Total + Eps) throw new ApiException($"قيمة مرتجع البند ({o.Name}) أكبر من قيمته في الفاتورة ({o.Total:0.00})");
                    var total = Math.Round(l.Total, 2);
                    var net = o.TaxRate != 0 ? Math.Round(total / (1 + o.TaxRate / 100), 2) : total;
                    lines.Add(o with { Cost = net, Tax = Math.Round(total - net, 2), Total = total });
                }
            }
            if (lines.Count == 0) throw new ApiException("يرجى تحديد قيمة المرتجع");
            decimal tot = Math.Round(lines.Sum(x => x.Cost), 2), tax = Math.Round(lines.Sum(x => x.Tax), 2), sub = Math.Round(lines.Sum(x => x.Total), 2);
            if (sub <= 0) throw new ApiException("قيمة المرتجع يجب أن تكون أكبر من صفر");

            var cost = b.CostCenterGuid ?? Guid.Empty;
            List<SalesPoster.JLine> jl = kind == "reg"
                ? [new(b.MadenAccount(s.AccountGuid), sub, 0, notes, cost), new(b.DaenAccount(s.AccountGuid), 0, sub, notes, cost)]
                : (b.ChkVat && tax != 0 && b.VatAccountGuid is { } vat
                    ? [new(b.MadenAccount(s.AccountGuid), tot, 0, notes, Guid.Empty), new(vat, tax, 0, notes, Guid.Empty), new(b.DaenAccount(s.AccountGuid), 0, sub, notes, Guid.Empty)]
                    : [new(b.MadenAccount(s.AccountGuid), sub, 0, notes, Guid.Empty), new(b.DaenAccount(s.AccountGuid), 0, sub, notes, Guid.Empty)]);

            var date = i.Date == default ? Clock.Now : i.Date;
            var table = kind == "reg" ? "ReRegDoc" : "ReBill";
            Guid retGuid; string code; string? journal;
            if (guid is null)
            {
                var id = await c.ExecuteScalarAsync<int>($"SELECT ISNULL(MAX(ID),0) + 1 FROM dbo.{table} WITH (UPDLOCK, HOLDLOCK)", transaction: tx);
                code = id.ToString();
                if (kind == "reg")
                    await c.ExecuteAsync("dbo.AddReRegDoc", new
                    {
                        ID = id, Code = code, ReDate = date, DocGuid = b.Guid, BranchGuid = b.BranchGuid, AccountGuid = s.AccountGuid, SellerGuid = i.SellerGuid,
                        ReTotal = (double)tot, ReTax = (double)tax, ReSubTotal = (double)sub, ReNotes = notes, IsUse = true, RegDocGuid = s.Guid,
                    }, tx, commandType: CommandType.StoredProcedure);
                else
                    await c.ExecuteAsync("dbo.AddReBill", new
                    {
                        ID = id, Code = code, ReDate = date, DocGuid = b.Guid, AccountGuid = s.AccountGuid, BranchGuid = b.BranchGuid, UserGuid = me.UserGuid,
                        ReTotal = (double)tot, ReTax = (double)tax, ReSubTotal = (double)sub, ReNotes = notes, IsUse = true, BillGuid = s.Guid,
                    }, tx, commandType: CommandType.StoredProcedure);
                retGuid = await c.ExecuteScalarAsync<Guid>($"SELECT Guid FROM dbo.{table} WHERE ID = @id", new { id }, tx);
                await AddLinesAsync(c, tx, retGuid, lines, kind);
                (journal, _) = await SalesPoster.JournalAsync(c, tx, await SalesPoster.TypeAsync(c, tx, form), retGuid, code, b.BranchGuid, date, notes, jl);
                await MarkSourceAsync(c, tx, kind, s.Guid, true);
                await audit.WriteAsync(c, tx, form, retGuid, UserActionLog.New, code);
            }
            else
            {
                retGuid = guid.Value; code = oldCode!;
                if (kind == "reg")
                    await c.ExecuteAsync("dbo.UpdateReRegDoc", new
                    {
                        Guid = retGuid, ReDate = date, DocGuid = b.Guid, BranchGuid = b.BranchGuid, SellerGuid = i.SellerGuid,
                        ReTotal = (double)tot, ReTax = (double)tax, ReSubTotal = (double)sub, ReNotes = notes, i.IsUse,
                    }, tx, commandType: CommandType.StoredProcedure);
                else
                    await c.ExecuteAsync("dbo.UpdateRePay", new
                    {
                        Guid = retGuid, ReDate = date, DocGuid = b.Guid, BranchGuid = b.BranchGuid,
                        ReTotal = (double)tot, ReTax = (double)tax, ReSubTotal = (double)sub, ReNotes = notes, i.IsUse,
                    }, tx, commandType: CommandType.StoredProcedure);
                await c.ExecuteAsync("dbo.DeleteReRegDocDiplom", new { RegDocGuid = retGuid }, tx, commandType: CommandType.StoredProcedure);
                await AddLinesAsync(c, tx, retGuid, lines, kind);
                journal = await SalesPoster.ReplaceJournalAsync(c, tx, retGuid, b.BranchGuid, date, notes, i.IsUse, jl);
                await MarkSourceAsync(c, tx, kind, s.Guid, i.IsUse);
                await audit.WriteAsync(c, tx, form, retGuid, i.IsUse ? UserActionLog.Update : "إلغاء", code, i.Reason!.Trim());
            }
            var what = kind == "reg" ? "مرتجع التسجيل" : "مرتجع المبيعات";
            return new
            {
                message = guid is null ? $"تم حفظ {what} رقم {code} بنجاح" : i.IsUse ? $"تم تعديل {what} رقم {code} بنجاح" : $"تم إلغاء {what} رقم {code}",
                guid = retGuid, code, journal, total = tot, tax, subTotal = sub,
            };
        }, IsolationLevel.Serializable, ct);
    }

    private static async Task AddLinesAsync(DbConnection c, DbTransaction tx, Guid retGuid, List<ReturnLine> lines, string kind)
    {
        foreach (var l in lines)
            await c.ExecuteAsync("dbo.AddRegDocDiplom", new
            {
                RegDocGuid = retGuid, l.DiplomGuid, Cost = (double)l.Cost, Tax = (double)l.Tax, SubTotal = (double)l.Total, DiplomType = l.DiplomType, Type_ = kind == "reg" ? 0 : 1,
            }, tx, commandType: CommandType.StoredProcedure);
    }

    private static Task MarkSourceAsync(DbConnection c, DbTransaction tx, string kind, Guid source, bool returned) => kind == "reg"
        ? c.ExecuteAsync("dbo.ReReDoc", new { RegDocGuid = source, ReReg = returned }, tx, commandType: CommandType.StoredProcedure)
        : c.ExecuteAsync("dbo.RePayBill", new { BillGuid = source, RePay = returned }, tx, commandType: CommandType.StoredProcedure);
}
