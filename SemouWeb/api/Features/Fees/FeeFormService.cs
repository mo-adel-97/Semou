using System.Data;
using System.Data.Common;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Security;
using Semou.Api.Features.Sales;

namespace Semou.Api.Features.Fees;

/*
 * استمارة الرسوم (RegFessForm): charges a student extra fees (exam, certificate…) from a fee book.
 *   RegOtherFess + RegOtherFessDetail + journal (type "regfess"):
 *     Maden  SubTotal → student (ChkMaden) or the book's MadenGuid
 *     Daen   Total (VAT book) / SubTotal → student (ChkDaen) or DaenGuid, with the book cost center
 *     Daen   Tax → VatAccountGuid (VAT book only)
 * No money is received here: the student pays later with "سداد رسوم".
 * Prices always come from the book's price list (never from the browser).
 */
public sealed class FeeLineIn { public Guid DiplomGuid { get; set; } }

public sealed class FeeFormInput
{
    public Guid StudentGuid { get; set; }
    public Guid BookGuid { get; set; }
    public Guid? SellerGuid { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public List<FeeLineIn> Lines { get; set; } = [];
    /// <summary>Edit only: false = cancel the form (and its journal).</summary>
    public bool IsUse { get; set; } = true;
    public string? Reason { get; set; }
}

public sealed record FeeItem(string Code, string Name, Guid DiplomGuid, decimal Cost, decimal Tax, decimal Total);

public sealed class FeeBook
{
    public Guid Guid { get; set; }
    public int Code { get; set; }
    public string DocName { get; set; } = "";
    public int? FessType { get; set; }
    public Guid BranchGuid { get; set; }
    public string? BranchName { get; set; }
    public Guid PriceListGuid { get; set; }
    public bool ChkVat { get; set; }
    public bool ChkMaden { get; set; }
    public Guid? MadenGuid { get; set; }
    public bool ChkDaen { get; set; }
    public Guid? DaenGuid { get; set; }
    public Guid? CostCenterGuid { get; set; }
    public Guid? VatAccountGuid { get; set; }
}

public sealed class FeeFormService(Db db, UserActionLog audit, CurrentUser me)
{
    public const string Form = "regfess";

    private const string BookSql = """
        SELECT R.Guid, R.Code, R.DocName, R.FessType, R.BranchGuid, B.BrName AS BranchName, R.PriceListGuid,
               CAST(ISNULL(R.ChkVat,0) AS bit) AS ChkVat, CAST(ISNULL(R.ChkMaden,0) AS bit) AS ChkMaden, R.MadenGuid,
               CAST(ISNULL(R.ChkDaen,0) AS bit) AS ChkDaen, R.DaenGuid, R.CostCenterGuid, R.VatAccountGuid
        FROM dbo.RegSetting R JOIN dbo.Branch B ON B.Guid = R.BranchGuid
        """;

    /// <summary>LoadRegSettingForNew: the fee books this user is allowed to use.</summary>
    public async Task<IEnumerable<FeeBook>> BooksAsync(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await BooksAsync(c, null);
    }

    private async Task<IEnumerable<FeeBook>> BooksAsync(DbConnection c, DbTransaction? tx)
    {
        var type = await c.ExecuteScalarAsync<Guid>("dbo.FormTypeGuid", new { FormName = Form }, tx, commandType: CommandType.StoredProcedure);
        return await c.QueryAsync<FeeBook>(BookSql + """
             JOIN dbo.RegSettingUser U ON U.RegSettingGuid = R.Guid
            WHERE U.UserGuid = @user AND R.IsUse = 1 AND R.DocTypeGuid = @type ORDER BY R.Code
            """, new { user = me.UserGuid, type }, tx);
    }

    private async Task<FeeBook> BookAsync(DbConnection c, DbTransaction? tx, Guid book, bool mustBeAllowed)
    {
        if (mustBeAllowed)
            return (await BooksAsync(c, tx)).FirstOrDefault(b => b.Guid == book) ?? throw new ApiException("دفتر الرسوم غير متاح لك، يرجى اختيار نوع المستند");
        return await c.QueryFirstOrDefaultAsync<FeeBook>(BookSql + " WHERE R.Guid = @book", new { book }, tx) ?? throw new ApiException("دفتر الرسوم غير موجود");
    }

    /// <summary>ShowPriceListDiplom (@Type = book FessType) priced like MainFeesGrid_CellDoubleClick.</summary>
    private static async Task<List<FeeItem>> ItemsAsync(DbConnection c, DbTransaction? tx, FeeBook b)
    {
        var rows = await c.QueryAsync(new CommandDefinition("dbo.ShowPriceListDiplom", new { PriceListGuid = b.PriceListGuid, Type = b.FessType }, tx, commandType: CommandType.StoredProcedure));
        return rows.Cast<IDictionary<string, object?>>().Select(r =>
        {
            decimal cost = D(r["DiplomCost"]), taxV = D(r["DiplomTaxValue"]), sub = D(r["DiplomSubTotal"]);
            if (b.ChkVat)
            {
                var net = Math.Round(cost / 1.15m, 2);
                return new FeeItem(S(r["Code"]), S(r["DiplomName"]), (Guid)r["diplomguid"]!, net, Math.Round(cost - net, 2), sub);
            }
            return new FeeItem(S(r["Code"]), S(r["DiplomName"]), (Guid)r["diplomguid"]!, cost, taxV, sub);
        }).ToList();
    }

    public async Task<object> PrepareAsync(Guid book, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var b = await BookAsync(c, null, book, true);
        return new
        {
            book = new { b.Guid, b.Code, b.DocName, b.BranchName, b.ChkVat },
            items = await ItemsAsync(c, null, b),
            sellers = await c.QueryAsync("dbo.ViewSalesManForReg", new { BranchGuid = b.BranchGuid }, commandType: CommandType.StoredProcedure),
        };
    }

    public async Task<object> GetAsync(Guid guid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var h = await c.QueryFirstOrDefaultAsync("""
            SELECT F.Guid, F.Code, F.RegFessDate AS Date, F.DocGuid AS BookGuid, R.DocName AS BookName, R.Code AS BookCode, CAST(ISNULL(R.ChkVat,0) AS bit) AS ChkVat,
                   F.SalesManGuid AS SellerGuid, SM.ManFullName AS SellerName, S.Guid AS StudentGuid, S.StudentName, S.NationalId, S.StudentTel,
                   CAST(ISNULL(F.ActionTotal,0) AS decimal(18,2)) AS Total, CAST(ISNULL(F.ActionTax,0) AS decimal(18,2)) AS Tax, CAST(ISNULL(F.ActionSubTotal,0) AS decimal(18,2)) AS SubTotal,
                   F.Notes, CAST(ISNULL(F.IsUse,1) AS bit) AS IsUse, B.BrName AS BranchName,
                   (SELECT TOP 1 CAST(D.Code AS nvarchar(50)) FROM dbo.Day1 D WHERE D.DocGuid = F.Guid ORDER BY D.ID DESC) AS JournalCode
            FROM dbo.RegOtherFess F JOIN dbo.RegSetting R ON R.Guid = F.DocGuid JOIN dbo.Student S ON S.AccountGuid = F.AccountGuid
            LEFT JOIN dbo.SalesMan SM ON SM.Guid = F.SalesManGuid LEFT JOIN dbo.Branch B ON B.Guid = F.BranchGuid
            WHERE F.Guid = @guid
            """, new { guid }) ?? throw ApiException.NotFound("استمارة الرسوم غير موجودة");
        var lines = await c.QueryAsync("dbo.ShowRegOtherFessDetail", new { RegOtherFessGuid = guid }, commandType: CommandType.StoredProcedure);
        return new
        {
            head = h,
            lines = lines.Cast<IDictionary<string, object?>>().Select(r => new
            {
                code = S(r["Code"]), name = S(r["DiplomName"]), diplomGuid = (Guid)r["DiplomGuid"]!, cost = D(r["DiplomCost"]), tax = D(r["DiplomTax"]), total = D(r["DiplomSubTotal"]),
            }),
        };
    }

    public Task<object> SaveAsync(Guid? guid, FeeFormInput i, CancellationToken ct) => db.InTransactionAsync<object>(async (c, tx) =>
    {
        var st = await c.QueryFirstOrDefaultAsync<(Guid AccountGuid, string StudentName, bool? IsUse, bool? LockProfile)>(
            "SELECT AccountGuid, StudentName, IsUse, LockProfile FROM dbo.Student WHERE Guid = @g", new { g = i.StudentGuid }, tx);
        if (st.StudentName is null) throw ApiException.NotFound("الطالب غير موجود");
        if (st.IsUse == false) throw new ApiException("الطالب موقوف عن التعامل");
        if (st.LockProfile == true) throw new ApiException("ملف الطالب مغلق، يرجى التواصل مع الإدارة");

        (Guid Guid, string Code, Guid AccountGuid, Guid Day1)? old = null;
        if (guid is { } g)
        {
            var o = await c.QueryFirstOrDefaultAsync<(Guid Guid, string Code, Guid AccountGuid)>(
                "SELECT Guid, Code, AccountGuid FROM dbo.RegOtherFess WITH (UPDLOCK) WHERE Guid = @g", new { g }, tx);
            if (o.Code is null) throw ApiException.NotFound("استمارة الرسوم غير موجودة");
            if (o.AccountGuid != st.AccountGuid) throw new ApiException("لا يمكن نقل استمارة الرسوم لطالب آخر");
            if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب التعديل");
            var d1 = await c.ExecuteScalarAsync<Guid?>("SELECT TOP 1 Guid FROM dbo.Day1 WHERE DocGuid = @g ORDER BY ID DESC", new { g }, tx)
                     ?? throw new ApiException("قيد استمارة الرسوم غير موجود");
            old = (o.Guid, o.Code, o.AccountGuid, d1);
        }

        /* VALIDATE_DATA */
        var b = await BookAsync(c, tx, i.BookGuid, true);
        if (i.SellerGuid is null) throw new ApiException("يرجى اختيار مندوب البيع");
        var sellerOk = (await c.QueryAsync("dbo.ViewSalesManForReg", new { BranchGuid = b.BranchGuid }, tx, commandType: CommandType.StoredProcedure))
            .Any(r => (Guid)((IDictionary<string, object?>)r)["Guid"]! == i.SellerGuid);
        if (!sellerOk) throw new ApiException("مندوب البيع غير مسموح له في هذا الفرع");
        var catalog = await ItemsAsync(c, tx, b);
        var lines = i.Lines.Select(l => l.DiplomGuid).Distinct()
            .Select(d => catalog.FirstOrDefault(x => x.DiplomGuid == d) ?? throw new ApiException("أحد الرسوم المختارة غير موجود في قائمة أسعار الدفتر")).ToList();
        if (lines.Count == 0) throw new ApiException("لا يمكن حفظ الاستمارة بدون اختيار الرسوم");
        decimal total = Math.Round(lines.Sum(x => x.Cost), 2), tax = Math.Round(lines.Sum(x => x.Tax), 2), sub = Math.Round(lines.Sum(x => x.Total), 2);
        if (sub == 0) throw new ApiException("لا يمكن حفظ الاستمارة بدون اختيار الرسوم");

        var maden = b.ChkMaden ? st.AccountGuid : b.MadenGuid ?? throw new ApiException("حساب المدين غير محدد في دفتر الرسوم");
        var daen = b.ChkDaen ? st.AccountGuid : b.DaenGuid ?? throw new ApiException("حساب الدائن غير محدد في دفتر الرسوم");
        var notes = $"{b.DocName}\r\n - {st.StudentName}\r\n";
        var jl = new List<SalesPoster.JLine>
        {
            new(maden, sub, 0, notes, Guid.Empty),
            new(daen, 0, b.ChkVat ? total : sub, notes, b.CostCenterGuid ?? Guid.Empty),
        };
        if (b.ChkVat && tax != 0) jl.Add(new(b.VatAccountGuid ?? throw new ApiException("حساب الضريبة غير محدد في دفتر الرسوم"), 0, tax, notes, Guid.Empty));
        var md = jl.Sum(x => x.Maden); var dn = jl.Sum(x => x.Daen);
        if (Math.Round(md, 2) != Math.Round(dn, 2)) throw new ApiException($"القيد غير متزن ({md:0.00} / {dn:0.00})، يرجى مراجعة قائمة أسعار الدفتر");

        var date = i.Date == default ? Clock.Now : i.Date;
        var userNotes = (i.Notes ?? "").Trim();
        Guid feeGuid; string code; string? journal;
        if (old is null)
        {
            // GET_REG__NO: next Id/Code under a lock so two users can't get the same number.
            var id = await c.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(Id),0) + 1 FROM dbo.RegOtherFess WITH (UPDLOCK, HOLDLOCK)", transaction: tx);
            code = id.ToString();
            await c.ExecuteAsync("dbo.AddRegOtherFess", new
            {
                Id = id, Code = code, RegFessDate = date, DocGuid = b.Guid, BranchGuid = b.BranchGuid, AccountGuid = st.AccountGuid, SalesManGuid = i.SellerGuid,
                ActionTotal = (double)total, ActionTax = (double)tax, ActionSubTotal = (double)sub, Notes = userNotes,
            }, tx, commandType: CommandType.StoredProcedure);
            feeGuid = await c.ExecuteScalarAsync<Guid>("SELECT Guid FROM dbo.RegOtherFess WHERE Code = @code", new { code }, tx);
            await AddLinesAsync(c, tx, feeGuid, lines);
            var type = await SalesPoster.TypeAsync(c, tx, Form);
            (journal, _) = await SalesPoster.JournalAsync(c, tx, type, feeGuid, code, b.BranchGuid, date, notes, jl);
            await audit.WriteAsync(c, tx, Form, feeGuid, UserActionLog.New, code);
        }
        else
        {
            feeGuid = old.Value.Guid; code = old.Value.Code;
            await c.ExecuteAsync("dbo.UpdateRegOtherFess", new
            {
                Guid = feeGuid, RegFessDate = date, DocGuid = b.Guid, BranchGuid = b.BranchGuid, SalesManGuid = i.SellerGuid,
                ActionTotal = (double)total, ActionTax = (double)tax, ActionSubTotal = (double)sub, Notes = userNotes, IsUse = i.IsUse,
            }, tx, commandType: CommandType.StoredProcedure);
            await c.ExecuteAsync("dbo.DeleteRegOtherFessDetail", new { RegOtherFessGuid = feeGuid }, tx, commandType: CommandType.StoredProcedure);
            await AddLinesAsync(c, tx, feeGuid, lines);
            await c.ExecuteAsync("dbo.UpdateDay1", new { Guid = old.Value.Day1, DayDate = date, Notes = Cut(notes, 200), IsUse = i.IsUse, BranchGuid = b.BranchGuid }, tx, commandType: CommandType.StoredProcedure);
            await c.ExecuteAsync("dbo.DeleteDay2", new { Day1Guid = old.Value.Day1 }, tx, commandType: CommandType.StoredProcedure);
            foreach (var l in jl)
                await c.ExecuteAsync("dbo.AddDay2", new { Day1Guid = old.Value.Day1, AccountGuid = l.Account, Maden = (double)l.Maden, Daen = (double)l.Daen, Notes = Cut(l.Notes, 200), CostGuid = l.Cost, IsUse = true },
                    tx, commandType: CommandType.StoredProcedure);
            journal = await c.ExecuteScalarAsync<string?>("SELECT CAST(Code AS nvarchar(50)) FROM dbo.Day1 WHERE Guid = @g", new { g = old.Value.Day1 }, tx);
            await audit.WriteAsync(c, tx, Form, feeGuid, i.IsUse ? UserActionLog.Update : "إلغاء", code, i.Reason!.Trim());
        }
        return new
        {
            message = old is null ? $"تم حفظ استمارة الرسوم رقم {code} بنجاح" : i.IsUse ? $"تم تعديل استمارة الرسوم رقم {code} بنجاح" : $"تم إلغاء استمارة الرسوم رقم {code}",
            guid = feeGuid, code, journal, total, tax, subTotal = sub,
        };
    }, IsolationLevel.Serializable, ct);

    private static async Task AddLinesAsync(DbConnection c, DbTransaction tx, Guid feeGuid, List<FeeItem> lines)
    {
        foreach (var l in lines)
            await c.ExecuteAsync("dbo.AddRegOtherFessDetail", new { RegOtherFessGuid = feeGuid, l.DiplomGuid, DiplomCost = (double)l.Cost, DiplomTax = (double)l.Tax, DiplomSubTotal = (double)l.Total },
                tx, commandType: CommandType.StoredProcedure);
    }

    private static string Cut(string s, int n) => s.Length > n ? s[..n] : s;
    private static decimal D(object? v) => v is null or DBNull ? 0 : Math.Round(Convert.ToDecimal(v), 2);
    private static string S(object? v) => v?.ToString() ?? "";
}
