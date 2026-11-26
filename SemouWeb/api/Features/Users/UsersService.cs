using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
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
               (SELECT COUNT(*) FROM dbo.User_Premision P WHERE P.UserGuid = U.Guid AND P.View_ = 1) AS Screens,
               J.Name AS JobTitle, ET.Name AS EmploymentType, PR.WorkMobile, PR.PersonalMobile
        FROM dbo.User_Info U LEFT JOIN dbo.SalesMan S ON S.Guid = U.SellerGuid LEFT JOIN dbo.UserProfile PR ON PR.UserGuid = U.Guid
             LEFT JOIN dbo.HrJobTitle J ON J.Guid = PR.JobTitleGuid LEFT JOIN dbo.HrEmploymentType ET ON ET.Guid = PR.EmploymentTypeGuid
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
        async Task<IEnumerable<HrItem>> list(string table, string iban = "CAST(NULL AS nvarchar(2))") => await c.QueryAsync<HrItem>(new CommandDefinition(
            $"SELECT Guid, Name, IsUse, {iban} AS IbanCode FROM dbo.[{table}] ORDER BY ID", cancellationToken: ct));
        var hr = new HrLists(await list("HrJobTitle"), await list("HrEmploymentType"), await list("HrSpecialization"), await list("HrBank", "IbanCode"));
        return new UserCatalog(forms, menus, sellers, users, hr);
    }

    private sealed record InfoRow(int? Code, Guid Guid, string? UserName, string? FullName, bool? Staut_, bool? ChkBranch, Guid? BranchGuid,
                                  bool? ChkAmount, bool? ChkOtherFess, Guid? SellerGuid);
    private sealed record PermDb(Guid FormGuid, bool? Add_, bool? Edit, bool? Del, bool? Print_, bool? Find_, bool? View_);
    private sealed record MenuDb(Guid MenuGuid, bool? View_);
    /// <summary>Typed (not dynamic) so the JSON keys come out camelCase like the rest of the card.</summary>
    private sealed record ProfileDb(string? NationalId, Guid? JobTitleGuid, Guid? EmploymentTypeGuid, Guid? SpecializationGuid, string? PersonalMobile, string? WorkMobile,
                                    string? Email, string? Iban, Guid? BankGuid, string? HireDate, string? BirthDate, string? Notes);

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
        var profile = await c.QueryFirstOrDefaultAsync<ProfileDb>(new CommandDefinition("""
            SELECT NationalId, JobTitleGuid, EmploymentTypeGuid, SpecializationGuid, PersonalMobile, WorkMobile, Email, Iban, BankGuid,
                   CONVERT(char(10), HireDate, 23) AS HireDate, CONVERT(char(10), BirthDate, 23) AS BirthDate, Notes
            FROM dbo.UserProfile WHERE UserGuid = @guid
            """, new { guid }, cancellationToken: ct));
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
                Profile = profile,
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

        if (u.Profile is { } p)
        {
            NormalizeProfile(p);
            if (p.NationalId is { } nid)
            {
                var other = await c.ExecuteScalarAsync<string?>("""
                    SELECT TOP 1 ISNULL(U.FullName, U.UserName) FROM dbo.UserProfile P JOIN dbo.User_Info U ON U.Guid = P.UserGuid
                    WHERE P.NationalId = @nid AND (@self IS NULL OR P.UserGuid <> @self)
                    """, new { nid, self }, tx);
                if (other != null) throw new ApiException($"رقم الهوية مسجل مسبقاً للمستخدم: {other}");
            }
            async Task mustExist(Guid? g, string table, string message)
            {
                if (g is null) return;
                if (await c.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM dbo.[{table}] WHERE Guid = @g", new { g }, tx) == 0) throw new ApiException(message);
            }
            await mustExist(p.JobTitleGuid, "HrJobTitle", "المسمى الوظيفي غير موجود");
            await mustExist(p.EmploymentTypeGuid, "HrEmploymentType", "الحالة الوظيفية غير موجودة");
            await mustExist(p.SpecializationGuid, "HrSpecialization", "التخصص غير موجود");
            await mustExist(p.BankGuid, "HrBank", "البنك غير موجود");
        }
    }

    private static readonly Regex Digits = new(@"\D", RegexOptions.Compiled);
    private static readonly Regex EmailRx = new(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$", RegexOptions.Compiled);

    /// <summary>Trims every field, turns blanks into NULL and checks the format of what was typed.</summary>
    private static void NormalizeProfile(UserProfileInput p)
    {
        static string? clean(string? v, int max, string tooLong)
        {
            v = string.Join(" ", (v ?? "").Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
            if (v.Length == 0) return null;
            if (v.Length > max) throw new ApiException(tooLong);
            return v;
        }
        static string? mobile(string? v, string label)
        {
            var d = Digits.Replace(v ?? "", "");
            if (d.Length == 0) return null;
            if (d.StartsWith("966")) d = d[3..];
            if (d.Length == 9 && d[0] == '5') d = "0" + d;
            if (d.Length != 10 || !d.StartsWith("05")) throw new ApiException($"يرجى إدخال {label} صحيح (05xxxxxxxx)");
            return d;
        }

        static Guid? orNull(Guid? g) => g == Guid.Empty ? null : g;
        p.JobTitleGuid = orNull(p.JobTitleGuid); p.EmploymentTypeGuid = orNull(p.EmploymentTypeGuid);
        p.SpecializationGuid = orNull(p.SpecializationGuid); p.BankGuid = orNull(p.BankGuid);
        p.Notes = string.IsNullOrWhiteSpace(p.Notes) ? null : p.Notes.Trim();
        if (p.Notes?.Length > 500) throw new ApiException("الملاحظات طويلة جداً (الحد الأقصى 500 حرف)");

        var nid = Digits.Replace(p.NationalId ?? "", "");
        if (nid.Length == 0) p.NationalId = null;
        else if (nid.Length != 10 || nid[0] is not ('1' or '2')) throw new ApiException("يرجى إدخال رقم هوية صحيح (10 أرقام تبدأ بـ 1 أو 2)");
        else p.NationalId = nid;

        p.PersonalMobile = mobile(p.PersonalMobile, "رقم الجوال الشخصي");
        p.WorkMobile = mobile(p.WorkMobile, "رقم جوال العمل");

        p.Email = clean(p.Email, 150, "البريد الإلكتروني طويل جداً");
        if (p.Email != null && !EmailRx.IsMatch(p.Email)) throw new ApiException("يرجى إدخال بريد إلكتروني صحيح");

        var iban = new string((p.Iban ?? "").Where(ch => !char.IsWhiteSpace(ch) && ch != '-').ToArray()).ToUpperInvariant();
        if (iban.Length == 0) p.Iban = null;
        else
        {
            if (!IbanOk(iban)) throw new ApiException("رقم الآيبان غير صحيح، يرجى مراجعته");
            p.Iban = iban;
        }

        var today = Clock.Now.Date;
        if (p.BirthDate is { } bd && (bd.Date > today.AddYears(-15) || bd.Date < today.AddYears(-80))) throw new ApiException("يرجى إدخال تاريخ ميلاد صحيح");
        if (p.HireDate is { } hd && (hd.Date > today.AddYears(1) || hd.Date < today.AddYears(-50))) throw new ApiException("يرجى إدخال تاريخ مباشرة صحيح");
        p.BirthDate = p.BirthDate?.Date; p.HireDate = p.HireDate?.Date;
    }

    /// <summary>ISO 13616: letters/digits, Saudi IBANs are SA + 22 digits, and the mod-97 check must give 1.</summary>
    public static bool IbanOk(string iban)
    {
        if (iban.Length < 15 || iban.Length > 34 || !iban.All(char.IsAsciiLetterOrDigit) || !char.IsAsciiLetter(iban[0]) || !char.IsAsciiLetter(iban[1])) return false;
        if (iban.StartsWith("SA") && (iban.Length != 24 || !iban[2..].All(char.IsAsciiDigit))) return false;
        var rem = 0;
        foreach (var ch in iban[4..] + iban[..4])
        {
            var v = char.IsAsciiDigit(ch) ? ch - '0' : ch - 'A' + 10;
            rem = v >= 10 ? (rem * 100 + v) % 97 : (rem * 10 + v) % 97;
        }
        return rem == 1;
    }

    private static async Task SaveProfileAsync(DbConnection c, DbTransaction tx, Guid userGuid, UserProfileInput? p)
    {
        if (p is null) return; // older clients: leave the employee data as it is
        await c.ExecuteAsync("""
            MERGE dbo.UserProfile AS T USING (SELECT @userGuid AS UserGuid) AS S ON T.UserGuid = S.UserGuid
            WHEN MATCHED THEN UPDATE SET NationalId = @NationalId, JobTitleGuid = @JobTitleGuid, EmploymentTypeGuid = @EmploymentTypeGuid, SpecializationGuid = @SpecializationGuid,
                 PersonalMobile = @PersonalMobile, WorkMobile = @WorkMobile, Email = @Email, Iban = @Iban, BankGuid = @BankGuid,
                 HireDate = @HireDate, BirthDate = @BirthDate, Notes = @Notes, UpdatedAt = GETDATE()
            WHEN NOT MATCHED THEN INSERT (UserGuid, NationalId, JobTitleGuid, EmploymentTypeGuid, SpecializationGuid, PersonalMobile, WorkMobile, Email, Iban, BankGuid, HireDate, BirthDate, Notes)
                 VALUES (@userGuid, @NationalId, @JobTitleGuid, @EmploymentTypeGuid, @SpecializationGuid, @PersonalMobile, @WorkMobile, @Email, @Iban, @BankGuid, @HireDate, @BirthDate, @Notes);
            """, new { userGuid, p.NationalId, p.JobTitleGuid, p.EmploymentTypeGuid, p.SpecializationGuid, p.PersonalMobile, p.WorkMobile, p.Email, p.Iban, p.BankGuid, p.HireDate, p.BirthDate, p.Notes }, tx);
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
            await SaveProfileAsync(c, tx, guid, u.Profile);
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
            await SaveProfileAsync(c, tx, guid, u.Profile);
            await audit.WriteAsync(c, tx, Form, guid, UserActionLog.Update, current.Code.ToString(), req.Reason.Trim());
            return new UserSaved("تم تعديل بيانات المستخدم بنجاح", current.Code.ToString()!, guid);
        }, ct: ct);

        perms.Invalidate(guid); // new permissions apply right away
        return saved;
    }
}
