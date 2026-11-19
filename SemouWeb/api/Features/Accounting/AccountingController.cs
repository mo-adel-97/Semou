using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Accounting;

/// <summary>
/// Chart of accounts, cost centers, cash boxes and banks.
/// Tree view is allowed with either the chart screen ("accountchart"/"centertree") or the card screen ("addaccount"/"addcostcenter").
/// </summary>
[ApiController]
[Authorize]
[Route("api/accounting")]
public sealed class AccountingController(ChartService charts, CashBoxService cash, PermissionService perms, CurrentUser me) : ControllerBase
{
    private static ChartKind Kind(string chart) => chart switch
    {
        "accounts" => ChartKind.Accounts,
        "cost-centers" => ChartKind.CostCenters,
        _ => throw ApiException.NotFound("غير موجود"),
    };

    private async Task Require(string form, Perm p, string msg)
    {
        if (!(await perms.GetAsync(me.UserGuid, form)).Has(p)) throw ApiException.Forbidden(msg);
    }

    private async Task RequireTree(ChartKind k)
    {
        var card = ChartService.FormOf(k);
        var tree = k == ChartKind.Accounts ? "accountchart" : "centertree";
        if (!(await perms.GetAsync(me.UserGuid, card)).Find && !(await perms.GetAsync(me.UserGuid, tree)).Find)
            throw ApiException.Forbidden("لا تملك صلاحية استعراض البيانات");
    }

    [HttpGet("{chart}/tree")]
    public async Task<IActionResult> Tree(string chart, CancellationToken ct)
    {
        var k = Kind(chart);
        await RequireTree(k);
        return Ok(await charts.TreeAsync(k, ct));
    }

    [HttpGet("{chart}/{guid:guid}")]
    public async Task<IActionResult> Get(string chart, Guid guid, CancellationToken ct)
    {
        var k = Kind(chart);
        await RequireTree(k);
        return Ok(await charts.GetAsync(k, guid, ct));
    }

    [HttpPost("{chart}")]
    public async Task<ChartSaved> Create(string chart, ChartNodeInput input, CancellationToken ct)
    {
        var k = Kind(chart);
        await Require(ChartService.FormOf(k), Perm.Add, "لا تملك صلاحية إضافة جديد");
        return await charts.CreateAsync(k, input, ct);
    }

    [HttpPut("{chart}/{guid:guid}")]
    public async Task<ChartSaved> Update(string chart, Guid guid, ChartNodeInput input, CancellationToken ct)
    {
        var k = Kind(chart);
        await Require(ChartService.FormOf(k), Perm.Edit, "لا تملك صلاحية تعديل البيانات");
        return await charts.UpdateAsync(k, guid, input, ct);
    }

    /* ---------------- Cash boxes & banks ---------------- */

    private static CashBoxService.Kind CashKind(string kind) => kind switch
    {
        "cash-boxes" => CashBoxService.CashBox,
        "banks" => CashBoxService.Bank,
        _ => throw ApiException.NotFound("غير موجود"),
    };

    [HttpGet("{kind}")]
    public async Task<IActionResult> CashList(string kind, [FromQuery] bool? isUse, CancellationToken ct)
    {
        var k = CashKind(kind);
        await Require(k.Form, Perm.Find, "لا تملك صلاحية استعراض البيانات");
        return Ok(await cash.ListAsync(k, isUse, ct));
    }

    [HttpGet("{kind}/parent")]
    public async Task<IActionResult> CashParent(string kind, CancellationToken ct)
    {
        var k = CashKind(kind);
        await Require(k.Form, Perm.View, "لا تملك صلاحية فتح هذه الشاشة");
        return Ok(await cash.ParentAsync(k, ct));
    }

    [HttpGet("{kind}/item/{guid:guid}")]
    public async Task<IActionResult> CashGet(string kind, Guid guid, CancellationToken ct)
    {
        var k = CashKind(kind);
        await Require(k.Form, Perm.Find, "لا تملك صلاحية استعراض البيانات");
        return Ok(await cash.GetAsync(k, guid, ct));
    }

    [HttpPost("{kind}/item")]
    public async Task<ChartSaved> CashCreate(string kind, CashBoxService.Input input, CancellationToken ct)
    {
        var k = CashKind(kind);
        await Require(k.Form, Perm.Add, "لا تملك صلاحية إضافة جديد");
        return await cash.CreateAsync(k, input, ct);
    }

    [HttpPut("{kind}/item/{guid:guid}")]
    public async Task<ChartSaved> CashUpdate(string kind, Guid guid, CashBoxService.Input input, CancellationToken ct)
    {
        var k = CashKind(kind);
        await Require(k.Form, Perm.Edit, "لا تملك صلاحية تعديل البيانات");
        return await cash.UpdateAsync(k, guid, input, ct);
    }
}
