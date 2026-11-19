using System.Collections.Concurrent;
using Dapper;
using Semou.Api.Data;

namespace Semou.Api.Features.Screens;

public sealed record ProcParam(string Name, string SqlType, bool IsOutput)
{
    /// <summary>Name without the leading @.</summary>
    public string Bare => Name.TrimStart('@');
}

/// <summary>
/// Reads stored-procedure parameters from SQL Server (sys.parameters) once and caches them,
/// so the engine always calls the desktop procs with exactly the parameters they declare.
/// </summary>
public sealed class ProcCatalog(Db db)
{
    private readonly ConcurrentDictionary<string, ProcParam[]> _cache = new(StringComparer.OrdinalIgnoreCase);

    public async Task<ProcParam[]> GetAsync(string proc)
    {
        if (_cache.TryGetValue(proc, out var hit)) return hit;
        await using var c = await db.OpenAsync();
        var rows = (await c.QueryAsync<ProcParam>(
            """
            SELECT p.name AS Name, TYPE_NAME(p.user_type_id) AS SqlType, p.is_output AS IsOutput
            FROM sys.parameters p WHERE p.object_id = OBJECT_ID(@name) ORDER BY p.parameter_id
            """, new { name = "dbo." + proc })).ToArray();
        if (rows.Length == 0)
        {
            var exists = await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM sys.procedures WHERE object_id = OBJECT_ID(@name)", new { name = "dbo." + proc });
            if (exists == 0) throw new InvalidOperationException($"Stored procedure dbo.{proc} was not found.");
        }
        return _cache[proc] = rows;
    }
}
