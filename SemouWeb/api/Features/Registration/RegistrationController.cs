using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Registration;

[ApiController]
[Authorize]
[Route("api/registration")]
public sealed class RegistrationController(RegistrationService reg, AttachmentService attachments, Db db, Requests.RequestsService requests, PermissionService perms, CurrentUser me) : ControllerBase
{
    [HttpGet("prepare")]
    [RequirePermission(RegistrationService.Form, Perm.Add)]
    public async Task<IActionResult> Prepare([FromQuery] Guid student, [FromQuery] Guid branch, [FromQuery] int regType, CancellationToken ct)
        => Ok(await reg.PrepareAsync(student, branch, regType, ct));

    [HttpGet("course-prices")]
    [RequirePermission(RegistrationService.Form, Perm.Add)]
    public async Task<IActionResult> CoursePrices([FromQuery] Guid student, [FromQuery] Guid branch, [FromQuery] Guid course, CancellationToken ct)
        => Ok(await reg.CoursePricesAsync(student, branch, course, ct));

    /// <summary>Seats left per diploma in a batch (so the wizard shows availability before saving).</summary>
    [HttpGet("diplom-seats")]
    [RequirePermission(RegistrationService.Form, Perm.Add)]
    public async Task<IActionResult> DiplomSeats([FromQuery] Guid branch, [FromQuery] Guid batch, CancellationToken ct)
        => Ok(await reg.DiplomSeatsAsync(branch, batch, ct));

    /// <summary>Study branches with registration open (NewBranchList with IsRegOn).</summary>
    [HttpGet("branches")]
    [RequirePermission(RegistrationService.Form, Perm.Add)]
    public async Task<IActionResult> Branches(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return Ok(await c.QueryAsync(new CommandDefinition("SELECT Guid, BrName AS Name FROM dbo.Branch WHERE IsUse = 1 AND IsRegOn = 1 ORDER BY TRY_CONVERT(int, Code)", cancellationToken: ct)));
    }

    /// <summary>multipart/form-data: nationalId + files named by attachment code (ID, Certifcat, trans, trans2, contract, Letter).</summary>
    [HttpPost("attachments")]
    [RequirePermission(RegistrationService.Form, Perm.Add)]
    [RequestSizeLimit(60 * 1024 * 1024)]
    public async Task<IActionResult> Upload([FromForm] string nationalId, CancellationToken ct)
    {
        var files = Request.Form.Files.Select(f => (f.Name, f)).ToList();
        var result = await attachments.UploadAsync(nationalId, files, ct);
        await requests.StageAsync(nationalId, files, ct); // a copy for the reviewer of the طلب التحاق
        return Ok(result);
    }

    [HttpGet("attachments")]
    [RequirePermission(RegistrationService.Form, Perm.Add)]
    public IActionResult Uploaded([FromQuery] string nationalId) => Ok(new { uploaded = attachments.Uploaded(nationalId) });

    /// <summary>PRINTREGDOC: the registration form print (proc PrintRegDoc + header data).</summary>
    [HttpGet("{guid:guid}/print")]
    public async Task<IActionResult> Print(Guid guid, CancellationToken ct)
    {
        if (!(await perms.GetAsync(me.UserGuid, RegistrationService.Form)).Has(Perm.Find) && !(await perms.GetAsync(me.UserGuid, WebForms.RegOrders)).Has(Perm.Print))
            throw new ApiException(RequirePermissionAttribute.Message(Perm.Find), 403);
        await using var c = await db.OpenAsync(ct);
        var rows = (await c.QueryAsync(new CommandDefinition("dbo.PrintRegDoc", new { RegDocGuid = guid }, commandType: System.Data.CommandType.StoredProcedure, cancellationToken: ct)))
            .Cast<IDictionary<string, object?>>().ToList();
        if (rows.Count == 0) throw ApiException.NotFound("لم يتم العثور على بيانات الاستمارة");
        var head = await c.QueryFirstAsync(new CommandDefinition("""
            SELECT R.RegType, R.ChkContract, R.Notes, S.DocName, B.BrName, B.BrAddress, B.BrVatId, B.BrTel, B.BrFax, BT.BatchName, L.LevelName,
                   CO.CourseName, CO.DateStart, CO.DateEnd, CAST(ISNULL(R.IsPay,0) AS bit) AS IsPay, R.PayCode,
                   CAST(ISNULL((SELECT SUM(BL.BillTotal) FROM dbo.Bill BL WHERE BL.Guid = R.PayGuid),0) AS decimal(18,2)) AS PaidAmount
            FROM dbo.RegDoc R JOIN dbo.RegSetting S ON S.Guid = R.DocGuid JOIN dbo.Branch B ON B.Guid = R.BranchGuid
            LEFT JOIN dbo.Batch BT ON BT.Guid = R.BatchGuid LEFT JOIN dbo.Levels L ON L.Guid = R.LevelGuid LEFT JOIN dbo.Course CO ON CO.Guid = R.CourseGuid
            WHERE R.Guid = @guid
            """, new { guid }, cancellationToken: ct));
        var fees = await c.QueryAsync(new CommandDefinition("""
            SELECT D.DiplomName AS name, RD.Cost AS cost, RD.Tax AS tax, RD.SubTotal AS total FROM dbo.RegDocDiplom RD JOIN dbo.Diplom D ON D.Guid = RD.DiplomGuid
            WHERE RD.RegDocGuid = @guid AND RD.Type_ = 0 AND RD.DiplomType = 1 ORDER BY RD.ID
            """, new { guid }, cancellationToken: ct));
        var f = rows[0];
        return Ok(new
        {
            header = head,
            student = new { name = f["StudentName"], tel = f["StudentTel"], nationalId = f["NationalId"] },
            code = f["Code"]?.ToString(), date = Convert.ToDateTime(f["RegDate"]).ToString("yyyy/MM/dd   hh:mm tt", System.Globalization.CultureInfo.InvariantCulture),
            seller = f["ManFullName"], totals = new { total = f["DocTotal"], tax = f["DocTax"], subTotal = f["DocSubTotal"] },
            lines = rows.Select(r => new { name = r["DiplomName"], cost = r["Cost"], tax = r["Tax"], total = r["SubTotal"], taxRate = r["TaxValue"] }),
            fees,
        });
    }

    // No direct save from the web: the wizard sends a طلب التحاق (POST api/requests/registration) that is reviewed first.
}
