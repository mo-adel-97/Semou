using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Reception;

[ApiController]
[Authorize]
[Route("api/reception")]
public sealed class ReceptionController(ReceptionService reception, AcceptanceService acceptance, StatementService statement, StudentActionsService actions) : ControllerBase
{
    /// <summary>by = nid | tel | name. Like the desktop, an exact 10-digit id/mobile is required unless filtering by branch/case.</summary>
    [HttpGet("search")]
    [RequirePermission("resdesk", Perm.View)]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] string by = "nid", [FromQuery] Guid? branch = null, [FromQuery] Guid? regCase = null, CancellationToken ct = default)
    {
        if (by is not ("nid" or "tel" or "name")) throw new ApiException("نوع البحث غير صحيح");
        if (by == "name" && q?.Trim().Length is > 0 and < 3) throw new ApiException("اكتب 3 حروف على الأقل من الاسم");
        return Ok(await reception.SearchAsync(q, by, branch, regCase, ct));
    }

    [HttpGet("archive")]
    [RequirePermission("resdesk", Perm.View)]
    public async Task<IActionResult> Archive([FromQuery] string q, [FromQuery] string by = "nid", CancellationToken ct = default)
    {
        if (by is not ("nid" or "tel" or "name")) throw new ApiException("نوع البحث غير صحيح");
        // The archive computes each balance with scalar functions (slow): like the desktop, search by the full
        // 10-digit id / mobile, or by at least 4 letters of the name.
        q = q?.Trim() ?? "";
        if (by != "name" && (q.Length != 10 || !q.All(char.IsDigit))) throw new ApiException("اكتب الرقم كاملاً (10 أرقام) للبحث في الأرشيف");
        if (by == "name" && q.Length < 4) throw new ApiException("اكتب 4 حروف على الأقل من الاسم للبحث في الأرشيف");
        return Ok(await reception.ArchiveSearchAsync(q, by, ct));
    }

    [HttpGet("archive/preview")]
    [RequirePermission(ReceptionService.StudentForm, Perm.Add)]
    public async Task<IActionResult> ArchivePreview([FromQuery] string custNo, [FromQuery] int oldBranchId, [FromQuery] string nationalId, CancellationToken ct)
        => Ok(await reception.ArchivePreviewAsync(custNo, oldBranchId, nationalId, ct));

    [HttpGet("lookups")]
    [RequirePermission("resdesk", Perm.View)]
    public async Task<IActionResult> Lookups([FromQuery] Guid? branch, CancellationToken ct) => Ok(await reception.LookupsAsync(branch, ct));

    [HttpGet("students/{guid:guid}")]
    [RequirePermission(ReceptionService.StudentForm, Perm.Find)]
    public async Task<IActionResult> Get(Guid guid, CancellationToken ct) => Ok(await reception.GetStudentAsync(guid, ct));

    [HttpPost("students")]
    [RequirePermission(ReceptionService.StudentForm, Perm.Add)]
    public Task<StudentSaved> Create(StudentInput input, CancellationToken ct) => reception.CreateAsync(input, ct);

    [HttpPut("students/{guid:guid}")]
    [RequirePermission(ReceptionService.StudentForm, Perm.Edit)]
    public Task<StudentSaved> Update(Guid guid, StudentInput input, CancellationToken ct) => reception.UpdateAsync(guid, input, ct);

    /// <summary>Registration forms waiting for supervisor acceptance (no training file yet).</summary>
    [HttpGet("students/{account:guid}/pending-regs")]
    [RequirePermission("resdesk", Perm.View)]
    public async Task<IActionResult> PendingRegs(Guid account, CancellationToken ct) => Ok(await acceptance.PendingAsync(account, ct));

    /// <summary>قبول التسجيل: opens the training file (StudentLevel).</summary>
    [HttpPost("regdocs/{guid:guid}/accept")]
    [RequirePermission(AcceptanceService.Form, Perm.Add)]
    public Task<object> Accept(Guid guid, AcceptInput input, CancellationToken ct) => acceptance.AcceptAsync(guid, input, ct);

    /// <summary>كشف حساب الطالب (same permission as the desktop FORFIND("studentstatment")).</summary>
    [HttpGet("students/{account:guid}/statement")]
    [RequirePermission(StatementService.Form, Perm.Find)]
    public async Task<IActionResult> Statement(Guid account, [FromQuery] string? q, CancellationToken ct) => Ok(await statement.GetAsync(account, q, ct));

    /* ----- بيانات الدراسة / الحالة / الإيقاف / سجل المستندات ----- */
    [HttpGet("files/{fileId:int}/study")]
    [RequirePermission("resdesk", Perm.View)]
    public async Task<IActionResult> Study(int fileId, CancellationToken ct) => Ok(await actions.StudyAsync(fileId, ct));

    [HttpPut("files/{fileId:int}/class")]
    [RequirePermission("resdesk", Perm.View)]
    public Task<object> SetClass(int fileId, ClassInput input, CancellationToken ct) => actions.SetClassAsync(fileId, input, ct);

    [HttpPut("students/{account:guid}/case")]
    [RequirePermission(ReceptionService.StudentForm, Perm.Edit)]
    public Task<object> ChangeCase(Guid account, CaseInput input, CancellationToken ct) => actions.ChangeCaseAsync(account, input, ct);

    [HttpPut("students/{account:guid}/active")]
    [RequirePermission(ReceptionService.StudentForm, Perm.Edit)]
    public Task<object> SetActive(Guid account, ActiveInput input, CancellationToken ct) => actions.SetActiveAsync(account, input, ct);

    [HttpGet("students/{account:guid}/history")]
    [RequirePermission("resdesk", Perm.View)]
    public async Task<IActionResult> History(Guid account, CancellationToken ct) => Ok(await actions.HistoryAsync(account, ct));
}
