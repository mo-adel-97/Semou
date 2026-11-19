using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Security;

namespace Semou.Api.Features.Branches;

/// <summary>Port of the desktop "بطاقة فرع" screen (EncodingBranch + NewBranchList). Permission key: addbranch.</summary>
[ApiController]
[Authorize]
[Route("api/branches")]
public sealed class BranchesController(BranchService branches) : ControllerBase
{
    [HttpGet]
    [RequirePermission(BranchService.Form, Perm.Find)]
    public Task<IEnumerable<BranchListItem>> Search([FromQuery] string? q, [FromQuery] bool? isUse,
        [FromQuery] bool? isRegOn, CancellationToken ct) => branches.SearchAsync(q, isUse, isRegOn, ct);

    [HttpGet("{code}")]
    [RequirePermission(BranchService.Form, Perm.Find)]
    public Task<BranchDetails> Get(string code, CancellationToken ct) => branches.GetAsync(code, ct);

    [HttpPost]
    [RequirePermission(BranchService.Form, Perm.Add)]
    public Task<BranchSaved> Create(BranchInput input, CancellationToken ct) => branches.CreateAsync(input, ct);

    [HttpPut("{guid:guid}")]
    [RequirePermission(BranchService.Form, Perm.Edit)]
    public Task<BranchSaved> Update(Guid guid, UpdateBranchRequest req, CancellationToken ct) => branches.UpdateAsync(guid, req, ct);
}
