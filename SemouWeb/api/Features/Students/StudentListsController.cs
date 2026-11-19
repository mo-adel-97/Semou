using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Students;

/// <summary>kind = diploma (طلاب الدبلومات, diplomstudent) | course (طلاب الدورات, coursestudent).</summary>
[ApiController]
[Authorize]
[Route("api/students")]
public sealed class StudentListsController(StudentListsService lists, PermissionService perms, CurrentUser me) : ControllerBase
{
    private async Task Need(string form, Perm p)
    {
        if (!(await perms.GetAsync(me.UserGuid, form)).Has(p)) throw new ApiException(RequirePermissionAttribute.Message(p), 403);
    }
    private static string FormOf(string kind) => kind switch { "diploma" => StudentListsService.DiplomaForm, "course" => StudentListsService.CourseForm, _ => throw new ApiException("نوع القائمة غير صحيح") };

    [HttpGet("{kind}/lookups")]
    public async Task<IActionResult> Lookups(string kind, [FromQuery] Guid? branch, CancellationToken ct) { await Need(FormOf(kind), Perm.View); return Ok(await lists.LookupsAsync(branch, ct)); }

    [HttpGet("{kind}")]
    public async Task<IActionResult> List(string kind, [FromQuery] Guid branch, [FromQuery] Guid? diplom, [FromQuery] Guid? course, [FromQuery] Guid? @case,
        [FromQuery] Guid? batch, [FromQuery] Guid? level, [FromQuery] bool? active, [FromQuery] string? q, CancellationToken ct)
    {
        await Need(FormOf(kind), Perm.View);
        if (branch == Guid.Empty) throw new ApiException("يرجى اختيار الفرع");
        return Ok(await lists.ListAsync(kind == "course" ? 1 : 0, branch, diplom, course, @case, batch, level, active, q, ct));
    }

    [HttpPost("{kind}/level")]
    public async Task<object> Level(string kind, LevelInput input, CancellationToken ct) { await Need(FormOf(kind), Perm.View); return await lists.SetLevelAsync(input, ct); }

    [HttpPost("{kind}/notes")]
    public async Task<object> Notes(string kind, NotesInput input, CancellationToken ct) { await Need(FormOf(kind), Perm.View); return await lists.SetNotesAsync(input, ct); }

    [HttpPost("{kind}/case")]
    public async Task<object> Case(string kind, CaseBulkInput input, CancellationToken ct) { FormOf(kind); await Need("changestaut", Perm.Add); return await lists.SetCaseAsync(input, ct); }

    [HttpPost("{kind}/finish")]
    public async Task<object> Finish(string kind, FinishInput input, CancellationToken ct) { FormOf(kind); await Need("finshstudy", Perm.Add); return await lists.FinishAsync(input, ct); }

    [HttpPost("{kind}/transfer")]
    public async Task<object> Transfer(string kind, TransferInput input, CancellationToken ct) { await Need(FormOf(kind), Perm.View); await Need("addstudent", Perm.Edit); return await lists.TransferAsync(input, ct); }
}
