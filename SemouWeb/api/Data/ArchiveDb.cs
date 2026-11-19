using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Semou.Api.Data;

/// <summary>
/// The old system's archive database (S_SSTLI2024) on the same server, used by the reception desk
/// to find students from the previous system and read their old balances. Read-only from the web app.
/// </summary>
public sealed class ArchiveDb(IConfiguration config)
{
    private readonly string? _cs = config.GetConnectionString("Archive") is { Length: > 0 } cs
        ? new SqlConnectionStringBuilder(cs) { ApplicationIntent = ApplicationIntent.ReadOnly }.ConnectionString
        : null;

    public bool Configured => _cs != null;

    public async Task<DbConnection> OpenAsync(CancellationToken ct = default)
    {
        if (_cs is null) throw new InvalidOperationException("ConnectionStrings:Archive is not configured.");
        var c = new SqlConnection(_cs);
        await c.OpenAsync(ct);
        return c;
    }
}
