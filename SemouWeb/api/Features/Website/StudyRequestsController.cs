using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Website;

public sealed class StudyRequestRow
{
    public Guid Guid { get; set; }
    public int Id { get; set; }
    public DateTime RequestDate { get; set; }
    public string StudentName { get; set; } = "";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? NationalId { get; set; }
    public string? City { get; set; }
    public string? Program { get; set; }
    public int StudyMode { get; set; }
    public string? Notes { get; set; }
    public int Status { get; set; }
    public string? FollowNotes { get; set; }
    public Guid? StudentGuid { get; set; }
    public string? ExistingStudent { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public sealed record StudyRequestUpdate(int Status, string? FollowNotes);

/// <summary>
/// المبيعات ← طلبات الدراسة (webstudyrequest): requests from the website's "سجلي الآن", split حضوري / عن بعد.
/// View = open the screen, Edit = change status / follow-up notes.
/// </summary>
[ApiController]
[Authorize]
[Route("api/study-requests")]
public sealed class StudyRequestsController(Db db, PermissionService perms, CurrentUser me, UserActionLog audit, WebsiteSyncService sync, WebsiteClient site) : ControllerBase
{
    private static readonly string[] StatusNames = ["جديد", "جاري التواصل", "تم التسجيل", "غير مهتم"];

    private async Task Need(Perm p)
    {
        if (!(await perms.GetAsync(me.UserGuid, WebForms.StudyRequests)).Has(p)) throw new ApiException(RequirePermissionAttribute.Message(p), 403);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? mode, [FromQuery] int? status, [FromQuery] string? q, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        await Need(Perm.View);
        q = string.IsNullOrWhiteSpace(q) ? null : q.Trim();
        await using var c = await db.OpenAsync(ct);
        var rows = await c.QueryAsync<StudyRequestRow>(new CommandDefinition("""
            SELECT TOP 1000 W.Guid, W.ID AS Id, W.RequestDate, W.StudentName, W.Phone, W.Email, W.NationalId, W.City, W.Program, W.StudyMode, W.Notes,
                   W.Status, W.FollowNotes, W.StudentGuid, S.StudentName AS ExistingStudent, W.UpdatedAt, U.FullName AS UpdatedBy
            FROM dbo.WebStudyRequest W
            LEFT JOIN dbo.Student S ON S.Guid = W.StudentGuid
            LEFT JOIN dbo.User_Info U ON U.Guid = W.UpdatedUserGuid
            WHERE (@mode IS NULL OR W.StudyMode = @mode) AND (@status IS NULL OR W.Status = @status)
              AND (@from IS NULL OR W.RequestDate >= @from) AND (@to IS NULL OR W.RequestDate < DATEADD(day, 1, @to))
              AND (@q IS NULL OR W.StudentName LIKE '%' + @q + '%' OR W.Phone LIKE '%' + @q + '%' OR W.NationalId LIKE '%' + @q + '%' OR W.Program LIKE '%' + @q + '%')
            ORDER BY W.RequestDate DESC, W.ID DESC
            """, new { mode, status, q, from, to }, cancellationToken: ct));
        var counts = await c.QueryAsync<(int Mode, int Status, int N)>(new CommandDefinition(
            "SELECT StudyMode AS Mode, Status, COUNT(*) AS N FROM dbo.WebStudyRequest GROUP BY StudyMode, Status", cancellationToken: ct));
        var lastPull = await c.ExecuteScalarAsync<DateTime?>(new CommandDefinition("SELECT UpdatedAt FROM dbo.WebSyncState WHERE Name = 'requests.last_id'", cancellationToken: ct));
        var lastPush = await c.ExecuteScalarAsync<string?>(new CommandDefinition("SELECT Value FROM dbo.WebSyncState WHERE Name = 'students.last_push'", cancellationToken: ct));
        return Ok(new
        {
            rows,
            counts = counts.Select(x => new { mode = x.Mode, status = x.Status, n = x.N }),
            statuses = StatusNames,
            sync = new { enabled = site.Configured, lastPull, lastPush },
        });
    }

    [HttpPut("{guid:guid}")]
    public async Task<IActionResult> Update(Guid guid, StudyRequestUpdate input, CancellationToken ct)
    {
        await Need(Perm.Edit);
        if (input.Status is < 0 or > 3) throw new ApiException("الحالة غير صحيحة");
        var notes = (input.FollowNotes ?? "").Trim();
        if (notes.Length > 1000) throw new ApiException("ملاحظات المتابعة طويلة جداً (الحد الأقصى 1000 حرف)");
        return Ok(await db.InTransactionAsync<object>(async (c, tx) =>
        {
            var row = await c.QueryFirstOrDefaultAsync<(int Id, string StudentName)>(
                "SELECT ID, StudentName FROM dbo.WebStudyRequest WHERE Guid = @guid", new { guid }, tx);
            if (row.StudentName is null) throw ApiException.NotFound("الطلب غير موجود");
            await c.ExecuteAsync("""
                UPDATE dbo.WebStudyRequest SET Status = @s, FollowNotes = @n, UpdatedAt = @now, UpdatedUserGuid = @u WHERE Guid = @guid
                """, new { s = input.Status, n = notes.Length > 0 ? notes : null, u = me.UserGuid, guid, now = Clock.Now }, tx);
            await audit.WriteAsync(c, tx, WebForms.StudyRequests, guid, $"طلب دراسة: {StatusNames[input.Status]}", row.Id.ToString(), notes.Length > 0 ? notes : null);
            return new { message = "تم حفظ المتابعة" };
        }, ct: ct));
    }

    /// <summary>Badge for the sidebar: new requests not handled yet.</summary>
    [HttpGet("count")]
    public async Task<IActionResult> Count(CancellationToken ct)
    {
        if (!(await perms.GetAsync(me.UserGuid, WebForms.StudyRequests)).Has(Perm.View)) return Ok(new { count = 0 });
        await using var c = await db.OpenAsync(ct);
        return Ok(new { count = await c.ExecuteScalarAsync<int>(new CommandDefinition("SELECT COUNT(*) FROM dbo.WebStudyRequest WHERE Status = 0", cancellationToken: ct)) });
    }

    /// <summary>Pull now instead of waiting for the next minute.</summary>
    [HttpPost("sync")]
    public async Task<IActionResult> SyncNow(CancellationToken ct)
    {
        await Need(Perm.View);
        if (!site.Configured) throw new ApiException("الربط مع الموقع غير مفعل في إعدادات الخادم");
        var n = await sync.PullRequestsAsync(ct);
        return Ok(new { message = n > 0 ? $"تم استلام {n} طلب جديد من الموقع" : "لا توجد طلبات جديدة" });
    }
}
