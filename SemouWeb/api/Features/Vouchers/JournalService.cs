using System.Data;
using System.Data.Common;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Vouchers;

/*
 * قيد يومية (EncodingDaily, form "daily") and قيد افتتاحي (EncodingOpenDaily, form "opendaily").
 * Both write dbo.Day1 (header) + dbo.Day2 (lines). Journal entries created by other documents
 * (vouchers, invoices: DayType = 1) are listed too but are read-only here, like the desktop.
 */
public sealed record JournalKind(string Form, string Title, bool Opening, string AddProc);

public sealed class JournalLine
{
    public Guid AccountGuid { get; set; }
    public decimal Maden { get; set; }
    public decimal Daen { get; set; }
    public Guid? CostGuid { get; set; }
    public string? Notes { get; set; }
}

public sealed class JournalInput
{
    public DateTime Date { get; set; }
    public string Notes { get; set; } = "";
    public bool IsUse { get; set; } = true;
    public List<JournalLine> Lines { get; set; } = [];
    public string? Reason { get; set; }
}

public sealed record JournalSaved(string Message, string Code, Guid Guid);

public sealed class JournalService(Db db, UserActionLog audit, CurrentUser me)
{
    public static readonly JournalKind Daily = new("daily", "قيد اليومية", false, "AddDailyDay1");
    public static readonly JournalKind Opening = new("opendaily", "القيد الافتتاحي", true, "AddOpenDailyDay1");

    private static Task<Guid> OpenGuidAsync(DbConnection c, DbTransaction? tx = null) =>
        c.ExecuteScalarAsync<Guid>("dbo.FormTypeGuid", new { FormName = "opendaily" }, tx, commandType: CommandType.StoredProcedure);

    public async Task<IEnumerable<dynamic>> ListAsync(JournalKind k, DateTime? from, DateTime? to, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var open = await OpenGuidAsync(c);
        return await c.QueryAsync(new CommandDefinition($"""
            SELECT CAST(D.Code AS nvarchar(50)) AS Code, D.Guid, D.DayDate AS Date, D.Notes, CAST(ISNULL(D.IsUse,0) AS bit) AS IsUse,
                   CAST(ISNULL(D.DayType,0) AS bit) AS Linked, F.Name AS SourceName, D.ActionCode AS SourceCode,
                   CAST(ISNULL((SELECT SUM(ISNULL(X.Maden,0)) FROM dbo.Day2 X WHERE X.Day1Guid = D.Guid),0) AS decimal(18,2)) AS Total
            FROM dbo.Day1 D LEFT JOIN dbo.Form_Name F ON F.Guid = D.TypeGuid
            WHERE D.TypeGuid {(k.Opening ? "=" : "<>")} @open
              AND (@from IS NULL OR D.DayDate >= @from) AND (@to IS NULL OR D.DayDate < DATEADD(day, 1, @to))
            ORDER BY D.ID DESC
            """, new { open, from = from?.Date, to = to?.Date }, cancellationToken: ct));
    }

    public async Task<object> GetAsync(JournalKind k, Guid guid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var open = await OpenGuidAsync(c);
        var head = await c.QueryFirstOrDefaultAsync(new CommandDefinition($"""
            SELECT CAST(D.Code AS nvarchar(50)) AS Code, D.Guid, D.DayDate AS Date, D.Notes, CAST(ISNULL(D.IsUse,0) AS bit) AS IsUse,
                   CAST(ISNULL(D.DayType,0) AS bit) AS Linked, F.Name AS SourceName, D.ActionCode AS SourceCode
            FROM dbo.Day1 D LEFT JOIN dbo.Form_Name F ON F.Guid = D.TypeGuid
            WHERE D.Guid = @guid AND D.TypeGuid {(k.Opening ? "=" : "<>")} @open
            """, new { guid, open }, cancellationToken: ct)) ?? throw ApiException.NotFound($"{k.Title} غير موجود");
        var lines = await c.QueryAsync(new CommandDefinition("dbo.ShowDay1Details", new { Day1Guid = guid }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return new
        {
            head,
            lines = lines.Select(l => new
            {
                AccountCode = ((object?)l.Code)?.ToString(), AccountName = (string?)l.AccountName, Notes = (string?)l.Notes,
                Maden = Convert.ToDecimal(l.Maden ?? 0), Daen = Convert.ToDecimal(l.Daen ?? 0), CostName = (string?)l.CenterName,
                AccountGuid = (Guid)l.AccountGuid, CostGuid = (Guid?)l.CostGuid, AccountType = (int?)l.Type_,
            }),
        };
    }

    private sealed record AccountInfo(Guid Guid, int? AccountType, int? Type_, bool? IsUse);

    /// <summary>VALIDATE_DATA + NOCOSTCENTER_ + NODAY2NOTES_.</summary>
    private async Task ValidateAsync(DbConnection c, DbTransaction tx, JournalInput i)
    {
        i.Lines = i.Lines.Where(l => l.AccountGuid != Guid.Empty).ToList();
        if (string.IsNullOrWhiteSpace(i.Notes)) throw new ApiException("يرجى إدخال ملاحظات القيد لتسهيل عملية مراجعة الحسابات");
        if (i.Notes.Length > 200) throw new ApiException("الملاحظات طويلة جداً (الحد الأقصى 200 حرف)");
        if (i.Date == default) throw new ApiException("يرجى اختيار تاريخ القيد");
        if (i.Lines.Any(l => l.Maden < 0 || l.Daen < 0)) throw new ApiException("لا يمكن إدخال مبالغ بالسالب");
        if (i.Lines.Any(l => l.Maden > 0 && l.Daen > 0)) throw new ApiException("السطر الواحد يكون مدين أو دائن فقط");

        var zeroAllowed = await c.ExecuteScalarAsync<bool?>("SELECT ChkAmount FROM dbo.User_Info WHERE Guid = @g", new { g = me.UserGuid }, tx) == true;
        var maden = Math.Round(i.Lines.Sum(l => l.Maden), 2);
        var daen = Math.Round(i.Lines.Sum(l => l.Daen), 2);
        if (i.Lines.Count == 0 || (maden == 0 && !zeroAllowed)) throw new ApiException("يرجى التأكد من المبالغ المدرجة في الجانب المدين");
        if (daen == 0 && !zeroAllowed) throw new ApiException("يرجى التأكد من المبالغ المدرجة في الجانب الدائن");
        if (maden != daen) throw new ApiException("القيد غير متزن، يرجى مراجعة تفاصيل القيد");

        var infos = (await c.QueryAsync<AccountInfo>("SELECT Guid, AccountType, Type_, IsUse FROM dbo.Account WHERE Guid IN @ids",
            new { ids = i.Lines.Select(l => l.AccountGuid).Distinct() }, tx)).ToDictionary(a => a.Guid);
        foreach (var l in i.Lines)
        {
            if (!infos.TryGetValue(l.AccountGuid, out var a) || a.AccountType == 2 || a.IsUse == false)
                throw new ApiException("أحد الحسابات المختارة غير صالح أو موقوف");
            if (a.Type_ is not (0 or 1)) l.CostGuid = Guid.Empty; // "الحساب غير مطلوب له مركز تكلفة"
            else if (l.CostGuid is null || l.CostGuid == Guid.Empty) throw new ApiException("لا يمكن الحفظ بدون اختيار مركز التكلفة");
            if (string.IsNullOrWhiteSpace(l.Notes)) throw new ApiException("لا يمكن الحفظ بدون إدخال البيان لتسهيل عملية المراجعة");
            if (l.Notes.Length > 200) throw new ApiException("البيان طويل جداً (الحد الأقصى 200 حرف)");
        }
    }

    private static async Task AddLinesAsync(DbConnection c, DbTransaction tx, Guid day1, JournalInput i)
    {
        foreach (var l in i.Lines)
            await c.ExecuteAsync("dbo.AddDay2", new
            {
                Day1Guid = day1, l.AccountGuid, Maden = (double)l.Maden, Daen = (double)l.Daen, Notes = l.Notes!.Trim(), CostGuid = l.CostGuid ?? Guid.Empty, IsUse = true,
            }, tx, commandType: CommandType.StoredProcedure);
    }

    private static DateTime Stamp(DateTime picked, DateTime? existing = null)
    {
        if (existing is { } e && e.Date == picked.Date) return e;
        var now = Clock.Now;
        return picked.Date == now.Date ? now : picked.Date;
    }

    public Task<JournalSaved> CreateAsync(JournalKind k, JournalInput i, CancellationToken ct) =>
        db.InTransactionAsync(async (c, tx) =>
        {
            await ValidateAsync(c, tx, i);
            i.Date = Stamp(i.Date);
            var typeGuid = await c.ExecuteScalarAsync<Guid>("dbo.FormTypeGuid", new { FormName = k.Form }, tx, commandType: CommandType.StoredProcedure);
            var p = new DynamicParameters(new
            {
                DayDate = i.Date, Notes = i.Notes.Trim(), DayType = false, DocGuid = Guid.Empty, i.IsUse, TypeGuid = typeGuid, BranchGuid = Guid.Empty,
            });
            if (!k.Opening) p.Add("OpenGuid", await OpenGuidAsync(c, tx));
            p.Add("CodeOut", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await c.ExecuteAsync("dbo." + k.AddProc, p, tx, commandType: CommandType.StoredProcedure);
            var code = p.Get<int>("CodeOut").ToString();
            var guid = await c.ExecuteScalarAsync<Guid>("SELECT TOP 1 Guid FROM dbo.Day1 WHERE CAST(Code AS nvarchar(50)) = @code AND TypeGuid = @typeGuid ORDER BY ID DESC", new { code, typeGuid }, tx);

            await AddLinesAsync(c, tx, guid, i);
            await audit.WriteAsync(c, tx, k.Form, guid, UserActionLog.New, code);
            return new JournalSaved($"تم حفظ {k.Title} بنجاح", code, guid);
        }, IsolationLevel.Serializable, ct);

    public async Task<JournalSaved> UpdateAsync(JournalKind k, Guid guid, JournalInput i, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب الإجراء أولاً");
        return await db.InTransactionAsync(async (c, tx) =>
        {
            var open = await OpenGuidAsync(c, tx);
            var cur = await c.QueryFirstOrDefaultAsync<(string Code, bool? DayType, DateTime? Date)>(
                $"SELECT CAST(Code AS nvarchar(50)), DayType, DayDate FROM dbo.Day1 WITH (UPDLOCK) WHERE Guid = @guid AND TypeGuid {(k.Opening ? "=" : "<>")} @open",
                new { guid, open }, tx);
            if (cur.Code is null) throw ApiException.NotFound($"{k.Title} غير موجود");
            if (cur.DayType == true) throw new ApiException("لا يمكن التعديل على قيد يومية مرتبط بمستند آخر");
            await ValidateAsync(c, tx, i);
            i.Date = Stamp(i.Date, cur.Date);

            await c.ExecuteAsync("dbo.UpdateDay1", new { Guid = guid, DayDate = i.Date, Notes = i.Notes.Trim(), i.IsUse, BranchGuid = Guid.Empty }, tx, commandType: CommandType.StoredProcedure);
            await c.ExecuteAsync("dbo.DeleteDay2", new { Day1Guid = guid }, tx, commandType: CommandType.StoredProcedure);
            await AddLinesAsync(c, tx, guid, i);
            await audit.WriteAsync(c, tx, k.Form, guid, UserActionLog.Update, cur.Code, i.Reason!.Trim());
            return new JournalSaved($"تم تعديل {k.Title} بنجاح", cur.Code, guid);
        }, ct: ct);
    }
}
