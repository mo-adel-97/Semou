using System.Data;
using System.Data.Common;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Security;

namespace Semou.Api.Features.Users;

/// <summary>Port of EncodingUser: user card, screen permissions, main menus, report branches.</summary>
public sealed class UsersService(Db db, UserActionLog audit, PermissionService perms, CurrentUser me)
{
    public const string Form = "adduser";

    private const string ListSql = """
        SELECT U.Code, U.Guid, U.UserName, U.FullName, U.Staut_, S.ManFullName AS SellerName,
               (SELECT COUNT(*) FROM dbo.User_Premision P WHERE P.UserGuid = U.Guid AND P.View_ = 1) AS Screens
        FROM dbo.User_Info U LEFT JOIN dbo.SalesMan S ON S.Guid = U.SellerGuid
        """;

    public async Task<IEnumerable<UserListItem>> ListAsync(bool? isUse, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync<UserListItem>(new CommandDefinition(
            ListSql + " WHERE (@isUse IS NULL OR U.Staut_ = @isUse) ORDER BY U.Code", new { isUse }, cancellationToken: ct));
    }

    /// <summary>Everything the card needs to build its grids (LoadMainForm, LoadMainMenu, SalesManList).</summary>
    public async Task<UserCatalog> CatalogAsync(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var forms = (await c.QueryAsync<FormItem>(new CommandDefinition("SELECT Guid, Name, UnqName, Code FROM dbo.Form_Name WHERE IsUse = 1 ORDER BY Code", cancellationToken: ct))).ToList();
        // Web-only screens (permissions kept in dbo.WebFormPermission).
        forms.AddRange(WebForms.All.Select((f, i) => new FormItem(WebForms.GuidOf(f.Key), f.Name, f.Key, 9000 + i)));
        var menus = await c.QueryAsync<MenuItem>(new CommandDefinition("SELECT Guid, MenuName, UnqName FROM dbo.MainMenu WHERE IsUse = 1 ORDER BY Code", cancellationToken: ct));
        var sellers = await c.QueryAsync<SellerItem>(new CommandDefinition("SELECT Guid, ManFullName AS Name, IsUse FROM dbo.SalesMan ORDER BY TRY_CONVERT(int, Code)", cancellationToken: ct));
        var users = await c.QueryAsync<UserListItem>(new CommandDefinition(ListSql + " ORDER BY U.Code", cancellationToken: ct));
        return new UserCatalog(forms, menus, sellers, users);
    }

    private sealed record InfoRow(int? Code, Guid Guid, string? UserName, string? FullName, bool? Staut_, bool? ChkBranch, Guid? BranchGuid,
                                  bool? ChkAmount, bool? ChkOtherFess, Guid? SellerGuid);
    private sealed record PermDb(Guid FormGuid, bool? Add_, bool? Edit, bool? Del, bool? Print_, bool? Find_, bool? View_);
    private sealed record MenuDb(Guid MenuGuid, bool? View_);

    /// <summary>SHOWUSERINFO + SHOWUSERMAINMENU + SHOWUSERFORM + SHOWUSERBRANCH (without the password).</summary>
    public async Task<object> GetAsync(Guid guid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var u = await c.QueryFirstOrDefaultAsync<InfoRow>(new CommandDefinition(
            "SELECT Code, Guid, UserName, FullName, Staut_, ChkBranch, BranchGuid, ChkAmount, ChkOtherFess, SellerGuid FROM dbo.User_Info WHERE Guid = @guid",
            new { guid }, cancellationToken: ct)) ?? throw ApiException.NotFound("المستخدم غير موجود");

        var menus = await c.QueryAsync<MenuDb>(new CommandDefinition("SELECT MenuGuid, View_ FROM dbo.UserMenu WHERE UserGuid = @guid ORDER BY ID", new { guid }, cancellationToken: ct));
        var forms = await c.QueryAsync<PermDb>(new CommandDefinition("SELECT FormGuid, Add_, Edit, Del, Print_, Find_, View_ FROM dbo.User_Premision WHERE UserGuid = @guid ORDER BY ID", new { guid }, cancellationToken: ct));
        var branches = await c.QueryAsync<Guid>(new CommandDefinition("SELECT BranchGuid FROM dbo.UserBranch WHERE UserGuid = @guid ORDER BY ID", new { guid }, cancellationToken: ct));
        var web = await c.QueryAsync<(string FormKey, bool Add_, bool Edit, bool Del, bool Print_, bool Find_, bool View_)>(new CommandDefinition(
            "SELECT FormKey, Add_, Edit, Del, Print_, Find_, View_ FROM dbo.WebFormPermission WHERE UserGuid = @guid", new { guid }, cancellationToken: ct));
        var webPerms = web.Where(w => WebForms.IsWebForm(w.FormKey)).Select(w => new PermRow
        {
            FormGuid = WebForms.GuidOf(w.FormKey), Add = w.Add_, Edit = w.Edit, Delete = w.Del, Print = w.Print_, Find = w.Find_, View = w.View_,
        });

        return new
        {
            code = u.Code?.ToString(),
            u.Guid,
            user = new
            {
                u.FullName, u.UserName, Staut_ = u.Staut_ ?? false, ChkBranch = u.ChkBranch ?? false,
                BranchGuid = u.BranchGuid == Guid.Empty ? null : u.BranchGuid,
                ChkAmount = u.ChkAmount ?? false, ChkOtherFess = u.ChkOtherFess ?? false,
                SellerGuid = u.SellerGuid == Guid.Empty ? null : u.SellerGuid,
                Menus = menus.GroupBy(m => m.MenuGuid).Select(g => new MenuRow { MenuGuid = g.Key, View = g.First().View_ == true }),
                Perms = forms.GroupBy(f => f.FormGuid).Select(g => g.First()).Select(f => new PermRow
                {
                    FormGuid = f.FormGuid, Add = f.Add_ == true, Edit = f.Edit == true, Delete = f.Del == true,
                    Print = f.Print_ == true, Find = f.Find_ == true, View = f.View_ == true,
                }).Concat(webPerms),
                Branches = branches.Distinct(),
            },
        };
    }

    /// <summary>VALIDATE_DATA + UsernameExists + the checks the desktop relied on implicitly.</summary>
    private static async Task ValidateAsync(DbConnection c, DbTransaction tx, UserInput u, Guid? self, bool isNew)
    {
        if (isNew && string.IsNullOrEmpty(u.Password)) throw new ApiException("يرجى إدخال كلمة المرور");
        // Login does Guid.Parse(SellerGuid): a user without a salesman could not log in to the desktop app.
        if (u.SellerGuid is null || u.SellerGuid == Guid.Empty) throw new ApiException("يرجى اختيار مندوب البيع");
        if (u.ChkBranch && (u.BranchGuid is null || u.BranchGuid == Guid.Empty)) throw new ApiException("يرجى اختيار الفرع الافتراضي");

        var exists = await c.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM dbo.User_Info WHERE UserName = @n AND (@self IS NULL OR Guid <> @self)", new { n = u.UserName.Trim(), self }, tx);
        if (exists > 0) throw new ApiException("اسم المستخدم الذي تم ادخاله مستخدم مسبقاً");
    }

    private static object InfoParams(UserInput u, string password) => new
    {
        UserName = u.UserName.Trim(),
        Password = password,
        FullName = u.FullName.Trim(),
        u.Staut_,
        u.ChkBranch,
        BranchGuid = u.ChkBranch ? u.BranchGuid!.Value : Guid.Empty, // desktop writes the empty guid when unchecked
        u.ChkAmount,
        u.ChkOtherFess,
        SellerGuid = u.SellerGuid!.Value,
    };

    /// <summary>ADDUSERMENU + ADDUSERPRIMISION + ADDUSERBRANCH (after deleting the old rows on edit).</summary>
    private static async Task SaveAccessAsync(DbConnection c, DbTransaction tx, Guid userGuid, UserInput u, bool isNew)
    {
        if (!isNew)
        {
            await c.ExecuteAsync("dbo.DeleteUserMenu", new { UserGuid = userGuid }, tx, commandType: CommandType.StoredProcedure);
            await c.ExecuteAsync("dbo.DeleteUserPrimision", new { UserGuid = userGuid }, tx, commandType: CommandType.StoredProcedure);
            await c.ExecuteAsync("dbo.DeleteUserBranch", new { UserGuid = userGuid }, tx, commandType: CommandType.StoredProcedure);
        }

        // Only real menus/screens are accepted (no arbitrary guids from the client).
        var validMenus = (await c.QueryAsync<Guid>("SELECT Guid FROM dbo.MainMenu WHERE IsUse = 1", transaction: tx)).ToHashSet();
        var validForms = (await c.QueryAsync<Guid>("SELECT Guid FROM dbo.Form_Name WHERE IsUse = 1", transaction: tx)).ToHashSet();
        var validBranches = (await c.QueryAsync<Guid>("SELECT Guid FROM dbo.Branch", transaction: tx)).ToHashSet();

        // Every main menu gets a row (like the desktop grid), checked or not.
        var menuView = u.Menus.Where(m => validMenus.Contains(m.MenuGuid)).GroupBy(m => m.MenuGuid).ToDictionary(g => g.Key, g => g.First().View);
        foreach (var m in validMenus)
            await c.ExecuteAsync("dbo.AddUserMenu", new { UserGuid = userGuid, MenuGuid = m, View_ = menuView.GetValueOrDefault(m) }, tx, commandType: CommandType.StoredProcedure);

        // Screens: only rows with at least one right (no row = no permission, same result as an all-false row).
        foreach (var p in u.Perms.Where(p => p.Any && validForms.Contains(p.FormGuid)).DistinctBy(p => p.FormGuid))
            await c.ExecuteAsync("dbo.AddUserPremision", new
            {
                UserGuid = userGuid, p.FormGuid, Add_ = p.Add, p.Edit, Del = p.Delete, Print_ = p.Print, Find_ = p.Find, View_ = p.View,
            }, tx, commandType: CommandType.StoredProcedure);

        // Web-only screens: their own table (the desktop rewrites User_Premision on every user save).
        await c.ExecuteAsync("DELETE FROM dbo.WebFormPermission WHERE UserGuid = @userGuid", new { userGuid }, tx);
        foreach (var p in u.Perms.Where(p => p.Any).DistinctBy(p => p.FormGuid))
            if (WebForms.ByGuid(p.FormGuid) is { } wf)
                await c.ExecuteAsync(
                    "INSERT INTO dbo.WebFormPermission (UserGuid, FormKey, Add_, Edit, Del, Print_, Find_, View_) VALUES (@userGuid, @key, @Add, @Edit, @Delete, @Print, @Find, @View)",
                    new { userGuid, key = wf.Key, p.Add, p.Edit, p.Delete, p.Print, p.Find, p.View }, tx);

        foreach (var b in u.Branches.Where(validBranches.Contains).Distinct())
            await c.ExecuteAsync("dbo.AddUserBranch", new { UserGuid = userGuid, BranchGuid = b }, tx, commandType: CommandType.StoredProcedure);
    }

    public async Task<UserSaved> CreateAsync(UserInput u, CancellationToken ct)
    {
        var saved = await db.InTransactionAsync(async (c, tx) =>
        {
            await ValidateAsync(c, tx, u, null, isNew: true);
            var id = await c.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(Id), 0) + 1 FROM dbo.User_Info WITH (UPDLOCK, HOLDLOCK)", transaction: tx);

            var p = new DynamicParameters(InfoParams(u, u.Password!));
            p.Add("Id", id);
            p.Add("Code", id);
            await c.ExecuteAsync("dbo.AddUser", p, tx, commandType: CommandType.StoredProcedure);
            var guid = await c.ExecuteScalarAsync<Guid>("SELECT Guid FROM dbo.User_Info WHERE Code = @id", new { id }, tx);

            await SaveAccessAsync(c, tx, guid, u, isNew: true);
            await audit.WriteAsync(c, tx, Form, guid, UserActionLog.New, id.ToString());
            return new UserSaved("تم حفظ بيانات المستخدم بنجاح", id.ToString(), guid);
        }, IsolationLevel.Serializable, ct);
        return saved;
    }

    public async Task<UserSaved> UpdateAsync(Guid guid, UpdateUserRequest req, CancellationToken ct)
    {
        var u = req.User;
        if (guid == me.UserGuid && !u.Staut_) throw new ApiException("لا يمكنك إيقاف حسابك الحالي");

        var saved = await db.InTransactionAsync(async (c, tx) =>
        {
            var current = await c.QueryFirstOrDefaultAsync<(int? Code, string? Password)>(
                "SELECT Code, Password FROM dbo.User_Info WHERE Guid = @guid", new { guid }, tx);
            if (current.Code is null) throw ApiException.NotFound("المستخدم غير موجود");
            await ValidateAsync(c, tx, u, guid, isNew: false);

            var password = string.IsNullOrEmpty(u.Password) ? current.Password ?? "" : u.Password;
            var p = new DynamicParameters(InfoParams(u, password));
            p.Add("Guid", guid);
            await c.ExecuteAsync("dbo.UpdateUser", p, tx, commandType: CommandType.StoredProcedure);

            await SaveAccessAsync(c, tx, guid, u, isNew: false);
            await audit.WriteAsync(c, tx, Form, guid, UserActionLog.Update, current.Code.ToString(), req.Reason.Trim());
            return new UserSaved("تم تعديل بيانات المستخدم بنجاح", current.Code.ToString()!, guid);
        }, ct: ct);

        perms.Invalidate(guid); // new permissions apply right away
        return saved;
    }
}
