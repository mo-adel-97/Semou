using System.Data;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Students;

/*
 * طلاب الدبلومات (LevelReportForm, "diplomstudent") and طلاب الدورات (StudentCourseList, "coursestudent").
 * One row per training file (StudentLevel). The desktop needed diploma + case before showing anything;
 * here the branch is enough and every other filter is optional.
 * Actions (same effect as the desktop procs, applied by file id so only the chosen files change):
 *   ترحيل المستوى (UpdateStudentLevel), ملاحظات (AddStudentLevelNotes), تغيير الحالة (ChangeStaut),
 *   إنهاء الدراسة (FinshStudy: case + mark + notes + LevelStaut 0), نقل فرع الدراسة / التسجيل
 *   (ChangeAllStudentLevelDiplom / ChangeRegBranch).
 */
public sealed class FileIdsInput { public List<int> FileIds { get; set; } = []; public string? Reason { get; set; } }
public sealed class LevelInput { public List<int> FileIds { get; set; } = []; public Guid LevelGuid { get; set; } }
public sealed class CaseBulkInput { public List<int> FileIds { get; set; } = []; public Guid CaseGuid { get; set; } public string? Reason { get; set; } }
public sealed class NotesInput { public int FileId { get; set; } public string? Notes { get; set; } }
public sealed class FinishInput { public int FileId { get; set; } public Guid CaseGuid { get; set; } public decimal Mark { get; set; } public string? Notes { get; set; } }
public sealed class TransferInput { public List<int> FileIds { get; set; } = []; public Guid? StudyBranchGuid { get; set; } public Guid? RegBranchGuid { get; set; } public string? Reason { get; set; } }

public sealed class StudentListsService(Db db, UserActionLog audit)
{
    public const string DiplomaForm = "diplomstudent";
    public const string CourseForm = "coursestudent";
    private const string AuditForm = "addstudent";

    public async Task<object> LookupsAsync(Guid? branch, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return new
        {
            diploms = await c.QueryAsync(new CommandDefinition("SELECT Guid AS guid, DiplomName AS name FROM dbo.Diplom WHERE ISNULL(IsUse,1) = 1 AND ISNULL(DiplomType,0) = 0 ORDER BY TRY_CONVERT(int, Code)", cancellationToken: ct)),
            courses = await c.QueryAsync(new CommandDefinition("""
                SELECT C.Guid AS guid, D.DiplomName AS name, C.CourseName AS code, C.DateStart AS dateStart FROM dbo.Course C JOIN dbo.Diplom D ON D.Guid = C.DiplomGuid
                WHERE (@branch IS NULL OR C.BranchGuid = @branch) ORDER BY C.DateStart DESC
                """, new { branch }, cancellationToken: ct)),
            cases = await c.QueryAsync(new CommandDefinition("SELECT Guid AS guid, StautName AS name FROM dbo.Stauts WHERE IsUse = 1 ORDER BY TRY_CONVERT(int, Code)", cancellationToken: ct)),
            batches = await c.QueryAsync(new CommandDefinition("SELECT Guid AS guid, BatchName AS name FROM dbo.Batch WHERE ISNULL(IsUse,1) = 1 ORDER BY TRY_CONVERT(int, Code) DESC", cancellationToken: ct)),
            levels = await c.QueryAsync(new CommandDefinition("SELECT Guid AS guid, LevelName AS name FROM dbo.Levels WHERE IsUse = 1 ORDER BY TRY_CONVERT(int, Code)", cancellationToken: ct)),
        };
    }

    private const string ListSql = """
        SELECT L.Id AS FileId, S.Guid AS StudentGuid, S.AccountGuid, S.AcadmyId, S.StudentName, S.NationalId, S.StudentTel,
               D.DiplomName, CO.CourseName AS CourseCode, CO.DateStart, CO.DateEnd, BT.BatchName, LV.LevelName, ST.StautName AS CaseName,
               L.LevelNotes AS Notes, CAST(ISNULL(L.LevelMark,0) AS decimal(18,2)) AS Mark, CAST(ISNULL(L.LevelStaut,1) AS bit) AS Active,
               CL.ClassCode AS ClassName, RB.BrName AS RegBranch, SB.BrName AS StudyBranch, CAST(ISNULL(S.IsUse,1) AS bit) AS StudentActive,
               (SELECT CAST(ISNULL(SUM(ISNULL(X.Maden,0) - ISNULL(X.Daen,0)),0) AS decimal(18,2))
                  FROM dbo.Day2 X JOIN dbo.Day1 Y ON Y.Guid = X.Day1Guid WHERE X.AccountGuid = S.AccountGuid AND Y.IsUse = 1) AS Balance
        FROM dbo.StudentLevel L JOIN dbo.Student S ON S.AccountGuid = L.StudentGuid
        LEFT JOIN dbo.Diplom D ON D.Guid = L.DiplomGuid LEFT JOIN dbo.Course CO ON CO.Guid = L.CourseGuid
        LEFT JOIN dbo.Batch BT ON BT.Guid = L.BatchGuid LEFT JOIN dbo.Levels LV ON LV.Guid = L.LevelGuid
        LEFT JOIN dbo.Stauts ST ON ST.Guid = L.CaseGuid LEFT JOIN dbo.Class CL ON CL.Guid = L.ClassGuid
        LEFT JOIN dbo.Branch RB ON RB.Guid = S.BarnchGuid LEFT JOIN dbo.Branch SB ON SB.Guid = L.BranchGuid
        WHERE ISNULL(L.IsUse,1) = 1 AND ISNULL(L.RegType,0) = @regType AND L.BranchGuid = @branch
          AND (@diplom IS NULL OR L.DiplomGuid = @diplom) AND (@course IS NULL OR L.CourseGuid = @course)
          AND (@case IS NULL OR L.CaseGuid = @case) AND (@batch IS NULL OR L.BatchGuid = @batch) AND (@level IS NULL OR L.LevelGuid = @level)
          AND (@active IS NULL OR ISNULL(L.LevelStaut,1) = @active)
          AND (@q IS NULL OR S.StudentName LIKE '%' + @q + '%' OR S.NationalId LIKE '%' + @q + '%' OR S.StudentTel LIKE '%' + @q + '%')
        ORDER BY S.StudentName
        """;

    public async Task<IEnumerable<dynamic>> ListAsync(int regType, Guid branch, Guid? diplom, Guid? course, Guid? @case, Guid? batch, Guid? level, bool? active, string? q, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync(new CommandDefinition(ListSql, new { regType, branch, diplom, course, @case, batch, level, active, q = string.IsNullOrWhiteSpace(q) ? null : q.Trim() },
            cancellationToken: ct, commandTimeout: 90));
    }

    /* ---------------- actions ---------------- */
    private static List<int> Ids(List<int> ids) => ids.Distinct().ToList() is { Count: > 0 } l ? l : throw new ApiException("يرجى تحديد طالب واحد على الأقل");

    private async Task AuditFilesAsync(System.Data.Common.DbConnection c, System.Data.Common.DbTransaction tx, List<int> ids, string action, string? reason)
    {
        var files = await c.QueryAsync<(Guid Guid, string Code)>("SELECT L.Guid, CAST(S.Code AS nvarchar(50)) FROM dbo.StudentLevel L JOIN dbo.Student S ON S.AccountGuid = L.StudentGuid WHERE L.Id IN @ids", new { ids }, tx);
        foreach (var f in files) await audit.WriteAsync(c, tx, AuditForm, f.Guid, action, f.Code, reason);
    }

    public Task<object> SetLevelAsync(LevelInput i, CancellationToken ct) => db.InTransactionAsync<object>(async (c, tx) =>
    {
        var ids = Ids(i.FileIds);
        var name = await c.ExecuteScalarAsync<string?>("SELECT LevelName FROM dbo.Levels WHERE Guid = @g AND IsUse = 1", new { g = i.LevelGuid }, tx) ?? throw new ApiException("يرجى اختيار المستوى المراد الترحيل إليه");
        var n = await c.ExecuteAsync("UPDATE dbo.StudentLevel SET LevelGuid = @lv WHERE Id IN @ids AND ISNULL(RegType,0) = 0", new { lv = i.LevelGuid, ids }, tx);
        await AuditFilesAsync(c, tx, ids, $"ترحيل إلى {name}", null);
        return new { message = $"تم ترحيل {n} طالب إلى {name} بنجاح" };
    }, ct: ct);

    public Task<object> SetCaseAsync(CaseBulkInput i, CancellationToken ct) => db.InTransactionAsync<object>(async (c, tx) =>
    {
        var ids = Ids(i.FileIds);
        if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب تغيير الحالة");
        var name = await c.ExecuteScalarAsync<string?>("SELECT StautName FROM dbo.Stauts WHERE Guid = @g AND IsUse = 1", new { g = i.CaseGuid }, tx) ?? throw new ApiException("يرجى اختيار حالة التسجيل");
        var n = await c.ExecuteAsync("UPDATE dbo.StudentLevel SET CaseGuid = @g WHERE Id IN @ids", new { g = i.CaseGuid, ids }, tx);
        await c.ExecuteAsync("UPDATE S SET RegCaseGuid = @g FROM dbo.Student S WHERE S.AccountGuid IN (SELECT StudentGuid FROM dbo.StudentLevel WHERE Id IN @ids)", new { g = i.CaseGuid, ids }, tx);
        await AuditFilesAsync(c, tx, ids, $"تغيير الحالة إلى {name}", i.Reason.Trim());
        return new { message = $"تم تغيير حالة {n} طالب إلى {name}" };
    }, ct: ct);

    public Task<object> SetNotesAsync(NotesInput i, CancellationToken ct) => db.InTransactionAsync<object>(async (c, tx) =>
    {
        var notes = (i.Notes ?? "").Trim();
        if (notes.Length > 1000) throw new ApiException("الملاحظات طويلة جداً (الحد الأقصى 1000 حرف)");
        var n = await c.ExecuteAsync("UPDATE dbo.StudentLevel SET LevelNotes = @notes WHERE Id = @id", new { notes, id = i.FileId }, tx);
        if (n == 0) throw ApiException.NotFound("الملف التدريبي غير موجود");
        await AuditFilesAsync(c, tx, [i.FileId], "تعديل ملاحظات الطالب", null);
        return new { message = "تم حفظ الملاحظات" };
    }, ct: ct);

    public Task<object> FinishAsync(FinishInput i, CancellationToken ct) => db.InTransactionAsync<object>(async (c, tx) =>
    {
        if (i.Mark < 0 || i.Mark > 100) throw new ApiException("يرجى تحديد نسبة الاختبار الخاصة بالطالب (من 0 إلى 100)");
        var name = await c.ExecuteScalarAsync<string?>("SELECT StautName FROM dbo.Stauts WHERE Guid = @g AND IsUse = 1", new { g = i.CaseGuid }, tx) ?? throw new ApiException("يرجى اختيار حالة التسجيل");
        var f = await c.QueryFirstOrDefaultAsync<(Guid StudentGuid, bool Active)>("SELECT StudentGuid, CAST(ISNULL(LevelStaut,1) AS bit) FROM dbo.StudentLevel WITH (UPDLOCK) WHERE Id = @id", new { id = i.FileId }, tx);
        if (f.StudentGuid == Guid.Empty) throw ApiException.NotFound("الملف التدريبي غير موجود");
        if (!f.Active) throw new ApiException("أنهى الطالب الدراسة في هذا الملف من قبل");
        await c.ExecuteAsync("UPDATE dbo.StudentLevel SET CaseGuid = @g, LevelMark = @m, LevelNotes = @n, LevelStaut = 0 WHERE Id = @id",
            new { g = i.CaseGuid, m = (double)i.Mark, n = (i.Notes ?? "").Trim(), id = i.FileId }, tx);
        await c.ExecuteAsync("dbo.ChangeStaut", new { Guid = f.StudentGuid, RegCaseGuid = i.CaseGuid }, tx, commandType: CommandType.StoredProcedure);
        await AuditFilesAsync(c, tx, [i.FileId], $"إنهاء الدراسة ({name}) بنسبة {i.Mark:0.##}%", i.Notes);
        return new { message = "تم إنهاء دراسة الطالب بنجاح" };
    }, ct: ct);

    public Task<object> TransferAsync(TransferInput i, CancellationToken ct) => db.InTransactionAsync<object>(async (c, tx) =>
    {
        var ids = Ids(i.FileIds);
        if (i.StudyBranchGuid is null && i.RegBranchGuid is null) throw new ApiException("يرجى اختيار الفرع المراد النقل إليه");
        if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب النقل");
        foreach (var b in new[] { i.StudyBranchGuid, i.RegBranchGuid }.Where(x => x is not null))
            if (await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Branch WHERE Guid = @b AND ISNULL(IsUse,1) = 1", new { b }, tx) == 0) throw new ApiException("الفرع المختار غير صالح");
        if (i.StudyBranchGuid is { } sb) await c.ExecuteAsync("UPDATE dbo.StudentLevel SET BranchGuid = @sb WHERE Id IN @ids", new { sb, ids }, tx);
        if (i.RegBranchGuid is { } rb) await c.ExecuteAsync("UPDATE S SET BarnchGuid = @rb FROM dbo.Student S WHERE S.AccountGuid IN (SELECT StudentGuid FROM dbo.StudentLevel WHERE Id IN @ids)", new { rb, ids }, tx);
        await AuditFilesAsync(c, tx, ids, i.StudyBranchGuid is not null && i.RegBranchGuid is not null ? "نقل فرع الدراسة والتسجيل" : i.StudyBranchGuid is not null ? "نقل فرع الدراسة" : "نقل فرع التسجيل", i.Reason.Trim());
        return new { message = $"تم نقل {ids.Count} طالب بنجاح" };
    }, ct: ct);
}
