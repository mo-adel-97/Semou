using System.Data;
using System.Data.Common;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Vouchers;

/*
 * سند قبض (EncodingCashDayReceipt, CashDayType 0) and سند صرف (EncodingCashDayPay, CashDayType 1).
 *
 *   Receipt: cash/bank account  = Debit  (مدين) for the amount; detail lines = Credit (دائن)
 *   Payment: cash/bank account  = Credit (دائن) for the amount; detail lines = Debit  (مدين)
 *
 * Every save also writes the journal entry (Day1 header + Day2 lines) inside the same transaction,
 * using the same procs and the same order as the desktop FORSAVE / FORUPDATE.
 */
public sealed record VoucherKind(int CashDayType, string Form, string Title, string HeaderSide, string LineSide,
                                 string AddProc, string AddDetailProc, string DetailColumn);

public sealed class VoucherLine
{
    public Guid AccountGuid { get; set; }
    public decimal Amount { get; set; }
    public Guid? CostGuid { get; set; }
    public string? Notes { get; set; }
}

public sealed class VoucherInput
{
    public DateTime Date { get; set; }
    /// <summary>The cash box / bank ledger account (CashBox.AccountGuid).</summary>
    public Guid? AccountGuid { get; set; }
    public Guid? BranchGuid { get; set; }
    public decimal Amount { get; set; }
    public string Notes { get; set; } = "";
    public bool IsUse { get; set; } = true;
    public string? VendorName { get; set; }
    public string? VendorTax { get; set; }
    public string? VendorTel { get; set; }
    public List<VoucherLine> Lines { get; set; } = [];
    public string? Reason { get; set; }
}

public sealed record VoucherSaved(string Message, string Code, Guid Guid, string? JournalCode);

public sealed class VoucherService(Db db, UserActionLog audit, CurrentUser me)
{
    public static readonly VoucherKind Receipt = new(0, "receipt", "سند القبض", "مدين", "دائن", "AddCashDay", "AddCashDayDetail", "Daen");
    public static readonly VoucherKind Payment = new(1, "pay", "سند الصرف", "دائن", "مدين", "AddCashDayPay", "AddCashDayPayDetail", "Maden");

    private const string ListSql = """
        SELECT CAST(C.Code AS nvarchar(50)) AS Code, C.Guid, C.CashDayDate AS Date, CAST(ISNULL(C.Amount,0) AS decimal(18,2)) AS Amount,
               ISNULL(CB.CashName, A.AccountName) AS AccountName, C.Notes, CAST(ISNULL(C.IsUse,0) AS bit) AS IsUse,
               B.BrName AS BranchName, C.VendorName, F.Name AS LinkName, C.LinkRef
        FROM dbo.CashDay C
        LEFT JOIN dbo.CashBox CB ON CB.AccountGuid = C.FAccountGuid
        LEFT JOIN dbo.Account A ON A.Guid = C.FAccountGuid
        LEFT JOIN dbo.Branch B ON B.Guid = C.BranchGuid
        LEFT JOIN dbo.Form_Name F ON F.Guid = C.LinkGuid
        """;

    public async Task<IEnumerable<dynamic>> ListAsync(VoucherKind k, DateTime? from, DateTime? to, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync(new CommandDefinition(ListSql + """
             WHERE C.CashDayType = @t AND (@from IS NULL OR C.CashDayDate >= @from) AND (@to IS NULL OR C.CashDayDate < DATEADD(day, 1, @to))
             ORDER BY C.Id DESC
            """, new { t = k.CashDayType, from = from?.Date, to = to?.Date }, cancellationToken: ct));
    }

    public async Task<object> GetAsync(VoucherKind k, Guid guid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var head = await c.QueryFirstOrDefaultAsync(new CommandDefinition("""
            SELECT CAST(C.Code AS nvarchar(50)) AS Code, C.Guid, C.CashDayDate AS Date, C.FAccountGuid AS AccountGuid, ISNULL(CB.CashName, A.AccountName) AS AccountName,
                   C.BranchGuid, B.BrName AS BranchName, CAST(ISNULL(C.Amount,0) AS decimal(18,2)) AS Amount, C.Notes, CAST(ISNULL(C.IsUse,0) AS bit) AS IsUse,
                   C.VendorName, C.VendorTax, C.VendorTel, F.Name AS LinkName, C.LinkRef, CB.CostCenterGuid AS DefaultCostGuid, CC.CenterName AS DefaultCostName,
                   (SELECT TOP 1 CAST(D.Code AS nvarchar(50)) FROM dbo.Day1 D WHERE D.DocGuid = C.Guid) AS JournalCode,
                   dbo.FNTafkeet(ISNULL(C.Amount,0)) AS AmountText
            FROM dbo.CashDay C
            LEFT JOIN dbo.CashBox CB ON CB.AccountGuid = C.FAccountGuid
            LEFT JOIN dbo.Account A ON A.Guid = C.FAccountGuid
            LEFT JOIN dbo.Branch B ON B.Guid = C.BranchGuid
            LEFT JOIN dbo.CostCenter CC ON CC.Guid = CB.CostCenterGuid
            LEFT JOIN dbo.Form_Name F ON F.Guid = C.LinkGuid
            WHERE C.Guid = @guid AND C.CashDayType = @t
            """, new { guid, t = k.CashDayType }, cancellationToken: ct)) ?? throw ApiException.NotFound($"{k.Title} غير موجود");

        var lines = await c.QueryAsync(new CommandDefinition("""
            SELECT D.AccountGuid, CAST(A.Code AS nvarchar(50)) AS AccountCode, A.AccountName, A.Type_ AS AccountType,
                   CAST(ISNULL(D.Maden,0) + ISNULL(D.Daen,0) AS decimal(18,2)) AS Amount, D.CostGuid, CC.CenterName AS CostName, D.Notes
            FROM dbo.CashDayDetail D JOIN dbo.Account A ON A.Guid = D.AccountGuid LEFT JOIN dbo.CostCenter CC ON CC.Guid = D.CostGuid
            WHERE D.CashDayGuid = @guid ORDER BY D.Id
            """, new { guid }, cancellationToken: ct));
        return new { head, lines };
    }

    private sealed record AccountInfo(Guid Guid, int? AccountType, int? Type_, bool? IsUse);

    /// <summary>VALIDATE_DATA + NOCOSTCENTER_ + NODAY2NOTES_ (+ checks the grid enforced implicitly).</summary>
    private async Task ValidateAsync(DbConnection c, DbTransaction tx, VoucherKind k, VoucherInput i)
    {
        var lines = i.Lines.Where(l => l.AccountGuid != Guid.Empty).ToList();
        i.Lines = lines;

        if (k == Receipt && i.BranchGuid is null) throw new ApiException("يرجى اختيار الفرع أولاً");
        if (i.AccountGuid is null) throw new ApiException(k == Receipt ? "يرجى اختيار الحساب المدين أولاً" : "يرجى اختيار الحساب الدائن أولاً");
        if (string.IsNullOrWhiteSpace(i.Notes)) throw new ApiException($"يرجى إدخال ملاحظات {k.Title} لتسهيل عملية مراجعة الحسابات");
        if (i.Notes.Length > 200) throw new ApiException("الملاحظات طويلة جداً (الحد الأقصى 200 حرف)");
        if (i.Date == default) throw new ApiException("يرجى اختيار تاريخ السند");
        if (i.Amount < 0 || lines.Any(l => l.Amount < 0)) throw new ApiException("لا يمكن إدخال مبالغ بالسالب");

        // Desktop: payments may be zero only for users with "إدخال مبالغ صفرية".
        var zeroAllowed = k == Payment && await c.ExecuteScalarAsync<bool?>("SELECT ChkAmount FROM dbo.User_Info WHERE Guid = @g", new { g = me.UserGuid }, tx) == true;
        var total = Math.Round(lines.Sum(l => l.Amount), 2);
        if (i.Amount == 0 && !zeroAllowed) throw new ApiException($"يرجى إدخال قيمة {k.Title}");
        if (lines.Count == 0 || (total == 0 && !zeroAllowed)) throw new ApiException($"يرجى التأكد من المبالغ المدرجة في الجانب ال{k.LineSide}");
        if (Math.Round(i.Amount - total, 2) != 0) throw new ApiException("السند غير متزن، يرجى مراجعة تفاصيل السند");

        var header = await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.CashBox WHERE AccountGuid = @a", new { a = i.AccountGuid }, tx);
        if (header == 0) throw new ApiException("الحساب المختار ليس خزينة أو بنك");

        var infos = (await c.QueryAsync<AccountInfo>("SELECT Guid, AccountType, Type_, IsUse FROM dbo.Account WHERE Guid IN @ids",
            new { ids = lines.Select(l => l.AccountGuid).Distinct() }, tx)).ToDictionary(a => a.Guid);
        foreach (var l in lines)
        {
            if (!infos.TryGetValue(l.AccountGuid, out var a) || a.AccountType == 2 || a.IsUse == false)
                throw new ApiException("أحد الحسابات المختارة غير صالح أو موقوف");
            // Payment: only revenue/expense accounts need a cost center (desktop BranchCostList rule).
            var needsCost = k == Receipt || a.Type_ is 0 or 1;
            if (!needsCost) l.CostGuid = Guid.Empty;
            else if (l.CostGuid is null || l.CostGuid == Guid.Empty) throw new ApiException("لا يمكن الحفظ بدون اختيار مركز التكلفة");
            if (string.IsNullOrWhiteSpace(l.Notes)) throw new ApiException("لا يمكن الحفظ بدون إدخال البيان لتسهيل عملية المراجعة");
            if (l.Notes.Length > 200) throw new ApiException("البيان طويل جداً (الحد الأقصى 200 حرف)");
        }
    }

    /// <summary>The desktop stored date + time. New: today keeps the current time. Edit: same day keeps the original time.</summary>
    private static DateTime Stamp(DateTime picked, DateTime? existing = null)
    {
        if (existing is { } e && e.Date == picked.Date) return e;
        var now = Clock.Now;
        return picked.Date == now.Date ? now : picked.Date;
    }

    private static object HeaderParams(VoucherKind k, VoucherInput i) => new
    {
        CashDayDate = i.Date,
        FAccountGuid = i.AccountGuid,
        Amount = (double)i.Amount,
        BranchGuid = k == Receipt ? i.BranchGuid!.Value : Guid.Empty, // the desktop payment voucher stores the empty branch
        Notes = i.Notes.Trim(),
        i.IsUse,
        Ref = "",
        PayDate = i.Date,
    };

    private static async Task SaveDetailAndJournalAsync(DbConnection c, DbTransaction tx, VoucherKind k, Guid voucher, Guid day1, VoucherInput i, Guid? headerCost)
    {
        foreach (var l in i.Lines)
        {
            var p = new DynamicParameters(new { CashDayGuid = voucher, l.AccountGuid, CostGuid = l.CostGuid ?? Guid.Empty, Notes = l.Notes!.Trim() });
            p.Add(k.DetailColumn, (double)l.Amount);
            await c.ExecuteAsync("dbo." + k.AddDetailProc, p, tx, commandType: CommandType.StoredProcedure);
        }

        // ADD*DAY2: the cash/bank line, then one line per detail row.
        var amount = (double)i.Amount;
        await c.ExecuteAsync("dbo.AddDay2", new
        {
            Day1Guid = day1, AccountGuid = i.AccountGuid, Maden = k == Receipt ? amount : 0, Daen = k == Receipt ? 0 : amount,
            Notes = i.Notes.Trim(), CostGuid = headerCost ?? Guid.Empty, IsUse = true,
        }, tx, commandType: CommandType.StoredProcedure);
        foreach (var l in i.Lines)
            await c.ExecuteAsync("dbo.AddDay2", new
            {
                Day1Guid = day1, l.AccountGuid, Maden = k == Receipt ? 0 : (double)l.Amount, Daen = k == Receipt ? (double)l.Amount : 0,
                Notes = l.Notes!.Trim(), CostGuid = l.CostGuid ?? Guid.Empty, IsUse = true,
            }, tx, commandType: CommandType.StoredProcedure);
    }

    public async Task<VoucherSaved> CreateAsync(VoucherKind k, VoucherInput i, CancellationToken ct) =>
        await db.InTransactionAsync(async (c, tx) =>
        {
            await ValidateAsync(c, tx, k, i);
            i.Date = Stamp(i.Date);
            var typeGuid = await c.ExecuteScalarAsync<Guid>("dbo.FormTypeGuid", new { FormName = k.Form }, tx, commandType: CommandType.StoredProcedure);
            var openGuid = await c.ExecuteScalarAsync<Guid>("dbo.FormTypeGuid", new { FormName = "opendaily" }, tx, commandType: CommandType.StoredProcedure);
            var headerCost = await c.ExecuteScalarAsync<Guid?>("SELECT TOP 1 CostCenterGuid FROM dbo.CashBox WHERE AccountGuid = @a", new { a = i.AccountGuid }, tx);

            // ADDRECEPIT / ADDPAY
            var p = new DynamicParameters(HeaderParams(k, i));
            p.Add("LinkGuid", Guid.Empty);
            p.Add("LinkRef", "");
            p.Add("CashDayType", k.CashDayType);
            if (k == Payment)
            {
                p.Add("VendorName", i.VendorName?.Trim() ?? "");
                p.Add("VendorTax", i.VendorTax?.Trim() ?? "");
                p.Add("VendorTel", i.VendorTel?.Trim() ?? "");
            }
            p.Add("CodeOut", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await c.ExecuteAsync("dbo." + k.AddProc, p, tx, commandType: CommandType.StoredProcedure);
            var code = p.Get<int>("CodeOut").ToString();
            var guid = await c.ExecuteScalarAsync<Guid>("SELECT Guid FROM dbo.CashDay WHERE Code = @code AND CashDayType = @t", new { code, t = k.CashDayType }, tx);

            // ADD*DAY1 (journal header)
            var d = new DynamicParameters(new
            {
                OpenGuid = openGuid, DayDate = i.Date, Notes = i.Notes.Trim(), DayType = true, DocGuid = guid, IsUse = true,
                TypeGuid = typeGuid, ActionCode = code, BranchGuid = k == Receipt ? i.BranchGuid!.Value : Guid.Empty,
            });
            d.Add("CodeOut", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await c.ExecuteAsync("dbo.AddDay1", d, tx, commandType: CommandType.StoredProcedure);
            var journalCode = d.Get<int>("CodeOut").ToString();
            var day1 = await c.ExecuteScalarAsync<Guid>("SELECT TOP 1 Guid FROM dbo.Day1 WHERE DocGuid = @guid", new { guid }, tx);

            await SaveDetailAndJournalAsync(c, tx, k, guid, day1, i, headerCost);
            await audit.WriteAsync(c, tx, k.Form, guid, UserActionLog.New, code);
            return new VoucherSaved($"تم حفظ {k.Title} بنجاح", code, guid, journalCode);
        }, IsolationLevel.Serializable, ct);

    public async Task<VoucherSaved> UpdateAsync(VoucherKind k, Guid guid, VoucherInput i, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب الإجراء أولاً");
        return await db.InTransactionAsync(async (c, tx) =>
        {
            var cur = await c.QueryFirstOrDefaultAsync<(string Code, Guid? LinkGuid, DateTime? Date)>(
                "SELECT CAST(Code AS nvarchar(50)), LinkGuid, CashDayDate FROM dbo.CashDay WITH (UPDLOCK) WHERE Guid = @guid AND CashDayType = @t", new { guid, t = k.CashDayType }, tx);
            if (cur.Code is null) throw ApiException.NotFound($"{k.Title} غير موجود");
            if (cur.LinkGuid is { } lg && lg != Guid.Empty)
                throw new ApiException("لا يمكن إجراء أي تعديلات على السند المحدد لارتباطه بمستند آخر");
            await ValidateAsync(c, tx, k, i);
            i.Date = Stamp(i.Date, cur.Date);
            var headerCost = await c.ExecuteScalarAsync<Guid?>("SELECT TOP 1 CostCenterGuid FROM dbo.CashBox WHERE AccountGuid = @a", new { a = i.AccountGuid }, tx);

            // UPDATE*  (both vouchers use UpdateCashDay, the payment one with the vendor fields)
            var p = new DynamicParameters(HeaderParams(k, i));
            p.Add("Guid", guid);
            p.Add("VendorName", k == Payment ? i.VendorName?.Trim() ?? "" : null);
            p.Add("VendorTax", k == Payment ? i.VendorTax?.Trim() ?? "" : null);
            p.Add("VendorTel", k == Payment ? i.VendorTel?.Trim() ?? "" : null);
            await c.ExecuteAsync("dbo.UpdateCashDay", p, tx, commandType: CommandType.StoredProcedure);
            await c.ExecuteAsync("dbo.DeleteCashDayDetail", new { CashDayGuid = guid }, tx, commandType: CommandType.StoredProcedure);

            var day1 = await c.ExecuteScalarAsync<Guid?>("SELECT TOP 1 Guid FROM dbo.Day1 WHERE DocGuid = @guid", new { guid }, tx)
                       ?? throw new ApiException("لم يتم العثور على القيد المحاسبي الخاص بالسند");
            await c.ExecuteAsync("dbo.UpdateDay1", new
            {
                Guid = day1, DayDate = i.Date, Notes = i.Notes.Trim(), i.IsUse, BranchGuid = k == Receipt ? i.BranchGuid!.Value : Guid.Empty,
            }, tx, commandType: CommandType.StoredProcedure);
            await c.ExecuteAsync("dbo.DeleteDay2", new { Day1Guid = day1 }, tx, commandType: CommandType.StoredProcedure);

            await SaveDetailAndJournalAsync(c, tx, k, guid, day1, i, headerCost);
            await audit.WriteAsync(c, tx, k.Form, guid, UserActionLog.Update, cur.Code, i.Reason!.Trim());
            var journal = await c.ExecuteScalarAsync<string?>("SELECT CAST(Code AS nvarchar(50)) FROM dbo.Day1 WHERE Guid = @day1", new { day1 }, tx);
            return new VoucherSaved($"تم تعديل {k.Title} بنجاح", cur.Code, guid, journal);
        }, ct: ct);
    }
}
