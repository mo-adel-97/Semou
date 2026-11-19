using System.Data;
using System.Data.Common;
using Dapper;
using Semou.Api.Data;

namespace Semou.Api.Features.Branches;

/// <summary>Data access for dbo.Branch. Uses the same stored procedures as the desktop app wherever they exist.</summary>
public sealed class BranchRepository(Db db)
{
    public async Task<IEnumerable<BranchListItem>> SearchAsync(string? name, bool? isUse, bool? isRegOn, CancellationToken ct)
    {
        // Same filters as proc LoadBranch, but returns the Arabic name and real booleans for the grid.
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync<BranchListItem>(new CommandDefinition(
            """
            SELECT B.Code, B.Guid, B.BrName, B.BrEName, B.IsUse, B.IsRegOn, B.BranchQCode, B.ClassTime, B.BrTel, B.BrVatId
            FROM dbo.Branch B
            WHERE (B.BrName LIKE '%' + @name + '%' OR B.BrEName LIKE '%' + @name + '%' OR B.Code = @name OR @name IS NULL)
              AND (B.IsUse = @isUse OR @isUse IS NULL)
              AND (B.IsRegOn = @isRegOn OR @isRegOn IS NULL)
            ORDER BY TRY_CONVERT(int, B.Code)
            """, new { name, isUse, isRegOn }, cancellationToken: ct));
    }

    public async Task<BranchDetails?> GetByCodeAsync(string code, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var b = await c.QueryFirstOrDefaultAsync<BranchDetails>(new CommandDefinition(
            "dbo.ShowBarnchInfo", new { Code = code }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        if (b != null) b.Code = code;
        return b;
    }

    /// <summary>Next ID (= Code) like GET___NO, locked so two users saving at once can't get the same number.</summary>
    public Task<int> NextIdAsync(DbConnection c, DbTransaction tx) =>
        c.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(ID), 0) + 1 FROM dbo.Branch WITH (UPDLOCK, HOLDLOCK)", transaction: tx);

    public Task<string?> GetCodeAsync(DbConnection c, DbTransaction tx, Guid guid) =>
        c.ExecuteScalarAsync<string?>("SELECT Code FROM dbo.Branch WHERE Guid = @guid", new { guid }, tx);

    public Task<Guid> GetGuidAsync(DbConnection c, DbTransaction tx, string code) =>
        c.ExecuteScalarAsync<Guid>("SELECT Guid FROM dbo.Branch WHERE Code = @code", new { code }, tx);

    public Task InsertAsync(DbConnection c, DbTransaction tx, int id, string code, BranchInput b)
    {
        var p = Params(b);
        p.Add("ID", id);
        p.Add("Code", code);
        return c.ExecuteAsync("dbo.AddBranch", p, tx, commandType: CommandType.StoredProcedure);
    }

    public Task UpdateAsync(DbConnection c, DbTransaction tx, Guid guid, BranchInput b)
    {
        var p = Params(b);
        p.Add("Guid", guid);
        return c.ExecuteAsync("dbo.UpdateBranch", p, tx, commandType: CommandType.StoredProcedure);
    }

    private static DynamicParameters Params(BranchInput b) => new(new
    {
        BrName = b.BrName.Trim(),
        BrEName = b.BrEName ?? "",
        BrAddress = b.BrAddress ?? "",
        BrWebSite = b.BrWebSite ?? "",
        BrEmail = b.BrEmail ?? "",
        BrTel = b.BrTel ?? "",
        BrFax = b.BrFax ?? "",
        BrVatId = b.BrVatId.Trim(),
        b.BrVatSMain,
        b.IsUse,
        b.CostCenterGuid,
        BranchQCode = b.BranchQCode ?? "",
        VatName = b.VatName.Trim(),
        b.IsRegOn
    });
}
