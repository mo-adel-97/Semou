using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "semou-web";
    public string Key { get; set; } = "";
    public int Hours { get; set; } = 12;
}

public sealed record LoginRequest(string UserName, string Password);
public sealed record LoginResponse(string Token, DateTime ExpiresAt);
public sealed record ChangePasswordRequest(string OldPassword, string NewPassword);
public sealed record MeResponse(
    Guid UserGuid, Guid? SellerGuid, string FullName,
    IReadOnlyDictionary<string, bool> Menus,
    IReadOnlyDictionary<string, FormPerms> Perms);

/// <summary>Port of LogIn + ChangeUserPassword from the desktop app.</summary>
public sealed class AuthService(Db db, PermissionService perms, CurrentUser me, IOptions<JwtOptions> jwt)
{
    private const string WrongLogin = "يرجى التأكد من اسم المستخدم أو كلمة المرور وإعادة المحاولة";

    private sealed record UserRow(Guid Guid, string? FullName, bool? Staut_, Guid? SellerGuid);

    /// <summary>LogIn.CmdOk_Click: match User_Info by UserName + Password and refuse stopped users.</summary>
    public async Task<LoginResponse> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.UserName) || string.IsNullOrEmpty(req.Password))
            throw new ApiException(WrongLogin);

        await using var c = await db.OpenAsync(ct);
        var u = await c.QueryFirstOrDefaultAsync<UserRow>(new CommandDefinition(
            "SELECT TOP 1 Guid, FullName, Staut_, SellerGuid FROM dbo.User_Info WHERE UserName = @UserName AND Password = @Password",
            new { UserName = req.UserName.Trim(), req.Password }, cancellationToken: ct));

        if (u is null) throw ApiException.Unauthorized(WrongLogin);
        if (u.Staut_ == false) throw ApiException.Forbidden("المستخدم موقوف يرجى مراجعة الإدارة");

        perms.Invalidate(u.Guid); // fresh permissions on every login
        return IssueToken(u);
    }

    private LoginResponse IssueToken(UserRow u)
    {
        var o = jwt.Value;
        var claims = new List<Claim>
        {
            new(SemouClaims.UserGuid, u.Guid.ToString()),
            new(SemouClaims.FullName, u.FullName ?? ""),
        };
        if (u.SellerGuid is { } s) claims.Add(new(SemouClaims.SellerGuid, s.ToString()));

        var expires = DateTime.UtcNow.AddHours(o.Hours);
        var token = new JwtSecurityToken(o.Issuer, o.Issuer, claims, expires: expires,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(o.Key)), SecurityAlgorithms.HmacSha256));
        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public async Task<MeResponse> MeAsync()
    {
        var uid = me.UserGuid;
        return new MeResponse(uid, me.SellerGuid, me.FullName,
            await perms.GetMenusAsync(uid), await perms.GetFormPermsAsync(uid));
    }

    /// <summary>ChangeUserPassword: the old password must match and the new one must not be empty; then proc ChangePassword.</summary>
    public async Task ChangePasswordAsync(ChangePasswordRequest req, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(req.NewPassword)) throw new ApiException("يرجى إدخال كلمة المرور الجديدة");
        if (req.NewPassword.Length > 50) throw new ApiException("كلمة المرور طويلة جداً (الحد الأقصى 50 حرف)");

        await using var c = await db.OpenAsync(ct);
        var current = await c.ExecuteScalarAsync<string?>(new CommandDefinition(
            "SELECT Password FROM dbo.User_Info WHERE Guid = @g", new { g = me.UserGuid }, cancellationToken: ct));
        if (current is null || current != req.OldPassword)
            throw new ApiException("كلمة المرور الحالية غير صحيحة، يرجى المحاولة مرة أخرى");

        await c.ExecuteAsync(new CommandDefinition("dbo.ChangePassword",
            new { Password = req.NewPassword, Guid = me.UserGuid },
            commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }
}
