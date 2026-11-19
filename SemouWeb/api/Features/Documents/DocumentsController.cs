using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Semou.Api.Features.Documents;

[ApiController]
public sealed class DocumentsController(DocumentSealService seals, Semou.Api.Security.CurrentUser me) : ControllerBase
{
    /// <summary>Seal for a document about to be printed (code + verification link).</summary>
    [HttpGet("api/docs/seal/{type}/{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> Seal(string type, Guid guid, CancellationToken ct) => Ok(await seals.SealAsync(type, guid, me.FullName, ct));

    /// <summary>Public: anyone holding the paper can check it (scanned QR).</summary>
    [HttpGet("api/verify/{type}/{guid:guid}")]
    [AllowAnonymous]
    [EnableRateLimiting("verify")]
    public async Task<IActionResult> Verify(string type, Guid guid, [FromQuery] string? c, CancellationToken ct) => Ok(await seals.VerifyAsync(type, guid, c, ct));
}
