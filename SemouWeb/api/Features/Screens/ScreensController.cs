using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Screens;

/// <summary>Generic endpoints for every <see cref="ScreenDef"/>. Permissions are checked per screen against its Form_Name key.</summary>
[ApiController]
[Authorize]
[Route("api/screens/{key}")]
public sealed class ScreensController(ScreenService screens, PermissionService perms, CurrentUser me) : ControllerBase
{
    private static readonly Dictionary<Perm, string> Denied = new()
    {
        [Perm.Add] = "لا تملك صلاحية إضافة جديد",
        [Perm.Edit] = "لا تملك صلاحية تعديل البيانات",
        [Perm.Find] = "لا تملك صلاحية استعراض البيانات",
        [Perm.View] = "لا تملك صلاحية فتح هذه الشاشة",
    };

    private async Task<ScreenDef> Authorize(string key, Perm perm)
    {
        var s = ScreenRegistry.Find(key) ?? throw ApiException.NotFound("الشاشة غير موجودة");
        if (!(await perms.GetAsync(me.UserGuid, s.Form)).Has(perm)) throw ApiException.Forbidden(Denied[perm]);
        return s;
    }

    /// <summary>The screen layout for the React CrudScreen (no SQL is exposed).</summary>
    [HttpGet("meta")]
    public async Task<IActionResult> Meta(string key)
    {
        var s = await Authorize(key, Perm.View);
        return Ok(new
        {
            s.Key, s.Form, s.Title, s.Singular, s.Subtitle, s.NameField, s.ScopeField,
            Fields = s.Fields.Select(f => new
            {
                f.Name, f.Label, Type = f.Type.ToString(), f.Required, f.NonZero, f.MaxLength, f.Options, f.Suggestions,
                f.Lookup, f.Section, f.Span, f.Suffix, f.Hint, f.LockOnEdit, f.Ltr, f.Default,
            }),
            Columns = s.Columns.Select(c => new { c.Name, c.Label, Kind = c.Kind.ToString(), c.OptionsFrom }),
            Children = s.Children.Select(c => new { c.Name, c.Label, Kind = c.Kind.ToString(), c.ValueLabel, c.Required }),
        });
    }

    [HttpGet]
    public async Task<IActionResult> List(string key, [FromQuery] Guid? scope, [FromQuery] bool? isUse, CancellationToken ct)
        => Ok(await screens.ListAsync(await Authorize(key, Perm.Find), scope, isUse, ct));

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Get(string key, Guid guid, CancellationToken ct)
        => Ok(await screens.GetAsync(await Authorize(key, Perm.Find), guid, ct));

    [HttpPost]
    public async Task<SaveResult> Create(string key, SaveRequest req, CancellationToken ct)
        => await screens.CreateAsync(await Authorize(key, Perm.Add), req, ct);

    [HttpPut("{guid:guid}")]
    public async Task<SaveResult> Update(string key, Guid guid, SaveRequest req, CancellationToken ct)
        => await screens.UpdateAsync(await Authorize(key, Perm.Edit), guid, req, ct);
}
