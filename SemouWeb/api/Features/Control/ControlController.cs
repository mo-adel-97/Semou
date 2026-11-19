using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Features.Discounts;
using Semou.Api.Security;

namespace Semou.Api.Features.Control;

[ApiController]
[Authorize]
[Route("api/control")]
public sealed class ControlController(ControlService control) : ControllerBase
{
    [HttpGet("price-lists")]
    [RequirePermission(ControlService.PriceForm, Perm.Find)]
    public async Task<IActionResult> PriceLists(CancellationToken ct) => Ok(await control.PriceListsAsync(ct));

    [HttpGet("price-items")]
    [RequirePermission(ControlService.PriceForm, Perm.View)]
    public async Task<IActionResult> PriceItems(CancellationToken ct) => Ok(await control.PriceItemsAsync(ct));

    [HttpGet("price-lists/{guid:guid}")]
    [RequirePermission(ControlService.PriceForm, Perm.Find)]
    public async Task<IActionResult> PriceList(Guid guid, CancellationToken ct) => Ok(await control.PriceListAsync(guid, ct));

    [HttpPost("price-lists")]
    [RequirePermission(ControlService.PriceForm, Perm.Add)]
    public Task<object> CreatePriceList(PriceListInput input, CancellationToken ct) => control.SavePriceListAsync(null, input, ct);

    [HttpPut("price-lists/{guid:guid}")]
    [RequirePermission(ControlService.PriceForm, Perm.Edit)]
    public Task<object> UpdatePriceList(Guid guid, PriceListInput input, CancellationToken ct) => control.SavePriceListAsync(guid, input, ct);

    [HttpGet("batches")]
    [RequirePermission(ControlService.SeatsForm, Perm.View)]
    public async Task<IActionResult> Batches([FromQuery] Guid? branch, CancellationToken ct) => Ok(await control.BatchesAsync(branch, ct));

    [HttpGet("batch-seats")]
    [RequirePermission(ControlService.SeatsForm, Perm.View)]
    public async Task<IActionResult> BatchSeats([FromQuery] Guid branch, [FromQuery] Guid batch, CancellationToken ct) => Ok(await control.BatchSeatsAsync(branch, batch, ct));

    [HttpPut("batch-seats")]
    [RequirePermission(ControlService.SeatsForm, Perm.Edit)]
    public Task<object> SaveBatchSeats(BatchSeatsInput input, CancellationToken ct) => control.SaveBatchSeatsAsync(input, ct);

    [HttpGet("new-students")]
    [RequirePermission(ControlService.NewStudentsForm, Perm.View)]
    public async Task<IActionResult> NewStudents([FromQuery] Guid branch, [FromQuery] int regType = 0, [FromQuery] string? name = null, CancellationToken ct = default)
        => Ok(await control.NewStudentsAsync(branch, regType, name, ct));

    [HttpGet("discounts")]
    [RequirePermission(DiscountService.Form, Perm.Find)]
    public async Task<IActionResult> Discounts([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct) => Ok(await control.DiscountsAsync(from, to, ct));
}
