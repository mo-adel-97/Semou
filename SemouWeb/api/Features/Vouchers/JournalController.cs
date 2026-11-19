using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Vouchers;

[ApiController]
[Authorize]
[Route("api/journal")]
public sealed class JournalController(JournalService journal, PermissionService perms, CurrentUser me) : ControllerBase
{
    private async Task<JournalKind> Authorize(string kind, Perm p, string msg)
    {
        var k = kind switch { "daily" => JournalService.Daily, "opening" => JournalService.Opening, _ => throw ApiException.NotFound("غير موجود") };
        if (!(await perms.GetAsync(me.UserGuid, k.Form)).Has(p)) throw ApiException.Forbidden(msg);
        return k;
    }

    [HttpGet("{kind}")]
    public async Task<IActionResult> List(string kind, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
        => Ok(await journal.ListAsync(await Authorize(kind, Perm.Find, "لا تملك صلاحية استعراض البيانات"), from, to, ct));

    [HttpGet("{kind}/{guid:guid}")]
    public async Task<IActionResult> Get(string kind, Guid guid, CancellationToken ct)
        => Ok(await journal.GetAsync(await Authorize(kind, Perm.Find, "لا تملك صلاحية استعراض البيانات"), guid, ct));

    [HttpPost("{kind}")]
    public async Task<JournalSaved> Create(string kind, JournalInput input, CancellationToken ct)
        => await journal.CreateAsync(await Authorize(kind, Perm.Add, "لا تملك صلاحية إضافة جديد"), input, ct);

    [HttpPut("{kind}/{guid:guid}")]
    public async Task<JournalSaved> Update(string kind, Guid guid, JournalInput input, CancellationToken ct)
        => await journal.UpdateAsync(await Authorize(kind, Perm.Edit, "لا تملك صلاحية تعديل البيانات"), guid, input, ct);
}
