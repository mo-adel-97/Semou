using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Reports;

/// <summary>
/// قائمة التقارير. Each report needs the View permission of its desktop menu item
/// (reviewdesk, salesreport, taxreport, salesmanreport, custbalance, customerbalance, expenseanalysis).
/// The tax-returns report had no permission on the desktop; it is guarded by taxreport here.
/// </summary>
[ApiController]
[Authorize]
[Route("api/reports/sales")]
public sealed class SalesReportsController(SalesReportsService reports, PermissionService perms, CurrentUser me) : ControllerBase
{
    private async Task Need(params string[] forms)
    {
        foreach (var f in forms) if ((await perms.GetAsync(me.UserGuid, f)).Has(Perm.View)) return;
        throw new ApiException(RequirePermissionAttribute.Message(Perm.View), 403);
    }

    [HttpGet("lookups")]
    public async Task<IActionResult> Lookups(CancellationToken ct)
    { await Need("reviewdesk", "salesreport", "taxreport", "salesmanreport", "custbalance", "customerbalance", "expenseanalysis"); return Ok(await reports.LookupsAsync(ct)); }

    [HttpGet("registrations")]
    public async Task<IActionResult> Registrations([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? branch, [FromQuery] Guid? seller, [FromQuery] Guid? batch, [FromQuery] int? regType, [FromQuery] Guid? program, [FromQuery] string? q, CancellationToken ct)
    { await Need("reviewdesk"); return Ok(await reports.RegistrationsAsync(from, to, branch, seller, batch, regType, program, q, ct)); }

    [HttpGet("invoices")]
    public async Task<IActionResult> Invoices([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? branch, [FromQuery] Guid? user, [FromQuery] Guid? book, [FromQuery] bool? active, [FromQuery] string? kinds, [FromQuery] string? q, CancellationToken ct)
    { await Need("salesreport"); return Ok(await reports.SalesAsync(from, to, branch, user, book, active, kinds, q, ct)); }

    [HttpGet("tax")]
    public async Task<IActionResult> Tax([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? branch, [FromQuery] bool? active, [FromQuery] string? q, CancellationToken ct)
    { await Need("taxreport"); return Ok(await reports.TaxAsync(from, to, branch, active, q, ct)); }

    [HttpGet("tax-returns")]
    public async Task<IActionResult> TaxReturns([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? branch, [FromQuery] bool? active, [FromQuery] string? q, CancellationToken ct)
    { await Need("taxreport"); return Ok(await reports.TaxReturnsAsync(from, to, branch, active, q, ct)); }

    [HttpGet("sellers")]
    public async Task<IActionResult> Sellers([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? branch, [FromQuery] Guid? seller, [FromQuery] int? regType, CancellationToken ct)
    { await Need("salesmanreport"); return Ok(await reports.SellersAsync(from, to, branch, seller, regType, ct)); }

    [HttpGet("payment-follow")]
    public async Task<IActionResult> PaymentFollow([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? branch, [FromQuery] Guid? diplom, [FromQuery] Guid? batch, [FromQuery] string? q, CancellationToken ct)
    { await Need("custbalance"); return Ok(await reports.BalancesAsync(from, to, branch, diplom, batch, q, false, ct)); }

    [HttpGet("customer-balances")]
    public async Task<IActionResult> CustomerBalances([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string? q, CancellationToken ct)
    { await Need("customerbalance"); return Ok(await reports.BalancesAsync(from, to, null, null, null, q, true, ct)); }

    [HttpGet("expenses")]
    public async Task<IActionResult> Expenses([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid cash, [FromQuery] Guid? cost, CancellationToken ct)
    {
        await Need("expenseanalysis");
        if (cash == Guid.Empty) throw new ApiException("يرجى اختيار الخزينة أو البنك أولاً");
        return Ok(await reports.ExpensesAsync(from, to, cash, cost, ct));
    }
}
