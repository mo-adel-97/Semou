using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Settings;

/// <summary>الإعدادات العامة (websettings): View = open the screen and the history, Edit = save / restore.</summary>
[ApiController]
[Authorize]
[Route("api/ui-config")]
public sealed class UiConfigController(UiConfigService config, PermissionService perms, CurrentUser me) : ControllerBase
{
    private async Task Need(Perm p)
    {
        if (!(await perms.GetAsync(me.UserGuid, WebForms.Settings)).Has(p)) throw new ApiException(RequirePermissionAttribute.Message(p), 403);
    }

    /// <summary>Every logged-in user reads the current settings: they shape the interface itself.</summary>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await config.GetAsync(ct));

    [HttpPut("{section}")]
    public async Task<IActionResult> Save(string section, UiConfigService.SaveInput input, CancellationToken ct)
    { await Need(Perm.Edit); return Ok(await config.SaveAsync(section, input, ct)); }

    [HttpGet("{section}/history")]
    public async Task<IActionResult> History(string section, CancellationToken ct) { await Need(Perm.View); return Ok(await config.HistoryAsync(section, ct)); }

    [HttpGet("{section}/history/{id:int}")]
    public async Task<IActionResult> Version(string section, int id, CancellationToken ct) { await Need(Perm.View); return Ok(await config.VersionAsync(section, id, ct)); }
}
