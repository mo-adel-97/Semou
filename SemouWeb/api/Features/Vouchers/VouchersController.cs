using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Vouchers;

[ApiController]
[Authorize]
[Route("api/vouchers")]
public sealed class VouchersController(VoucherService vouchers, PermissionService perms, CurrentUser me, Db db) : ControllerBase
{
    private static VoucherKind Kind(string kind) => kind switch
    {
        "receipt" => VoucherService.Receipt,
        "payment" => VoucherService.Payment,
        _ => throw ApiException.NotFound("غير موجود"),
    };

    private async Task<VoucherKind> Authorize(string kind, Perm p, string msg)
    {
        var k = Kind(kind);
        if (!(await perms.GetAsync(me.UserGuid, k.Form)).Has(p)) throw ApiException.Forbidden(msg);
        return k;
    }

    [HttpGet("{kind}")]
    public async Task<IActionResult> List(string kind, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
        => Ok(await vouchers.ListAsync(await Authorize(kind, Perm.Find, "لا تملك صلاحية استعراض البيانات"), from, to, ct));

    [HttpGet("{kind}/{guid:guid}")]
    public async Task<IActionResult> Get(string kind, Guid guid, CancellationToken ct)
        => Ok(await vouchers.GetAsync(await Authorize(kind, Perm.Find, "لا تملك صلاحية استعراض البيانات"), guid, ct));

    [HttpPost("{kind}")]
    public async Task<VoucherSaved> Create(string kind, VoucherInput input, CancellationToken ct)
        => await vouchers.CreateAsync(await Authorize(kind, Perm.Add, "لا تملك صلاحية إضافة جديد"), input, ct);

    [HttpPut("{kind}/{guid:guid}")]
    public async Task<VoucherSaved> Update(string kind, Guid guid, VoucherInput input, CancellationToken ct)
        => await vouchers.UpdateAsync(await Authorize(kind, Perm.Edit, "لا تملك صلاحية تعديل البيانات"), guid, input, ct);

    /* ---------------- Pick-lists used by the voucher screens ---------------- */

    /// <summary>CashBoxList: active cash boxes and banks (the voucher's own side).</summary>
    [HttpGet("lookups/cash-accounts")]
    public async Task<IActionResult> CashAccounts(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return Ok(await c.QueryAsync(new CommandDefinition("""
            SELECT CB.AccountGuid AS Guid, CB.CashName AS Name, CB.CashType, CB.CostCenterGuid, CC.CenterName AS CostName
            FROM dbo.CashBox CB LEFT JOIN dbo.CostCenter CC ON CC.Guid = CB.CostCenterGuid
            WHERE CB.IsUse = 1 AND CB.AccountGuid IS NOT NULL ORDER BY CB.CashType, TRY_CONVERT(int, CB.Code)
            """, cancellationToken: ct)));
    }

    /// <summary>AccountList with AccountType2 = 2: active accounts except the roots. Searched by code or name (top 50).</summary>
    [HttpGet("lookups/accounts")]
    public async Task<IActionResult> Accounts([FromQuery] string? q, CancellationToken ct)
    {
        q = string.IsNullOrWhiteSpace(q) ? null : q.Trim();
        await using var c = await db.OpenAsync(ct);
        return Ok(await c.QueryAsync(new CommandDefinition("""
            SELECT TOP 50 A.Guid, CAST(A.Code AS nvarchar(50)) AS Code, A.AccountName AS Name, A.Type_ AS AccountType, A.AccountType AS Level
            FROM dbo.Account A
            WHERE ISNULL(A.IsUse, 1) = 1 AND ISNULL(A.AccountType, 0) <> 2
              AND (@q IS NULL OR A.AccountName LIKE '%' + @q + '%' OR CAST(A.Code AS nvarchar(50)) LIKE @q + '%')
            ORDER BY CASE WHEN A.AccountType = 1 THEN 0 ELSE 1 END, A.BacicCode
            """, new { q }, cancellationToken: ct)));
    }

    /// <summary>BranchCostList (payment voucher): the cost centers linked to branches.</summary>
    [HttpGet("lookups/branch-cost-centers")]
    public async Task<IActionResult> BranchCostCenters(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return Ok(await c.QueryAsync(new CommandDefinition("dbo.ShowBranchCostCenter", commandType: System.Data.CommandType.StoredProcedure, cancellationToken: ct)));
    }

    /// <summary>Amount in Arabic words (proc Tafkeet / dbo.FNTafkeet).</summary>
    [HttpGet("lookups/tafkeet")]
    public async Task<IActionResult> Tafkeet([FromQuery] decimal amount, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return Ok(new { text = await c.ExecuteScalarAsync<string>(new CommandDefinition("SELECT dbo.FNTafkeet(@a)", new { a = (double)amount }, cancellationToken: ct)) });
    }

    /// <summary>The user's default branch and zero-amount right (ChkDefBranch).</summary>
    [HttpGet("lookups/defaults")]
    public async Task<IActionResult> Defaults(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return Ok(await c.QueryFirstOrDefaultAsync(new CommandDefinition("""
            SELECT CAST(ISNULL(U.ChkBranch,0) AS bit) AS ChkBranch, NULLIF(U.BranchGuid, '00000000-0000-0000-0000-000000000000') AS BranchGuid,
                   CAST(ISNULL(U.ChkAmount,0) AS bit) AS ZeroAmount
            FROM dbo.User_Info U WHERE U.Guid = @g
            """, new { g = me.UserGuid }, cancellationToken: ct)));
    }
}
