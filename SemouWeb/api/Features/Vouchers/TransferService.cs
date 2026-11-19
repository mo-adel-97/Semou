using System.Data;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Vouchers;

/*
 * تحويل نقدية (EncodingTransMoney, form "transmoney"): money moved between two cash boxes / banks.
 * Journal: the receiving box is debited, the sending box is credited, both on the chosen cost center.
 */
public sealed class TransferInput
{
    public DateTime Date { get; set; }
    public Guid? BranchGuid { get; set; }
    /// <summary>CashBox.AccountGuid of the sending box/bank.</summary>
    public Guid? FromGuid { get; set; }
    public Guid? ToGuid { get; set; }
    public Guid? CostCenterGuid { get; set; }
    public decimal Amount { get; set; }
    public string? Ref { get; set; }
    public string Notes { get; set; } = "";
    public bool IsUse { get; set; } = true;
    public string? Reason { get; set; }
}

public sealed class TransferService(Db db, UserActionLog audit)
{
    public const string Form = "transmoney";

    public async Task<IEnumerable<dynamic>> ListAsync(DateTime? from, DateTime? to, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync(new CommandDefinition("""
            SELECT CAST(T.Code AS nvarchar(50)) AS Code, T.Guid, T.ActionDate AS Date, F.CashName AS FromName, TT.CashName AS ToName,
                   CAST(ISNULL(T.Amount,0) AS decimal(18,2)) AS Amount, T.Ref, T.Notes, CAST(ISNULL(T.IsUse,0) AS bit) AS IsUse, B.BrName AS BranchName
            FROM dbo.MoneyTrans T
            LEFT JOIN dbo.CashBox F ON F.AccountGuid = T.FromCashBoxGuid LEFT JOIN dbo.CashBox TT ON TT.AccountGuid = T.ToCashBoxGuid
            LEFT JOIN dbo.Branch B ON B.Guid = T.BranchGuid
            WHERE (@from IS NULL OR T.ActionDate >= @from) AND (@to IS NULL OR T.ActionDate < DATEADD(day, 1, @to))
            ORDER BY T.Id DESC
            """, new { from = from?.Date, to = to?.Date }, cancellationToken: ct));
    }

    public async Task<object> GetAsync(Guid guid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryFirstOrDefaultAsync(new CommandDefinition("""
            SELECT CAST(T.Code AS nvarchar(50)) AS Code, T.Guid, T.ActionDate AS Date, T.BranchGuid, T.FromCashBoxGuid AS FromGuid, T.ToCashBoxGuid AS ToGuid,
                   T.CostCenterGuid, CC.CenterName AS CostName, CAST(ISNULL(T.Amount,0) AS decimal(18,2)) AS Amount, T.Ref, T.Notes, CAST(ISNULL(T.IsUse,0) AS bit) AS IsUse,
                   (SELECT TOP 1 CAST(D.Code AS nvarchar(50)) FROM dbo.Day1 D WHERE D.DocGuid = T.Guid) AS JournalCode, dbo.FNTafkeet(ISNULL(T.Amount,0)) AS AmountText
            FROM dbo.MoneyTrans T LEFT JOIN dbo.CostCenter CC ON CC.Guid = T.CostCenterGuid WHERE T.Guid = @guid
            """, new { guid }, cancellationToken: ct)) ?? throw ApiException.NotFound("التحويل غير موجود");
    }

    private static void Validate(TransferInput i)
    {
        if (i.BranchGuid is null) throw new ApiException("يرجى اختيار الفرع أولاً");
        if (i.FromGuid is null) throw new ApiException("يرجى اختيار الخزينة / البنك المحول منها");
        if (i.ToGuid is null) throw new ApiException("يرجى اختيار الخزينة / البنك المحول إليها");
        if (i.FromGuid == i.ToGuid) throw new ApiException("لا يمكن التحويل من وإلى نفس الخزينة / البنك");
        if (i.CostCenterGuid is null) throw new ApiException("يرجى اختيار مركز التكلفة الخاص بعملية السداد");
        if (string.IsNullOrWhiteSpace(i.Ref)) throw new ApiException("يرجى إدخال رقم المرجع الخاص بعملية السداد");
        if (i.Ref.Trim().Length > 50) throw new ApiException("رقم المرجع طويل جداً");
        if (i.Amount <= 0) throw new ApiException("يرجى إدخال المبلغ الخاص بعملية السداد");
        if (string.IsNullOrWhiteSpace(i.Notes)) throw new ApiException("يرجى إدخال البيان الخاص بعملية التحويل");
        if (i.Date == default) throw new ApiException("يرجى اختيار التاريخ");
    }

    private static async Task CheckBoxesAsync(System.Data.Common.DbConnection c, System.Data.Common.DbTransaction tx, TransferInput i)
    {
        var n = await c.ExecuteScalarAsync<int>("SELECT COUNT(DISTINCT AccountGuid) FROM dbo.CashBox WHERE AccountGuid IN (@a, @b)", new { a = i.FromGuid, b = i.ToGuid }, tx);
        if (n != 2) throw new ApiException("الخزينة / البنك المختار غير صالح");
    }

    private static async Task JournalLinesAsync(System.Data.Common.DbConnection c, System.Data.Common.DbTransaction tx, Guid day1, TransferInput i)
    {
        var amount = (double)i.Amount;
        await c.ExecuteAsync("dbo.AddDay2", new { Day1Guid = day1, AccountGuid = i.ToGuid, Maden = amount, Daen = 0, Notes = i.Notes.Trim(), CostGuid = i.CostCenterGuid, IsUse = true }, tx, commandType: CommandType.StoredProcedure);
        await c.ExecuteAsync("dbo.AddDay2", new { Day1Guid = day1, AccountGuid = i.FromGuid, Maden = 0, Daen = amount, Notes = i.Notes.Trim(), CostGuid = i.CostCenterGuid, IsUse = true }, tx, commandType: CommandType.StoredProcedure);
    }

    private static DateTime Stamp(DateTime picked, DateTime? existing = null)
    {
        if (existing is { } e && e.Date == picked.Date) return e;
        var now = Clock.Now;
        return picked.Date == now.Date ? now : picked.Date;
    }

    public async Task<VoucherSaved> CreateAsync(TransferInput i, CancellationToken ct)
    {
        Validate(i);
        return await db.InTransactionAsync(async (c, tx) =>
        {
            await CheckBoxesAsync(c, tx, i);
            i.Date = Stamp(i.Date);
            var id = await c.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(Id), 0) + 1 FROM dbo.MoneyTrans WITH (UPDLOCK, HOLDLOCK)", transaction: tx);
            var code = id.ToString();
            await c.ExecuteAsync("dbo.AddMoneyTrans", new
            {
                Id = id, Code = code, ActionDate = i.Date, i.BranchGuid, FromCashBoxGuid = i.FromGuid, ToCashBoxGuid = i.ToGuid, i.CostCenterGuid,
                Amount = (double)i.Amount, Ref = i.Ref!.Trim(), Notes = i.Notes.Trim(), i.IsUse,
            }, tx, commandType: CommandType.StoredProcedure);
            var guid = await c.ExecuteScalarAsync<Guid>("SELECT Guid FROM dbo.MoneyTrans WHERE Id = @id", new { id }, tx);

            var typeGuid = await c.ExecuteScalarAsync<Guid>("dbo.FormTypeGuid", new { FormName = Form }, tx, commandType: CommandType.StoredProcedure);
            var openGuid = await c.ExecuteScalarAsync<Guid>("dbo.FormTypeGuid", new { FormName = "opendaily" }, tx, commandType: CommandType.StoredProcedure);
            var d = new DynamicParameters(new { OpenGuid = openGuid, DayDate = i.Date, Notes = i.Notes.Trim(), DayType = true, DocGuid = guid, IsUse = true, TypeGuid = typeGuid, ActionCode = code, i.BranchGuid });
            d.Add("CodeOut", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await c.ExecuteAsync("dbo.AddDay1", d, tx, commandType: CommandType.StoredProcedure);
            var day1 = await c.ExecuteScalarAsync<Guid>("SELECT TOP 1 Guid FROM dbo.Day1 WHERE DocGuid = @guid", new { guid }, tx);
            await JournalLinesAsync(c, tx, day1, i);
            await audit.WriteAsync(c, tx, Form, guid, UserActionLog.New, code);
            return new VoucherSaved("تم حفظ تحويل النقدية بنجاح", code, guid, d.Get<int>("CodeOut").ToString());
        }, IsolationLevel.Serializable, ct);
    }

    public async Task<VoucherSaved> UpdateAsync(Guid guid, TransferInput i, CancellationToken ct)
    {
        Validate(i);
        if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب الإجراء أولاً");
        return await db.InTransactionAsync(async (c, tx) =>
        {
            var cur = await c.QueryFirstOrDefaultAsync<(string Code, DateTime? Date)>("SELECT CAST(Code AS nvarchar(50)), ActionDate FROM dbo.MoneyTrans WITH (UPDLOCK) WHERE Guid = @guid", new { guid }, tx);
            if (cur.Code is null) throw ApiException.NotFound("التحويل غير موجود");
            await CheckBoxesAsync(c, tx, i);
            i.Date = Stamp(i.Date, cur.Date);
            await c.ExecuteAsync("dbo.UpdateMoneyTrans", new
            {
                Guid = guid, ActionDate = i.Date, i.BranchGuid, FromCashBoxGuid = i.FromGuid, ToCashBoxGuid = i.ToGuid, i.CostCenterGuid,
                Amount = (double)i.Amount, Ref = i.Ref!.Trim(), Notes = i.Notes.Trim(), i.IsUse,
            }, tx, commandType: CommandType.StoredProcedure);
            var day1 = await c.ExecuteScalarAsync<Guid?>("SELECT TOP 1 Guid FROM dbo.Day1 WHERE DocGuid = @guid", new { guid }, tx)
                       ?? throw new ApiException("لم يتم العثور على القيد المحاسبي الخاص بالتحويل");
            await c.ExecuteAsync("dbo.UpdateDay1", new { Guid = day1, DayDate = i.Date, Notes = i.Notes.Trim(), i.IsUse, i.BranchGuid }, tx, commandType: CommandType.StoredProcedure);
            await c.ExecuteAsync("dbo.DeleteDay2", new { Day1Guid = day1 }, tx, commandType: CommandType.StoredProcedure);
            await JournalLinesAsync(c, tx, day1, i);
            await audit.WriteAsync(c, tx, Form, guid, UserActionLog.Update, cur.Code, i.Reason!.Trim());
            return new VoucherSaved("تم تعديل تحويل النقدية بنجاح", cur.Code, guid, null);
        }, ct: ct);
    }
}

[ApiController]
[Authorize]
[Route("api/transfers")]
public sealed class TransfersController(TransferService transfers) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TransferService.Form, Perm.Find)]
    public async Task<IActionResult> List([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct) => Ok(await transfers.ListAsync(from, to, ct));

    [HttpGet("{guid:guid}")]
    [RequirePermission(TransferService.Form, Perm.Find)]
    public async Task<IActionResult> Get(Guid guid, CancellationToken ct) => Ok(await transfers.GetAsync(guid, ct));

    [HttpPost]
    [RequirePermission(TransferService.Form, Perm.Add)]
    public Task<VoucherSaved> Create(TransferInput input, CancellationToken ct) => transfers.CreateAsync(input, ct);

    // The desktop checked the "add" right here by mistake (FORSAVE in FORUPDATE); the web uses "edit".
    [HttpPut("{guid:guid}")]
    [RequirePermission(TransferService.Form, Perm.Edit)]
    public Task<VoucherSaved> Update(Guid guid, TransferInput input, CancellationToken ct) => transfers.UpdateAsync(guid, input, ct);
}
