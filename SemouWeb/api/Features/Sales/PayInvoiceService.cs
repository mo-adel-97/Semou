using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Sales;

/*
 * فاتورة سداد (EncodingNewPayInvoice), opened from the reception desk:
 *   kind 1 "سداد"      : pays the student's diploma/course (StudentLevel diploma, any amount)
 *   kind 2 "سداد رسوم" : pays fees of the registration form, or fees added from the price list
 * The book (RegSetting of the sales-invoice type) is picked by study branch + invoice type (AutoGetInvoiceType).
 */
public sealed class PayLineInput { public Guid DiplomGuid { get; set; } public decimal Total { get; set; } }

public sealed class PayInvoiceInput
{
    public Guid StudentGuid { get; set; }
    /// <summary>StudentLevel.ID of the file being paid.</summary>
    public int FileId { get; set; }
    /// <summary>1 سداد / 2 سداد رسوم.</summary>
    public int Kind { get; set; }
    public DateTime Date { get; set; }
    public List<PayLineInput> Lines { get; set; } = [];
    public Guid? CashAccountGuid { get; set; }
    public Guid? CostCenterGuid { get; set; }
    public string? Ref { get; set; }
    public DateTime? PayDate { get; set; }
    public string? Notes { get; set; }
    public bool Split { get; set; }
    public decimal Amount1 { get; set; }
    public decimal Amount2 { get; set; }
    public Guid? CashAccountGuid2 { get; set; }
    public Guid? CostCenterGuid2 { get; set; }
    public string? Ref2 { get; set; }
    public DateTime? PayDate2 { get; set; }
}

public sealed record PaySaved(string Message, string BillCode, Guid BillGuid, string TaxNo, string ReceiptCode, string? ReceiptCode2, string?[] AttachNames);

public sealed record PayItem(string Code, string Name, Guid DiplomGuid, int DiplomType, decimal TaxRate, decimal Total, string Source);

public sealed class PayInvoiceService(Db db, SalesPoster poster, CurrentUser me)
{
    public const string Form = "salesinvoice";
    private const decimal ForeignVat = 15m;

    private sealed record FileRow(Guid StudentGuid, Guid AccountGuid, string StudentName, string? NationalId, int? StudentNational, bool? IsUse, bool? Locked,
                                  Guid? DiplomGuid, string? DiplomName, Guid? BranchGuid, Guid? RegDocGuid, bool? LevelStaut);

    private static async Task<FileRow> FileAsync(DbConnection c, DbTransaction? tx, Guid student, int fileId)
    {
        var f = await c.QueryFirstOrDefaultAsync<FileRow>("""
            SELECT S.Guid AS StudentGuid, S.AccountGuid, S.StudentName, S.NationalId, S.StudentNational, S.IsUse, S.LockProfile AS Locked,
                   L.DiplomGuid, D.DiplomName, L.BranchGuid, L.RegDocGuid, L.LevelStaut
            FROM dbo.Student S JOIN dbo.StudentLevel L ON L.StudentGuid = S.AccountGuid LEFT JOIN dbo.Diplom D ON D.Guid = L.DiplomGuid
            WHERE S.Guid = @student AND L.ID = @fileId
            """, new { student, fileId }, tx) ?? throw new ApiException("لا تتوفر أي بيانات تدريبية للطالب");
        if (f.IsUse == false) throw new ApiException("الطالب موقوف عن التعامل");
        if (f.Locked == true) throw new ApiException("ملف الطالب مغلق، يرجى التواصل مع الإدارة");
        if (f.LevelStaut == false) throw new ApiException("أنهى الطالب الدراسة في هذا الملف التدريبي، ولذلك لا يمكن إجراء أي عملية سداد عليه");
        if (f.BranchGuid is null || f.DiplomGuid is null) throw new ApiException("لا تتوفر أي بيانات تدريبية للطالب");
        return f;
    }

    private sealed record Book(Guid Guid, int Code, string DocName, Guid BranchGuid, string? BranchName, Guid PriceListGuid, Guid? VatAccountGuid, bool ChkVat);

    /// <summary>AutoGetInvoiceType (study branch + invoice type 1/2) + SHOWREFSETTINGINFO for VAT and price list.</summary>
    private static async Task<Book> BookAsync(DbConnection c, DbTransaction? tx, Guid branch, int kind)
    {
        var type = await SalesPoster.TypeAsync(c, tx!, Form);
        return await c.QueryFirstOrDefaultAsync<Book>("""
            SELECT TOP 1 R.Guid, R.Code, R.DocName, R.BranchGuid, B.BrName AS BranchName, R.PriceListGuid, R.VatAccountGuid, CAST(ISNULL(R.ChkVat,0) AS bit) AS ChkVat
            FROM dbo.RegSetting R JOIN dbo.Branch B ON B.Guid = R.BranchGuid
            WHERE R.DocTypeGuid = @type AND R.BranchGuid = @branch AND R.InvoiceType = @kind AND R.IsUse = 1 ORDER BY R.Code
            """, new { type, branch, kind }, tx) ?? throw new ApiException("لا يتوفر دفتر للسداد، يرجى مراجعة مدير النظام");
    }

    /// <summary>Items that may appear on this invoice, with the tax rate the desktop used.</summary>
    private static async Task<List<PayItem>> ItemsAsync(DbConnection c, DbTransaction? tx, FileRow f, Book book, int kind)
    {
        var foreign = f.StudentNational == 1;
        if (kind == 1)
        {
            // SHOWDIPLOMFROMSTUDENTLEVEL / GetDiplomInfo: amount typed by the user, VAT 15% for foreigners.
            var d = await c.QueryFirstAsync<(string Code, string DiplomName, decimal TaxValue, int? DiplomType)>(
                "SELECT CAST(Code AS nvarchar(50)), DiplomName, CAST(ISNULL(TaxValue,0) AS decimal(18,3)), DiplomType FROM dbo.Diplom WHERE Guid = @g", new { g = f.DiplomGuid }, tx);
            return [new PayItem(d.Code, d.DiplomName, f.DiplomGuid!.Value, d.DiplomType ?? 0, foreign ? ForeignVat : d.TaxValue, 0, "file")];
        }
        var items = new List<PayItem>();
        if (f.RegDocGuid is { } rd && rd != Guid.Empty)
            items.AddRange((await c.QueryAsync<(string Code, string DiplomName, decimal TaxValue, Guid Guid, decimal SubTotal, int? DiplomType)>("""
                SELECT CAST(D.Code AS nvarchar(50)), D.DiplomName, CAST(ISNULL(D.TaxValue,0) AS decimal(18,3)), D.Guid, CAST(ISNULL(R.SubTotal,0) AS decimal(18,3)), R.DiplomType
                FROM dbo.RegDocDiplom R JOIN dbo.Diplom D ON D.Guid = R.DiplomGuid WHERE R.RegDocGuid = @rd AND R.DiplomType = 1 AND R.Type_ = 0 ORDER BY R.ID
                """, new { rd }, tx)).Select(r => new PayItem(r.Code, r.DiplomName, r.Guid, r.DiplomType ?? 1, r.TaxValue, r.SubTotal, "regdoc")));
        // CmdAddItem / AddStudentOtherFess: fees from the book's price list (no VAT, like the desktop).
        items.AddRange((await c.QueryAsync("dbo.ShowPriceListDiplom", new { PriceListGuid = book.PriceListGuid, Type = 1 }, tx, commandType: CommandType.StoredProcedure))
            .Cast<IDictionary<string, object?>>()
            .Select(r => new PayItem(r["Code"]?.ToString() ?? "", r["DiplomName"]?.ToString() ?? "", (Guid)r["diplomguid"]!, 1, 0, Convert.ToDecimal(r["DiplomSubTotal"] ?? 0m), "pricelist"))
            .Where(p => !items.Any(i => i.DiplomGuid == p.DiplomGuid)));
        return items;
    }

    public async Task<object> PrepareAsync(Guid student, int fileId, int kind, CancellationToken ct)
    {
        if (kind is not (1 or 2)) throw new ApiException("نوع السداد غير صحيح");
        await using var c = await db.OpenAsync(ct);
        var f = await FileAsync(c, null, student, fileId);
        var book = await BookAsync(c, null, f.BranchGuid!.Value, kind);
        var items = await ItemsAsync(c, null, f, book, kind);
        var last = await c.QueryFirstOrDefaultAsync("dbo.AskForLastBill", new { StudentGuid = f.AccountGuid }, commandType: CommandType.StoredProcedure);
        var zero = await c.ExecuteScalarAsync<bool?>("SELECT ChkAmount FROM dbo.User_Info WHERE Guid = @g", new { g = me.UserGuid }) == true;
        return new
        {
            student = new { f.StudentGuid, f.StudentName, f.NationalId, foreign = f.StudentNational == 1, f.DiplomName },
            book = new { book.Guid, book.Code, book.DocName, book.BranchName, book.ChkVat },
            items, lastBill = last, zeroAmount = zero,
            cashBoxes = await c.QueryAsync("""
                SELECT CB.AccountGuid AS Guid, CB.CashName AS Name, CB.CashType, CB.CostCenterGuid, CC.CenterName AS CostName
                FROM dbo.CashBox CB LEFT JOIN dbo.CostCenter CC ON CC.Guid = CB.CostCenterGuid WHERE CB.IsUse = 1 AND CB.UseInSales = 1 ORDER BY CB.CashType, TRY_CONVERT(int, CB.Code)
                """),
        };
    }

    /// <summary>Direct save (kept for internal use; the web sends a طلب سداد that is posted on confirmation).</summary>
    public async Task<object> SaveAsync(PayInvoiceInput i, CancellationToken ct)
    {
        Normalize(i);
        return await db.InTransactionAsync(async (c, tx) => await PostAsync(c, tx, i, await PlanAsync(c, tx, i, me.UserGuid), me.UserGuid), IsolationLevel.Serializable, ct);
    }

    public static void Normalize(PayInvoiceInput i)
    {
        if (i.Kind is not (1 or 2)) throw new ApiException("نوع السداد غير صحيح");
        i.Date = i.Date == default || i.Date.Date == Clock.Now.Date ? Clock.Now : i.Date.Date;
    }

    public sealed class PayPlan
    {
        public required Guid AccountGuid { get; init; }
        public required string StudentName { get; init; }
        public string? NationalId { get; init; }
        public Guid BranchGuid { get; init; }
        public string? DiplomName { get; init; }
        public required Guid BookGuid { get; init; }
        public required string BookName { get; init; }
        public bool ChkVat { get; init; }
        public Guid? VatAccount { get; init; }
        public required List<InvoiceLine> Lines { get; init; }
        public decimal Sub { get; init; }
        public decimal Tax { get; init; }
        public string? CashName { get; init; }
        public string? CashName2 { get; init; }
    }

    /// <summary>Every check of the desktop save + server pricing, without writing (used on request and on confirmation).</summary>
    public async Task<PayPlan> PlanAsync(DbConnection c, DbTransaction tx, PayInvoiceInput i, Guid actor)
    {
            var f = await FileAsync(c, tx, i.StudentGuid, i.FileId);
            var book = await BookAsync(c, tx, f.BranchGuid!.Value, i.Kind);
            var allowed = await ItemsAsync(c, tx, f, book, i.Kind);
            var zero = await c.ExecuteScalarAsync<bool?>("SELECT ChkAmount FROM dbo.User_Info WHERE Guid = @g", new { g = actor }, tx) == true;

            /* VALIDATE_DATA */
            if (i.CashAccountGuid is null) throw new ApiException("يرجى اختيار الخزينة / البنك الخاص بعملية السداد");
            var cashName = await c.ExecuteScalarAsync<string?>("SELECT CashName FROM dbo.CashBox WHERE AccountGuid = @a AND IsUse = 1 AND UseInSales = 1", new { a = i.CashAccountGuid }, tx)
                           ?? throw new ApiException("الخزينة / البنك غير متاح لفواتير المبيعات");
            if (i.CostCenterGuid is null) throw new ApiException("يرجى اختيار مركز التكلفة الخاص بعملية السداد");
            if (string.IsNullOrWhiteSpace(i.Ref)) throw new ApiException("يرجى إدخال رقم المرجع الخاص بعملية السداد");
            if (i.PayDate is null) throw new ApiException("يرجى إدخال تاريخ الحوالة البنكية / السداد");
            var lines = new List<InvoiceLine>();
            foreach (var l in i.Lines.Where(l => l.Total != 0).DistinctBy(l => l.DiplomGuid))
            {
                if (l.Total < 0) throw new ApiException("لا يمكن إدخال قيمة بالسالب");
                var it = allowed.FirstOrDefault(a => a.DiplomGuid == l.DiplomGuid) ?? throw new ApiException("أحد البنود غير مسموح في هذه الفاتورة");
                // PayGrid_CellEndEdit (column 7): the total includes VAT.
                var cost = it.TaxRate != 0 ? Math.Round(l.Total / (1 + it.TaxRate / 100), 3) : l.Total;
                var lineTax = it.TaxRate != 0 ? Math.Round(cost * it.TaxRate / 100, 3) : 0;
                lines.Add(new InvoiceLine(it.DiplomGuid, it.DiplomType, it.Name, cost, lineTax, l.Total));
            }
            var sub = Math.Round(lines.Sum(l => l.Total), 2);
            if (sub == 0 && !zero) throw new ApiException("يرجى إدخال المبلغ الخاص بعملية السداد");
            if (lines.Count == 0) { var it = allowed[0]; lines.Add(new InvoiceLine(it.DiplomGuid, it.DiplomType, it.Name, 0, 0, 0)); }
            string? cashName2 = null;
            if (i.Split)
            {
                if (i.CashAccountGuid2 is null) throw new ApiException("يرجى اختيار الخزينة / البنك الثاني");
                cashName2 = await c.ExecuteScalarAsync<string?>("SELECT CashName FROM dbo.CashBox WHERE AccountGuid = @a AND IsUse = 1 AND UseInSales = 1", new { a = i.CashAccountGuid2 }, tx)
                            ?? throw new ApiException("الخزينة / البنك 2 غير متاح لفواتير المبيعات");
                if (i.CostCenterGuid2 is null) throw new ApiException("يرجى اختيار مركز التكلفة الثاني الخاص بعملية السداد");
                if (string.IsNullOrWhiteSpace(i.Ref2)) throw new ApiException("يرجى إدخال رقم المرجع الثاني الخاص بعملية السداد");
                if (i.PayDate2 is null) throw new ApiException("يرجى إدخال تاريخ الحوالة البنكية الثانية / السداد");
                if (i.Amount1 <= 0) throw new ApiException("يرجى إدخال قيمة الحوالة الأولى");
                if (i.Amount2 <= 0) throw new ApiException("يرجى إدخال قيمة الحوالة الثانية");
                if (Math.Round(i.Amount1 + i.Amount2, 2) != sub) throw new ApiException("مبالغ الحوالات لا تساوي إجمالي الفاتورة");
            }

            return new PayPlan
            {
                AccountGuid = f.AccountGuid, StudentName = f.StudentName, NationalId = f.NationalId, BranchGuid = f.BranchGuid!.Value,
                DiplomName = i.Kind == 1 ? f.DiplomName : string.Join("، ", lines.Select(l => l.Name)),
                BookGuid = book.Guid, BookName = book.DocName, ChkVat = book.ChkVat, VatAccount = book.VatAccountGuid,
                Lines = lines, Sub = sub, Tax = Math.Round(lines.Sum(l => l.Tax), 2), CashName = cashName, CashName2 = cashName2,
            };
    }

    public async Task<PaySaved> PostAsync(DbConnection c, DbTransaction tx, PayInvoiceInput i, PayPlan p, Guid actor)
    {
            var f = new { p.AccountGuid, p.StudentName };
            var book = new { Guid = p.BookGuid, DocName = p.BookName, p.ChkVat, VatAccountGuid = p.VatAccount };
            var lines = p.Lines; var diplomName = p.DiplomName; var tax = p.Tax; var cashName = p.CashName; var cashName2 = p.CashName2;
            var posted = await poster.PostAsync(c, tx, new InvoicePosting
            {
                UserGuid = actor,
                StudentAccount = f.AccountGuid, Date = i.Date, PayDocGuid = book.Guid, Lines = lines,
                CashAccount = i.CashAccountGuid!.Value, CostCenter = i.CostCenterGuid!.Value, Ref = i.Ref!, PayDate = i.PayDate!.Value, Notes = i.Notes,
                Split = i.Split, Amount1 = i.Amount1, Amount2 = i.Amount2, CashAccount2 = i.CashAccountGuid2, CostCenter2 = i.CostCenterGuid2, Ref2 = i.Ref2, PayDate2 = i.PayDate2,
                ChkVat = book.ChkVat, VatAccount = book.VatAccountGuid, WriteVatLine = book.ChkVat && tax != 0, // EncodingNewPayInvoice: ChkVat And TxtPayTax <> 0
                BillJournalNotes = $"فاتورة مبيعات  {f.StudentName}\r\n {book.DocName}\r\n{diplomName}",
                ReceiptNotes = $"سداد فاتورة مبيعات {f.StudentName}\r\n {book.DocName}\r\n{diplomName}",
            });
            // Names for the transfer images (AADATTACHFILE: {CashName}_{BillCode}_{ddMMyyyy}).
            return new PaySaved("تم حفظ فاتورة المبيعات بنجاح", posted.BillCode, posted.BillGuid, posted.TaxNo, posted.ReceiptCode, posted.ReceiptCode2,
            [
                cashName == "صندوق المكتب" ? null : $"{cashName}_{posted.BillCode}_{i.PayDate!.Value:ddMMyyyy}",
                !i.Split || cashName2 == "صندوق المكتب" ? null : $"{cashName2}_{posted.BillCode}_{i.PayDate2!.Value:ddMMyyyy}",
            ]);
    }

    /* ============================ Tax invoice print (PRINTTAXINVOICE) ============================ */

    public async Task<object> PrintAsync(Guid billGuid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var rows = (await c.QueryAsync(new CommandDefinition("dbo.PrintBill", new { BillGuid = billGuid }, commandType: CommandType.StoredProcedure, cancellationToken: ct))).Cast<IDictionary<string, object?>>().ToList();
        if (rows.Count == 0) throw ApiException.NotFound("لم يتم العثور على بيانات الفاتورة!");
        var info = await c.QueryFirstAsync<(Guid BranchGuid, string? Notes, bool? IsUse)>("SELECT BranchGuid, Notes, IsUse FROM dbo.Bill WHERE Guid = @billGuid", new { billGuid });
        var br = await c.QueryFirstOrDefaultAsync<(string? BrName, string? BrAddress, string? BrVatId, string? BrTel, string? VatName, string? BrFax)>(
            "dbo.LoadBranchInfo", new { BranchGuid = info.BranchGuid }, commandType: CommandType.StoredProcedure);
        var first = rows[0];
        var date = Convert.ToDateTime(first["BillDate"], CultureInfo.InvariantCulture);
        var inv = CultureInfo.InvariantCulture;
        // ZATCA phase-1 QR: TLV(1 seller, 2 VAT no, 3 timestamp, 4 total incl. VAT, 5 VAT) → base64, same fields as the desktop.
        var qr = Tlv(br.VatName ?? "", br.BrVatId ?? "", Utc(date).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", inv),
                     Convert.ToDecimal(first["BillSubTotal"], inv).ToString("0.00", inv), Convert.ToDecimal(first["BillTax"], inv).ToString("0.00", inv));
        return new
        {
            header = new
            {
                companyName = br.BrName, address = br.BrAddress, vatNo = br.BrVatId, crNo = br.BrFax, phone = br.BrTel, notes = info.Notes, cancelled = info.IsUse == false,
                invoiceNo = first["Code"]?.ToString(), date = date.ToString("yyyy/MM/dd   hh:mm tt", inv), seller = first["FullName"]?.ToString(),
                studentName = first["StudentName"]?.ToString(), studentTel = first["StudentTel"]?.ToString(), nationalId = first["NationalId"]?.ToString(),
                total = first["BillTotal"], tax = first["BillTax"], subTotal = first["BillSubTotal"],
                amountText = await c.ExecuteScalarAsync<string>("SELECT dbo.FNTafkeet(@a)", new { a = Convert.ToDouble(first["BillSubTotal"], inv) }),
            },
            lines = rows.Select(r => new { code = r["ItemCode"]?.ToString(), name = r["DiplomName"]?.ToString(), qty = r["Qty"] ?? 1, cost = r["Cost"], taxRate = r["TaxValue"], tax = r["Tax"], total = r["SubTotal"] }),
            qr,
        };
    }

    private static readonly TimeZoneInfo Ksa = TimeZoneInfo.TryFindSystemTimeZoneById("Asia/Riyadh", out var tz) ? tz : TimeZoneInfo.Local;
    private static DateTime Utc(DateTime local) => TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), Ksa);

    private static string Tlv(params string[] values)
    {
        using var ms = new MemoryStream();
        for (var t = 0; t < values.Length; t++)
        {
            var bytes = Encoding.UTF8.GetBytes(values[t]);
            ms.WriteByte((byte)(t + 1));
            ms.WriteByte((byte)bytes.Length);
            ms.Write(bytes);
        }
        return Convert.ToBase64String(ms.ToArray());
    }
}
