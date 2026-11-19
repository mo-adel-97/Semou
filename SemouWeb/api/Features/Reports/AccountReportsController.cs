using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Reports;

/// <summary>Each report needs the View permission of its desktop screen (statment, dailyhome, mezan, instatment, cashhistory, openbalance).</summary>
[ApiController]
[Authorize]
[Route("api/reports/accounts")]
public sealed class AccountReportsController(AccountReportsService reports, PermissionService perms, CurrentUser me) : ControllerBase
{
    private async Task Need(params string[] forms)
    {
        foreach (var f in forms) if ((await perms.GetAsync(me.UserGuid, f)).Has(Perm.View)) return;
        throw new ApiException(RequirePermissionAttribute.Message(Perm.View), 403);
    }

    [HttpGet("lookups")]
    public async Task<IActionResult> Lookups(CancellationToken ct) { await Need("statment", "dailyhome", "cashhistory", "mezan", "instatment", "openbalance"); return Ok(await reports.LookupsAsync(ct)); }

    [HttpGet("statement")]
    public async Task<IActionResult> Statement([FromQuery] Guid account, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? branch, [FromQuery] Guid? cost, CancellationToken ct)
    { await Need("statment"); return Ok(await reports.StatementAsync(account, from, to, branch, cost, ct)); }

    [HttpGet("journal")]
    public async Task<IActionResult> Journal([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? type, [FromQuery] Guid? branch, [FromQuery] Guid? cost, CancellationToken ct)
    { await Need("dailyhome"); return Ok(await reports.JournalAsync(from, to, type, branch, cost, ct)); }

    [HttpGet("trial")]
    public async Task<IActionResult> Trial([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? branch, CancellationToken ct)
    { await Need("mezan"); return Ok(await reports.TrialAsync(from, to, branch, ct)); }

    [HttpGet("income")]
    public async Task<IActionResult> Income([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? cost, CancellationToken ct)
    { await Need("instatment"); return Ok(await reports.IncomeAsync(from, to, cost, ct)); }

    [HttpGet("cash")]
    public async Task<IActionResult> Cash([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? cash, [FromQuery] Guid? branch, [FromQuery] Guid? cost, [FromQuery] Guid? type, [FromQuery] Guid? user, CancellationToken ct)
    { await Need("cashhistory"); return Ok(await reports.CashHistoryAsync(from, to, cash, branch, cost, type, user, ct)); }

    [HttpGet("opening")]
    public async Task<IActionResult> Opening([FromQuery] Guid branch, CancellationToken ct)
    { await Need("openbalance"); return Ok(await reports.OpeningBalancesAsync(branch, ct)); }
}
