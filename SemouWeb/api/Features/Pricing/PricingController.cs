using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Security;

namespace Semou.Api.Features.Pricing;

/// <summary>ضبط الأسعار والدفعات المقدمة: View = open, Add = new package, Edit = edit a package or the prices.</summary>
[ApiController]
[Authorize]
[Route("api/pricing")]
public sealed class PricingController(PricingService pricing, CurrentUser me) : ControllerBase
{
    [HttpGet]
    [RequirePermission(PricingService.Form, Perm.View)]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await pricing.GetAsync(ct));

    [HttpPost("plans")]
    [RequirePermission(PricingService.Form, Perm.Add)]
    public async Task<IActionResult> AddPlan(PlanInput input, CancellationToken ct) => Ok(await pricing.AddPlanAsync(input, ct));

    [HttpPut("plans/{guid:guid}")]
    [RequirePermission(PricingService.Form, Perm.Edit)]
    public async Task<IActionResult> UpdatePlan(Guid guid, PlanInput input, CancellationToken ct) => Ok(await pricing.UpdatePlanAsync(guid, input, ct));

    [HttpPut("prices")]
    [RequirePermission(PricingService.Form, Perm.Edit)]
    public async Task<IActionResult> SavePrices(PricesInput input, CancellationToken ct) => Ok(await pricing.SavePricesAsync(input, me.UserGuid, ct));
}
