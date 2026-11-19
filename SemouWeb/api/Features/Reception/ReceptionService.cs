using System.Data;
using System.Data.Common;
using System.Security.Cryptography;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Reception;

/*
 * مكتب الاستقبال (MainFindStudent) + بطاقة الطالب (EncodingStudent) + قبول طالب من الأرشيف
 * (OldDataGrid → IfStudentNotRegister / Student2 / Student3, balances from S_SSTLI2024).
 */
public sealed class StudentInput
{
    public string StudentName { get; set; } = "";
    public string StudentTel { get; set; } = "";
    public string NationalId { get; set; } = "";
    public string? Email { get; set; }
    public Guid? BranchGuid { get; set; }
    public Guid? RegCaseGuid { get; set; }
    /// <summary>0 ذكر / 1 أنثى.</summary>
    public int? StudentType { get; set; }
    /// <summary>0 مواطن / 1 أجنبي.</summary>
    public int? StudentNational { get; set; }
    public bool IsUse { get; set; } = true;
    /// <summary>مسجل في معهد آخر (requires notes).</summary>
    public bool NoData { get; set; }
    public string? Notes { get; set; }
    public string? Reason { get; set; }
    /// <summary>Set when accepting a student from the old archive.</summary>
    public ArchiveAcceptance? Archive { get; set; }
}

/// <summary>Data gathered by the desktop IfStudentNotRegister / CourseStudentInfo dialogs.</summary>
public sealed class ArchiveAcceptance
{
    public string CustNo { get; set; } = "";
    /// <summary>Old system branch id (BRNID) used for the balance.</summary>
    public int OldBranchId { get; set; }
    /// <summary>0 دبلوم / 1 دورة.</summary>
    public int Type { get; set; }
    public Guid? DiplomGuid { get; set; }
    public Guid? BatchGuid { get; set; }
    public Guid? LevelGuid { get; set; }
    public Guid? CourseGuid { get; set; }
}

public sealed record StudentSaved(string Message, string Code, Guid Guid, Guid AccountGuid);

public sealed class ReceptionService(Db db, ArchiveDb archive, UserActionLog audit)
{
    public const string StudentForm = "addstudent";

    /* ============================ Search ============================ */

    /// <summary>MainFindStudent: one row per student file (StudentLevel), with the flags the desktop buttons check.</summary>
    public async Task<IEnumerable<dynamic>> SearchAsync(string? q, string by, Guid? branch, Guid? regCase, CancellationToken ct)
    {
        q = string.IsNullOrWhiteSpace(q) ? null : q.Trim();
        if (q is null && branch is null && regCase is null) return [];
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync(new CommandDefinition("""
            SELECT S.Code, S.Guid AS StudentGuid, S.AccountGuid, S.AcadmyId, S.StudentName, S.StudentTel, S.NationalId, S.Email,
                   CAST(ISNULL(S.IsUse,0) AS bit) AS IsUse, CAST(ISNULL(S.LockProfile,0) AS bit) AS Locked,
                   S.StudentType, S.StudentNational, RB.BrName AS RegBranchName, S.BarnchGuid AS RegBranchGuid, RC.StautName AS RegCaseName,
                   L.ID AS FileId, L.DiplomGuid, D.DiplomName, L.BranchGuid AS StudyBranchGuid, SB.BrName AS StudyBranchName, L.RegDocGuid,
                   LC.StautName AS CaseName, L.RegType, CAST(ISNULL(L.LevelStaut, 1) AS bit) AS LevelActive,
                   (SELECT CAST(ISNULL(SUM(ISNULL(X.Maden,0) - ISNULL(X.Daen,0)),0) AS decimal(18,2))
                      FROM dbo.Day2 X JOIN dbo.Day1 Y ON Y.Guid = X.Day1Guid WHERE X.AccountGuid = S.AccountGuid AND ISNULL(Y.IsUse,1) = 1 AND ISNULL(X.IsUse,1) = 1) AS Balance
            FROM dbo.Student S
            LEFT JOIN dbo.Branch RB ON RB.Guid = S.BarnchGuid
            LEFT JOIN dbo.Stauts RC ON RC.Guid = S.RegCaseGuid
            LEFT JOIN dbo.StudentLevel L ON L.StudentGuid = S.AccountGuid
            LEFT JOIN dbo.Diplom D ON D.Guid = L.DiplomGuid
            LEFT JOIN dbo.Branch SB ON SB.Guid = L.BranchGuid
            LEFT JOIN dbo.Stauts LC ON LC.Guid = L.CaseGuid
            WHERE (@nid IS NULL OR S.NationalId = @nid) AND (@tel IS NULL OR S.StudentTel = @tel)
              AND (@name IS NULL OR S.StudentName LIKE '%' + @name + '%')
              AND (@branch IS NULL OR S.BarnchGuid = @branch) AND (@regCase IS NULL OR S.RegCaseGuid = @regCase)
            ORDER BY TRY_CONVERT(int, S.Code), L.ID
            """, new
        {
            nid = by == "nid" ? q : null,
            tel = by == "tel" ? q : null,
            name = by == "name" ? q : null,
            branch, regCase,
        }, cancellationToken: ct));
    }

    /// <summary>FindStudentData in the archive (S_SSTLI2024), with the old balance.</summary>
    public async Task<IEnumerable<dynamic>> ArchiveSearchAsync(string q, string by, CancellationToken ct)
    {
        if (!archive.Configured) throw new ApiException("قاعدة بيانات الأرشيف غير مضبوطة على الخادم");
        await using var c = await archive.OpenAsync(ct);
        var rows = await c.QueryAsync(new CommandDefinition("dbo.FindStudentData", new
        {
            PROOFNO = by == "nid" ? q.Trim() : null,
            CUSTMOBILE = by == "tel" ? q.Trim() : null,
            StudentName = by == "name" ? q.Trim() : null,
        }, commandType: CommandType.StoredProcedure, cancellationToken: ct));

        // Mark the ones already accepted into the new system (same national id).
        var list = rows.Select(r => (IDictionary<string, object?>)r).ToList();
        var ids = list.Select(r => r["PROOFNO"]?.ToString()).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
        await using var s = await db.OpenAsync(ct);
        var existing = ids.Count == 0 ? new HashSet<string>() :
            (await s.QueryAsync<string>("SELECT NationalId FROM dbo.Student WHERE NationalId IN @ids", new { ids })).ToHashSet();
        return list.Select(r => new
        {
            Name = r["CUSTNAME"]?.ToString(), Mobile = r["CUSTMOBILE"]?.ToString(), NationalId = r["PROOFNO"]?.ToString(),
            BranchName = r["BRNNAME"]?.ToString(), AccountName = r["ACCTNAME"]?.ToString(), OldBranchId = Convert.ToInt32(r["BRNID"]),
            CustNo = r["CUSTNO"]?.ToString(), Balance = Math.Round(Convert.ToDecimal(r["BALANCE"] ?? 0), 2),
            Accepted = existing.Contains(r["PROOFNO"]?.ToString() ?? ""),
        });
    }

    /// <summary>CalcCustBAlance in the archive: debit/credit opening balance for an old customer.</summary>
    private async Task<(decimal Maden, decimal Daen)> ArchiveBalanceAsync(string custNo, int oldBranchId)
    {
        await using var c = await archive.OpenAsync();
        var row = await c.QueryFirstOrDefaultAsync("dbo.CalcCustBAlance", new { CustNo = custNo, BrnId = oldBranchId }, commandType: CommandType.StoredProcedure);
        if (row is null) return (0, 0);
        var bal = Math.Round(Convert.ToDecimal(((IDictionary<string, object?>)row)["BALANCE"] ?? 0), 2);
        return bal > 0 ? (bal, 0) : (0, Math.Abs(bal));
    }

    public async Task<object> ArchivePreviewAsync(string custNo, int oldBranchId, string nationalId, CancellationToken ct)
    {
        var (maden, daen) = await ArchiveBalanceAsync(custNo, oldBranchId);
        await using var c = await db.OpenAsync(ct);
        // Student2 (diplomas) / Student3 (courses): data prepared during the old migration.
        var diploma = await c.QueryFirstOrDefaultAsync("dbo.FoundStudentInOld", new { NationalCode = nationalId }, commandType: CommandType.StoredProcedure);
        var courses = await c.QueryAsync("SELECT Branch, CourseName, DateStart, DateEnd, CourseStaut, TypeNotes, Notes FROM dbo.Student3 WHERE NationalId = @nationalId AND ISNULL(IsUsed,0) = 0", new { nationalId });
        return new { maden, daen, diploma, courses };
    }

    /* ============================ Student card ============================ */

    public async Task<object> GetStudentAsync(Guid guid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryFirstOrDefaultAsync(new CommandDefinition("""
            SELECT S.Code, S.Guid, S.AcadmyId, S.StudentName, S.StudentTel, S.NationalId, S.Email, S.BarnchGuid AS BranchGuid, S.RegCaseGuid,
                   S.StudentType, S.StudentNational, CAST(ISNULL(S.IsUse,0) AS bit) AS IsUse, CAST(ISNULL(S.NoData,0) AS bit) AS NoData, S.Notes,
                   S.AccountGuid, CAST(A.Code AS nvarchar(50)) AS AccountCode, CAST(ISNULL(S.LockProfile,0) AS bit) AS Locked
            FROM dbo.Student S LEFT JOIN dbo.Account A ON A.Guid = S.AccountGuid WHERE S.Guid = @guid
            """, new { guid }, cancellationToken: ct)) ?? throw ApiException.NotFound("الطالب غير موجود");
    }

    /// <summary>VALIDATE_DATA + StudentExists + StudentMobileExists.</summary>
    private static async Task ValidateAsync(DbConnection c, DbTransaction tx, StudentInput i, Guid? self)
    {
        i.StudentName = i.StudentName.Trim(); i.StudentTel = i.StudentTel.Trim(); i.NationalId = i.NationalId.Trim();
        if (i.StudentName.Length == 0) throw new ApiException("يرجى إدخال اسم الطالب");
        if (i.StudentName.Length > 150) throw new ApiException("اسم الطالب طويل جداً");
        if (i.StudentTel.Length == 0) throw new ApiException("يرجى إدخال رقم الجوال");
        if (i.StudentTel.Length != 10 || !i.StudentTel.All(char.IsDigit)) throw new ApiException("يرجى إدخال رقم جوال صحيح");
        if (i.NationalId.Length == 0) throw new ApiException("يرجى إدخال رقم الهوية");
        if (i.NationalId.Length != 10 || !i.NationalId.All(char.IsDigit) || i.NationalId.StartsWith('0')) throw new ApiException("يرجى إدخال رقم هوية صحيح");
        if (i.BranchGuid is null) throw new ApiException("يرجى اختيار الفرع القائم بتسجيل الطالب");
        if (i.RegCaseGuid is null) throw new ApiException("يرجى اختيار حالة تسجيل الطالب");
        if (string.IsNullOrWhiteSpace(i.Email)) throw new ApiException("يرجى إدخال البريد الإلكتروني الخاص بالطالب");
        if (i.Email.Trim().Length > 150) throw new ApiException("البريد الإلكتروني طويل جداً");
        if (i.StudentType is not (0 or 1)) throw new ApiException("يرجى تحديد نوع الطالب");
        if (i.StudentNational is not (0 or 1)) throw new ApiException("يرجى تحديد جنسية الطالب");
        if (i.NoData && string.IsNullOrWhiteSpace(i.Notes)) throw new ApiException("يرجى إدخال بيانات المعهد الآخر المسجل به الطالب");
        if ((i.Notes?.Length ?? 0) > 250) throw new ApiException("الملاحظات طويلة جداً (الحد الأقصى 250 حرف)");

        if (await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Student WHERE NationalId = @n AND (@self IS NULL OR Guid <> @self)", new { n = i.NationalId, self }, tx) > 0)
            throw new ApiException("رقم الهوية مستخدم مسبقاً");
        if (await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Student WHERE StudentTel = @t AND (@self IS NULL OR Guid <> @self)", new { t = i.StudentTel, self }, tx) > 0)
            throw new ApiException("رقم الجوال مستخدم مسبقاً");
    }

    private static object StudentParams(StudentInput i) => new
    {
        i.StudentName, i.StudentTel, i.NationalId, i.RegCaseGuid, BarnchGuid = i.BranchGuid, i.IsUse, Notes = i.Notes?.Trim() ?? "",
        i.NoData, Email = i.Email!.Trim(), i.StudentType, i.StudentNational,
    };

    public async Task<StudentSaved> CreateAsync(StudentInput i, CancellationToken ct)
    {
        // Archive acceptance: validate the extra data and read the old balance before opening the transaction.
        decimal maden = 0, daen = 0;
        if (i.Archive is { } a)
        {
            if (a.Type == 0 && (a.DiplomGuid is null || a.BatchGuid is null || a.LevelGuid is null))
                throw new ApiException(a.DiplomGuid is null ? "يرجى اختيار الدبلوم أولاً" : a.BatchGuid is null ? "يرجى اختيار الدفعة أولاً" : "يرجى اختيار المستوى أولاً");
            if (a.Type == 1 && (a.CourseGuid is null || a.DiplomGuid is null)) throw new ApiException("يرجى اختيار الدورة أولاً");
            if (a.Type is not (0 or 1)) throw new ApiException("يرجى اختيار نوع التسجيل أولاً");
            (maden, daen) = await ArchiveBalanceAsync(a.CustNo, a.OldBranchId);
        }

        return await db.InTransactionAsync(async (c, tx) =>
        {
            await ValidateAsync(c, tx, i, null);

            // Parent "students" account from إعدادات الحسابات (DepGuid 10), like CHEKPARENTINFO("10").
            var parent = await c.QueryFirstOrDefaultAsync<(Guid Guid, string Code, int? EndAccount, Guid? OpenAccount)>("""
                SELECT A.Guid, CAST(A.Code AS nvarchar(50)), A.EndAccount, S.AccountOpenGuid FROM dbo.AccountSetting S JOIN dbo.Account A WITH (UPDLOCK) ON A.Guid = S.AccountGuid
                WHERE S.DepGuid = 10 ORDER BY S.ID DESC
                """, transaction: tx);
            if (parent.Code is null) throw new ApiException("لا يمكن إضافة طالب لعدم تحديد الحساب الرئيسي، يرجى ضبط إعدادات الحسابات");

            // 1) Ledger account (ADDACCOUNT): sub account, hidden from the chart (IsShow = 0).
            var max = await c.ExecuteScalarAsync<long?>("SELECT MAX(CAST(BacicCode AS bigint)) FROM dbo.Account WITH (UPDLOCK, HOLDLOCK) WHERE ParentGuid = @p", new { p = parent.Guid }, tx);
            var basic = max.HasValue ? max.Value + 1 : 1 + long.Parse(parent.Code) * 100;
            if (basic > int.MaxValue) throw new ApiException("تم الوصول للحد الأقصى لحسابات الطلاب");
            var accCode = basic.ToString();
            await c.ExecuteAsync("dbo.AddBasicAccount", new
            {
                BacicCode = (int)basic, Code = accCode, AccountName = i.StudentName, AccountKind = true, AccountType = 1, EndAccount = parent.EndAccount ?? 0,
                ParentGuid = parent.Guid, Maden = (double)maden, Daen = (double)daen, IsShow = false, Type_ = 2,
            }, tx, commandType: CommandType.StoredProcedure);
            var accountGuid = await c.ExecuteScalarAsync<Guid>("SELECT TOP 1 Guid FROM dbo.Account WHERE CAST(Code AS nvarchar(50)) = @accCode AND ParentGuid = @p", new { accCode, p = parent.Guid }, tx);

            // 2) Student (ADDSTUDENT). AcadmyId = "S" + random number, like CREATEOTHERID.
            var id = await c.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(ID), 0) + 1 FROM dbo.Student WITH (UPDLOCK, HOLDLOCK)", transaction: tx);
            var p = new DynamicParameters(StudentParams(i));
            p.Add("ID", id); p.Add("Code", id.ToString()); p.Add("AcadmyId", "S" + RandomNumberGenerator.GetInt32(int.MaxValue)); p.Add("AccountGuid", accountGuid);
            await c.ExecuteAsync("dbo.AddStudent", p, tx, commandType: CommandType.StoredProcedure);
            var guid = await c.ExecuteScalarAsync<Guid>("SELECT Guid FROM dbo.Student WHERE ID = @id", new { id }, tx);

            if (i.Archive is { } arc)
            {
                var costGuid = await c.ExecuteScalarAsync<Guid?>("SELECT CostCenterGuid FROM dbo.Branch WHERE Guid = @b", new { b = i.BranchGuid }, tx) ?? Guid.Empty;

                // 3) Opening balance from the archive (ADDDAY1 + ADDPAYDAY2_*), against the "open" account of the setting.
                if (maden != 0 || daen != 0)
                {
                    if (parent.OpenAccount is null || parent.OpenAccount == Guid.Empty) throw new ApiException("لم يتم تحديد حساب الأرصدة الافتتاحية في إعدادات الحسابات");
                    var openType = await c.ExecuteScalarAsync<Guid>("dbo.FormTypeGuid", new { FormName = "opendaily" }, tx, commandType: CommandType.StoredProcedure);
                    var notes = ("رصيد افتتاحي للطالب " + (i.Notes ?? "")).Trim();
                    if (notes.Length > 200) notes = notes[..200];
                    var d = new DynamicParameters(new { DayDate = Clock.Now, Notes = notes, DayType = false, DocGuid = guid, IsUse = true, TypeGuid = openType, BranchGuid = i.BranchGuid });
                    d.Add("CodeOut", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    await c.ExecuteAsync("dbo.AddOpenDay1", d, tx, commandType: CommandType.StoredProcedure);
                    var day1 = await c.ExecuteScalarAsync<Guid>("SELECT TOP 1 Guid FROM dbo.Day1 WHERE DocGuid = @guid AND TypeGuid = @openType ORDER BY ID DESC", new { guid, openType }, tx);
                    async Task line(Guid acc, decimal m, decimal dd, string n) => await c.ExecuteAsync("dbo.AddDay2",
                        new { Day1Guid = day1, AccountGuid = acc, Maden = (double)m, Daen = (double)dd, Notes = n, CostGuid = costGuid, IsUse = true }, tx, commandType: CommandType.StoredProcedure);
                    if (maden != 0) { await line(accountGuid, maden, 0, notes); await line(parent.OpenAccount.Value, 0, maden, notes); }
                    else { await line(parent.OpenAccount.Value, daen, 0, "الرصيد الافتتاحي للطالب"); await line(accountGuid, 0, daen, "الرصيد الافتتاحي للطالب"); }
                }

                // 4) Student file (ADDSTUDENTLEVEL / ADDSTUDENTCOURSE).
                await c.ExecuteAsync("dbo.AddStudentLevel", new
                {
                    StudentGuid = accountGuid, RegDocGuid = Guid.Empty, LevelGuid = arc.Type == 0 ? arc.LevelGuid : Guid.Empty, LevelMark = 0.0, LevelStaut = true,
                    BatchGuid = arc.Type == 0 ? arc.BatchGuid : Guid.Empty, arc.DiplomGuid, BranchGuid = i.BranchGuid,
                    CourseGuid = arc.Type == 1 ? arc.CourseGuid : Guid.Empty, CaseGuid = i.RegCaseGuid, RegType = arc.Type,
                }, tx, commandType: CommandType.StoredProcedure);

                // 5) Mark the migration rows as used (UPDATESTUEDNT2 / UPDATESTUEDNT3).
                await c.ExecuteAsync("dbo.UpdatStudent2", new { NationalId = i.NationalId }, tx, commandType: CommandType.StoredProcedure);
            }

            await audit.WriteAsync(c, tx, StudentForm, guid, i.Archive is null ? UserActionLog.New : "قبول من الأرشيف", id.ToString());
            return new StudentSaved(i.Archive is null ? "تم حفظ بيانات الطالب بنجاح" : "تم قبول الطالب من الأرشيف بنجاح", id.ToString(), guid, accountGuid);
        }, IsolationLevel.Serializable, ct);
    }

    public async Task<StudentSaved> UpdateAsync(Guid guid, StudentInput i, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب الإجراء أولاً");
        return await db.InTransactionAsync(async (c, tx) =>
        {
            var cur = await c.QueryFirstOrDefaultAsync<(string Code, Guid AccountGuid)>("SELECT CAST(Code AS nvarchar(50)), AccountGuid FROM dbo.Student WITH (UPDLOCK) WHERE Guid = @guid", new { guid }, tx);
            if (cur.Code is null) throw ApiException.NotFound("الطالب غير موجود");
            await ValidateAsync(c, tx, i, guid);
            var p = new DynamicParameters(StudentParams(i));
            p.Add("Guid", guid);
            await c.ExecuteAsync("dbo.UpdateStudent", p, tx, commandType: CommandType.StoredProcedure);
            await c.ExecuteAsync("dbo.UpdateAccount", new { Guid = cur.AccountGuid, AccountName = i.StudentName, i.IsUse }, tx, commandType: CommandType.StoredProcedure);
            await audit.WriteAsync(c, tx, StudentForm, guid, UserActionLog.Update, cur.Code, i.Reason!.Trim());
            return new StudentSaved("تم تعديل بيانات الطالب بنجاح", cur.Code, guid, cur.AccountGuid);
        }, ct: ct);
    }

    /* ============================ Lookups for the acceptance dialog ============================ */

    public async Task<object> LookupsAsync(Guid? branch, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return new
        {
            cases = await c.QueryAsync("SELECT Guid, StautName AS Name FROM dbo.Stauts WHERE IsUse = 1 ORDER BY TRY_CONVERT(int, Code)"),
            diploms = await c.QueryAsync("SELECT Guid, DiplomName AS Name, StudyType FROM dbo.Diplom WHERE IsUse = 1 AND ISNULL(DiplomType,0) = 0 ORDER BY TRY_CONVERT(int, Code)"),
            batches = await c.QueryAsync("SELECT Guid, BatchName AS Name FROM dbo.Batch WHERE IsUse = 1 ORDER BY ID DESC"),
            levels = await c.QueryAsync("SELECT Guid, LevelName AS Name FROM dbo.Levels WHERE IsUse = 1 ORDER BY TRY_CONVERT(int, Code)"),
            courses = await c.QueryAsync("""
                SELECT C.Guid, D.DiplomName + ISNULL(' (' + C.CourseName + ')', '') AS Name, C.DiplomGuid, C.DateStart, C.DateEnd
                FROM dbo.Course C JOIN dbo.Diplom D ON D.Guid = C.DiplomGuid
                WHERE C.IsUse = 1 AND (@branch IS NULL OR C.BranchGuid = @branch) ORDER BY TRY_CONVERT(int, C.Code) DESC
                """, new { branch }),
        };
    }
}
