using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Semou.Api.Features.External;

/// <summary>
/// Read-only API for outside systems. No user login: the caller sends the key from External:ApiKey (server config,
/// never in git) in the X-Api-Key header. Wrong or missing key = 401 without any detail; 60 calls a minute per key.
/// Every call is logged (with the national id masked).
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/external")]
[EnableRateLimiting("external")]
public sealed class ExternalController(ExternalStudentsService students, IConfiguration config, ILogger<ExternalController> log) : ControllerBase
{
    private bool KeyOk()
    {
        var expected = config["External:ApiKey"];
        var given = Request.Headers["X-Api-Key"].ToString();
        if (string.IsNullOrEmpty(expected) || expected.Length < 32 || string.IsNullOrEmpty(given)) return false;
        return CryptographicOperations.FixedTimeEquals(SHA256.HashData(Encoding.UTF8.GetBytes(expected)), SHA256.HashData(Encoding.UTF8.GetBytes(given)));
    }

    /// <summary>بيانات الطالب الكاملة برقم الهوية / الإقامة.</summary>
    [HttpGet("students/{nationalId}")]
    public async Task<IActionResult> Student(string nationalId, CancellationToken ct)
    {
        var ip = Request.Headers["CF-Connecting-IP"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString();
        if (!KeyOk()) { log.LogWarning("external API: rejected key from {Ip}", ip); return Unauthorized(new { message = "مفتاح الوصول غير صحيح" }); }
        var id = ExternalStudentsService.CleanId(nationalId);
        if (id is null) return BadRequest(new { message = "رقم الهوية يجب أن يكون 10 أرقام" });
        log.LogInformation("external API: student ******{Tail} from {Ip}", id[^4..], ip);
        var data = await students.ByNationalIdAsync(id, ct);
        return data is null ? NotFound(new { message = "لا يوجد طالب بهذا الرقم" }) : Ok(data);
    }
}
