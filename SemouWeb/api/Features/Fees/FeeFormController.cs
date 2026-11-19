using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Security;

namespace Semou.Api.Features.Fees;

[ApiController]
[Authorize]
[Route("api/fee-forms")]
public sealed class FeeFormController(FeeFormService fees) : ControllerBase
{
    [HttpGet("books")]
    [RequirePermission(FeeFormService.Form, Perm.Add)]
    public async Task<IActionResult> Books(CancellationToken ct) => Ok(await fees.BooksAsync(ct));

    [HttpGet("prepare")]
    [RequirePermission(FeeFormService.Form, Perm.Add)]
    public async Task<IActionResult> Prepare([FromQuery] Guid book, CancellationToken ct) => Ok(await fees.PrepareAsync(book, ct));

    [HttpGet("{guid:guid}")]
    [RequirePermission(FeeFormService.Form, Perm.Find)]
    public async Task<IActionResult> Get(Guid guid, CancellationToken ct) => Ok(await fees.GetAsync(guid, ct));

    [HttpPost]
    [RequirePermission(FeeFormService.Form, Perm.Add)]
    public Task<object> Create(FeeFormInput input, CancellationToken ct) => fees.SaveAsync(null, input, ct);

    [HttpPut("{guid:guid}")]
    [RequirePermission(FeeFormService.Form, Perm.Edit)]
    public Task<object> Update(Guid guid, FeeFormInput input, CancellationToken ct) => fees.SaveAsync(guid, input, ct);
}
