using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text.Json;
using Dapper;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Sales;

/*
 * Shared "sales invoice + payment" posting used by the registration form and the payment invoice
 * (both desktop forms had their own copy of this code):
 *   Bill + items (RegDocDiplom Type_=1) + journal (+VAT line) → receipt voucher(s) + details + journal
 *   → BillByJson (ZATCA queue, type 388). Must be called inside the caller's transaction.
 */
public sealed record InvoiceLine(Guid DiplomGuid, int DiplomType, string Name, decimal Cost, decimal Tax, decimal Total);

public sealed class InvoicePosting
{
    /// <summary>The employee recorded on the invoice (the one who received the money). Defaults to the current user.</summary>
    public Guid? UserGuid { get; init; }
    public required Guid StudentAccount { get; init; }
    public required DateTime Date { get; init; }
    /// <summary>The invoice book (RegSetting of the sales-invoice type).</summary>
    public required Guid PayDocGuid { get; init; }
    public required IReadOnlyList<InvoiceLine> Lines { get; init; }
    public required Guid CashAccount { get; init; }
    public required Guid CostCenter { get; init; }
    public required string Ref { get; init; }
    public required DateTime PayDate { get; init; }
    public string? Notes { get; init; }
    public bool Split { get; init; }
    public decimal Amount1 { get; init; }
    public decimal Amount2 { get; init; }
    public Guid? CashAccount2 { get; init; }
    public Guid? CostCenter2 { get; init; }
    public string? Ref2 { get; init; }
    public DateTime? PayDate2 { get; init; }
    /// <summary>ChkVat of the book that drives the VAT treatment (reg book for registration, invoice book for payments).</summary>
    public required bool ChkVat { get; init; }
    public Guid? VatAccount { get; init; }
    /// <summary>Whether the VAT line is written (desktop conditions differ between the two forms).</summary>
    public required bool WriteVatLine { get; init; }
    public required string BillJournalNotes { get; init; }
    public required string ReceiptNotes { get; init; }
}

public sealed record InvoicePosted(string BillCode, Guid BillGuid, string TaxNo, string ReceiptCode, string? ReceiptCode2);

public sealed class SalesPoster(UserActionLog audit, CurrentUser me)
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static string Cut(string s, int max) => s.Length <= max ? s : s[..max];
    private static decimal D(object? v) => v is null or DBNull ? 0 : Convert.ToDecimal(v, Inv);
    private static string S(object? v) => v?.ToString() ?? "";

    public sealed record JLine(Guid Account, decimal Maden, decimal Daen, string Notes, Guid Cost);

    public static Task<Guid> TypeAsync(DbConnection c, DbTransaction tx, string form) =>
        c.ExecuteScalarAsync<Guid>("dbo.FormTypeGuid", new { FormName = form }, tx, commandType: CommandType.StoredProcedure);

    /// <summary>AddDay1 + AddDay2 lines; refuses unbalanced entries.</summary>
    public static async Task<(string Code, Guid Guid)> JournalAsync(DbConnection c, DbTransaction tx, Guid typeGuid, Guid docGuid, string actionCode,
        Guid branch, DateTime date, string notes, IReadOnlyList<JLine> lines)
    {
        var md = Math.Round(lines.Sum(l => l.Maden), 2); var dn = Math.Round(lines.Sum(l => l.Daen), 2);
        if (md != dn) throw new ApiException($"القيد غير متزن ({md:0.00} / {dn:0.00})، يرجى مراجعة إعدادات الدفتر");
        var openGuid = await TypeAsync(c, tx, "opendaily");
        var p = new DynamicParameters(new { OpenGuid = openGuid, DayDate = date, Notes = Cut(notes, 200), DayType = true, DocGuid = docGuid, IsUse = true, TypeGuid = typeGuid, ActionCode = actionCode, BranchGuid = branch });
        p.Add("CodeOut", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await c.ExecuteAsync("dbo.AddDay1", p, tx, commandType: CommandType.StoredProcedure);
        var day1 = await c.ExecuteScalarAsync<Guid>("SELECT TOP 1 Guid FROM dbo.Day1 WHERE DocGuid = @docGuid AND TypeGuid = @typeGuid ORDER BY ID DESC", new { docGuid, typeGuid }, tx);
        foreach (var l in lines)
            await c.ExecuteAsync("dbo.AddDay2", new { Day1Guid = day1, AccountGuid = l.Account, Maden = (double)l.Maden, Daen = (double)l.Daen, Notes = Cut(l.Notes, 200), CostGuid = l.Cost, IsUse = true },
                tx, commandType: CommandType.StoredProcedure);
        return (p.Get<int>("CodeOut").ToString(), day1);
    }

    /// <summary>Edit of a posted document: UpdateDay1 (date/notes/active) + DeleteDay2 + new balanced lines.</summary>
    public static async Task<string?> ReplaceJournalAsync(DbConnection c, DbTransaction tx, Guid docGuid, Guid branch, DateTime date, string notes, bool isUse, IReadOnlyList<JLine> lines)
    {
        var md = Math.Round(lines.Sum(l => l.Maden), 2); var dn = Math.Round(lines.Sum(l => l.Daen), 2);
        if (md != dn) throw new ApiException($"القيد غير متزن ({md:0.00} / {dn:0.00})، يرجى مراجعة إعدادات الدفتر");
        var day1 = await c.ExecuteScalarAsync<Guid?>("SELECT TOP 1 Guid FROM dbo.Day1 WHERE DocGuid = @docGuid ORDER BY ID DESC", new { docGuid }, tx)
                   ?? throw new ApiException("قيد اليومية الخاص بالمستند غير موجود");
        await c.ExecuteAsync("dbo.UpdateDay1", new { Guid = day1, DayDate = date, Notes = Cut(notes, 200), IsUse = isUse, BranchGuid = branch }, tx, commandType: CommandType.StoredProcedure);
        await c.ExecuteAsync("dbo.DeleteDay2", new { Day1Guid = day1 }, tx, commandType: CommandType.StoredProcedure);
        foreach (var l in lines)
            await c.ExecuteAsync("dbo.AddDay2", new { Day1Guid = day1, AccountGuid = l.Account, Maden = (double)l.Maden, Daen = (double)l.Daen, Notes = Cut(l.Notes, 200), CostGuid = l.Cost, IsUse = true },
                tx, commandType: CommandType.StoredProcedure);
        return await c.ExecuteScalarAsync<string?>("SELECT CAST(Code AS nvarchar(50)) FROM dbo.Day1 WHERE Guid = @day1", new { day1 }, tx);
    }

    public async Task<InvoicePosted> PostAsync(DbConnection c, DbTransaction tx, InvoicePosting x)
    {
        decimal total = Math.Round(x.Lines.Sum(l => l.Cost), 2), tax = Math.Round(x.Lines.Sum(l => l.Tax), 2), sub = Math.Round(x.Lines.Sum(l => l.Total), 2);
        var book = await c.QueryFirstOrDefaultAsync<(bool ChkMaden, Guid? MadenGuid, bool ChkDaen, Guid? DaenGuid, Guid BranchGuid)>(
            "SELECT CAST(ISNULL(ChkMaden,0) AS bit), MadenGuid, CAST(ISNULL(ChkDaen,0) AS bit), DaenGuid, BranchGuid FROM dbo.RegSetting WHERE Guid = @g", new { g = x.PayDocGuid }, tx);
        if (book.BranchGuid == Guid.Empty) throw new ApiException("دفتر الفاتورة غير موجود");
        var branch = book.BranchGuid;

        /* ADDBILL */
        var bp = new DynamicParameters(new
        {
            BillDate = x.Date, DocGuid = x.PayDocGuid, BranchGuid = branch, CostCenterGuid = x.CostCenter, UserGuid = x.UserGuid ?? me.UserGuid, AccountGuid = x.StudentAccount,
            BillTotal = (double)total, BillTax = (double)tax, BillSubTotal = (double)sub, Notes = Cut(x.Notes?.Trim() ?? "", 250), IsUse = true,
            Refrance = Cut(x.Ref.Trim(), 50), CashBoxGuid = x.CashAccount, ChkPayDate = true, PayDate = x.PayDate.Date,
            ChkOtherBank = x.Split, CashBoxguid2 = x.Split ? x.CashAccount2 : Guid.Empty, CostCentGuid2 = x.Split ? x.CostCenter2 : Guid.Empty,
            ChkPayDate2 = x.Split, PayDate2 = (x.Split ? x.PayDate2!.Value : x.PayDate).Date, Ref2 = x.Split ? Cut(x.Ref2!.Trim(), 50) : "",
        });
        bp.Add("CodeOut", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await c.ExecuteAsync("dbo.AddBill", bp, tx, commandType: CommandType.StoredProcedure);
        var billCode = bp.Get<int>("CodeOut").ToString();
        var billGuid = await c.ExecuteScalarAsync<Guid>("SELECT Guid FROM dbo.Bill WHERE Code = @billCode", new { billCode }, tx);
        foreach (var l in x.Lines)
            await c.ExecuteAsync("dbo.AddRegDocDiplom", new { RegDocGuid = billGuid, l.DiplomGuid, Cost = (double)l.Cost, Tax = (double)l.Tax, SubTotal = (double)l.Total, l.DiplomType, Type_ = 1 },
                tx, commandType: CommandType.StoredProcedure);

        /* ADDPAYDAY1 / ADDPAYDAY2_MADEN / ADDPAYDAY2_DAEN / ADDPAYTAXDAY2_DAEN */
        var salesType = await TypeAsync(c, tx, "salesinvoice");
        var lines = new List<JLine>
        {
            new(book.ChkMaden ? x.StudentAccount : book.MadenGuid ?? throw new ApiException("دفتر الفاتورة بدون حساب مدين"), x.ChkVat ? sub : total, 0, x.BillJournalNotes, x.CostCenter),
            new(book.ChkDaen ? x.StudentAccount : book.DaenGuid ?? throw new ApiException("دفتر الفاتورة بدون حساب دائن"), 0, total, x.BillJournalNotes, x.CostCenter),
        };
        if (x.WriteVatLine) lines.Add(new(x.VatAccount ?? throw new ApiException("الدفتر بدون حساب ضريبة القيمة المضافة"), 0, tax, x.BillJournalNotes, x.CostCenter));
        await JournalAsync(c, tx, salesType, billGuid, billCode, branch, x.Date, x.BillJournalNotes, lines);
        await audit.WriteAsync(c, tx, "salesinvoice", billGuid, UserActionLog.New, billCode);

        /* Receipt voucher(s) linked to the invoice */
        var recType = await TypeAsync(c, tx, "receipt");
        async Task<string> receipt(Guid cash, Guid cost, decimal amount, string reference, DateTime payDate)
        {
            var cp = new DynamicParameters(new
            {
                CashDayDate = x.Date, FAccountGuid = cash, Amount = (double)amount, BranchGuid = branch, Notes = Cut(x.ReceiptNotes, 200), LinkGuid = salesType, LinkRef = billCode,
                CashDayType = 0, IsUse = true, Ref = Cut(reference.Trim(), 50), PayDate = payDate.Date,
            });
            cp.Add("CodeOut", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await c.ExecuteAsync("dbo.AddCashDay", cp, tx, commandType: CommandType.StoredProcedure);
            var code = cp.Get<int>("CodeOut").ToString();
            var guid = await c.ExecuteScalarAsync<Guid>("SELECT Guid FROM dbo.CashDay WHERE Code = @code AND CashDayType = 0", new { code }, tx);
            await c.ExecuteAsync("dbo.AddCashDayDetail", new { CashDayGuid = guid, AccountGuid = x.StudentAccount, Daen = (double)amount, CostGuid = cost, Notes = Cut(x.ReceiptNotes, 200) }, tx, commandType: CommandType.StoredProcedure);
            await JournalAsync(c, tx, recType, guid, code, branch, x.Date, x.ReceiptNotes, [new(cash, amount, 0, x.ReceiptNotes, cost), new(x.StudentAccount, 0, amount, x.ReceiptNotes, cost)]);
            await audit.WriteAsync(c, tx, "receipt", guid, UserActionLog.New, code);
            return code;
        }
        var r1 = await receipt(x.CashAccount, x.CostCenter, x.Split ? x.Amount1 : sub, x.Ref, x.PayDate);
        var r2 = x.Split ? await receipt(x.CashAccount2!.Value, x.CostCenter2!.Value, x.Amount2, x.Ref2!, x.PayDate2!.Value) : null;

        /* ZATCA queue: same JSON as the desktop GetInvoiceDataAsync, type 388 */
        var tn = await c.ExecuteScalarAsync<int?>("SELECT TaxNo FROM dbo.Bill WHERE Guid = @billGuid", new { billGuid }, tx);
        var taxNo = tn?.ToString("00") ?? billCode;
        var json = await InvoiceJsonAsync(c, tx, billCode, billGuid, taxNo);
        await c.ExecuteAsync("dbo.AddBillByJson", new { BillCode = billCode, BillGuid = billGuid, JsonCintent = json, ZacatType = 388 }, tx, commandType: CommandType.StoredProcedure);
        return new InvoicePosted(billCode, billGuid, taxNo, r1, r2);
    }

    private static async Task<string> InvoiceJsonAsync(DbConnection c, DbTransaction tx, string billCode, Guid billGuid, string taxNo)
    {
        var head = (IDictionary<string, object?>)(await c.QueryFirstAsync("dbo.ShowBillInfo", new { Code = billCode }, tx, commandType: CommandType.StoredProcedure));
        var date = Convert.ToDateTime(head["BillDate"], Inv);
        string f2(object? v) => D(v).ToString("F2", Inv);
        var lines = (await c.QueryAsync("dbo.ShowBillDetail", new { BillGuid = billGuid, Type_ = 1 }, tx, commandType: CommandType.StoredProcedure))
            .Cast<IDictionary<string, object?>>()
            .Select(r => new Dictionary<string, object?>
            {
                ["id"] = S(r["Code"]), ["quantity"] = f2(r["Qty"] ?? 1), ["itemName"] = S(r["DiplomName"]), ["price"] = f2(r["Cost"]),
                ["taxRate"] = D(r["TaxValue"]).ToString(Inv), ["taxCategoryCode"] = S(r["TAXCATOGREY"]), ["lineExtensionAmount"] = f2(r["SubTotal"]),
                ["taxAmount"] = f2(r["Tax"]), ["roundingAmount"] = f2(r["SubTotal"]),
            }).ToList();
        var data = new Dictionary<string, object?>
        {
            ["id"] = taxNo,
            ["issueDate"] = date.ToString("yyyy-MM-dd", Inv),
            ["issueTime"] = date.ToString("HH:mm:ss", Inv),
            ["customer"] = new Dictionary<string, object?> { ["registrationName"] = S(head["StudentName"]), ["taxId"] = null, ["address"] = null },
            ["invoiceTotals"] = new Dictionary<string, object?>
            {
                ["lineExtensionAmount"] = f2(head["BillTotal"]), ["taxExclusiveAmount"] = f2(head["BillTotal"]), ["taxInclusiveAmount"] = f2(head["BillSubTotal"]),
                ["prepaidAmount"] = 0.0, ["payableAmount"] = f2(head["BillSubTotal"]),
            },
            ["invoiceLines"] = lines,
        };
        return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
    }
}
