using System.Data;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Data;

namespace Semou.Api.Features.Lookups;

public sealed record LookupItem(string Code, string Name, Guid Guid);

/// <summary>Pick-lists shared by many screens (the desktop *List dialogs).</summary>
[ApiController]
[Authorize]
[Route("api/lookups")]
public sealed class LookupsController(Db db) : ControllerBase
{
    private sealed record CostCenterRow(string Code, string CenterName, string STAUT_, Guid Guid, Guid? ParentGuid);

    /// <summary>CostCenterList dialog: proc CostCenterList. The branch screen passes accountType = 1.</summary>
    [HttpGet("cost-centers")]
    public async Task<IEnumerable<LookupItem>> CostCenters([FromQuery] string? q, [FromQuery] bool? isUse,
        [FromQuery] int? accountType, [FromQuery] int? accountType2, CancellationToken ct)
    {
        q = string.IsNullOrWhiteSpace(q) ? null : q.Trim();
        var byCode = q != null && q.All(char.IsDigit);
        await using var c = await db.OpenAsync(ct);
        var rows = await c.QueryAsync<CostCenterRow>(new CommandDefinition("dbo.CostCenterList", new
        {
            CenterName = byCode ? null : q,
            IsUse = isUse,
            AccountType = accountType,
            AccountType2 = accountType2,
            Code = byCode ? q : null
        }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.Select(r => new LookupItem(r.Code, r.CenterName, r.Guid));
    }

    public sealed record BranchLookup(string Code, string Name, Guid Guid, bool IsUse);

    /// <summary>Branches for drop-downs and branch sub-grids (LoadBranch). Inactive ones are flagged so old records still show them.</summary>
    [HttpGet("branches")]
    public async Task<IEnumerable<BranchLookup>> Branches(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync<BranchLookup>(new CommandDefinition(
            "SELECT Code, ISNULL(BrName, BrEName) AS Name, Guid, ISNULL(IsUse, 0) AS IsUse FROM dbo.Branch ORDER BY TRY_CONVERT(int, Code)",
            cancellationToken: ct));
    }
}
