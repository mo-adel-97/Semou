using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Security;

namespace Semou.Api.Features.Discounts;

[ApiController]
[Authorize]
[Route("api/discounts")]
public sealed class DiscountController(DiscountService discounts) : ControllerBase
{
    [HttpGet("prepare")]
    [RequirePermission(DiscountService.Form, Perm.Add)]
    public async Task<IActionResult> Prepare([FromQuery] Guid student, CancellationToken ct) => Ok(await discounts.PrepareAsync(student, ct));

    [HttpGet("{guid:guid}")]
    [RequirePermission(DiscountService.Form, Perm.Find)]
    public async Task<IActionResult> Get(Guid guid, CancellationToken ct) => Ok(await discounts.GetAsync(guid, ct));

    [HttpPost]
    [RequirePermission(DiscountService.Form, Perm.Add)]
    public Task<object> Create(DiscountInput input, CancellationToken ct) => discounts.SaveAsync(null, input, ct);

    [HttpPut("{guid:guid}")]
    [RequirePermission(DiscountService.Form, Perm.Edit)]
    public Task<object> Update(Guid guid, DiscountInput input, CancellationToken ct) => discounts.SaveAsync(guid, input, ct);
}
