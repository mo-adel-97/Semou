using System.ComponentModel.DataAnnotations;

namespace Semou.Api.Features.Branches;

/// <summary>Row in the branches grid (NewBranchList).</summary>
public sealed record BranchListItem(
    string Code, Guid Guid, string? BrName, string? BrEName, bool? IsUse, bool? IsRegOn,
    string? BranchQCode, int? ClassTime, string? BrTel, string? BrVatId);

/// <summary>The fields of the "بطاقة فرع" card. Lengths match the columns of dbo.AddBranch / UpdateBranch.</summary>
public sealed class BranchInput
{
    [Required(ErrorMessage = "يرجى إدخال اسم الفرع"), StringLength(150, ErrorMessage = "اسم الفرع طويل جداً")]
    public string BrName { get; set; } = "";
    [StringLength(150)] public string? BrEName { get; set; }
    [StringLength(150)] public string? BrAddress { get; set; }
    [StringLength(50)] public string? BrWebSite { get; set; }
    // No e-mail format check: existing rows hold values like "0" and the desktop accepted any text.
    [StringLength(50)] public string? BrEmail { get; set; }
    [StringLength(50)] public string? BrTel { get; set; }
    /// <summary>Shown as "السجل التجاري" (commercial registration number) in the desktop app.</summary>
    [StringLength(50)] public string? BrFax { get; set; }
    [Required(ErrorMessage = "يرجى إدخال الرقم الضريبي"), StringLength(50)]
    public string BrVatId { get; set; } = "";
    public bool BrVatSMain { get; set; }
    public bool IsUse { get; set; } = true;
    [Required(ErrorMessage = "يرجى اختيار مركز التكلفة الافتراضي")]
    public Guid? CostCenterGuid { get; set; }
    [StringLength(50)] public string? BranchQCode { get; set; }
    [Required(ErrorMessage = "يرجى إدخال الاسم الضريبي"), StringLength(200)]
    public string VatName { get; set; } = "";
    public bool IsRegOn { get; set; } = true;
}

public sealed class BranchDetails
{
    public string Code { get; set; } = "";
    public Guid Guid { get; set; }
    public string? BrName { get; set; }
    public string? BrEName { get; set; }
    public string? BrAddress { get; set; }
    public string? BrWebSite { get; set; }
    public string? BrEmail { get; set; }
    public string? BrTel { get; set; }
    public string? BrFax { get; set; }
    public string? BrVatId { get; set; }
    public bool? BrVatSMain { get; set; }
    public bool? IsUse { get; set; }
    public Guid? CostCenterGuid { get; set; }
    public string? CenterName { get; set; }
    public string? BranchQCode { get; set; }
    public string? VatName { get; set; }
    public bool? IsRegOn { get; set; }
}

public sealed class UpdateBranchRequest
{
    [Required] public BranchInput Branch { get; set; } = new();

    /// <summary>Required, like the desktop UpdateResaon dialog.</summary>
    [Required(ErrorMessage = "يرجى إدخال سبب الإجراء أولاً"), StringLength(250)]
    public string Reason { get; set; } = "";
}

public sealed record BranchSaved(string Message, string Code, Guid Guid);
