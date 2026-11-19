using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Semou.Api.Data;

namespace Semou.Api.Security;

public enum Perm { Add, Edit, Delete, Print, Find, View }

public sealed record FormPerms(bool Add, bool Edit, bool Delete, bool Print, bool Find, bool View)
{
    public static readonly FormPerms None = new(false, false, false, false, false, false);

    public bool Has(Perm p) => p switch
    {
        Perm.Add => Add,
        Perm.Edit => Edit,
        Perm.Delete => Delete,
        Perm.Print => Print,
        Perm.Find => Find,
        Perm.View => View,
        _ => false
    };
}

/// <summary>
/// Same rules as the desktop app:
///  - Screen permissions: dbo.User_Premision joined to dbo.Form_Name by UnqName (proc DoUserHavePremision).
///    No row means no permission (FORSAVE / FORUPDATE / FORFIND / FORVIEW all return False).
///  - Main menu (ribbon page) visibility: dbo.UserMenu joined to dbo.MainMenu (proc ShowUserProfile).
///    Desktop behaviour kept: with NO row the menu stays visible; only an explicit View_ = 0 hides it.
/// Cached per user for a short time; the cache is cleared on login.
/// </summary>
public sealed class PermissionService(Db db, IMemoryCache cache)
{
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);

    private sealed record PermRow(string? UnqName, bool? Add_, bool? Edit, bool? Del, bool? Print_, bool? Find_, bool? View_);
    private sealed record MenuRow(string? UnqName, bool? View_);

    public async Task<IReadOnlyDictionary<string, FormPerms>> GetFormPermsAsync(Guid userGuid)
    {
        var result = await cache.GetOrCreateAsync($"perms:{userGuid}", async e =>
        {
            e.AbsoluteExpirationRelativeToNow = Ttl;
            await using var c = await db.OpenAsync();
            var rows = await c.QueryAsync<PermRow>(
                """
                SELECT F.UnqName, P.Add_, P.Edit, P.Del, P.Print_, P.Find_, P.View_
                FROM dbo.Form_Name F INNER JOIN dbo.User_Premision P ON F.Guid = P.FormGuid
                WHERE P.UserGuid = @userGuid
                ORDER BY P.ID
                """, new { userGuid });
            var map = new Dictionary<string, FormPerms>(StringComparer.OrdinalIgnoreCase);
            foreach (var r in rows)
            {
                var key = (r.UnqName ?? "").Trim();
                if (key.Length == 0 || map.ContainsKey(key)) continue; // the desktop only reads the first row
                map[key] = new FormPerms(r.Add_ == true, r.Edit == true, r.Del == true,
                                         r.Print_ == true, r.Find_ == true, r.View_ == true);
            }
            // Web-only screens (WebForms): their own table, which the desktop never touches.
            var web = await c.QueryAsync<PermRow>(
                """
                IF OBJECT_ID('dbo.WebFormPermission') IS NOT NULL
                    SELECT FormKey AS UnqName, Add_, Edit, Del, Print_, Find_, View_ FROM dbo.WebFormPermission WHERE UserGuid = @userGuid
                """, new { userGuid });
            foreach (var r in web)
            {
                var key = (r.UnqName ?? "").Trim();
                if (key.Length == 0 || !WebForms.IsWebForm(key)) continue;
                map[key] = new FormPerms(r.Add_ == true, r.Edit == true, r.Del == true,
                                         r.Print_ == true, r.Find_ == true, r.View_ == true);
            }
            return (IReadOnlyDictionary<string, FormPerms>)map;
        });
        return result!;
    }

    public async Task<FormPerms> GetAsync(Guid userGuid, string form)
        => (await GetFormPermsAsync(userGuid)).TryGetValue(form.Trim(), out var p) ? p : FormPerms.None;

    public async Task<IReadOnlyDictionary<string, bool>> GetMenusAsync(Guid userGuid)
    {
        var result = await cache.GetOrCreateAsync($"menus:{userGuid}", async e =>
        {
            e.AbsoluteExpirationRelativeToNow = Ttl;
            await using var c = await db.OpenAsync();
            var menus = await c.QueryAsync<string>("SELECT UnqName FROM dbo.MainMenu WHERE UnqName IS NOT NULL");
            var views = (await c.QueryAsync<MenuRow>(
                """
                SELECT M.UnqName, U.View_ FROM dbo.MainMenu M
                INNER JOIN dbo.UserMenu U ON M.Guid = U.MenuGuid
                WHERE M.IsUse = 1 AND U.UserGuid = @userGuid
                ORDER BY U.ID
                """, new { userGuid }))
                .Where(v => v.UnqName != null)
                .GroupBy(v => v.UnqName!.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().View_, StringComparer.OrdinalIgnoreCase);

            var map = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            foreach (var m in menus)
            {
                var key = m.Trim();
                map[key] = !views.TryGetValue(key, out var v) || v != false;
            }
            return (IReadOnlyDictionary<string, bool>)map;
        });
        return result!;
    }

    public void Invalidate(Guid userGuid)
    {
        cache.Remove($"perms:{userGuid}");
        cache.Remove($"menus:{userGuid}");
    }
}
