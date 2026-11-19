using System.Data;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Reception;

/*
 * Reception side actions on a student:
 *   بيانات الدراسة (StudyInfo_Student) + ربط بشعبة (AddStudentToClass)
 *   تغيير حالة التسجيل (ChangeStaut) — the desktop passed the national id as the file type and the
 *       student Guid as the account, so it never updated anything; here it updates the student AND the chosen file.
 *   إيقاف / تفعيل الطالب (ChangeStudentStaut)
 *   سجل المستندات (ShowActionHistory) — for the student and every document of the student.
 */
public sealed class CaseInput { public Guid CaseGuid { get; set; } public int? FileId { get; set; } public string? Reason { get; set; } }
public sealed class ActiveInput { public bool IsUse { get; set; } public string? Reason { get; set; } }
public sealed class ClassInput { public Guid ClassGuid { get; set; } }

public sealed class HistoryRow
{
    public DateTime Date { get; set; }
    public DateTime? Time { get; set; }
    public string? UserName { get; set; }
    public string? Action { get; set; }
    public string? Reason { get; set; }
    public string? Code { get; set; }
    public string? DocType { get; set; }
}

public sealed class StudentActionsService(Db db, UserActionLog audit)
{
    /* ---------------- Study info ---------------- */
    public async Task<object> StudyAsync(int fileId, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var f = await c.QueryFirstOrDefaultAsync(new CommandDefinition("""
            SELECT L.Id AS FileId, L.Guid, L.RegType, L.DiplomGuid, D.DiplomName, L.BranchGuid, BR.BrName AS BranchName, BT.BatchName, LV.LevelName, L.LevelGuid,
                   CO.CourseName, CO.DateStart, CO.DateEnd, ST.StautName AS CaseName, L.LevelNotes AS Notes, L.ClassGuid, CL.ClassCode AS ClassName,
                   CAST(ISNULL(L.LevelStaut,1) AS bit) AS Active, CAST(ISNULL(L.IsReg,0) AS bit) AS InClass, R.Code AS RegCode, R.RegDate,
                   S.StudentName, S.NationalId, S.StudentTel, CAST(ISNULL(S.LockProfile,0) AS bit) AS Locked
            FROM dbo.StudentLevel L JOIN dbo.Student S ON S.AccountGuid = L.StudentGuid
            LEFT JOIN dbo.Diplom D ON D.Guid = L.DiplomGuid LEFT JOIN dbo.Branch BR ON BR.Guid = L.BranchGuid
            LEFT JOIN dbo.Batch BT ON BT.Guid = L.BatchGuid LEFT JOIN dbo.Levels LV ON LV.Guid = L.LevelGuid
            LEFT JOIN dbo.Course CO ON CO.Guid = L.CourseGuid LEFT JOIN dbo.Stauts ST ON ST.Guid = L.CaseGuid
            LEFT JOIN dbo.Class CL ON CL.Guid = L.ClassGuid LEFT JOIN dbo.RegDoc R ON R.Guid = L.RegDocGuid
            WHERE L.Id = @fileId
            """, new { fileId }, cancellationToken: ct)) ?? throw ApiException.NotFound("الملف التدريبي غير موجود");
        var r = (IDictionary<string, object?>)f;
        if ((bool)r["Locked"]!) throw new ApiException("ملف الطالب مغلق، يرجى التواصل مع الإدارة");
        var classes = Convert.ToInt32(r["RegType"] ?? 0) == 0 && r["LevelGuid"] is Guid lv
            ? await c.QueryAsync(new CommandDefinition("""
                SELECT Guid, ClassCode AS Name, Code FROM dbo.Class
                WHERE BranchGuid = @b AND DiplomGuid = @d AND LavelGuid = @lv ORDER BY TRY_CONVERT(int, Code)
                """, new { b = r["BranchGuid"], d = r["DiplomGuid"], lv }, cancellationToken: ct))
            : [];
        return new { file = f, classes };
    }

    public Task<object> SetClassAsync(int fileId, ClassInput i, CancellationToken ct) => db.InTransactionAsync<object>(async (c, tx) =>
    {
        var f = await c.QueryFirstOrDefaultAsync<(Guid Guid, int RegType, Guid BranchGuid, Guid DiplomGuid, Guid? LevelGuid)>(
            "SELECT Guid, ISNULL(RegType,0), BranchGuid, DiplomGuid, LevelGuid FROM dbo.StudentLevel WITH (UPDLOCK) WHERE Id = @fileId", new { fileId }, tx);
        if (f.Guid == Guid.Empty) throw ApiException.NotFound("الملف التدريبي غير موجود");
        if (f.RegType != 0) throw new ApiException("لا يسمح بربط ملف الدورة التدريبية بشعبة");
        var ok = await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Class WHERE Guid = @g AND BranchGuid = @b AND DiplomGuid = @d AND LavelGuid = @lv",
            new { g = i.ClassGuid, b = f.BranchGuid, d = f.DiplomGuid, lv = f.LevelGuid }, tx);
        if (ok == 0) throw new ApiException("الشعبة المختارة لا تتبع فرع ودبلوم ومستوى الطالب");
        await c.ExecuteAsync("dbo.AddStudentToClass", new { Guid = f.Guid, ClassGuid = i.ClassGuid, IsReg = true }, tx, commandType: CommandType.StoredProcedure);
        await audit.WriteAsync(c, tx, ReceptionService.StudentForm, f.Guid, "ربط بشعبة", fileId.ToString());
        return new { message = "تم ربط الطالب بالشعبة بنجاح" };
    }, ct: ct);

    /* ---------------- Registration case ---------------- */
    public Task<object> ChangeCaseAsync(Guid account, CaseInput i, CancellationToken ct) => db.InTransactionAsync<object>(async (c, tx) =>
    {
        var st = await c.QueryFirstOrDefaultAsync<(Guid Guid, string Code, bool? Locked)>("SELECT Guid, Code, LockProfile FROM dbo.Student WHERE AccountGuid = @account", new { account }, tx);
        if (st.Code is null) throw ApiException.NotFound("الطالب غير موجود");
        if (st.Locked == true) throw new ApiException("ملف الطالب مغلق، يرجى التواصل مع الإدارة");
        var exists = await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Stauts WHERE Guid = @g AND IsUse = 1", new { g = i.CaseGuid }, tx);
        if (exists == 0) throw new ApiException("يرجى اختيار حالة التسجيل");
        if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب تغيير الحالة");
        await c.ExecuteAsync("dbo.ChangeStaut", new { Guid = account, RegCaseGuid = i.CaseGuid }, tx, commandType: CommandType.StoredProcedure);
        if (i.FileId is { } fid)
        {
            var n = await c.ExecuteAsync("UPDATE dbo.StudentLevel SET CaseGuid = @g WHERE Id = @fid AND StudentGuid = @account", new { g = i.CaseGuid, fid, account }, tx);
            if (n == 0) throw new ApiException("الملف التدريبي المختار لا يخص الطالب");
        }
        await audit.WriteAsync(c, tx, ReceptionService.StudentForm, st.Guid, "تغيير حالة التسجيل", st.Code, i.Reason.Trim());
        return new { message = "تم تغيير حالة الطالب بنجاح" };
    }, ct: ct);

    public Task<object> SetActiveAsync(Guid account, ActiveInput i, CancellationToken ct) => db.InTransactionAsync<object>(async (c, tx) =>
    {
        var st = await c.QueryFirstOrDefaultAsync<(Guid Guid, string Code, bool? Locked)>("SELECT Guid, Code, LockProfile FROM dbo.Student WHERE AccountGuid = @account", new { account }, tx);
        if (st.Code is null) throw ApiException.NotFound("الطالب غير موجود");
        if (st.Locked == true) throw new ApiException("ملف الطالب مغلق، يرجى التواصل مع الإدارة");
        if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب الإجراء");
        await c.ExecuteAsync("dbo.ChangeStudentStaut", new { AccountGuid = account, i.IsUse }, tx, commandType: CommandType.StoredProcedure);
        await audit.WriteAsync(c, tx, ReceptionService.StudentForm, st.Guid, i.IsUse ? "تفعيل الطالب" : "إيقاف الطالب", st.Code, i.Reason.Trim());
        return new { message = i.IsUse ? "تم تفعيل الطالب بنجاح" : "تم إيقاف الطالب عن التعامل" };
    }, ct: ct);

    /* ---------------- Document history ---------------- */
    /// <summary>Every audit entry on the student record, the files and every document of the student.</summary>
    public async Task<IEnumerable<HistoryRow>> HistoryAsync(Guid account, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync<HistoryRow>(new CommandDefinition("""
            WITH G AS (
                SELECT Guid AS g FROM dbo.Student WHERE AccountGuid = @account
                UNION SELECT @account
                UNION SELECT Guid FROM dbo.StudentLevel WHERE StudentGuid = @account
                UNION SELECT Guid FROM dbo.RegDoc WHERE AccountGuid = @account
                UNION SELECT Guid FROM dbo.Bill WHERE AccountGuid = @account
                UNION SELECT Guid FROM dbo.RegOtherFess WHERE AccountGuid = @account
                UNION SELECT Guid FROM dbo.ReRegDoc WHERE AccountGuid = @account
                UNION SELECT Guid FROM dbo.ReBill WHERE AccountGuid = @account
                UNION SELECT Guid FROM dbo.StudentDiscount WHERE AccountGuid = @account
                UNION SELECT D.DocGuid FROM dbo.Day1 D JOIN dbo.Day2 X ON X.Day1Guid = D.Guid WHERE X.AccountGuid = @account AND D.DocGuid IS NOT NULL
                UNION SELECT D.Guid FROM dbo.Day1 D JOIN dbo.Day2 X ON X.Day1Guid = D.Guid WHERE X.AccountGuid = @account
            )
            SELECT A.Date_ AS Date, A.Time_ AS Time, U.FullName AS UserName, A.Notes_ AS Action, A.ActionReason AS Reason, A.ActionCode AS Code, F.Name AS DocType
            FROM dbo.User_Action A JOIN G ON G.g = A.ActionGuid
            LEFT JOIN dbo.User_Info U ON U.Guid = A.UserGuid LEFT JOIN dbo.Form_Name F ON F.Guid = A.ActionTypeGuid
            ORDER BY A.Date_ DESC, A.Time_ DESC, A.Id DESC
            """, new { account }, cancellationToken: ct));
    }
}
