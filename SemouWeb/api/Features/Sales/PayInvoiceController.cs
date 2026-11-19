using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Features.Registration;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Sales;

[ApiController]
[Authorize]
[Route("api/invoices")]
public sealed class PayInvoiceController(PayInvoiceService invoices, PermissionService perms, CurrentUser me) : ControllerBase
{
    [HttpGet("prepare")]
    [RequirePermission(PayInvoiceService.Form, Perm.Add)]
    public async Task<IActionResult> Prepare([FromQuery] Guid student, [FromQuery] int file, [FromQuery] int kind, CancellationToken ct)
        => Ok(await invoices.PrepareAsync(student, file, kind, ct));

    // No direct save from the web: the invoice screen sends a طلب سداد (POST api/requests/payment) that is reviewed first;
    // the transfer images travel with the request and reach the archive when it is confirmed.

    [HttpGet("{guid:guid}/print")]
    public async Task<IActionResult> Print(Guid guid, CancellationToken ct)
    {
        if (!(await perms.GetAsync(me.UserGuid, PayInvoiceService.Form)).Has(Perm.Find)
            && !(await perms.GetAsync(me.UserGuid, WebForms.PayOrders)).Has(Perm.Print)
            && !(await perms.GetAsync(me.UserGuid, WebForms.RegOrders)).Has(Perm.Print))
            throw new ApiException(RequirePermissionAttribute.Message(Perm.Find), 403);
        return Ok(await invoices.PrintAsync(guid, ct));
    }
}
