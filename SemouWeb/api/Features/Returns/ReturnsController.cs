using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Returns;

/// <summary>kind = reg (مرتجع استمارة, form reregister) | bill (مرتجع فاتورة, form repay). Permissions are checked per kind.</summary>
[ApiController]
[Authorize]
[Route("api/returns")]
public sealed class ReturnsController(ReturnsService returns, PermissionService perms, CurrentUser me) : ControllerBase
{
    private async Task Need(string kind, Perm p)
    {
        var form = kind switch { "reg" => ReturnsService.RegForm, "bill" => ReturnsService.BillForm, _ => throw new ApiException("نوع المرتجع غير صحيح") };
        if (!(await perms.GetAsync(me.UserGuid, form)).Has(p)) throw new ApiException(RequirePermissionAttribute.Message(p), 403);
    }

    [HttpGet("candidates/{account:guid}")]
    [RequirePermission("resdesk", Perm.View)]
    public async Task<IActionResult> Candidates(Guid account, CancellationToken ct) => Ok(await returns.CandidatesAsync(account, ct));

    [HttpGet("{kind}/prepare")]
    public async Task<IActionResult> Prepare(string kind, [FromQuery] Guid source, [FromQuery] bool edit, CancellationToken ct)
    { await Need(kind, edit ? Perm.Edit : Perm.Add); return Ok(await returns.PrepareAsync(kind, source, edit, ct)); }

    [HttpGet("{kind}/{guid:guid}")]
    public async Task<IActionResult> Get(string kind, Guid guid, CancellationToken ct) { await Need(kind, Perm.Find); return Ok(await returns.GetAsync(kind, guid, ct)); }

    [HttpPost("{kind}")]
    public async Task<object> Create(string kind, ReturnInput input, CancellationToken ct) { await Need(kind, Perm.Add); return await returns.SaveAsync(kind, null, input, ct); }

    [HttpPut("{kind}/{guid:guid}")]
    public async Task<object> Update(string kind, Guid guid, ReturnInput input, CancellationToken ct) { await Need(kind, Perm.Edit); return await returns.SaveAsync(kind, guid, input, ct); }
}
