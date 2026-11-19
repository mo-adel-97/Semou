using System.Data.Common;
using Dapper;
using Microsoft.Extensions.Options;
using Semou.Api.Data;

namespace Semou.Api.Features.Website;

/*
 * Background link between the ERP and the alsmow.com website:
 *   - every PullSeconds  : pull new "سجلي الآن" requests  -> dbo.WebStudyRequest (المبيعات ← طلبات الدراسة)
 *   - every PushMinutes  : push every student's portal data (programs, finance, payments) -> بوابة المتدربات
 * Read-only on every desktop table; the only writes are to WebStudyRequest / WebSyncState.
 */

public sealed record SiteRequest(int Id, string Name, string? Phone, string? Email, string? NationalId, string? City,
    string? Program, string? StudyMode, string? Body, string CreatedAt);
public sealed record SiteRequestsPage(List<SiteRequest> Items, int LastId);

public sealed class WebsiteSyncService(Db db, WebsiteClient site, ILogger<WebsiteSyncService> log)
{
    public async Task<int> PullRequestsAsync(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var after = int.TryParse(await State(c, "requests.last_id", ct), out var a) ? a : 0;
        var total = 0;
        while (true)
        {
            var page = await site.GetAsync<SiteRequestsPage>("/api/erp/requests", $"after={after}&limit=200", ct);
            if (page.Items.Count == 0) break;
            var acks = new List<object>();
            foreach (var r in page.Items)
            {
                var nid = Digits(r.NationalId);
                var studentGuid = nid.Length >= 8
                    ? await c.ExecuteScalarAsync<Guid?>(new CommandDefinition("SELECT TOP 1 Guid FROM dbo.Student WHERE NationalId = @nid", new { nid }, cancellationToken: ct))
                    : null;
                var code = await c.ExecuteScalarAsync<int>(new CommandDefinition("""
                    IF NOT EXISTS (SELECT 1 FROM dbo.WebStudyRequest WHERE SiteId = @Id)
                    INSERT INTO dbo.WebStudyRequest (SiteId, RequestDate, ReceivedAt, StudentName, Phone, Email, NationalId, City, Program, StudyMode, Notes, StudentGuid)
                    VALUES (@Id, @CreatedAt, @Now, @Name, @Phone, @Email, @Nid, @City, @Program, @Mode, @Body, @StudentGuid);
                    SELECT ID FROM dbo.WebStudyRequest WHERE SiteId = @Id;
                    """, new
                {
                    r.Id, CreatedAt = ParseSiteTime(r.CreatedAt), Name = Cut(r.Name, 120) ?? "—", Phone = Cut(r.Phone, 30), Email = Cut(r.Email, 190), Nid = nid.Length > 0 ? nid : null,
                    Now = Semou.Api.Infrastructure.Clock.Now, City = Cut(r.City, 80), Program = Cut(r.Program, 220), Mode = r.StudyMode switch { "onsite" => 1, "online" => 2, _ => 0 }, r.Body, StudentGuid = studentGuid,
                }, cancellationToken: ct));
                acks.Add(new { id = r.Id, @ref = $"WSR-{code}" });
            }
            await site.PostAsync<object>("/api/erp/requests/ack", new { items = acks }, ct);
            after = page.LastId;
            await SetState(c, "requests.last_id", after.ToString(), ct);
            total += page.Items.Count;
            if (page.Items.Count < 200) break;
        }
        if (total > 0) log.LogInformation("Website: pulled {Count} study requests", total);
        return total;
    }

    public async Task<int> PushStudentsAsync(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var students = (await c.QueryAsync<(Guid Guid, Guid AccountGuid, string? Code, string StudentName, string NationalId, string StudentTel, bool Active, string? Branch)>(
            new CommandDefinition("""
                SELECT S.Guid, S.AccountGuid, CAST(S.Code AS nvarchar(40)) AS Code, S.StudentName, S.NationalId, S.StudentTel,
                       CAST(ISNULL(S.IsUse,1) AS bit) AS Active, B.BrName AS Branch
                FROM dbo.Student S LEFT JOIN dbo.Branch B ON B.Guid = S.BarnchGuid
                WHERE LEN(S.NationalId) >= 8 AND LEN(S.StudentTel) >= 9 AND S.AccountGuid IS NOT NULL
                """, cancellationToken: ct))).ToList();

        // One set-based query per kind of data (not one call per student).
        var programs = (await c.QueryAsync<(Guid Account, string? Diplom, string? Course, string? LevelName, string? BatchName, string? Branch, string? CaseName, DateTime? Start, string? RegCode, int RegType)>(
            new CommandDefinition("""
                SELECT L.StudentGuid AS Account, D.DiplomName AS Diplom, CO.CourseName AS Course, LV.LevelName, BT.BatchName, SB.BrName AS Branch,
                       ST.StautName AS CaseName, COALESCE(TRY_CONVERT(date, CO.DateStart), TRY_CONVERT(date, R.RegDate)) AS Start, CAST(R.Code AS nvarchar(40)) AS RegCode, ISNULL(L.RegType,0) AS RegType
                FROM dbo.StudentLevel L
                LEFT JOIN dbo.Diplom D ON D.Guid = L.DiplomGuid LEFT JOIN dbo.Course CO ON CO.Guid = L.CourseGuid
                LEFT JOIN dbo.Levels LV ON LV.Guid = L.LevelGuid LEFT JOIN dbo.Batch BT ON BT.Guid = L.BatchGuid
                LEFT JOIN dbo.Branch SB ON SB.Guid = L.BranchGuid LEFT JOIN dbo.Stauts ST ON ST.Guid = L.CaseGuid
                LEFT JOIN dbo.RegDoc R ON R.Guid = L.RegDocGuid
                WHERE ISNULL(L.IsUse,1) = 1
                """, cancellationToken: ct))).ToLookup(p => p.Account);

        // Same rules as كشف حساب الطالب: salesinvoice / repay lines are not counted (the registration / pay journals carry the amount).
        var finance = (await c.QueryAsync<(Guid Account, string Kind, decimal Maden, decimal Daen)>(
            new CommandDefinition("""
                SELECT D2.AccountGuid AS Account, F.UnqName AS Kind, CAST(SUM(ISNULL(D2.Maden,0)) AS decimal(18,2)) AS Maden, CAST(SUM(ISNULL(D2.Daen,0)) AS decimal(18,2)) AS Daen
                FROM dbo.Day2 D2 JOIN dbo.Day1 D1 ON D1.Guid = D2.Day1Guid JOIN dbo.Form_Name F ON F.Guid = D1.TypeGuid
                WHERE D1.IsUse = 1 AND F.UnqName NOT IN ('salesinvoice','repay') AND D2.AccountGuid IN (SELECT AccountGuid FROM dbo.Student)
                GROUP BY D2.AccountGuid, F.UnqName
                """, cancellationToken: ct))).ToLookup(f => f.Account);

        var payments = (await c.QueryAsync<(Guid Account, DateTime Date, decimal Amount, string? Ref, string? Notes)>(
            new CommandDefinition("""
                SELECT D2.AccountGuid AS Account, D1.DayDate AS Date, CAST(D2.Daen AS decimal(18,2)) AS Amount, CAST(D1.ActionCode AS nvarchar(40)) AS Ref, D2.Notes
                FROM dbo.Day2 D2 JOIN dbo.Day1 D1 ON D1.Guid = D2.Day1Guid JOIN dbo.Form_Name F ON F.Guid = D1.TypeGuid
                WHERE D1.IsUse = 1 AND F.UnqName = 'receipt' AND ISNULL(D2.Daen,0) > 0 AND D2.AccountGuid IN (SELECT AccountGuid FROM dbo.Student)
                """, cancellationToken: ct))).ToLookup(p => p.Account);

        var payload = students.Select(s =>
        {
            var f = finance[s.AccountGuid].ToList();
            var total = f.Sum(x => x.Maden);
            var paid = f.Where(x => x.Kind == "receipt").Sum(x => x.Daen);
            var otherCredits = f.Where(x => x.Kind != "receipt").Sum(x => x.Daen); // discounts / adjustments
            return new
            {
                guid = s.Guid,
                national_id = s.NationalId.Trim(),
                phone = s.StudentTel.Trim(),
                name = s.StudentName.Trim(),
                code = s.Code,
                active = s.Active,
                branch = s.Branch,
                programs = programs[s.AccountGuid].Select(p => new
                {
                    title = p.RegType == 0 ? (p.Diplom ?? p.Course) : (p.Course ?? p.Diplom),
                    kind = p.RegType == 0 ? "diploma" : "course",
                    level = p.LevelName, batch = p.BatchName, branch = p.Branch, status = p.CaseName,
                    start_date = p.Start?.ToString("yyyy-MM-dd"), reg_code = p.RegCode,
                }),
                finance = new { total, discount = otherCredits, paid, balance = total - paid - otherCredits },
                payments = payments[s.AccountGuid].OrderByDescending(p => p.Date).Take(100).Select(p => new
                {
                    date = p.Date.ToString("yyyy-MM-dd"), amount = p.Amount, @ref = p.Ref, method = "سند قبض", note = p.Notes,
                }),
            };
        }).ToList();

        // Small batches, then one light call listing every current student so removed ones lose portal access.
        const int batch = 100;
        for (var i = 0; i < payload.Count; i += batch)
            await site.PostAsync<object>("/api/erp/students", new { students = payload.Skip(i).Take(batch) }, ct);
        await site.PostAsync<object>("/api/erp/students/prune", new { guids = payload.Select(p => p.guid) }, ct);
        await SetState(c, "students.last_push", Semou.Api.Infrastructure.Clock.Now.ToString("s"), ct);
        log.LogInformation("Website: pushed {Count} students to the trainee portal", payload.Count);
        return payload.Count;
    }

    private static async Task<string?> State(DbConnection c, string name, CancellationToken ct) =>
        await c.ExecuteScalarAsync<string?>(new CommandDefinition("SELECT Value FROM dbo.WebSyncState WHERE Name = @name", new { name }, cancellationToken: ct));

    private static Task SetState(DbConnection c, string name, string value, CancellationToken ct) =>
        c.ExecuteAsync(new CommandDefinition("""
            UPDATE dbo.WebSyncState SET Value = @value, UpdatedAt = @now WHERE Name = @name;
            IF @@ROWCOUNT = 0 INSERT INTO dbo.WebSyncState (Name, Value, UpdatedAt) VALUES (@name, @value, @now);
            """, new { name, value, now = Semou.Api.Infrastructure.Clock.Now }, cancellationToken: ct));

    /// <summary>The site sends "yyyy-MM-dd HH:mm:ss" (KSA time).</summary>
    private static DateTime ParseSiteTime(string s) =>
        DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var d) ? d : DateTime.Now;

    private static string Digits(string? s) => new((s ?? "").Where(char.IsAsciiDigit).ToArray());
    private static string? Cut(string? s, int max) => string.IsNullOrWhiteSpace(s) ? null : s.Trim().Length > max ? s.Trim()[..max] : s.Trim();
}

/// <summary>Runs the sync on a timer inside the ERP web app (it lives on the ERP server, 24/7).</summary>
public sealed class WebsiteSyncWorker(IServiceScopeFactory scopes, IOptions<WebsiteOptions> options, WebsiteClient site, ILogger<WebsiteSyncWorker> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stop)
    {
        if (!site.Configured) { log.LogInformation("Website sync disabled (Website:Enabled / BaseUrl / Secret not set)"); return; }
        var lastPush = DateTime.MinValue;
        while (!stop.IsCancellationRequested)
        {
            using (var scope = scopes.CreateScope())
            {
                var sync = scope.ServiceProvider.GetRequiredService<WebsiteSyncService>();
                try { await sync.PullRequestsAsync(stop); }
                catch (Exception e) when (e is not OperationCanceledException) { log.LogWarning(e, "Website: pulling study requests failed"); }

                if (DateTime.UtcNow - lastPush >= TimeSpan.FromMinutes(Math.Max(1, options.Value.PushMinutes)))
                {
                    try { await sync.PushStudentsAsync(stop); lastPush = DateTime.UtcNow; }
                    catch (Exception e) when (e is not OperationCanceledException) { log.LogWarning(e, "Website: pushing portal data failed"); lastPush = DateTime.UtcNow.AddMinutes(-Math.Max(1, options.Value.PushMinutes) + 2); }
                }
            }
            try { await Task.Delay(TimeSpan.FromSeconds(Math.Max(15, options.Value.PullSeconds)), stop); }
            catch (OperationCanceledException) { break; }
        }
    }
}
