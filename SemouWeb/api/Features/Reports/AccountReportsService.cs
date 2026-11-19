using System.Data;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Reports;

/*
 * تقارير الحسابات العامة:
 *   كشف حساب (GeneralAccountStatment)  — BeforAccountStatmentBalance + GenralAccountStatment, running balance
 *   اليومية العامة (DailyHome)          — ShowDaily, grouped by journal
 *   ميزان المراجعة (TrailHome)          — same figures as LoadTrail, but the branch filter works (it was commented out
 *                                          in the proc) and parent accounts show the totals of their children
 *   قائمة الدخل (IncomeStatment)        — ShowIncomeStatment Type_ 0 revenues / 1 expenses
 *   حركة الخزينة - البنك (CashBoxHistory) — cashBoxHistory
 *   الأرصدة الافتتاحية (AccountOpenBalnceList) — LoadAccountOpenBalance
 */
public sealed class TrialRow
{
    public Guid Guid { get; set; }
    public Guid? ParentGuid { get; set; }
    public string Code { get; set; } = "";
    public string AccountName { get; set; } = "";
    public int AccountType { get; set; }
    public decimal PrevMaden { get; set; }
    public decimal PrevDaen { get; set; }
    public decimal TransMaden { get; set; }
    public decimal TransDaen { get; set; }
    public int Level { get; set; }
    /// <summary>The account's own figures (before adding its children).</summary>
    public decimal OwnPM, OwnPD, OwnTM, OwnTD;
    public bool HasChildren { get; set; }
}

public sealed class AccountReportsService(Db db)
{
    private static DateTime Day(DateTime? d, DateTime fallback) => (d ?? fallback).Date;
    private static decimal R(decimal v) => Math.Round(v, 2);

    public async Task<object> LookupsAsync(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return new
        {
            types = await c.QueryAsync(new CommandDefinition("SELECT F.Guid AS guid, F.Name AS name FROM dbo.Form_Name F WHERE EXISTS (SELECT 1 FROM dbo.Day1 D WHERE D.TypeGuid = F.Guid) ORDER BY F.Id", cancellationToken: ct)),
            cashBoxes = await c.QueryAsync(new CommandDefinition("SELECT AccountGuid AS guid, CashName AS name, CashType AS type FROM dbo.CashBox WHERE ISNULL(IsUse,1) = 1 ORDER BY CashType, TRY_CONVERT(int, Code)", cancellationToken: ct)),
            users = await c.QueryAsync(new CommandDefinition("SELECT Guid AS guid, FullName AS name FROM dbo.User_Info ORDER BY FullName", cancellationToken: ct)),
        };
    }

    /* ---------------- كشف حساب ---------------- */
    public async Task<object> StatementAsync(Guid account, DateTime? from, DateTime? to, Guid? branch, Guid? cost, CancellationToken ct)
    {
        var f = Day(from, new DateTime(Clock.Now.Year, 1, 1)); var t = Day(to, Clock.Now);
        if (f > t) throw new ApiException("تاريخ البداية بعد تاريخ النهاية");
        await using var c = await db.OpenAsync(ct);
        var acc = await c.QueryFirstOrDefaultAsync(new CommandDefinition("SELECT Guid, CAST(Code AS nvarchar(50)) AS Code, AccountName FROM dbo.Account WHERE Guid = @account", new { account }, cancellationToken: ct))
                  ?? throw ApiException.NotFound("الحساب غير موجود");
        var before = await c.QueryFirstAsync<(decimal BMaden, decimal BDaen)>(new CommandDefinition("""
            SELECT CAST(COALESCE(SUM(X.Maden),0) AS decimal(18,2)), CAST(COALESCE(SUM(X.Daen),0) AS decimal(18,2))
            FROM dbo.Day1 D JOIN dbo.Day2 X ON D.Guid = X.Day1Guid
            WHERE CONVERT(date, D.DayDate) < @f AND X.AccountGuid = @account AND (@branch IS NULL OR D.BranchGuid = @branch)
              AND (@cost IS NULL OR X.CostGuid = @cost) AND D.IsUse = 1
            """, new { f, account, branch, cost }, cancellationToken: ct));
        var rows = (await c.QueryAsync(new CommandDefinition("dbo.GenralAccountStatment",
            new { FDate = f, TDate = t, AccountGuid = account, BranchGuid = branch, CostGuid = cost }, commandType: CommandType.StoredProcedure, cancellationToken: ct)))
            .Cast<IDictionary<string, object?>>().ToList();
        decimal bal = before.BMaden - before.BDaen, tm = 0, td = 0;
        var lines = rows.Select(r =>
        {
            decimal m = R(Convert.ToDecimal(r["Maden"] ?? 0)), d = R(Convert.ToDecimal(r["Daen"] ?? 0));
            bal += m - d; tm += m; td += d;
            return new { journal = r["Code"]?.ToString(), date = r["DayDate"], type = r["Name"]?.ToString(), docNo = r["ActionCode"]?.ToString(), maden = m, daen = d, balance = R(bal), notes = r["Notes"]?.ToString(), costCenter = r["CenterName"]?.ToString(), branch = r["BrEName"]?.ToString() };
        }).ToList();
        return new
        {
            account = acc, from = f, to = t,
            opening = new { maden = before.BMaden, daen = before.BDaen, balance = R(before.BMaden - before.BDaen) },
            lines,
            totals = new { maden = R(tm), daen = R(td), balance = R(bal) },
        };
    }

    /* ---------------- اليومية العامة ---------------- */
    public async Task<object> JournalAsync(DateTime? from, DateTime? to, Guid? type, Guid? branch, Guid? cost, CancellationToken ct)
    {
        var f = Day(from, Clock.Now); var t = Day(to, Clock.Now);
        if (f > t) throw new ApiException("تاريخ البداية بعد تاريخ النهاية");
        if ((t - f).TotalDays > 400) throw new ApiException("الحد الأقصى لفترة اليومية العامة سنة واحدة");
        await using var c = await db.OpenAsync(ct);
        var rows = await c.QueryAsync(new CommandDefinition("dbo.ShowDaily", new { FDate = f, TDate = t, TypeGuid = type, BranchGuid = branch, CostGuid = cost },
            commandType: CommandType.StoredProcedure, cancellationToken: ct));
        var entries = rows.Cast<IDictionary<string, object?>>()
            .GroupBy(r => r["Code"]?.ToString() ?? "")
            .Select(g =>
            {
                var h = g.First();
                var lines = g.Select(r => new { account = r["AccountName"]?.ToString(), maden = R(Convert.ToDecimal(r["Maden"] ?? 0)), daen = R(Convert.ToDecimal(r["Daen"] ?? 0)), notes = r["Notes"]?.ToString(), costCenter = r["CenterName"]?.ToString() }).ToList();
                return new { code = g.Key, date = h["DayDate"], type = h["Name"]?.ToString(), docNo = h["ActionCode"]?.ToString(), maden = lines.Sum(x => x.maden), daen = lines.Sum(x => x.daen), lines };
            }).ToList();
        return new { from = f, to = t, entries, totals = new { maden = entries.Sum(e => e.maden), daen = entries.Sum(e => e.daen), count = entries.Count } };
    }

    /* ---------------- ميزان المراجعة ---------------- */
    public async Task<object> TrialAsync(DateTime? from, DateTime? to, Guid? branch, CancellationToken ct)
    {
        var f = Day(from, new DateTime(Clock.Now.Year, 1, 1)); var t = Day(to, Clock.Now);
        if (f > t) throw new ApiException("تاريخ البداية بعد تاريخ النهاية");
        await using var c = await db.OpenAsync(ct);
        var rows = (await c.QueryAsync<TrialRow>(new CommandDefinition("""
            WITH M AS (
                SELECT X.AccountGuid,
                       SUM(CASE WHEN CONVERT(date, D.DayDate) < @f THEN ISNULL(X.Maden,0) ELSE 0 END) AS PrevMaden,
                       SUM(CASE WHEN CONVERT(date, D.DayDate) < @f THEN ISNULL(X.Daen,0) ELSE 0 END) AS PrevDaen,
                       SUM(CASE WHEN CONVERT(date, D.DayDate) BETWEEN @f AND @t THEN ISNULL(X.Maden,0) ELSE 0 END) AS TransMaden,
                       SUM(CASE WHEN CONVERT(date, D.DayDate) BETWEEN @f AND @t THEN ISNULL(X.Daen,0) ELSE 0 END) AS TransDaen
                FROM dbo.Day2 X JOIN dbo.Day1 D ON D.Guid = X.Day1Guid
                WHERE D.IsUse = 1 AND CONVERT(date, D.DayDate) <= @t AND (@branch IS NULL OR D.BranchGuid = @branch)
                GROUP BY X.AccountGuid)
            SELECT A.Guid, NULLIF(A.ParentGuid, '00000000-0000-0000-0000-000000000000') AS ParentGuid, CAST(A.Code AS nvarchar(50)) AS Code, A.AccountName, ISNULL(A.AccountType,0) AS AccountType,
                   CAST(ISNULL(M.PrevMaden,0) AS decimal(18,2)) AS PrevMaden, CAST(ISNULL(M.PrevDaen,0) AS decimal(18,2)) AS PrevDaen,
                   CAST(ISNULL(M.TransMaden,0) AS decimal(18,2)) AS TransMaden, CAST(ISNULL(M.TransDaen,0) AS decimal(18,2)) AS TransDaen
            FROM dbo.Account A LEFT JOIN M ON M.AccountGuid = A.Guid
            """, new { f, t, branch }, cancellationToken: ct))).ToList();

        foreach (var r in rows) { r.OwnPM = r.PrevMaden; r.OwnPD = r.PrevDaen; r.OwnTM = r.TransMaden; r.OwnTD = r.TransDaen; }
        // Roll the figures up the tree so every parent shows the total of its children.
        var byGuid = rows.ToDictionary(r => r.Guid);
        var children = rows.Where(r => r.ParentGuid is { } p && byGuid.ContainsKey(p)).ToLookup(r => r.ParentGuid!.Value);
        void Roll(TrialRow r, int level)
        {
            r.Level = level;
            foreach (var ch in children[r.Guid])
            {
                Roll(ch, level + 1);
                r.PrevMaden += ch.PrevMaden; r.PrevDaen += ch.PrevDaen; r.TransMaden += ch.TransMaden; r.TransDaen += ch.TransDaen;
                r.HasChildren = true;
            }
        }
        var roots = rows.Where(r => r.ParentGuid is not { } p || !byGuid.ContainsKey(p)).ToList();
        foreach (var r in roots) Roll(r, 1);

        static (decimal M, decimal D) Side(decimal v) => v >= 0 ? (R(v), 0) : (0, R(-v));
        var ordered = new List<object>();
        void Walk(TrialRow r)
        {
            var (om, od) = Side(r.PrevMaden - r.PrevDaen);
            var (bm, bd) = Side(r.PrevMaden - r.PrevDaen + r.TransMaden - r.TransDaen);
            if (om != 0 || od != 0 || r.TransMaden != 0 || r.TransDaen != 0 || r.HasChildren)
                ordered.Add(new { r.Guid, parentGuid = r.ParentGuid is { } pg && byGuid.ContainsKey(pg) ? pg : (Guid?)null, r.Code, name = r.AccountName, r.Level, parent = r.HasChildren, openMaden = om, openDaen = od, transMaden = R(r.TransMaden), transDaen = R(r.TransDaen), balMaden = bm, balDaen = bd });
            foreach (var ch in children[r.Guid].OrderBy(x => x.Code, StringComparer.Ordinal)) Walk(ch);
        }
        foreach (var r in roots.OrderBy(x => x.Code, StringComparer.Ordinal)) Walk(r);

        // Totals: every account counted once with its OWN figures (a main account may also have direct entries).
        decimal Sum(Func<TrialRow, decimal> f) => R(rows.Sum(f));
        var openM = Sum(r => Math.Max(0, r.OwnPM - r.OwnPD)); var openD = Sum(r => Math.Max(0, r.OwnPD - r.OwnPM));
        var balM = Sum(r => Math.Max(0, r.OwnPM - r.OwnPD + r.OwnTM - r.OwnTD)); var balD = Sum(r => Math.Max(0, r.OwnPD - r.OwnPM + r.OwnTD - r.OwnTM));
        return new
        {
            from = f, to = t, rows = ordered,
            totals = new { openMaden = openM, openDaen = openD, transMaden = Sum(r => r.OwnTM), transDaen = Sum(r => r.OwnTD), balMaden = balM, balDaen = balD },
        };
    }

    /* ---------------- قائمة الدخل ---------------- */
    public async Task<object> IncomeAsync(DateTime? from, DateTime? to, Guid? cost, CancellationToken ct)
    {
        var f = Day(from, new DateTime(Clock.Now.Year, 1, 1)); var t = Day(to, Clock.Now);
        if (f > t) throw new ApiException("تاريخ البداية بعد تاريخ النهاية");
        await using var c = await db.OpenAsync(ct);
        async Task<List<(string Code, string Name, decimal Amount)>> Load(int type) =>
            (await c.QueryAsync(new CommandDefinition("dbo.ShowIncomeStatment", new { FDate = f, TDate = t, Type_ = type, CostGuid = cost }, commandType: CommandType.StoredProcedure, cancellationToken: ct)))
            .Cast<IDictionary<string, object?>>()
            .Select(r => (r["Code"]?.ToString() ?? "", r["AccountName"]?.ToString() ?? "", R(Convert.ToDecimal(r["TransMaden"] ?? 0) - Convert.ToDecimal(r["TransDaen"] ?? 0))))
            .Where(x => x.Item3 != 0).ToList();
        var revenues = (await Load(0)).Select(x => new { x.Code, x.Name, amount = R(-x.Amount) }).ToList();   // revenue: credit balance
        var expenses = (await Load(1)).Select(x => new { x.Code, x.Name, amount = x.Amount }).ToList();
        decimal ti = R(revenues.Sum(x => x.amount)), te = R(expenses.Sum(x => x.amount));
        return new { from = f, to = t, revenues, expenses, totals = new { revenues = ti, expenses = te, net = R(ti - te) } };
    }

    /* ---------------- حركة الخزينة / البنك ---------------- */
    public async Task<object> CashHistoryAsync(DateTime? from, DateTime? to, Guid? cash, Guid? branch, Guid? cost, Guid? type, Guid? user, CancellationToken ct)
    {
        var f = Day(from, Clock.Now); var t = Day(to, Clock.Now);
        if (f > t) throw new ApiException("تاريخ البداية بعد تاريخ النهاية");
        await using var c = await db.OpenAsync(ct);
        var rows = (await c.QueryAsync(new CommandDefinition("dbo.cashBoxHistory",
            new { FDate = f, TDate = t, AccountGuid = cash, BranchGuid = branch, CostGuid = cost, TypeGuid = type, UserGuid = user },
            commandType: CommandType.StoredProcedure, cancellationToken: ct, commandTimeout: 120)))
            .Cast<IDictionary<string, object?>>()
            .Select(r => new
            {
                docNo = r["ActionCode"]?.ToString(), date = r["DayDate"], type = r["Name"]?.ToString(), maden = R(Convert.ToDecimal(r["Maden"] ?? 0)), daen = R(Convert.ToDecimal(r["Daen"] ?? 0)),
                notes = r["Notes"]?.ToString(), student = r["StudentName"]?.ToString(), costCenter = r["CenterName"]?.ToString(), branch = r["BrEName"]?.ToString(),
                vendor = r["VendorName"]?.ToString(), reference = r["Ref"]?.ToString(), user = r["FullName"]?.ToString(), linkType = r["Expr1"]?.ToString(), linkRef = r["LinkRef"]?.ToString(), payDate = r["PayDate"],
            }).ToList();
        return new { from = f, to = t, rows, totals = new { @in = R(rows.Sum(x => x.maden)), @out = R(rows.Sum(x => x.daen)), net = R(rows.Sum(x => x.maden - x.daen)), count = rows.Count } };
    }

    /* ---------------- الأرصدة الافتتاحية ---------------- */
    public async Task<object> OpeningBalancesAsync(Guid branch, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var rows = (await c.QueryAsync(new CommandDefinition("dbo.LoadAccountOpenBalance", new { BranchGuid = branch }, commandType: CommandType.StoredProcedure, cancellationToken: ct)))
            .Cast<IDictionary<string, object?>>()
            .Select(r => new { code = r["Code"]?.ToString(), name = r["AccountName"]?.ToString(), maden = R(Convert.ToDecimal(r["OpenMaden"] ?? 0)), daen = R(Convert.ToDecimal(r["OpenDaen"] ?? 0)), notes = r["OpenNotes"]?.ToString() }).ToList();
        return new { rows, totals = new { maden = R(rows.Sum(r => r.maden)), daen = R(rows.Sum(r => r.daen)) } };
    }
}
