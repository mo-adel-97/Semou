using System.ComponentModel.DataAnnotations;

namespace Semou.Api.Features.Users;

public sealed record UserListItem(int? Code, Guid Guid, string? UserName, string? FullName, bool? Staut_, string? SellerName, int Screens,
                                  string? JobTitle, string? EmploymentType, string? WorkMobile, string? PersonalMobile);

public sealed record FormItem(Guid Guid, string? Name, string? UnqName, int? Code);
public sealed record MenuItem(Guid Guid, string? MenuName, string? UnqName);
public sealed record SellerItem(Guid Guid, string? Name, bool? IsUse);
/// <summary>Suggestions for the free-text employee fields, learned from what was already saved (no fixed lists in code).</summary>
public sealed record ProfileSuggest(IEnumerable<string> Jobs, IEnumerable<string> Types, IEnumerable<string> Specs, IEnumerable<string> Banks, IEnumerable<BankCode> BankCodes);
/// <summary>IBAN bank code (characters 5-6 of a Saudi IBAN) → the bank name typed for it before.</summary>
public sealed record BankCode(string Code, string Name);
public sealed record UserCatalog(IEnumerable<FormItem> Forms, IEnumerable<MenuItem> Menus, IEnumerable<SellerItem> Sellers, IEnumerable<UserListItem> Users, ProfileSuggest Suggest);

/// <summary>بيانات الموظف (dbo.UserProfile). Every field is optional; the ones given are validated.</summary>
public sealed class UserProfileInput
{
    public string? NationalId { get; set; }
    public string? JobTitle { get; set; }
    public string? EmploymentType { get; set; }
    public string? Specialization { get; set; }
    public string? PersonalMobile { get; set; }
    public string? WorkMobile { get; set; }
    public string? Email { get; set; }
    public string? Iban { get; set; }
    public string? BankName { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Notes { get; set; }
}

public sealed class PermRow
{
    public Guid FormGuid { get; set; }
    public bool Add { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Print { get; set; }
    public bool Find { get; set; }
    public bool View { get; set; }
    public bool Any => Add || Edit || Delete || Print || Find || View;
}

public sealed class MenuRow
{
    public Guid MenuGuid { get; set; }
    public bool View { get; set; }
}

/// <summary>"بطاقة مستخدم" (EncodingUser). The password is never sent back to the browser.</summary>
public sealed class UserInput
{
    [Required(ErrorMessage = "يرجى إدخال اسم الموظف"), StringLength(150, ErrorMessage = "اسم الموظف طويل جداً")]
    public string FullName { get; set; } = "";
    [Required(ErrorMessage = "يرجى إدخال اسم المستخدم"), StringLength(50, ErrorMessage = "اسم المستخدم طويل جداً")]
    public string UserName { get; set; } = "";
    /// <summary>Required for a new user. On edit: empty = keep the current password.</summary>
    [StringLength(50, ErrorMessage = "كلمة المرور طويلة جداً (الحد الأقصى 50 حرف)")]
    public string? Password { get; set; }
    public bool Staut_ { get; set; } = true;
    /// <summary>فروع مخصصة للتقارير.</summary>
    public bool ChkBranch { get; set; }
    public Guid? BranchGuid { get; set; }
    /// <summary>إدخال مبالغ صفرية.</summary>
    public bool ChkAmount { get; set; }
    /// <summary>إضافة رسوم للطالب في عدم وجود ملف تدريبي.</summary>
    public bool ChkOtherFess { get; set; }
    public Guid? SellerGuid { get; set; }
    public UserProfileInput? Profile { get; set; }

    public List<MenuRow> Menus { get; set; } = [];
    public List<PermRow> Perms { get; set; } = [];
    public List<Guid> Branches { get; set; } = [];
}

public sealed class UpdateUserRequest
{
    [Required] public UserInput User { get; set; } = new();
    [Required(ErrorMessage = "يرجى إدخال سبب الإجراء أولاً"), StringLength(250)]
    public string Reason { get; set; } = "";
}

public sealed record UserSaved(string Message, string Code, Guid Guid);
