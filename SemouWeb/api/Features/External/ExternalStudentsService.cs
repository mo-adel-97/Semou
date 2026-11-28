using System.Data;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.External;

/*
 * بيانات الطالب الكاملة لأنظمة خارجية (GET /api/external/students/{nationalId}, header X-Api-Key).
 * Everything known about one student, with every id (GUID) so the caller can link back: the card, its branch and
 * status, every registration form with its items, batch, level, course, seller and package, the admission requests
 * still under review, and the account balance. Read-only.
 */
public sealed class ExternalStudentsService(Db db)
{
    public static string? CleanId(string? id)
    {
        var d = new string((id ?? "").Where(char.IsDigit).ToArray());
        return d.Length == 10 ? d : null;
    }

    public async Task<object?> ByNationalIdAsync(string nationalId, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var s = await c.QueryFirstOrDefaultAsync(new CommandDefinition("""
            SELECT S.Guid AS guid, S.AccountGuid AS accountGuid, S.Code AS code, S.AcadmyId AS academyId,
                   S.StudentName AS name, S.StudentNameEn AS nameEn, S.NationalId AS nationalId, S.StudentTel AS mobile, S.Email AS email, S.BirthDate AS birthDate,
                   S.StudentNational AS nationality, CASE WHEN S.StudentNational = 1 THEN N'غير سعودي' ELSE N'سعودي' END AS nationalityName,
                   S.StudentType AS studentType, CAST(ISNULL(S.IsUse,1) AS bit) AS active, CAST(ISNULL(S.LockProfile,0) AS bit) AS profileLocked, S.Notes AS notes,
                   S.BarnchGuid AS branchGuid, B.BrName AS branchName, S.RegCaseGuid AS statusGuid, RC.StautName AS statusName
            FROM dbo.Student S LEFT JOIN dbo.Branch B ON B.Guid = S.BarnchGuid LEFT JOIN dbo.Stauts RC ON RC.Guid = S.RegCaseGuid
            WHERE S.NationalId = @nationalId
            """, new { nationalId }, cancellationToken: ct));
        if (s is null) return null;
        var account = (Guid)((IDictionary<string, object?>)s)["accountGuid"]!;

        var regs = (await c.QueryAsync(new CommandDefinition("""
            SELECT R.Guid AS guid, CAST(R.Code AS nvarchar(50)) AS code, R.RegDate AS date, ISNULL(R.RegType,0) AS regType,
                   CASE WHEN ISNULL(R.RegType,0) = 1 THEN N'دورة' ELSE N'دبلوم' END AS regTypeName,
                   CAST(ISNULL(R.Linked,0) AS bit) AS accepted, CAST(ISNULL(R.IsReReg,0) AS bit) AS returned, CAST(CASE WHEN ISNULL(R.IstUse,1) = 0 THEN 1 ELSE 0 END AS bit) AS cancelled,
                   CAST(ISNULL(R.IsEnd,0) AS bit) AS ended, CAST(ISNULL(R.ChkContract,0) AS bit) AS contractSigned, R.ContractDate AS contractDate,
                   R.DocGuid AS bookGuid, RS.DocName AS bookName, R.BranchGuid AS branchGuid, B.BrName AS branchName,
                   NULLIF(R.BatchGuid, '00000000-0000-0000-0000-000000000000') AS batchGuid, BT.BatchName AS batchName,
                   NULLIF(R.LevelGuid, '00000000-0000-0000-0000-000000000000') AS levelGuid, L.LevelName AS levelName,
                   NULLIF(R.CourseGuid, '00000000-0000-0000-0000-000000000000') AS courseGuid, CO.CourseName AS courseCode,
                   R.SellerGuid AS sellerGuid, SM.ManFullName AS sellerName,
                   CAST(ISNULL(R.DocTotal,0) AS decimal(18,2)) AS total, CAST(ISNULL(R.DocTax,0) AS decimal(18,2)) AS tax, CAST(ISNULL(R.DocSubTotal,0) AS decimal(18,2)) AS subTotal,
                   CAST(ISNULL(R.IsPay,0) AS bit) AS downPaymentPaid, NULLIF(R.PayGuid, '00000000-0000-0000-0000-000000000000') AS downPaymentInvoiceGuid, NULLIF(R.PayCode,'') AS downPaymentInvoiceCode,
                   P.PlanGuid AS packageGuid, P.PlanName AS packageName, P.Months AS packageMonths, P.Price AS packagePrice, P.DownPayment AS packageDownPayment,
                   P.Installment AS packageInstallment, P.FirstDue AS packageFirstDue
            FROM dbo.RegDoc R
            LEFT JOIN dbo.RegSetting RS ON RS.Guid = R.DocGuid LEFT JOIN dbo.Branch B ON B.Guid = R.BranchGuid LEFT JOIN dbo.Batch BT ON BT.Guid = R.BatchGuid
            LEFT JOIN dbo.Levels L ON L.Guid = R.LevelGuid LEFT JOIN dbo.Course CO ON CO.Guid = R.CourseGuid LEFT JOIN dbo.SalesMan SM ON SM.Guid = R.SellerGuid
            LEFT JOIN dbo.WebRegPlan P ON P.RegDocGuid = R.Guid
            WHERE R.AccountGuid = @account ORDER BY R.RegDate DESC
            """, new { account }, cancellationToken: ct))).Select(r => (IDictionary<string, object?>)r).ToList();

        var items = (await c.QueryAsync(new CommandDefinition("""
            SELECT RD.RegDocGuid, RD.DiplomGuid AS diplomGuid, D.Code AS code, D.DiplomName AS name, D.EnglishName AS nameEn,
                   CASE WHEN RD.DiplomType = '1' THEN N'رسوم' ELSE N'برنامج' END AS kind,
                   CAST(ISNULL(RD.Cost,0) AS decimal(18,2)) AS cost, CAST(ISNULL(RD.Tax,0) AS decimal(18,2)) AS tax, CAST(ISNULL(RD.SubTotal,0) AS decimal(18,2)) AS total
            FROM dbo.RegDocDiplom RD JOIN dbo.RegDoc R ON R.Guid = RD.RegDocGuid LEFT JOIN dbo.Diplom D ON D.Guid = RD.DiplomGuid
            WHERE R.AccountGuid = @account AND RD.Type_ = '0' ORDER BY RD.ID
            """, new { account }, cancellationToken: ct))).Select(r => (IDictionary<string, object?>)r).ToList();
        foreach (var r in regs)
            r["items"] = items.Where(i => Equals(i["RegDocGuid"], r["guid"])).Select(i => { var x = new Dictionary<string, object?>(i); x.Remove("RegDocGuid"); return x; }).ToList();

        var requests = await c.QueryAsync(new CommandDefinition("""
            IF OBJECT_ID('dbo.WebRegOrder') IS NULL SELECT TOP 0 1 AS x ELSE
            SELECT O.Guid AS guid, O.Code AS code, O.OrderDate AS date, O.Status AS status,
                   CASE O.Status WHEN 0 THEN N'بانتظار المراجعة' WHEN 1 THEN N'مقبول' WHEN 2 THEN N'مرفوض' WHEN 3 THEN N'بانتظار تأكيد السداد' ELSE N'' END AS statusName,
                   O.Programs AS programs, CAST(ISNULL(O.DocSubTotal,0) AS decimal(18,2)) AS subTotal, CAST(ISNULL(O.PayAmount,0) AS decimal(18,2)) AS downPayment,
                   O.RegDocGuid AS registrationGuid, O.RegCode AS registrationCode
            FROM dbo.WebRegOrder O WHERE O.AccountGuid = @account ORDER BY O.OrderDate DESC
            """, new { account }, cancellationToken: ct));

        // Same rule as the account statement: invoice debits and return credits are shown but not counted.
        var bal = await c.QueryFirstAsync<(decimal Debit, decimal Credit)>(new CommandDefinition("""
            SELECT CAST(ISNULL(SUM(CASE WHEN F.UnqName <> 'salesinvoice' THEN ISNULL(X.Maden,0) ELSE 0 END),0) AS decimal(18,2)),
                   CAST(ISNULL(SUM(CASE WHEN F.UnqName <> 'repay' THEN ISNULL(X.Daen,0) ELSE 0 END),0) AS decimal(18,2))
            FROM dbo.Day2 X JOIN dbo.Day1 Y ON Y.Guid = X.Day1Guid JOIN dbo.Form_Name F ON F.Guid = Y.TypeGuid
            WHERE X.AccountGuid = @account AND Y.IsUse = 1
            """, new { account }, cancellationToken: ct));

        return new
        {
            student = s,
            account = new { debit = bal.Debit, credit = bal.Credit, balance = bal.Debit - bal.Credit },
            registrations = regs,
            requests,
            generatedAt = Clock.Now,
        };
    }
}
