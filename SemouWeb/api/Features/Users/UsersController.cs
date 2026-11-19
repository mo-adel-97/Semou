using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Security;

namespace Semou.Api.Features.Users;

/// <summary>Users and permissions (desktop "بطاقة مستخدم"). Permission key: adduser.</summary>
[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UsersController(UsersService users) : ControllerBase
{
    [HttpGet]
    [RequirePermission(UsersService.Form, Perm.Find)]
    public Task<IEnumerable<UserListItem>> List([FromQuery] bool? isUse, CancellationToken ct) => users.ListAsync(isUse, ct);

    [HttpGet("catalog")]
    [RequirePermission(UsersService.Form, Perm.View)]
    public Task<UserCatalog> Catalog(CancellationToken ct) => users.CatalogAsync(ct);

    [HttpGet("{guid:guid}")]
    [RequirePermission(UsersService.Form, Perm.Find)]
    public Task<object> Get(Guid guid, CancellationToken ct) => users.GetAsync(guid, ct);

    [HttpPost]
    [RequirePermission(UsersService.Form, Perm.Add)]
    public Task<UserSaved> Create(UserInput input, CancellationToken ct) => users.CreateAsync(input, ct);

    [HttpPut("{guid:guid}")]
    [RequirePermission(UsersService.Form, Perm.Edit)]
    public Task<UserSaved> Update(Guid guid, UpdateUserRequest req, CancellationToken ct) => users.UpdateAsync(guid, req, ct);
}
