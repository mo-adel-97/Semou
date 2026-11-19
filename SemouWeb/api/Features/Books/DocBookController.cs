using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semou.Api.Security;

namespace Semou.Api.Features.Books;

[ApiController]
[Authorize]
[Route("api/doc-books")]
public sealed class DocBookController(DocBookService books) : ControllerBase
{
    [HttpGet("lookups")]
    [RequirePermission(DocBookService.Form, Perm.View)]
    public async Task<IActionResult> Lookups(CancellationToken ct) => Ok(await books.LookupsAsync(ct));

    [HttpGet]
    [RequirePermission(DocBookService.Form, Perm.Find)]
    public async Task<IActionResult> List([FromQuery] Guid? type, CancellationToken ct) => Ok(await books.ListAsync(type, ct));

    [HttpGet("{guid:guid}")]
    [RequirePermission(DocBookService.Form, Perm.Find)]
    public async Task<IActionResult> Get(Guid guid, CancellationToken ct) => Ok(await books.GetAsync(guid, ct));

    [HttpPost]
    [RequirePermission(DocBookService.Form, Perm.Add)]
    public Task<object> Create(DocBookInput input, CancellationToken ct) => books.SaveAsync(null, input, ct);

    [HttpPut("{guid:guid}")]
    [RequirePermission(DocBookService.Form, Perm.Edit)]
    public Task<object> Update(Guid guid, DocBookInput input, CancellationToken ct) => books.SaveAsync(guid, input, ct);
}
