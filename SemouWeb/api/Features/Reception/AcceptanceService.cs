using System.Data;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Reception;

/*
 * قبول التسجيل (NewStudentHome / "قائمة الطلاب الجدد").
 * A registration form (RegDoc) does NOT give the student a training file by itself: the file (StudentLevel)
 * is created when a supervisor accepts the form, exactly like the desktop CmdAccept / CmdAccept2:
 *   AddStudentLevel + AddRegDocNotes + AcceptRegDoc (Linked = 1) + Add_UserAction("قبول التسجيل").
 */
public sealed class AcceptInput
{
    /// <summary>Only needed when an old form holds more than one diploma.</summary>
    public Guid? DiplomGuid { get; set; }
    /// <summary>مستندات التسجيل مكتملة.</summary>
    public bool AttachOk { get; set; }
    public string? AttachNotes { get; set; }
    /// <summary>اتفاقية التدريب.</summary>
    public bool ContractOk { get; set; }
    public string? ContractNotes { get; set; }
}

public sealed record PendingDiplom(Guid DiplomGuid, string DiplomName);

public sealed class PendingReg
{
    public Guid Guid { get; set; }
    public string Code { get; set; } = "";
    public DateTime RegDate { get; set; }
    public int RegType { get; set; }
    public decimal DocSubTotal { get; set; }
    public decimal Paid { get; set; }
    public string? PayCode { get; set; }
    public string? BranchName { get; set; }
    public string? BatchName { get; set; }
    public string? LevelName { get; set; }
    public string? CourseName { get; set; }
    public string? SellerName { get; set; }
    public List<PendingDiplom> Diploms { get; set; } = [];
}

public sealed class AcceptanceService(Db db, UserActionLog audit)
{
    public const string Form = "newstudenthome";
    private const string RegForm = "addregdoc";

    // Same filter as LoadStudentForAccept: not accepted yet, not a re-registration, not a zero form.
    private const string PendingSql = """
        SELECT R.Guid, R.Code, R.RegDate, ISNULL(R.RegType,0) AS RegType, CAST(ISNULL(R.DocSubTotal,0) AS decimal(18,2)) AS DocSubTotal,
               CAST(ISNULL((SELECT SUM(B.BillTotal) FROM dbo.Bill B WHERE B.Guid = R.PayGuid),0) AS decimal(18,2)) AS Paid, R.PayCode,
               BR.BrName AS BranchName, BT.BatchName, LV.LevelName, CO.CourseName, SM.ManFullName AS SellerName
        FROM dbo.RegDoc R
        LEFT JOIN dbo.Branch BR ON BR.Guid = R.BranchGuid
        LEFT JOIN dbo.Batch BT ON BT.Guid = R.BatchGuid
        LEFT JOIN dbo.Levels LV ON LV.Guid = R.LevelGuid
        LEFT JOIN dbo.Course CO ON CO.Guid = R.CourseGuid
        LEFT JOIN dbo.SalesMan SM ON SM.Guid = R.SellerGuid
        WHERE R.AccountGuid = @account AND ISNULL(R.Linked,0) = 0 AND ISNULL(R.IsReReg,0) = 0 AND R.DocTotal <> 0
        ORDER BY TRY_CONVERT(int, R.Code)
        """;

    /// <summary>Registration forms of this student still waiting for acceptance.</summary>
    public async Task<IEnumerable<PendingReg>> PendingAsync(Guid accountGuid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var regs = (await c.QueryAsync<PendingReg>(new CommandDefinition(PendingSql, new { account = accountGuid }, cancellationToken: ct))).ToList();
        if (regs.Count == 0) return regs;
        var dips = await c.QueryAsync<(Guid RegDocGuid, Guid DiplomGuid, string DiplomName)>(new CommandDefinition("""
            SELECT RD.RegDocGuid, RD.DiplomGuid, D.DiplomName FROM dbo.RegDocDiplom RD JOIN dbo.Diplom D ON D.Guid = RD.DiplomGuid
            WHERE RD.RegDocGuid IN @ids AND RD.DiplomType = '0' AND RD.Type_ = '0'
            """, new { ids = regs.Select(r => r.Guid).ToList() }, cancellationToken: ct));
        foreach (var r in regs) r.Diploms = dips.Where(d => d.RegDocGuid == r.Guid).Select(d => new PendingDiplom(d.DiplomGuid, d.DiplomName)).ToList();
        return regs;
    }

    public Task<object> AcceptAsync(Guid regDocGuid, AcceptInput i, CancellationToken ct)
    {
        if (!i.AttachOk) throw new ApiException("يرجى تحديد حالة مستندات التسجيل");
        if (!i.ContractOk) throw new ApiException("يرجى تحديد حالة اتفاقية التدريب");
        return db.InTransactionAsync<object>(async (c, tx) =>
        {
            var r = await c.QueryFirstOrDefaultAsync<(Guid Guid, string Code, Guid AccountGuid, int RegType, Guid? LevelGuid, Guid? BatchGuid, Guid BranchGuid, Guid? CourseGuid, bool Linked, bool IsReReg, decimal DocTotal)>("""
                SELECT Guid, Code, AccountGuid, ISNULL(RegType,0), LevelGuid, BatchGuid, BranchGuid, CourseGuid,
                       CAST(ISNULL(Linked,0) AS bit), CAST(ISNULL(IsReReg,0) AS bit), CAST(ISNULL(DocTotal,0) AS decimal(18,2))
                FROM dbo.RegDoc WITH (UPDLOCK, HOLDLOCK) WHERE Guid = @g
                """, new { g = regDocGuid }, tx);
            if (r.Code is null) throw ApiException.NotFound("استمارة التسجيل غير موجودة");
            if (r.Linked) throw new ApiException($"استمارة التسجيل رقم {r.Code} تم قبولها من قبل");
            if (r.IsReReg || r.DocTotal == 0) throw new ApiException("هذه الاستمارة لا تحتاج إلى قبول");

            var dips = (await c.QueryAsync<Guid>("SELECT DiplomGuid FROM dbo.RegDocDiplom WHERE RegDocGuid = @g AND DiplomType = '0' AND Type_ = '0'", new { g = r.Guid }, tx)).Distinct().ToList();
            if (dips.Count == 0) throw new ApiException("الاستمارة لا تحتوي على دبلوم");
            Guid diplom;
            if (i.DiplomGuid is { } chosen) diplom = dips.Contains(chosen) ? chosen : throw new ApiException("الدبلوم المختار غير موجود في الاستمارة");
            else if (dips.Count == 1) diplom = dips[0];
            else throw new ApiException("الاستمارة تحتوي على أكثر من دبلوم، يرجى اختيار الدبلوم الذي سيدرسه الطالب");

            if (r.RegType == 1 && r.CourseGuid is null) throw new ApiException("استمارة الدورة بدون دورة محددة");
            if (r.RegType == 0 && (r.LevelGuid is null || r.BatchGuid is null)) throw new ApiException("الاستمارة بدون دفعة أو مستوى");
            var caseGuid = await c.ExecuteScalarAsync<Guid?>("SELECT RegCaseGuid FROM dbo.Student WHERE AccountGuid = @a", new { a = r.AccountGuid }, tx);

            await c.ExecuteAsync("dbo.AddStudentLevel", new
            {
                StudentGuid = r.AccountGuid,
                RegDocGuid = r.Guid,
                LevelGuid = r.RegType == 0 ? r.LevelGuid : Guid.Empty,
                LevelMark = 0d,
                LevelStaut = true,
                BatchGuid = r.RegType == 0 ? r.BatchGuid : Guid.Empty,
                DiplomGuid = diplom,
                BranchGuid = r.BranchGuid,
                CourseGuid = r.RegType == 1 ? r.CourseGuid : Guid.Empty,
                CaseGuid = caseGuid ?? Guid.Empty,
                RegType = r.RegType,
            }, tx, commandType: CommandType.StoredProcedure);
            await c.ExecuteAsync("dbo.AddRegDocNotes", new
            {
                RegDocGuid = r.Guid,
                ChkAttach = 1L,
                AttachNotes = (i.AttachNotes ?? "").Trim() is var a && a.Length > 250 ? a[..250] : a,
                ChlAccept = true,
                AcceptNotes = (i.ContractNotes ?? "").Trim() is var n && n.Length > 250 ? n[..250] : n,
            }, tx, commandType: CommandType.StoredProcedure);
            await c.ExecuteAsync("dbo.AcceptRegDoc", new { Guid = r.Guid }, tx, commandType: CommandType.StoredProcedure);
            await audit.WriteAsync(c, tx, RegForm, r.Guid, "قبول التسجيل", r.Code);
            return new { message = $"تم قبول تسجيل الطالب بنجاح (استمارة {r.Code})، وتم فتح الملف التدريبي", code = r.Code };
        }, IsolationLevel.Serializable, ct);
    }
}
