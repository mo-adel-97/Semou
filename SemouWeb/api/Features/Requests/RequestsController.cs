using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Features.Registration;
using Semou.Api.Features.Sales;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Requests;

/// <summary>
/// طلبات الالتحاق (webregorder) و طلبات السداد (webpayorder).
/// Sending a request needs the right of the original screen (addregdoc / salesinvoice Add);
/// reviewing needs the web screen rights: View = open, Add = approve/confirm, Edit = correct transfer data, Delete = reject/cancel.
/// </summary>
[ApiController]
[Authorize]
[Route("api/requests")]
public sealed class RequestsController(RequestsService requests, PermissionService perms, CurrentUser me) : ControllerBase
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private async Task<bool> Has(string form, Perm p) => (await perms.GetAsync(me.UserGuid, form)).Has(p);
    private async Task Need(string form, Perm p) { if (!await Has(form, p)) throw new ApiException(RequirePermissionAttribute.Message(p), 403); }

    /* ---------------- طلبات الالتحاق ---------------- */

    [HttpPost("registration")]
    [RequirePermission(RegistrationService.Form, Perm.Add)]
    public async Task<IActionResult> CreateReg(RegistrationInput input, CancellationToken ct) => Ok(await requests.CreateRegAsync(input, ct));

    [HttpGet("registration")]
    public async Task<IActionResult> ListReg([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? status, [FromQuery] Guid? branch, [FromQuery] string? q, CancellationToken ct)
    { await Need(WebForms.RegOrders, Perm.View); return Ok(await requests.ListRegAsync(from, to, status, branch, q, ct)); }

    [HttpGet("registration/{guid:guid}")]
    public async Task<IActionResult> GetReg(Guid guid, CancellationToken ct) { await Need(WebForms.RegOrders, Perm.View); return Ok(await requests.GetRegAsync(guid, ct)); }

    [HttpPost("registration/{guid:guid}/approve")]
    public async Task<IActionResult> ApproveReg(Guid guid, ReviewInput input, CancellationToken ct) { await Need(WebForms.RegOrders, Perm.Add); return Ok(await requests.ApproveRegAsync(guid, input.Notes, ct)); }

    [HttpPost("registration/{guid:guid}/reject")]
    public async Task<IActionResult> RejectReg(Guid guid, ReviewInput input, CancellationToken ct) { await Need(WebForms.RegOrders, Perm.Delete); return Ok(await requests.RejectRegAsync(guid, input.Notes, ct)); }

    /* ---------------- طلبات السداد ---------------- */

    /// <summary>multipart/form-data: payload (PayInvoiceInput as JSON) + file1 / file2 (transfer images).</summary>
    [HttpPost("payment")]
    [RequirePermission(PayInvoiceService.Form, Perm.Add)]
    [RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<IActionResult> CreatePay([FromForm] string payload, CancellationToken ct)
    {
        var input = JsonSerializer.Deserialize<PayInvoiceInput>(payload, Json) ?? throw new ApiException("بيانات الطلب غير صحيحة");
        return Ok(await requests.CreatePayAsync(input, Request.Form.Files.GetFile("file1"), Request.Form.Files.GetFile("file2"), ct));
    }

    [HttpGet("payment")]
    public async Task<IActionResult> ListPay([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? status, [FromQuery] Guid? branch, [FromQuery] string? q, CancellationToken ct)
    { await Need(WebForms.PayOrders, Perm.View); return Ok(await requests.ListPayAsync(from, to, status, branch, q, ct)); }

    [HttpGet("payment/{guid:guid}")]
    public async Task<IActionResult> GetPay(Guid guid, CancellationToken ct) { await Need(WebForms.PayOrders, Perm.View); return Ok(await requests.GetPayAsync(guid, ct)); }

    [HttpPut("payment/{guid:guid}")]
    public async Task<IActionResult> UpdatePay(Guid guid, PayEditInput input, CancellationToken ct) { await Need(WebForms.PayOrders, Perm.Edit); return Ok(await requests.UpdatePayAsync(guid, input, ct)); }

    [HttpPost("payment/{guid:guid}/confirm")]
    public async Task<IActionResult> ConfirmPay(Guid guid, ReviewInput input, CancellationToken ct) { await Need(WebForms.PayOrders, Perm.Add); return Ok(await requests.ConfirmPayAsync(guid, input.Notes, ct)); }

    [HttpPost("payment/{guid:guid}/status")]
    public async Task<IActionResult> PayStatus(Guid guid, PayStatusInput input, CancellationToken ct) { await Need(WebForms.PayOrders, Perm.Delete); return Ok(await requests.SetPayStatusAsync(guid, input.Status, input.Reason, ct)); }

    [HttpPost("payment/{guid:guid}/archive")]
    public async Task<IActionResult> RetryArchive(Guid guid, CancellationToken ct) { await Need(WebForms.PayOrders, Perm.Add); return Ok(await requests.RetryArchiveAsync(guid, ct)); }

    /* ---------------- Shared ---------------- */

    [HttpGet("files/{guid:guid}")]
    public async Task<IActionResult> File(Guid guid, CancellationToken ct)
    {
        var (data, type, name) = await requests.FileAsync(guid, async (_, kind) =>
            kind == "reg" ? await Has(WebForms.RegOrders, Perm.View) : kind == "pay" && await Has(WebForms.PayOrders, Perm.View), ct);
        Response.Headers.CacheControl = "private, max-age=600";
        return File(data, type, name, enableRangeProcessing: false);
    }

    /// <summary>طلباتي: the requests the signed-in user sent and where each one stands.</summary>
    [HttpGet("mine")]
    public async Task<IActionResult> Mine([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string? q, CancellationToken ct)
    { await Need(WebForms.MyOrders, Perm.View); return Ok(await requests.ListMineAsync(from, to, q, ct)); }

    [HttpGet("counts")]
    public async Task<IActionResult> Counts(CancellationToken ct) =>
        Ok(await requests.CountsAsync(await Has(WebForms.RegOrders, Perm.View), await Has(WebForms.PayOrders, Perm.View), ct));

    [HttpGet("student/{account:guid}")]
    public async Task<IActionResult> OfStudent(Guid account, CancellationToken ct)
    {
        if (!await Has("resdesk", Perm.View) && !await Has(WebForms.RegOrders, Perm.View) && !await Has(WebForms.PayOrders, Perm.View))
            throw new ApiException(RequirePermissionAttribute.Message(Perm.View), 403);
        return Ok(await requests.OfStudentAsync(account, ct));
    }
}
