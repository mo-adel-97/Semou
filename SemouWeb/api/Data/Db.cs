using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Semou.Api.Data;

/// <summary>
/// Connection factory for the existing Semou SQL Server database (the same DB the desktop app uses).
/// Every repository goes through here so connection handling and transactions stay in one place.
/// </summary>
public sealed class Db(IConfiguration config)
{
    private readonly string _cs = config.GetConnectionString("Semou") is { Length: > 0 } cs
        ? cs
        : throw new InvalidOperationException("ConnectionStrings:Semou is not configured (use dotnet user-secrets or an environment variable).");

    public async Task<DbConnection> OpenAsync(CancellationToken ct = default)
    {
        var c = new SqlConnection(_cs);
        await c.OpenAsync(ct);
        return c;
    }

    /// <summary>
    /// Runs <paramref name="work"/> in one transaction: commits on success, rolls back on any exception.
    /// Serializable transactions are the ones that post documents (forms, invoices, receipts, journals): the desktop
    /// procedures number them with MAX(ID)+1 and the tables have no unique index on the number, so these web
    /// transactions take one exclusive application lock and run one after another — two users posting at the same
    /// moment can never get the same number. A deadlock with a desktop user is retried (the whole work is rolled back first).
    /// </summary>
    public async Task<T> InTransactionAsync<T>(Func<DbConnection, DbTransaction, Task<T>> work,
        IsolationLevel isolation = IsolationLevel.ReadCommitted, CancellationToken ct = default)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await using var c = await OpenAsync(ct);
                await using var tx = await c.BeginTransactionAsync(isolation, ct);
                if (isolation == IsolationLevel.Serializable) await PostingLockAsync(c, tx, ct);
                var result = await work(c, tx);
                await tx.CommitAsync(ct);
                return result;
            }
            catch (SqlException ex) when (ex.Number == 1205 && attempt < 3) // deadlock victim: nothing was kept, try again
            {
                await Task.Delay(150 * attempt, ct);
            }
        }
    }

    private static async Task PostingLockAsync(DbConnection c, DbTransaction tx, CancellationToken ct)
    {
        await using var cmd = c.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = "DECLARE @r int; EXEC @r = sp_getapplock @Resource = N'semou-web-posting', @LockMode = N'Exclusive', @LockOwner = N'Transaction', @LockTimeout = 60000; SELECT @r;";
        if (Convert.ToInt32(await cmd.ExecuteScalarAsync(ct)) < 0)
            throw new Infrastructure.ApiException("النظام مشغول بحفظ مستندات أخرى الآن، يرجى المحاولة بعد لحظات", 503);
    }
}
