using System.Security.Claims;

namespace Semou.Api.Security;

public static class SemouClaims
{
    public const string UserGuid = "uid";
    public const string SellerGuid = "sid";
    public const string FullName = "fname";
}

/// <summary>Replaces the desktop globals AllFiles.USER_GUID____ / SELLERMAN_GUID____ / ONLINE_USER_NAME____.</summary>
public sealed class CurrentUser(IHttpContextAccessor http)
{
    private ClaimsPrincipal P => http.HttpContext?.User ?? new ClaimsPrincipal();

    public Guid UserGuid => Guid.TryParse(P.FindFirstValue(SemouClaims.UserGuid), out var g)
        ? g : throw new UnauthorizedAccessException();
    public Guid? SellerGuid => Guid.TryParse(P.FindFirstValue(SemouClaims.SellerGuid), out var g) ? g : null;
    public string FullName => P.FindFirstValue(SemouClaims.FullName) ?? "";
}
