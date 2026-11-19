namespace Semou.Api.Features.Registration;

/// <summary>A sellable item (diploma, course or fee) priced for this student, exactly like the desktop grids.</summary>
public sealed record PriceRow(string Code, string Name, Guid DiplomGuid, int DiplomType, decimal Cost, decimal Tax, decimal Total, decimal PayTaxRate);

/// <summary>The registration book (RegSetting) chosen by branch + registration type, with its accounting rules.</summary>
public sealed class RegSettingInfo
{
    public Guid Guid { get; set; }
    public int Code { get; set; }
    public string DocName { get; set; } = "";
    public Guid BranchGuid { get; set; }
    public string? BranchName { get; set; }
    public Guid PriceListGuid { get; set; }
    public bool ChkMaden { get; set; }
    public Guid? MadenGuid { get; set; }
    public bool ChkDaen { get; set; }
    public Guid? DaenGuid { get; set; }
    public Guid? CostCenterGuid { get; set; }
    public bool ChkOtherFees { get; set; }
    public bool ChkStartPay { get; set; }
    public bool ChkAmountPay { get; set; }
    public decimal AmountPay { get; set; }
    public Guid? DocPayGuid { get; set; }
    public string? DocPayName { get; set; }
    public Guid? PayBranchGuid { get; set; }
    public int? PayDocCode { get; set; }
    public Guid? VatAccountGuid { get; set; }
    public bool ChkVat { get; set; }
}

public sealed class RegItem
{
    public Guid DiplomGuid { get; set; }
    /// <summary>Only for users allowed zero amounts: the edited total (price overridden).</summary>
    public decimal? Total { get; set; }
}

public sealed class PayLine
{
    public Guid DiplomGuid { get; set; }
    /// <summary>Amount incl. VAT paid now for this item.</summary>
    public decimal Total { get; set; }
}

public sealed class PaymentInput
{
    /// <summary>CashBox.AccountGuid of the box/bank (UseInSales).</summary>
    public Guid? CashAccountGuid { get; set; }
    public Guid? CostCenterGuid { get; set; }
    public string? Ref { get; set; }
    public DateTime? PayDate { get; set; }
    public string? Notes { get; set; }
    public List<PayLine> Lines { get; set; } = [];
    /// <summary>Second bank transfer (ChlOtherBank).</summary>
    public bool Split { get; set; }
    public decimal Amount1 { get; set; }
    public decimal Amount2 { get; set; }
    public Guid? CashAccountGuid2 { get; set; }
    public Guid? CostCenterGuid2 { get; set; }
    public string? Ref2 { get; set; }
    public DateTime? PayDate2 { get; set; }
}

public sealed class RegistrationInput
{
    public Guid StudentGuid { get; set; }
    /// <summary>0 دبلوم / 1 دورة.</summary>
    public int RegType { get; set; }
    /// <summary>فرع الدراسة: picks the registration book (AutoGetDocType).</summary>
    public Guid StudyBranchGuid { get; set; }
    /// <summary>هل وقع المتدرب اتفاقية التدريب؟</summary>
    public bool ChkContract { get; set; }
    public DateTime RegDate { get; set; }
    public Guid? BatchGuid { get; set; }
    public Guid? LevelGuid { get; set; }
    public Guid? CourseGuid { get; set; }
    public Guid? SellerGuid { get; set; }
    public string? Notes { get; set; }
    public List<RegItem> Diploms { get; set; } = [];
    public List<RegItem> Fees { get; set; } = [];
    public PaymentInput? Payment { get; set; }
}

public sealed record RegistrationSaved(string Message, string RegCode, Guid RegGuid, string? BillCode, Guid? BillGuid, string? TaxNo, string? ReceiptCode, string? ReceiptCode2);

/* ---- The validated, priced registration (PlanAsync) that PostAsync writes. ---- */
public sealed record RegPlanStudent(Guid AccountGuid, string StudentName, string? NationalId);
public sealed record RegPlanPayLine(PriceRow Item, decimal Cost, decimal Tax, decimal Total);

public sealed class RegPlan
{
    public required RegPlanStudent Student { get; init; }
    public required RegSettingInfo Setting { get; init; }
    public string? BatchName { get; init; }
    public string? CourseDiplomName { get; init; }
    public required List<PriceRow> Diploms { get; init; }
    public required List<PriceRow> Fees { get; init; }
    public decimal FTotal { get; init; }
    public decimal FTax { get; init; }
    public decimal FSub { get; init; }
    public required PriceRow MainDiplom { get; init; }
    public PaymentInput? Pay { get; init; }
    public required List<RegPlanPayLine> PayLines { get; init; }
    public decimal PaySub { get; init; }
    public string? CashName { get; init; }
    public string? CashName2 { get; init; }
}

/// <summary>A registration's down payment waiting to be posted (web: a طلب سداد of kind 0).</summary>
public sealed class DownPayment
{
    public Guid RegDocGuid { get; set; }
    public string RegCode { get; set; } = "";
    public Guid StudentAccount { get; set; }
    public string StudentName { get; set; } = "";
    public string DocName { get; set; } = "";
    public string NoteDip { get; set; } = "";
    public Guid PayDocGuid { get; set; }
    public bool ChkVat { get; set; }
    public Guid? VatAccount { get; set; }
    public decimal RegTax { get; set; }
    public PaymentInput Pay { get; set; } = new();
    public List<Semou.Api.Features.Sales.InvoiceLine> Lines { get; set; } = [];
}
