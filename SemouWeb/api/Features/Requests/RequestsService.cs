using System.Data;
using System.Data.Common;
using System.Text.Json;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Features.Registration;
using Semou.Api.Features.Sales;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Requests;

/*
 * دورة المراجعة على الويب (web only; the desktop keeps saving directly):
 *
 *  طلب التحاق (WebRegOrder): the registration wizard sends a request instead of saving. It goes through every check
 *  of the save (RegistrationService.PlanAsync) and holds a seat. A reviewer in "المبيعات › طلبات الالتحاق" approves it:
 *  the same checks run again and the registration form, its journal and the down-payment invoice + receipt are created
 *  (RegistrationService.PostAsync) under the author's name. Or the reviewer rejects it with a reason.
 *
 *  طلب سداد (WebPayOrder): the payment invoice sends a request with the transfer images. A reviewer in
 *  "الحسابات العامة › طلبات السداد" may correct the transfer data, then confirms it: the tax invoice + receipt + journals
 *  + ZATCA queue are created (PayInvoiceService.PostAsync) and the images go to the archive with the invoice number.
 *  Or it is marked duplicate / cancelled with a reason.
 *
 *  Documents are dated when approved (a tax invoice is issued on the day it is confirmed).
 */
public sealed record StudentRequest(string Kind, Guid Guid, int Code, DateTime Date, int Status, string StatusName, string? Title, decimal Amount, string? Notes, string? Doc);

public sealed class RequestsService(Db db, CurrentUser me, RegistrationService reg, PayInvoiceService pay, AttachmentService attachments, ILogger<RequestsService> log)
{
    public const string OfficeCash = "صندوق المكتب";
    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };
    private static string Cut(string? s, int max) => s is null ? "" : s.Length <= max ? s : s[..max];

    public static string RegStatusName(int s) => s switch { 0 => "بانتظار المراجعة", 1 => "مقبول", 2 => "مرفوض", 3 => "بانتظار تأكيد السداد", _ => "غير معروف" };
    public static string PayStatusName(int s) => s switch { 0 => "بانتظار المراجعة", 1 => "مؤكد", 2 => "مكرر", 3 => "ملغي", _ => "غير معروف" };

    private Task LogAsync(DbConnection c, DbTransaction tx, Guid order, string action, string? notes = null) =>
        c.ExecuteAsync("INSERT INTO dbo.WebOrderLog (OrderGuid, Action, UserGuid, At, Notes) VALUES (@order, @action, @u, @at, @notes)",
            new { order, action, u = me.UserGuid, at = Clock.Now, notes = Cut(notes, 1000) }, tx);

    private static Task<string?> NameAsync(DbConnection c, DbTransaction tx, string sql, Guid? g) =>
        g is null || g == Guid.Empty ? Task.FromResult<string?>(null) : c.ExecuteScalarAsync<string?>(sql, new { g }, tx);
    private const string CashSql = "SELECT CashName FROM dbo.CashBox WHERE AccountGuid = @g";
    private const string CostSql = "SELECT CenterName FROM dbo.CostCenter WHERE Guid = @g";

    /* =============================== Files =============================== */

    private static string StageKey(Guid user, string nationalId) => $"{user}:{new string(nationalId.Where(char.IsDigit).ToArray())}";

    /// <summary>Keeps a copy of the registration attachments (uploaded before the request is sent) for the reviewer.</summary>
    public async Task StageAsync(string nationalId, IReadOnlyList<(string Code, IFormFile File)> files, CancellationToken ct)
    {
        var key = StageKey(me.UserGuid, nationalId);
        await using var c = await db.OpenAsync(ct);
        foreach (var (code, f) in files)
        {
            await using var ms = new MemoryStream();
            await f.CopyToAsync(ms, ct);
            await c.ExecuteAsync(new CommandDefinition("""
                DELETE FROM dbo.WebOrderFile WHERE StageKey = @key AND Code = @code;
                INSERT INTO dbo.WebOrderFile (StageKey, Code, FileName, ContentType, Size, Data, UserGuid, CreatedAt)
                VALUES (@key, @code, @name, @type, @size, @data, @u, @at)
                """, new { key, code = Cut(code, 30), name = Cut(f.FileName, 200), type = Cut(f.ContentType ?? "application/octet-stream", 100), size = (int)ms.Length, data = ms.ToArray(), u = me.UserGuid, at = Clock.Now }, cancellationToken: ct));
        }
    }

    public async Task<(byte[] Data, string Type, string Name)> FileAsync(Guid file, Func<Guid, string, Task<bool>> canSee, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var f = await c.QueryFirstOrDefaultAsync<(Guid? OrderGuid, Guid UserGuid, string FileName, string ContentType, byte[] Data)>(new CommandDefinition(
            "SELECT OrderGuid, UserGuid, FileName, ContentType, Data FROM dbo.WebOrderFile WHERE Guid = @file", new { file }, cancellationToken: ct));
        if (f.Data is null) throw ApiException.NotFound("الملف غير موجود");
        var kind = f.OrderGuid is { } og
            ? await c.ExecuteScalarAsync<string?>("SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.WebRegOrder WHERE Guid = @og) THEN 'reg' WHEN EXISTS (SELECT 1 FROM dbo.WebPayOrder WHERE Guid = @og) THEN 'pay' END", new { og }) ?? ""
            : "";
        if (f.UserGuid != me.UserGuid && !await canSee(f.OrderGuid ?? Guid.Empty, kind)) throw new ApiException("لا تملك صلاحية عرض هذا الملف", 403);
        return (f.Data, f.ContentType, f.FileName);
    }

    private static async Task<IEnumerable<object>> FilesOfAsync(DbConnection c, Guid order) =>
        (await c.QueryAsync<(Guid Guid, string Code, string FileName, string ContentType, int Size)>(
            "SELECT Guid, Code, FileName, ContentType, Size FROM dbo.WebOrderFile WHERE OrderGuid = @order ORDER BY ID", new { order }))
        .Select(f => (object)new { f.Guid, f.Code, name = f.FileName, type = f.ContentType, f.Size });

    private static async Task<IEnumerable<object>> LogOfAsync(DbConnection c, Guid order) =>
        await c.QueryAsync("""
            SELECT L.Action AS action, L.At AS at, L.Notes AS notes, U.FullName AS [user]
            FROM dbo.WebOrderLog L LEFT JOIN dbo.User_Info U ON U.Guid = L.UserGuid WHERE L.OrderGuid = @order ORDER BY L.At, L.ID
            """, new { order });

    /// <summary>The student at a glance: balance (statement rule), training files, previous registrations.</summary>
    private static async Task<object?> StudentOfAsync(DbConnection c, Guid account)
    {
        var s = await c.QueryFirstOrDefaultAsync("""
            SELECT S.Guid AS guid, S.AccountGuid AS accountGuid, S.StudentName AS name, S.NationalId AS nationalId, S.StudentTel AS tel, S.AcadmyId AS academyId, B.BrName AS branch,
                   CAST(ISNULL(S.IsUse,1) AS bit) AS active,
                   (SELECT CAST(ISNULL(SUM(CASE WHEN F.UnqName <> 'salesinvoice' THEN ISNULL(X.Maden,0) ELSE 0 END) - SUM(CASE WHEN F.UnqName <> 'repay' THEN ISNULL(X.Daen,0) ELSE 0 END),0) AS decimal(18,2))
                      FROM dbo.Day2 X JOIN dbo.Day1 Y ON Y.Guid = X.Day1Guid JOIN dbo.Form_Name F ON F.Guid = Y.TypeGuid WHERE X.AccountGuid = S.AccountGuid AND Y.IsUse = 1) AS balance,
                   (SELECT COUNT(*) FROM dbo.RegDoc R WHERE R.AccountGuid = S.AccountGuid AND ISNULL(R.IstUse,1) = 1) AS registrations
            FROM dbo.Student S LEFT JOIN dbo.Branch B ON B.Guid = S.BarnchGuid WHERE S.AccountGuid = @account
            """, new { account });
        if (s is null) return null;
        var files = await c.QueryAsync("""
            SELECT D.DiplomName AS program, BT.BatchName AS batch, LV.LevelName AS level, CAST(ISNULL(L.LevelStaut,1) AS bit) AS active, ISNULL(L.RegType,0) AS regType
            FROM dbo.StudentLevel L LEFT JOIN dbo.Diplom D ON D.Guid = L.DiplomGuid LEFT JOIN dbo.Batch BT ON BT.Guid = L.BatchGuid LEFT JOIN dbo.Levels LV ON LV.Guid = L.LevelGuid
            WHERE L.StudentGuid = @account AND ISNULL(L.IsUse,1) = 1 ORDER BY L.ID DESC
            """, new { account });
        return new { info = s, files };
    }

    private static async Task<int> NextCodeAsync(DbConnection c, DbTransaction tx, string table) =>
        await c.ExecuteScalarAsync<int>($"SELECT ISNULL(MAX(Code),0) + 1 FROM dbo.{table} WITH (UPDLOCK, HOLDLOCK)", transaction: tx);

    /* =============================== طلب التحاق =============================== */

    private async Task<RegSnapshot> RegSnapshotAsync(DbConnection c, DbTransaction tx, RegistrationInput i, RegPlan p)
    {
        var level = await NameAsync(c, tx, "SELECT LevelName FROM dbo.Levels WHERE Guid = @g", i.LevelGuid);
        var course = i.CourseGuid is { } cg
            ? await c.ExecuteScalarAsync<string?>("SELECT D.DiplomName + N' (' + ISNULL(C.CourseName,'') + N')' FROM dbo.Course C JOIN dbo.Diplom D ON D.Guid = C.DiplomGuid WHERE C.Guid = @cg", new { cg }, tx)
            : null;
        return new RegSnapshot
        {
            Book = p.Setting.DocName, BranchName = p.Setting.BranchName, Batch = p.BatchName, Level = level, Course = course,
            Seller = await NameAsync(c, tx, "SELECT ManFullName FROM dbo.SalesMan WHERE Guid = @g", i.SellerGuid),
            Plan = p.Plan is { } pl ? pl.Name + (pl.Months > 0 ? $" • دفعة مقدمة {pl.DownPayment:#,0.##} • قسط {pl.Installment:#,0.##} × {pl.Months}" : " • سداد كامل") : null,
            Items = p.Diploms.Select(d => new SnapItem(d.Name, 0, d.Cost, d.Tax, d.Total)).Concat(p.Fees.Select(f => new SnapItem(f.Name, 1, f.Cost, f.Tax, f.Total))).ToList(),
            Total = p.FTotal, Tax = p.FTax, SubTotal = p.FSub,
            HasPayment = p.Pay != null, PayLines = p.PayLines.Select(l => new SnapPayLine(l.Item.Name, l.Cost, l.Tax, l.Total)).ToList(), PaySub = p.PaySub,
            Cash = p.CashName, Cost = await NameAsync(c, tx, CostSql, p.Pay?.CostCenterGuid),
            Cash2 = p.Pay?.Split == true ? p.CashName2 : null, Cost2 = p.Pay?.Split == true ? await NameAsync(c, tx, CostSql, p.Pay.CostCenterGuid2) : null,
        };
    }

    public async Task<object> CreateRegAsync(RegistrationInput i, CancellationToken ct)
    {
        RegistrationService.Normalize(i);
        return await db.InTransactionAsync(async (c, tx) =>
        {
            var p = await reg.PlanAsync(c, tx, i, me.UserGuid, checkAttachments: true, exceptOrder: null);
            var pending = await c.ExecuteScalarAsync<int?>(
                "SELECT TOP 1 Code FROM dbo.WebRegOrder WITH (UPDLOCK, HOLDLOCK) WHERE AccountGuid = @a AND RegType = @t AND Status IN (0,3)", new { a = p.Student.AccountGuid, t = i.RegType }, tx);
            if (pending is { } pc) throw new ApiException($"يوجد طلب التحاق رقم {pc} لهذا الطالب لم يكتمل بعد (بانتظار المراجعة أو تأكيد السداد)، لا يمكن إرسال طلب آخر قبل البت فيه");
            if (i.RegType == 0)
            {
                // The desktop only looks at training files (ChkIfStudentRegDiplom); a diploma form not yet accepted is a diploma too.
                var open = await c.ExecuteScalarAsync<string?>("""
                    SELECT TOP 1 CAST(R.Code AS nvarchar(50)) FROM dbo.RegDoc R
                    WHERE R.AccountGuid = @a AND R.RegType = 0 AND ISNULL(R.IstUse,1) = 1 AND ISNULL(R.IsReReg,0) = 0 AND ISNULL(R.Linked,0) = 0 AND ISNULL(R.DocSubTotal,0) <> 0
                    """, new { a = p.Student.AccountGuid }, tx);
                if (open != null) throw new ApiException($"لا يمكن تسجيل الطالب في أكثر من دبلوم: لديه استمارة دبلوم رقم {open} بانتظار القبول");
            }

            var snap = await RegSnapshotAsync(c, tx, i, p);
            var code = await NextCodeAsync(c, tx, "WebRegOrder");
            var guid = Guid.NewGuid();
            await c.ExecuteAsync("""
                INSERT INTO dbo.WebRegOrder (Guid, Code, OrderDate, UserGuid, StudentGuid, AccountGuid, RegType, BranchGuid, SellerGuid, BatchGuid, CourseGuid, DiplomGuid,
                    Programs, DocTotal, DocTax, DocSubTotal, PayAmount, CashAccountGuid, PayRef, PayDate, Notes, Payload, Status)
                VALUES (@guid, @code, @now, @u, @StudentGuid, @account, @RegType, @branch, @SellerGuid, @batch, @course, @diplom,
                    @programs, @FTotal, @FTax, @FSub, @PaySub, @cash, @ref, @payDate, @notes, @payload, 0)
                """, new
            {
                guid, code, now = Clock.Now, u = me.UserGuid, i.StudentGuid, account = p.Student.AccountGuid, i.RegType, branch = p.Setting.BranchGuid, i.SellerGuid,
                batch = i.RegType == 0 ? i.BatchGuid : null, course = i.RegType == 1 ? i.CourseGuid : null, diplom = p.MainDiplom.DiplomGuid,
                programs = Cut(string.Join("، ", p.Diploms.Select(d => d.Name)), 500), p.FTotal, p.FTax, p.FSub, p.PaySub,
                cash = p.Pay?.CashAccountGuid, @ref = Cut(p.Pay?.Ref?.Trim(), 100), payDate = p.Pay?.PayDate, notes = Cut(i.Notes?.Trim(), 500),
                payload = JsonSerializer.Serialize(new RegPayload { Input = i, Snapshot = snap }),
            }, tx);

            // The attachments uploaded in the wizard now belong to this request (latest file per type).
            if (p.Student.NationalId is { } nid)
                await c.ExecuteAsync("""
                    DELETE F FROM dbo.WebOrderFile F WHERE F.StageKey = @key AND EXISTS (SELECT 1 FROM dbo.WebOrderFile N WHERE N.StageKey = @key AND N.Code = F.Code AND N.ID > F.ID);
                    UPDATE dbo.WebOrderFile SET OrderGuid = @guid, StageKey = NULL WHERE StageKey = @key;
                    """, new { key = StageKey(me.UserGuid, nid), guid }, tx);
            await LogAsync(c, tx, guid, "إرسال الطلب", $"{snap.Book} — الإجمالي {p.FSub:N2}{(p.Pay != null ? $" — دفعة مقدمة {p.PaySub:N2}" : "")}");
            return (object)new { message = $"تم إرسال طلب الالتحاق رقم {code} للمراجعة", code, guid };
        }, IsolationLevel.Serializable, ct);
    }

    private const string RegListSql = """
        SELECT O.Guid, O.Code, O.OrderDate, O.Status, O.AccountGuid, O.StudentGuid, S.StudentName, S.NationalId, S.StudentTel, B.BrName AS Branch, O.Programs, O.RegType, 0 AS Kind,
               COALESCE(BT.BatchName, CO.CourseName) AS Batch, SM.ManFullName AS Seller, O.DocSubTotal AS SubTotal, O.PayAmount, CB.CashName AS Cash, O.PayRef, O.PayDate,
               U.FullName AS CreatedBy, RU.FullName AS ReviewedBy, O.ReviewDate, O.ReviewNotes, O.RegCode AS DocCode, O.RegDocGuid AS DocGuid, O.BillGuid,
               BL.TaxNo, (SELECT COUNT(*) FROM dbo.WebOrderFile F WHERE F.OrderGuid = O.Guid) AS Files, 0 AS Duplicates
        FROM dbo.WebRegOrder O JOIN dbo.Student S ON S.AccountGuid = O.AccountGuid
        LEFT JOIN dbo.Branch B ON B.Guid = O.BranchGuid LEFT JOIN dbo.Batch BT ON BT.Guid = O.BatchGuid LEFT JOIN dbo.Course CO ON CO.Guid = O.CourseGuid
        LEFT JOIN dbo.SalesMan SM ON SM.Guid = O.SellerGuid LEFT JOIN dbo.CashBox CB ON CB.AccountGuid = O.CashAccountGuid
        LEFT JOIN dbo.User_Info U ON U.Guid = O.UserGuid LEFT JOIN dbo.User_Info RU ON RU.Guid = O.ReviewUserGuid
        LEFT JOIN (SELECT Guid, CAST(TaxNo AS nvarchar(20)) AS TaxNo FROM dbo.Bill) BL ON BL.Guid = O.BillGuid
        """;

    public async Task<object> ListRegAsync(DateTime? from, DateTime? to, int? status, Guid? branch, string? q, CancellationToken ct)
    {
        var f = (from ?? Clock.Now.AddDays(-30)).Date; var t = (to ?? Clock.Now).Date.AddDays(1);
        q = string.IsNullOrWhiteSpace(q) ? null : q.Trim();
        await using var c = await db.OpenAsync(ct);
        // Pending requests are always listed whatever their date: nothing waiting may be hidden by the period.
        var rows = await c.QueryAsync<OrderListRow>(new CommandDefinition(RegListSql + """
             WHERE ((O.Status IN (0,3) AND (@status IS NULL OR @status IN (0,3))) OR (O.OrderDate >= @f AND O.OrderDate < @t)) AND (@status IS NULL OR O.Status = @status)
               AND (@branch IS NULL OR O.BranchGuid = @branch)
               AND (@q IS NULL OR S.StudentName LIKE '%' + @q + '%' OR S.NationalId LIKE @q + '%' OR S.StudentTel LIKE '%' + @q + '%' OR CAST(O.Code AS nvarchar(20)) = @q)
             ORDER BY CASE O.Status WHEN 0 THEN 0 WHEN 3 THEN 1 ELSE 2 END, O.OrderDate DESC
            """, new { f, t, status, branch, q }, cancellationToken: ct));
        var counts = await c.QueryFirstAsync<(int Pending, int Approved, int Rejected, int Awaiting)>(new CommandDefinition("""
            SELECT ISNULL(SUM(CASE WHEN Status = 0 THEN 1 ELSE 0 END),0), ISNULL(SUM(CASE WHEN Status = 1 AND OrderDate >= @f AND OrderDate < @t THEN 1 ELSE 0 END),0),
                   ISNULL(SUM(CASE WHEN Status = 2 AND OrderDate >= @f AND OrderDate < @t THEN 1 ELSE 0 END),0), ISNULL(SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END),0)
            FROM dbo.WebRegOrder WHERE (@branch IS NULL OR BranchGuid = @branch)
            """, new { f, t, branch }, cancellationToken: ct));
        return new { rows, counts = new { pending = counts.Pending, awaiting = counts.Awaiting, approved = counts.Approved, rejected = counts.Rejected } };
    }

    /// <summary>The request with everything the reviewer needs, plus a dry run of the approval checks.</summary>
    public async Task<object> GetRegAsync(Guid guid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var row = await c.QueryFirstOrDefaultAsync<OrderListRow>(new CommandDefinition(RegListSql + " WHERE O.Guid = @guid", new { guid }, cancellationToken: ct))
                  ?? throw ApiException.NotFound("الطلب غير موجود");
        var (payloadJson, author) = await c.QueryFirstAsync<(string Payload, Guid UserGuid)>("SELECT Payload, UserGuid FROM dbo.WebRegOrder WHERE Guid = @guid", new { guid });
        var payload = JsonSerializer.Deserialize<RegPayload>(payloadJson, Json)!;
        object check = new { ok = true, message = (string?)null };
        if (row.Status == 0)
        {
            // Dry run of the approval (read-only; rolled back) so the reviewer knows beforehand if it will pass.
            await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
            try
            {
                var i = payload.Input; i.RegDate = Clock.Now;
                var p = await reg.PlanAsync(c, tx, i, author, checkAttachments: false, exceptOrder: guid);
                check = p.FSub != payload.Snapshot.SubTotal || p.PaySub != payload.Snapshot.PaySub
                    ? new { ok = false, message = $"تغيّرت الأسعار منذ إرسال الطلب: إجمالي الاستمارة كان {payload.Snapshot.SubTotal:N2} وأصبح {p.FSub:N2}. يرجى رفض الطلب وإعادة إرساله." }
                    : new { ok = true, message = (string?)null };
            }
            catch (ApiException ex) { check = new { ok = false, message = ex.Message }; }
            finally { await tx.RollbackAsync(ct); }
        }
        return new
        {
            order = row, statusName = RegStatusName(row.Status), snapshot = payload.Snapshot, notes = payload.Input.Notes, contract = payload.Input.ChkContract,
            payRef = payload.Input.Payment?.Ref, payDate = payload.Input.Payment?.PayDate, payNotes = payload.Input.Payment?.Notes,
            split = payload.Input.Payment?.Split == true, ref2 = payload.Input.Payment?.Ref2, payDate2 = payload.Input.Payment?.PayDate2,
            amount1 = payload.Input.Payment?.Amount1, amount2 = payload.Input.Payment?.Amount2,
            student = await StudentOfAsync(c, row.AccountGuid), files = await FilesOfAsync(c, guid), log = await LogOfAsync(c, guid), check,
            payOrder = await c.QueryFirstOrDefaultAsync("SELECT TOP 1 Guid AS guid, Code AS code, Status AS status FROM dbo.WebPayOrder WHERE RegOrderGuid = @guid ORDER BY OrderDate DESC", new { guid }),
        };
    }

    public async Task<object> ApproveRegAsync(Guid guid, string? notes, CancellationToken ct)
    {
        var result = await db.InTransactionAsync(async (c, tx) =>
        {
            var o = await c.QueryFirstOrDefaultAsync<(int Code, int Status, Guid UserGuid, string Payload, decimal DocSubTotal, decimal PayAmount)>(
                "SELECT Code, Status, UserGuid, Payload, DocSubTotal, PayAmount FROM dbo.WebRegOrder WITH (UPDLOCK, HOLDLOCK) WHERE Guid = @guid", new { guid }, tx);
            if (o.Payload is null) throw ApiException.NotFound("الطلب غير موجود");
            if (o.Status != 0) throw new ApiException($"تمت مراجعة هذا الطلب من قبل ({RegStatusName(o.Status)})");
            var payload = JsonSerializer.Deserialize<RegPayload>(o.Payload, Json)!;
            var i = payload.Input;
            i.RegDate = Clock.Now; // the documents carry the approval date
            var p = await reg.PlanAsync(c, tx, i, o.UserGuid, checkAttachments: false, exceptOrder: guid);
            if (p.FSub != o.DocSubTotal || p.PaySub != o.PayAmount)
                throw new ApiException($"تغيّرت الأسعار منذ إرسال الطلب: إجمالي الاستمارة كان {o.DocSubTotal:N2} وأصبح {p.FSub:N2}. يرجى رفض الطلب وإعادة إرساله.");
            var extra = string.IsNullOrWhiteSpace(notes) ? "" : $" — {notes.Trim()}";
            if (p.Pay != null)
            {
                // With a down payment nothing is written yet: the accounts review it as a طلب سداد, and the form, its
                // invoice and receipt are all created together when it is confirmed (cancelled → the request is rejected).
                await c.ExecuteAsync("UPDATE dbo.WebRegOrder SET Status = 3, ReviewUserGuid = @u, ReviewDate = @now, ReviewNotes = @notes WHERE Guid = @guid",
                    new { guid, u = me.UserGuid, now = Clock.Now, notes = Cut(notes?.Trim(), 500) }, tx);
                var payCode = await DownPaymentOrderAsync(c, tx, guid, o.Code, o.UserGuid, i, p, payload.Snapshot);
                await LogAsync(c, tx, guid, "قبول الطلب", $"الدفعة المقدمة حُوّلت إلى طلب السداد رقم {payCode} — تُنشأ الاستمارة عند تأكيده{extra}");
                return new
                {
                    message = $"تم قبول طلب الالتحاق رقم {o.Code}، وتُنشأ استمارة التسجيل عند تأكيد الحسابات للدفعة المقدمة",
                    saved = (RegistrationSaved?)null, payOrderCode = (int?)payCode,
                };
            }
            var saved = await reg.PostAsync(c, tx, i, p, o.UserGuid);
            await c.ExecuteAsync("""
                UPDATE dbo.WebRegOrder SET Status = 1, ReviewUserGuid = @u, ReviewDate = @now, ReviewNotes = @notes, RegDocGuid = @RegGuid, RegCode = @RegCode
                WHERE Guid = @guid
                """, new { guid, u = me.UserGuid, now = Clock.Now, notes = Cut(notes?.Trim(), 500), saved.RegGuid, saved.RegCode }, tx);
            await LogAsync(c, tx, guid, "قبول الطلب", $"استمارة التسجيل رقم {saved.RegCode}{extra}");
            return new
            {
                message = $"تم قبول طلب الالتحاق رقم {o.Code} وإنشاء استمارة التسجيل رقم {saved.RegCode}",
                saved = (RegistrationSaved?)saved, payOrderCode = (int?)null,
            };
        }, IsolationLevel.Serializable, ct);
        return result;
    }

    public async Task<object> RejectRegAsync(Guid guid, string? reason, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new ApiException("يرجى كتابة سبب رفض الطلب");
        return await db.InTransactionAsync(async (c, tx) =>
        {
            var o = await c.QueryFirstOrDefaultAsync<(int Code, int Status)>("SELECT Code, Status FROM dbo.WebRegOrder WITH (UPDLOCK, HOLDLOCK) WHERE Guid = @guid", new { guid }, tx);
            if (o.Code == 0) throw ApiException.NotFound("الطلب غير موجود");
            if (o.Status != 0) throw new ApiException($"تمت مراجعة هذا الطلب من قبل ({RegStatusName(o.Status)})");
            await c.ExecuteAsync("UPDATE dbo.WebRegOrder SET Status = 2, ReviewUserGuid = @u, ReviewDate = @now, ReviewNotes = @r WHERE Guid = @guid",
                new { guid, u = me.UserGuid, now = Clock.Now, r = Cut(reason.Trim(), 500) }, tx);
            await LogAsync(c, tx, guid, "رفض الطلب", reason.Trim());
            return (object)new { message = $"تم رفض طلب الالتحاق رقم {o.Code}" };
        }, IsolationLevel.ReadCommitted, ct);
    }

    /// <summary>
    /// The down payment of an approved registration becomes a طلب سداد (kind 0) for the accounts, carrying the
    /// transfer data and images that came with the طلب التحاق. Its invoice + receipt are posted when it is confirmed.
    /// </summary>
    private async Task<int> DownPaymentOrderAsync(DbConnection c, DbTransaction tx, Guid regOrder, int regOrderCode, Guid author, RegistrationInput i, RegPlan p, RegSnapshot snap)
    {
        var pay = p.Pay!;
        var code = await NextCodeAsync(c, tx, "WebPayOrder");
        var guid = Guid.NewGuid();
        var input = new PayInvoiceInput
        {
            StudentGuid = i.StudentGuid, FileId = 0, Kind = 0, Date = Clock.Now,
            Lines = p.PayLines.Select(l => new PayLineInput { DiplomGuid = l.Item.DiplomGuid, Total = l.Total }).ToList(),
            CashAccountGuid = pay.CashAccountGuid, CostCenterGuid = pay.CostCenterGuid, Ref = pay.Ref, PayDate = pay.PayDate, Notes = pay.Notes,
            Split = pay.Split, Amount1 = pay.Amount1, Amount2 = pay.Amount2, CashAccountGuid2 = pay.CashAccountGuid2, CostCenterGuid2 = pay.CostCenterGuid2, Ref2 = pay.Ref2, PayDate2 = pay.PayDate2,
        };
        var bookName = await c.ExecuteScalarAsync<string?>("SELECT DocName FROM dbo.RegSetting WHERE Guid = @g", new { g = p.Setting.DocPayGuid }, tx);
        var noteDip = i.RegType == 0 ? p.MainDiplom.Name : p.CourseDiplomName ?? p.MainDiplom.Name;
        var paySnap = new PaySnapshot
        {
            Book = bookName, Program = $"دفعة مقدمة — طلب التحاق {regOrderCode} — {noteDip}",
            Lines = p.PayLines.Select(l => new SnapPayLine(l.Item.Name, l.Cost, l.Tax, l.Total)).ToList(),
            Sub = p.PaySub, Tax = Math.Round(p.PayLines.Sum(l => l.Tax), 2),
            Cash = snap.Cash, Cost = snap.Cost, Cash2 = snap.Cash2, Cost2 = snap.Cost2,
        };
        await c.ExecuteAsync("""
            INSERT INTO dbo.WebPayOrder (Guid, Code, OrderDate, UserGuid, StudentGuid, AccountGuid, FileId, Kind, BranchGuid, Programs, Amount,
                CashAccountGuid, CashAccountGuid2, PayRef, PayRef2, PayDate, Notes, Payload, Status, RegOrderGuid, RegDocGuid, FilesArchived)
            VALUES (@guid, @code, @now, @author, @StudentGuid, @account, 0, 0, @branch, @programs, @amount,
                @CashAccountGuid, @cash2, @ref, @ref2, @PayDate, @notes, @payload, 0, @regOrder, NULL, 1)
            """, new
        {
            guid, code, now = Clock.Now, author, i.StudentGuid, account = p.Student.AccountGuid, branch = p.Setting.BranchGuid, programs = Cut(paySnap.Program, 500), amount = paySnap.Sub,
            input.CashAccountGuid, cash2 = input.Split ? input.CashAccountGuid2 : null, @ref = Cut(input.Ref?.Trim(), 100), ref2 = input.Split ? Cut(input.Ref2?.Trim(), 100) : null,
            input.PayDate, notes = Cut(input.Notes?.Trim(), 500), payload = JsonSerializer.Serialize(new PayPayload { Input = input, Snapshot = paySnap, RegOrder = regOrder }),
            regOrder,
        }, tx);
        // The transfer images sent with the طلب التحاق (already in the archive as trans_{nationalId}) go with it for the reviewer.
        await c.ExecuteAsync("""
            INSERT INTO dbo.WebOrderFile (OrderGuid, Code, FileName, ContentType, Size, Data, UserGuid, CreatedAt)
            SELECT @guid, Code, FileName, ContentType, Size, Data, UserGuid, CreatedAt FROM dbo.WebOrderFile WHERE OrderGuid = @regOrder AND Code IN ('trans', 'trans2')
            """, new { guid, regOrder }, tx);
        await c.ExecuteAsync("INSERT INTO dbo.WebOrderLog (OrderGuid, Action, UserGuid, At, Notes) VALUES (@guid, @a, @author, @at, @n)",
            new { guid, a = "إرسال الطلب", author, at = Clock.Now, n = $"دفعة مقدمة لطلب الالتحاق رقم {regOrderCode} — المبلغ {paySnap.Sub:N2} — {snap.Cash} — مرجع {input.Ref}" }, tx);
        await LogAsync(c, tx, guid, "إنشاء الطلب من طلب الالتحاق", $"بعد قبول طلب الالتحاق رقم {regOrderCode}؛ تُنشأ استمارة التسجيل والفاتورة وسند القبض عند التأكيد");
        return code;
    }

    /// <summary>
    /// The registration of a new-style down-payment order (kind 0 with RegOrder): its input with the (possibly corrected)
    /// transfer data of the order, re-planned now. Locks the طلب التحاق, which must still be awaiting the payment.
    /// </summary>
    private async Task<(int Code, Guid Author, RegistrationInput Input, RegPlan Plan)> RegOfDownAsync(DbConnection c, DbTransaction tx, PayPayload pp)
    {
        var o = await c.QueryFirstOrDefaultAsync<(int Code, int Status, Guid UserGuid, string Payload, decimal DocSubTotal)>(
            "SELECT Code, Status, UserGuid, Payload, DocSubTotal FROM dbo.WebRegOrder WITH (UPDLOCK, HOLDLOCK) WHERE Guid = @g", new { g = pp.RegOrder }, tx);
        if (o.Payload is null) throw new ApiException("طلب الالتحاق الخاص بهذه الدفعة المقدمة غير موجود");
        if (o.Status != 3) throw new ApiException($"طلب الالتحاق رقم {o.Code} ليس بانتظار تأكيد السداد ({RegStatusName(o.Status)})");
        var i = JsonSerializer.Deserialize<RegPayload>(o.Payload, Json)!.Input;
        var x = pp.Input;
        var lines = i.Payment?.Lines ?? [];
        i.Payment = new PaymentInput
        {
            CashAccountGuid = x.CashAccountGuid, CostCenterGuid = x.CostCenterGuid, Ref = x.Ref, PayDate = x.PayDate, Notes = x.Notes, Lines = lines,
            Split = x.Split, Amount1 = x.Amount1, Amount2 = x.Amount2, CashAccountGuid2 = x.CashAccountGuid2, CostCenterGuid2 = x.CostCenterGuid2, Ref2 = x.Ref2, PayDate2 = x.PayDate2,
        };
        i.RegDate = Clock.Now; // the form, invoice and receipt carry the confirmation date
        var p = await reg.PlanAsync(c, tx, i, o.UserGuid, checkAttachments: false, exceptOrder: pp.RegOrder);
        if (p.FSub != o.DocSubTotal || p.PaySub != pp.Snapshot.Sub)
            throw new ApiException($"تغيّرت الأسعار منذ إرسال طلب الالتحاق رقم {o.Code}: إجمالي الاستمارة كان {o.DocSubTotal:N2} وأصبح {p.FSub:N2}. يرجى إلغاء الطلب وإعادة إرساله.");
        return (o.Code, o.UserGuid, i, p);
    }

    /// <summary>The down payment of a kind-0 order with the (possibly corrected) transfer data of its input.</summary>
    private static DownPayment DownOf(PayPayload p)
    {
        var d = p.Down ?? throw new ApiException("بيانات الدفعة المقدمة غير موجودة في الطلب");
        var i = p.Input;
        d.Pay = new PaymentInput
        {
            CashAccountGuid = i.CashAccountGuid, CostCenterGuid = i.CostCenterGuid, Ref = i.Ref, PayDate = i.PayDate, Notes = i.Notes, Lines = d.Pay.Lines,
            Split = i.Split, Amount1 = i.Amount1, Amount2 = i.Amount2, CashAccountGuid2 = i.CashAccountGuid2, CostCenterGuid2 = i.CostCenterGuid2, Ref2 = i.Ref2, PayDate2 = i.PayDate2,
        };
        return d;
    }

    /* =============================== طلب سداد =============================== */

    private async Task<PaySnapshot> PaySnapshotAsync(DbConnection c, DbTransaction tx, PayInvoiceInput i, PayInvoiceService.PayPlan p) => new()
    {
        Book = p.BookName, Program = p.DiplomName, Lines = p.Lines.Select(l => new SnapPayLine(l.Name, l.Cost, l.Tax, l.Total)).ToList(), Sub = p.Sub, Tax = p.Tax,
        Cash = p.CashName, Cost = await NameAsync(c, tx, CostSql, i.CostCenterGuid),
        Cash2 = i.Split ? p.CashName2 : null, Cost2 = i.Split ? await NameAsync(c, tx, CostSql, i.CostCenterGuid2) : null,
    };

    public async Task<object> CreatePayAsync(PayInvoiceInput i, IFormFile? file1, IFormFile? file2, CancellationToken ct)
    {
        PayInvoiceService.Normalize(i);
        foreach (var f in new[] { file1, file2 }.OfType<IFormFile>())
            if (f.Length is 0 or > 10 * 1024 * 1024) throw new ApiException($"حجم الملف غير مسموح ({f.FileName}). الحد الأقصى 10 ميجابايت");
        return await db.InTransactionAsync(async (c, tx) =>
        {
            var p = await pay.PlanAsync(c, tx, i, me.UserGuid);
            if (p.CashName != OfficeCash && file1 is null) throw new ApiException("لا يمكن إرسال طلب السداد بدون إرفاق صورة الحوالة");
            if (i.Split && p.CashName2 != OfficeCash && file2 is null) throw new ApiException("يرجى إرفاق صورة الحوالة الثانية");
            var refText = i.Ref?.Trim() ?? "";
            var same = await c.ExecuteScalarAsync<int?>(
                "SELECT TOP 1 Code FROM dbo.WebPayOrder WITH (UPDLOCK, HOLDLOCK) WHERE AccountGuid = @a AND Status = 0 AND PayRef = @r", new { a = p.AccountGuid, r = refText }, tx);
            if (same is { } sc) throw new ApiException($"تم إرسال طلب سداد رقم {sc} بنفس رقم المرجع لهذا الطالب وهو بانتظار المراجعة");

            var snap = await PaySnapshotAsync(c, tx, i, p);
            var code = await NextCodeAsync(c, tx, "WebPayOrder");
            var guid = Guid.NewGuid();
            await c.ExecuteAsync("""
                INSERT INTO dbo.WebPayOrder (Guid, Code, OrderDate, UserGuid, StudentGuid, AccountGuid, FileId, Kind, BranchGuid, Programs, Amount,
                    CashAccountGuid, CashAccountGuid2, PayRef, PayRef2, PayDate, Notes, Payload, Status)
                VALUES (@guid, @code, @now, @u, @StudentGuid, @account, @FileId, @Kind, @branch, @programs, @amount,
                    @CashAccountGuid, @cash2, @ref, @ref2, @PayDate, @notes, @payload, 0)
                """, new
            {
                guid, code, now = Clock.Now, u = me.UserGuid, i.StudentGuid, account = p.AccountGuid, i.FileId, i.Kind, branch = p.BranchGuid,
                programs = Cut(p.DiplomName, 500), amount = p.Sub, i.CashAccountGuid, cash2 = i.Split ? i.CashAccountGuid2 : null,
                @ref = Cut(refText, 100), ref2 = i.Split ? Cut(i.Ref2?.Trim(), 100) : null, i.PayDate, notes = Cut(i.Notes?.Trim(), 500),
                payload = JsonSerializer.Serialize(new PayPayload { Input = i, Snapshot = snap }),
            }, tx);
            foreach (var (f, codeName) in new[] { (file1, "trans"), (file2, "trans2") })
            {
                if (f is null) continue;
                await using var ms = new MemoryStream();
                await f.CopyToAsync(ms, ct);
                await c.ExecuteAsync("""
                    INSERT INTO dbo.WebOrderFile (OrderGuid, Code, FileName, ContentType, Size, Data, UserGuid, CreatedAt)
                    VALUES (@guid, @codeName, @name, @type, @size, @data, @u, @at)
                    """, new { guid, codeName, name = Cut(f.FileName, 200), type = Cut(f.ContentType ?? "application/octet-stream", 100), size = (int)ms.Length, data = ms.ToArray(), u = me.UserGuid, at = Clock.Now }, tx);
            }
            await LogAsync(c, tx, guid, "إرسال الطلب", $"{p.BookName} — المبلغ {p.Sub:N2} — {p.CashName} — مرجع {refText}");
            return (object)new { message = $"تم إرسال طلب السداد رقم {code} للمراجعة", code, guid };
        }, IsolationLevel.Serializable, ct);
    }

    private const string PayListSql = """
        SELECT O.Guid, O.Code, O.OrderDate, O.Status, O.AccountGuid, O.StudentGuid, S.StudentName, S.NationalId, S.StudentTel, B.BrName AS Branch, O.Programs, 0 AS RegType, O.Kind,
               NULL AS Batch, NULL AS Seller, O.Amount AS SubTotal, O.Amount AS PayAmount, CB.CashName AS Cash, O.PayRef, O.PayDate,
               U.FullName AS CreatedBy, RU.FullName AS ReviewedBy, O.ReviewDate, O.ReviewNotes, O.BillCode AS DocCode, O.BillGuid AS DocGuid, O.BillGuid, O.TaxNo,
               (SELECT COUNT(*) FROM dbo.WebOrderFile F WHERE F.OrderGuid = O.Guid) AS Files,
               CASE WHEN ISNULL(O.PayRef,'') = '' THEN 0 ELSE
                 (SELECT COUNT(*) FROM dbo.Bill X WHERE ISNULL(X.IsUse,1) = 1 AND (X.Refrance = O.PayRef OR X.Ref2 = O.PayRef) AND (O.BillGuid IS NULL OR X.Guid <> O.BillGuid))
               + (SELECT COUNT(*) FROM dbo.WebPayOrder Y WHERE Y.Guid <> O.Guid AND Y.Status IN (0,1) AND Y.BillGuid IS NULL AND (Y.PayRef = O.PayRef OR Y.PayRef2 = O.PayRef)) END AS Duplicates
        FROM dbo.WebPayOrder O JOIN dbo.Student S ON S.AccountGuid = O.AccountGuid
        LEFT JOIN dbo.Branch B ON B.Guid = O.BranchGuid LEFT JOIN dbo.CashBox CB ON CB.AccountGuid = O.CashAccountGuid
        LEFT JOIN dbo.User_Info U ON U.Guid = O.UserGuid LEFT JOIN dbo.User_Info RU ON RU.Guid = O.ReviewUserGuid
        """;

    public async Task<object> ListPayAsync(DateTime? from, DateTime? to, int? status, Guid? branch, string? q, CancellationToken ct)
    {
        var f = (from ?? Clock.Now.AddDays(-30)).Date; var t = (to ?? Clock.Now).Date.AddDays(1);
        q = string.IsNullOrWhiteSpace(q) ? null : q.Trim();
        await using var c = await db.OpenAsync(ct);
        var rows = await c.QueryAsync<OrderListRow>(new CommandDefinition(PayListSql + """
             WHERE ((O.Status = 0 AND (@status IS NULL OR @status = 0)) OR (O.OrderDate >= @f AND O.OrderDate < @t)) AND (@status IS NULL OR O.Status = @status)
               AND (@branch IS NULL OR O.BranchGuid = @branch)
               AND (@q IS NULL OR S.StudentName LIKE '%' + @q + '%' OR S.NationalId LIKE @q + '%' OR O.PayRef LIKE '%' + @q + '%' OR CAST(O.Code AS nvarchar(20)) = @q)
             ORDER BY CASE WHEN O.Status = 0 THEN 0 ELSE 1 END, O.OrderDate DESC
            """, new { f, t, status, branch, q }, cancellationToken: ct));
        var counts = await c.QueryFirstAsync<(int Pending, int Confirmed, int Duplicate, int Cancelled)>(new CommandDefinition("""
            SELECT SUM(CASE WHEN Status = 0 THEN 1 ELSE 0 END), SUM(CASE WHEN Status = 1 AND OrderDate >= @f AND OrderDate < @t THEN 1 ELSE 0 END),
                   SUM(CASE WHEN Status = 2 AND OrderDate >= @f AND OrderDate < @t THEN 1 ELSE 0 END), SUM(CASE WHEN Status = 3 AND OrderDate >= @f AND OrderDate < @t THEN 1 ELSE 0 END)
            FROM dbo.WebPayOrder WHERE (@branch IS NULL OR BranchGuid = @branch)
            """, new { f, t, branch }, cancellationToken: ct));
        return new { rows, counts = new { pending = counts.Pending, confirmed = counts.Confirmed, duplicate = counts.Duplicate, cancelled = counts.Cancelled } };
    }

    public async Task<object> GetPayAsync(Guid guid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var row = await c.QueryFirstOrDefaultAsync<OrderListRow>(new CommandDefinition(PayListSql + " WHERE O.Guid = @guid", new { guid }, cancellationToken: ct))
                  ?? throw ApiException.NotFound("الطلب غير موجود");
        var (payloadJson, author, archived) = await c.QueryFirstAsync<(string Payload, Guid UserGuid, bool FilesArchived)>("SELECT Payload, UserGuid, FilesArchived FROM dbo.WebPayOrder WHERE Guid = @guid", new { guid });
        var payload = JsonSerializer.Deserialize<PayPayload>(payloadJson, Json)!;
        var i = payload.Input;
        var refs = new[] { i.Ref?.Trim(), i.Split ? i.Ref2?.Trim() : null }.Where(r => !string.IsNullOrEmpty(r)).Distinct().ToArray();
        var dupBills = refs.Length == 0 ? [] : await c.QueryAsync("""
            SELECT CAST(X.Code AS nvarchar(50)) AS code, CAST(X.TaxNo AS nvarchar(20)) AS taxNo, X.BillDate AS date, S.StudentName AS student,
                   CAST(ISNULL(X.BillSubTotal,0) AS decimal(18,2)) AS amount, CASE WHEN X.Refrance IN @refs THEN X.Refrance ELSE X.Ref2 END AS ref
            FROM dbo.Bill X JOIN dbo.Student S ON S.AccountGuid = X.AccountGuid
            WHERE ISNULL(X.IsUse,1) = 1 AND (X.Refrance IN @refs OR X.Ref2 IN @refs) AND (@bill IS NULL OR X.Guid <> @bill) ORDER BY X.BillDate DESC
            """, new { refs, bill = row.BillGuid });
        var dupOrders = refs.Length == 0 ? [] : await c.QueryAsync("""
            SELECT Y.Code AS code, Y.Status AS status, Y.OrderDate AS date, S.StudentName AS student, Y.Amount AS amount
            FROM dbo.WebPayOrder Y JOIN dbo.Student S ON S.AccountGuid = Y.AccountGuid
            WHERE Y.Guid <> @guid AND Y.Status IN (0,1) AND Y.BillGuid IS NULL AND (Y.PayRef IN @refs OR Y.PayRef2 IN @refs)
            """, new { guid, refs });
        object check = new { ok = true, message = (string?)null };
        if (row.Status == 0)
        {
            await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
            try
            {
                if (payload.RegOrder != null) await RegOfDownAsync(c, tx, JsonSerializer.Deserialize<PayPayload>(payloadJson, Json)!);
                else if (payload.Down != null) await RegistrationService.CheckDownPaymentAsync(c, tx, DownOf(JsonSerializer.Deserialize<PayPayload>(payloadJson, Json)!));
                else
                {
                    var copy = JsonSerializer.Deserialize<PayPayload>(payloadJson, Json)!.Input; copy.Date = Clock.Now;
                    var p = await pay.PlanAsync(c, tx, copy, author);
                    check = p.Sub != row.PayAmount ? new { ok = false, message = $"تغيّر مبلغ الفاتورة منذ إرسال الطلب ({row.PayAmount:N2} ← {p.Sub:N2})" } : new { ok = true, message = (string?)null };
                }
            }
            catch (ApiException ex) { check = new { ok = false, message = ex.Message }; }
            finally { await tx.RollbackAsync(ct); }
        }
        var lastBills = await c.QueryAsync("""
            SELECT TOP 5 CAST(B.TaxNo AS nvarchar(20)) AS taxNo, CAST(B.Code AS nvarchar(50)) AS code, B.BillDate AS date, CAST(ISNULL(B.BillSubTotal,0) AS decimal(18,2)) AS amount, B.Refrance AS ref
            FROM dbo.Bill B WHERE B.AccountGuid = @a AND ISNULL(B.IsUse,1) = 1 ORDER BY B.BillDate DESC
            """, new { a = row.AccountGuid });
        return new
        {
            order = row, statusName = PayStatusName(row.Status), snapshot = payload.Snapshot, filesArchived = archived,
            down = payload.Down is { } dp ? new { dp.RegCode, dp.RegDocGuid } : null,
            regOrder = payload.RegOrder is { } ro
                ? await c.QueryFirstOrDefaultAsync("SELECT Guid AS guid, Code AS code, Status AS status, RegCode AS regCode FROM dbo.WebRegOrder WHERE Guid = @ro", new { ro })
                : null,
            input = new
            {
                i.CashAccountGuid, i.CostCenterGuid, i.Ref, i.PayDate, i.Notes, i.Split, i.Amount1, i.Amount2, i.CashAccountGuid2, i.CostCenterGuid2, i.Ref2, i.PayDate2, i.Kind,
            },
            duplicates = new { bills = dupBills, orders = dupOrders },
            lastBills, student = await StudentOfAsync(c, row.AccountGuid), files = await FilesOfAsync(c, guid), log = await LogOfAsync(c, guid), check,
            cashBoxes = await c.QueryAsync("""
                SELECT CB.AccountGuid AS guid, CB.CashName AS name, CB.CashType AS type, CB.CostCenterGuid AS costGuid, CC.CenterName AS costName
                FROM dbo.CashBox CB LEFT JOIN dbo.CostCenter CC ON CC.Guid = CB.CostCenterGuid WHERE CB.IsUse = 1 AND CB.UseInSales = 1 ORDER BY CB.CashType, TRY_CONVERT(int, CB.Code)
                """),
        };
    }

    private static async Task<(int Code, Guid Author, PayPayload Payload)> LockPayAsync(DbConnection c, DbTransaction tx, Guid guid)
    {
        var o = await c.QueryFirstOrDefaultAsync<(int Code, int Status, Guid UserGuid, string Payload)>(
            "SELECT Code, Status, UserGuid, Payload FROM dbo.WebPayOrder WITH (UPDLOCK, HOLDLOCK) WHERE Guid = @guid", new { guid }, tx);
        if (o.Payload is null) throw ApiException.NotFound("الطلب غير موجود");
        if (o.Status != 0) throw new ApiException($"تمت مراجعة هذا الطلب من قبل ({PayStatusName(o.Status)})");
        return (o.Code, o.UserGuid, JsonSerializer.Deserialize<PayPayload>(o.Payload, Json)!);
    }

    /// <summary>The reviewer corrects the transfer data (bank, cost center, reference, dates). The request is re-checked.</summary>
    public async Task<object> UpdatePayAsync(Guid guid, PayEditInput e, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(e.Reason)) throw new ApiException("يرجى كتابة سبب التعديل");
        return await db.InTransactionAsync(async (c, tx) =>
        {
            var (code, author, payload) = await LockPayAsync(c, tx, guid);
            var i = payload.Input;
            var before = $"{payload.Snapshot.Cash} / {i.Ref} / {i.PayDate:dd/MM/yyyy}";
            i.CashAccountGuid = e.CashAccountGuid ?? i.CashAccountGuid; i.CostCenterGuid = e.CostCenterGuid ?? i.CostCenterGuid;
            i.Ref = e.Ref ?? i.Ref; i.PayDate = e.PayDate ?? i.PayDate; i.Notes = e.Notes ?? i.Notes;
            if (i.Split)
            {
                i.CashAccountGuid2 = e.CashAccountGuid2 ?? i.CashAccountGuid2; i.CostCenterGuid2 = e.CostCenterGuid2 ?? i.CostCenterGuid2;
                i.Ref2 = e.Ref2 ?? i.Ref2; i.PayDate2 = e.PayDate2 ?? i.PayDate2;
            }
            if (payload.RegOrder != null)
            {
                var (_, _, _, rp) = await RegOfDownAsync(c, tx, payload);
                payload.Snapshot.Cash = rp.CashName; payload.Snapshot.Cost = await NameAsync(c, tx, CostSql, i.CostCenterGuid);
                payload.Snapshot.Cash2 = i.Split ? rp.CashName2 : null; payload.Snapshot.Cost2 = i.Split ? await NameAsync(c, tx, CostSql, i.CostCenterGuid2) : null;
            }
            else if (payload.Down != null)
            {
                var (cash1, cash2) = await RegistrationService.CheckDownPaymentAsync(c, tx, DownOf(payload));
                payload.Snapshot.Cash = cash1; payload.Snapshot.Cost = await NameAsync(c, tx, CostSql, i.CostCenterGuid);
                payload.Snapshot.Cash2 = i.Split ? cash2 : null; payload.Snapshot.Cost2 = i.Split ? await NameAsync(c, tx, CostSql, i.CostCenterGuid2) : null;
            }
            else
            {
                var check = JsonSerializer.Deserialize<PayInvoiceInput>(JsonSerializer.Serialize(i), Json)!; check.Date = Clock.Now;
                var p = await pay.PlanAsync(c, tx, check, author);
                payload.Snapshot = await PaySnapshotAsync(c, tx, i, p);
            }
            await c.ExecuteAsync("""
                UPDATE dbo.WebPayOrder SET CashAccountGuid = @CashAccountGuid, CashAccountGuid2 = @cash2, PayRef = @ref, PayRef2 = @ref2, PayDate = @PayDate, Notes = @notes, Payload = @payload
                WHERE Guid = @guid
                """, new
            {
                guid, i.CashAccountGuid, cash2 = i.Split ? i.CashAccountGuid2 : null, @ref = Cut(i.Ref?.Trim(), 100), ref2 = i.Split ? Cut(i.Ref2?.Trim(), 100) : null,
                i.PayDate, notes = Cut(i.Notes?.Trim(), 500), payload = JsonSerializer.Serialize(payload),
            }, tx);
            await LogAsync(c, tx, guid, "تعديل بيانات الحوالة", $"{e.Reason!.Trim()} — قبل: {before} — بعد: {payload.Snapshot.Cash} / {i.Ref} / {i.PayDate:dd/MM/yyyy}");
            return (object)new { message = $"تم تعديل طلب السداد رقم {code}" };
        }, IsolationLevel.Serializable, ct);
    }

    public async Task<object> ConfirmPayAsync(Guid guid, string? notes, CancellationToken ct)
    {
        var (code, saved, nationalId, files, formCode) = await db.InTransactionAsync(async (c, tx) =>
        {
            var (code, author, payload) = await LockPayAsync(c, tx, guid);
            var amount = await c.ExecuteScalarAsync<decimal>("SELECT Amount FROM dbo.WebPayOrder WHERE Guid = @guid", new { guid }, tx);
            var i = payload.Input;
            i.Date = Clock.Now; // the tax invoice is issued on the day it is confirmed
            PaySaved s;
            string? formCode = null;
            string? nid;
            if (payload.RegOrder is { } regOrder)
            {
                // Down payment of a طلب التحاق: the form, its journal, the invoice and the receipt are all created now.
                var (regCode, regAuthor, ri, rp) = await RegOfDownAsync(c, tx, payload);
                var rs = await reg.PostAsync(c, tx, ri, rp, regAuthor);
                formCode = rs.RegCode;
                if (rs.BillGuid is not { } bg) throw new ApiException("لم تُنشأ فاتورة الدفعة المقدمة: دفتر التسجيل لا يتطلب دفعة مقدمة");
                s = new PaySaved("", rs.BillCode!, bg, rs.TaxNo!, rs.ReceiptCode!, rs.ReceiptCode2, []);
                await c.ExecuteAsync("""
                    UPDATE dbo.WebRegOrder SET Status = 1, RegDocGuid = @RegGuid, RegCode = @RegCode, BillGuid = @BillGuid WHERE Guid = @regOrder;
                    UPDATE dbo.WebPayOrder SET RegDocGuid = @RegGuid WHERE Guid = @guid;
                    """, new { rs.RegGuid, rs.RegCode, rs.BillGuid, regOrder, guid }, tx);
                await LogAsync(c, tx, regOrder, "تأكيد الدفعة المقدمة",
                    $"أكدت الحسابات طلب السداد رقم {code}: استمارة التسجيل رقم {rs.RegCode} — فاتورة {rs.TaxNo} — سند قبض {rs.ReceiptCode}");
                nid = null;
            }
            else if (payload.Down != null)
            {
                // Down payment of a registration form: its own book and VAT rule, then UpdatePayStaut on the form.
                var posted = await reg.PostDownPaymentAsync(c, tx, DownOf(payload), author, i.Date);
                s = new PaySaved("", posted.BillCode, posted.BillGuid, posted.TaxNo, posted.ReceiptCode, posted.ReceiptCode2, []);
                await c.ExecuteAsync("UPDATE dbo.WebRegOrder SET BillGuid = @b WHERE Guid = (SELECT RegOrderGuid FROM dbo.WebPayOrder WHERE Guid = @guid)", new { b = posted.BillGuid, guid }, tx);
                nid = null;
            }
            else
            {
                var p = await pay.PlanAsync(c, tx, i, author);
                if (p.Sub != amount) throw new ApiException($"تغيّر مبلغ الفاتورة منذ إرسال الطلب ({amount:N2} ← {p.Sub:N2}). يرجى إلغاء الطلب وإعادة إرساله.");
                s = await pay.PostAsync(c, tx, i, p, author);
                nid = p.NationalId;
            }
            await c.ExecuteAsync("""
                UPDATE dbo.WebPayOrder SET Status = 1, ReviewUserGuid = @u, ReviewDate = @now, ReviewNotes = @notes, BillGuid = @BillGuid, BillCode = @BillCode, TaxNo = @TaxNo
                WHERE Guid = @guid
                """, new { guid, u = me.UserGuid, now = Clock.Now, notes = Cut(notes?.Trim(), 500), s.BillGuid, s.BillCode, s.TaxNo }, tx);
            await LogAsync(c, tx, guid, "تأكيد الطلب", $"فاتورة {s.TaxNo} (داخلي {s.BillCode}) — سند قبض {s.ReceiptCode}{(s.ReceiptCode2 != null ? $" و{s.ReceiptCode2}" : "")}{(string.IsNullOrWhiteSpace(notes) ? "" : $" — {notes.Trim()}")}");
            var f = (await c.QueryAsync<(string Code, string FileName, byte[] Data)>("SELECT Code, FileName, Data FROM dbo.WebOrderFile WHERE OrderGuid = @guid", new { guid }, tx)).ToList();
            return (code, s, nid, f, formCode);
        }, IsolationLevel.Serializable, ct);

        // After commit: the transfer images go to the archive with the invoice-based names (like the desktop).
        // (A down payment's images are already in the archive: they were sent with the registration attachments.)
        var archived = saved.AttachNames.Length == 0 || await ArchiveAsync(guid, nationalId, saved.AttachNames, files, ct);
        return new
        {
            message = formCode != null ? $"تم تأكيد طلب السداد رقم {code}: أُنشئت استمارة التسجيل رقم {formCode} والفاتورة الضريبية رقم {saved.TaxNo}" : $"تم تأكيد طلب السداد رقم {code} وإصدار الفاتورة الضريبية رقم {saved.TaxNo}", regCode = formCode,
            saved.BillGuid, saved.BillCode, saved.TaxNo, saved.ReceiptCode, saved.ReceiptCode2, archived,
            warning = archived ? null : "تم إصدار الفاتورة، لكن تعذّر رفع صور الحوالة على خادم الأرشيف. يمكنك إعادة المحاولة من تفاصيل الطلب.",
        };
    }

    private async Task<bool> ArchiveAsync(Guid guid, string? nationalId, string?[] names, List<(string Code, string FileName, byte[] Data)> files, CancellationToken ct)
    {
        var items = new List<(string Name, IFormFile File)>();
        foreach (var (codeName, idx) in new[] { ("trans", 0), ("trans2", 1) })
        {
            var f = files.FirstOrDefault(x => x.Code == codeName);
            if (f.Data is null || idx >= names.Length || names[idx] is not { } name) continue;
            items.Add((name, new FormFile(new MemoryStream(f.Data), 0, f.Data.Length, "file", f.FileName)));
        }
        try
        {
            if (items.Count > 0) await attachments.UploadNamedAsync(nationalId ?? "", items, ct);
            await using var c = await db.OpenAsync(ct);
            await c.ExecuteAsync("UPDATE dbo.WebPayOrder SET FilesArchived = 1 WHERE Guid = @guid", new { guid });
            return true;
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "Archiving transfer images of pay order {Guid} failed", guid);
            return false;
        }
    }

    /// <summary>Retry sending the transfer images of a confirmed request to the archive.</summary>
    public async Task<object> RetryArchiveAsync(Guid guid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var o = await c.QueryFirstOrDefaultAsync<(int Status, string Payload, string? BillCode, bool FilesArchived, Guid AccountGuid)>(
            "SELECT Status, Payload, BillCode, FilesArchived, AccountGuid FROM dbo.WebPayOrder WHERE Guid = @guid", new { guid });
        if (o.Payload is null) throw ApiException.NotFound("الطلب غير موجود");
        if (o.Status != 1 || o.BillCode is null) throw new ApiException("الطلب غير مؤكد");
        if (o.FilesArchived) return new { message = "صور الحوالة مرفوعة على الأرشيف بالفعل" };
        var i = JsonSerializer.Deserialize<PayPayload>(o.Payload, Json)!;
        var nid = await c.ExecuteScalarAsync<string?>("SELECT NationalId FROM dbo.Student WHERE AccountGuid = @a", new { a = o.AccountGuid });
        var names = new[]
        {
            i.Snapshot.Cash == OfficeCash ? null : $"{i.Snapshot.Cash}_{o.BillCode}_{i.Input.PayDate:ddMMyyyy}",
            !i.Input.Split || i.Snapshot.Cash2 == OfficeCash ? null : $"{i.Snapshot.Cash2}_{o.BillCode}_{i.Input.PayDate2:ddMMyyyy}",
        };
        var files = (await c.QueryAsync<(string Code, string FileName, byte[] Data)>("SELECT Code, FileName, Data FROM dbo.WebOrderFile WHERE OrderGuid = @guid", new { guid })).ToList();
        return await ArchiveAsync(guid, nid, names, files, ct)
            ? new { message = "تم رفع صور الحوالة على الأرشيف" }
            : throw new ApiException("تعذّر رفع صور الحوالة على خادم الأرشيف، حاول مرة أخرى", 502);
    }

    public async Task<object> SetPayStatusAsync(Guid guid, int status, string? reason, CancellationToken ct)
    {
        if (status is not (2 or 3)) throw new ApiException("الحالة المطلوبة غير صحيحة");
        if (string.IsNullOrWhiteSpace(reason)) throw new ApiException(status == 2 ? "يرجى كتابة سبب اعتبار الطلب مكرراً" : "يرجى كتابة سبب إلغاء الطلب");
        return await db.InTransactionAsync(async (c, tx) =>
        {
            var (code, _, payload) = await LockPayAsync(c, tx, guid);
            await c.ExecuteAsync("UPDATE dbo.WebPayOrder SET Status = @status, ReviewUserGuid = @u, ReviewDate = @now, ReviewNotes = @r WHERE Guid = @guid",
                new { guid, status, u = me.UserGuid, now = Clock.Now, r = Cut(reason.Trim(), 500) }, tx);
            await LogAsync(c, tx, guid, status == 2 ? "تعليم الطلب مكرراً" : "إلغاء الطلب", reason.Trim());
            string? regMsg = null;
            if (payload.RegOrder is { } regOrder)
            {
                // No confirmed down payment → no registration: the طلب التحاق is rejected (nothing was written for it).
                var why = $"لم تؤكد الحسابات الدفعة المقدمة (طلب السداد رقم {code} {(status == 2 ? "مكرر" : "ملغي")}): {reason.Trim()}";
                var regCode = await c.ExecuteScalarAsync<int?>("""
                    UPDATE dbo.WebRegOrder SET Status = 2, ReviewNotes = @why OUTPUT inserted.Code WHERE Guid = @regOrder AND Status = 3
                    """, new { regOrder, why = Cut(why, 500) }, tx);
                if (regCode != null)
                {
                    await LogAsync(c, tx, regOrder, "رفض الطلب", why);
                    regMsg = $"، ورُفض طلب الالتحاق رقم {regCode} ولن تُنشأ له استمارة";
                }
            }
            return (object)new { message = (status == 2 ? $"تم تعليم طلب السداد رقم {code} كطلب مكرر" : $"تم إلغاء طلب السداد رقم {code}") + regMsg };
        }, IsolationLevel.ReadCommitted, ct);
    }

    /* =============================== Shared =============================== */

    /// <summary>Requests waiting for review (sidebar badges), only for the screens the user can open.</summary>
    public async Task<object> CountsAsync(bool reg, bool payOrders, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return new
        {
            reg = reg ? await c.ExecuteScalarAsync<int>(new CommandDefinition("SELECT COUNT(*) FROM dbo.WebRegOrder WHERE Status = 0", cancellationToken: ct)) : 0,
            pay = payOrders ? await c.ExecuteScalarAsync<int>(new CommandDefinition("SELECT COUNT(*) FROM dbo.WebPayOrder WHERE Status = 0", cancellationToken: ct)) : 0,
        };
    }

    /// <summary>طلباتي: every request the signed-in user sent (both kinds). Waiting ones always, reviewed ones within the period.</summary>
    public async Task<object> ListMineAsync(DateTime? from, DateTime? to, string? q, CancellationToken ct)
    {
        var f = (from ?? Clock.Now.AddDays(-30)).Date; var t = (to ?? Clock.Now).Date.AddDays(1);
        q = string.IsNullOrWhiteSpace(q) ? null : q.Trim();
        var args = new { f, t, q, me = me.UserGuid };
        await using var c = await db.OpenAsync(ct);
        var regs = await c.QueryAsync<OrderListRow>(new CommandDefinition(RegListSql + """
             WHERE O.UserGuid = @me AND (O.Status IN (0,3) OR (O.OrderDate >= @f AND O.OrderDate < @t))
               AND (@q IS NULL OR S.StudentName LIKE '%' + @q + '%' OR S.NationalId LIKE @q + '%' OR S.StudentTel LIKE '%' + @q + '%' OR CAST(O.Code AS nvarchar(20)) = @q)
            """, args, cancellationToken: ct));
        var pays = await c.QueryAsync<OrderListRow>(new CommandDefinition(PayListSql + """
             WHERE O.UserGuid = @me AND (O.Status = 0 OR (O.OrderDate >= @f AND O.OrderDate < @t))
               AND (@q IS NULL OR S.StudentName LIKE '%' + @q + '%' OR S.NationalId LIKE @q + '%' OR O.PayRef LIKE '%' + @q + '%' OR CAST(O.Code AS nvarchar(20)) = @q)
            """, args, cancellationToken: ct));
        return new { reg = regs, pay = pays };
    }

    /// <summary>A student's requests (shown on the reception card so nothing is sent twice).</summary>
    public async Task<object> OfStudentAsync(Guid account, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var regs = await c.QueryAsync<(Guid Guid, int Code, DateTime OrderDate, int Status, string? Programs, decimal DocSubTotal, string? ReviewNotes, string? RegCode)>(new CommandDefinition(
            "SELECT TOP 20 Guid, Code, OrderDate, Status, Programs, DocSubTotal, ReviewNotes, RegCode FROM dbo.WebRegOrder WHERE AccountGuid = @account ORDER BY OrderDate DESC", new { account }, cancellationToken: ct));
        var pays = await c.QueryAsync<(Guid Guid, int Code, DateTime OrderDate, int Status, string? Programs, decimal Amount, string? ReviewNotes, string? TaxNo)>(new CommandDefinition(
            "SELECT TOP 20 Guid, Code, OrderDate, Status, Programs, Amount, ReviewNotes, TaxNo FROM dbo.WebPayOrder WHERE AccountGuid = @account ORDER BY OrderDate DESC", new { account }, cancellationToken: ct));
        return regs.Select(r => new StudentRequest("reg", r.Guid, r.Code, r.OrderDate, r.Status, RegStatusName(r.Status), r.Programs, r.DocSubTotal, r.ReviewNotes, r.RegCode))
            .Concat(pays.Select(r => new StudentRequest("pay", r.Guid, r.Code, r.OrderDate, r.Status, PayStatusName(r.Status), r.Programs, r.Amount, r.ReviewNotes, r.TaxNo)))
            .OrderByDescending(x => x.Date).ToList();
    }
}
