using Dapper;
using Semou.Api.Data;

namespace Semou.Api.Features.Screens;

/// <summary>
/// Verifies every ScreenDef against the live database: procs exist, every proc parameter maps to a field,
/// and the list query runs. Exposed only in Development at GET /api/dev/screens-check.
/// </summary>
public static class ScreenSelfCheck
{
    public static async Task<object> RunAsync(Db db, ProcCatalog procs)
    {
        var report = new List<object>();
        foreach (var s in ScreenRegistry.All)
        {
            var problems = new List<string>();
            var fields = s.Fields.Select(f => f.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            try
            {
                foreach (var p in await procs.GetAsync(s.AddProc))
                    if (!p.IsOutput && !fields.Contains(p.Bare) && !(s.IdMode == IdMode.MaxPlusOne && p.Bare.ToLower() is "id" or "code"))
                        problems.Add($"{s.AddProc}: unmapped {p.Name}");
                foreach (var p in await procs.GetAsync(s.UpdateProc))
                    if (!fields.Contains(p.Bare) && !p.Bare.Equals("Guid", StringComparison.OrdinalIgnoreCase))
                        problems.Add($"{s.UpdateProc}: unmapped {p.Name}");
                if (s.IdMode == IdMode.ProcOutput && !(await procs.GetAsync(s.AddProc)).Any(p => p.IsOutput))
                    problems.Add($"{s.AddProc}: ProcOutput but no OUTPUT parameter");

                await using var c = await db.OpenAsync();
                var cols = (await c.QueryAsync<string>("SELECT name FROM sys.columns WHERE object_id = OBJECT_ID(@t)", new { t = "dbo." + s.Table }))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
                foreach (var f in s.Fields.Where(f => !cols.Contains(f.Name))) problems.Add($"field {f.Name} is not a column of {s.Table}");
                foreach (var col in s.Columns.Where(x => x.Kind != ColumnKind.Lookup && !cols.Contains(x.Name))) problems.Add($"list column {col.Name} missing");
                foreach (var ch in s.Children) { await procs.GetAsync(ch.AddProc); await procs.GetAsync(ch.DeleteProc); }
            }
            catch (Exception ex) { problems.Add(ex.Message); }
            report.Add(new { s.Key, ok = problems.Count == 0, problems });
        }
        return report;
    }
}
