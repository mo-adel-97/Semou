using Semou.Api.Features.Registration;
using Semou.Api.Features.Sales;

namespace Semou.Api.Features.Requests;

/* What the author saw when sending the request, shown to the reviewer exactly as it was. */
public sealed record SnapItem(string Name, int Type, decimal Cost, decimal Tax, decimal Total);
public sealed record SnapPayLine(string Name, decimal Cost, decimal Tax, decimal Total);

public sealed class RegSnapshot
{
    public string? Book { get; set; }
    public string? BranchName { get; set; }
    public string? Batch { get; set; }
    public string? Level { get; set; }
    public string? Course { get; set; }
    public string? Seller { get; set; }
    public List<SnapItem> Items { get; set; } = [];
    public decimal Total { get; set; }
    public decimal Tax { get; set; }
    public decimal SubTotal { get; set; }
    public bool HasPayment { get; set; }
    public List<SnapPayLine> PayLines { get; set; } = [];
    public decimal PaySub { get; set; }
    public string? Cash { get; set; }
    public string? Cost { get; set; }
    public string? Cash2 { get; set; }
    public string? Cost2 { get; set; }
}

public sealed class RegPayload
{
    public RegistrationInput Input { get; set; } = new();
    public RegSnapshot Snapshot { get; set; } = new();
}

public sealed class PaySnapshot
{
    public string? Book { get; set; }
    public string? Program { get; set; }
    public List<SnapPayLine> Lines { get; set; } = [];
    public decimal Sub { get; set; }
    public decimal Tax { get; set; }
    public string? Cash { get; set; }
    public string? Cost { get; set; }
    public string? Cash2 { get; set; }
    public string? Cost2 { get; set; }
}

public sealed class PayPayload
{
    public PayInvoiceInput Input { get; set; } = new();
    public PaySnapshot Snapshot { get; set; } = new();
    /// <summary>Kind 0: the down payment of a registration form (created when its طلب التحاق is approved).</summary>
    public DownPayment? Down { get; set; }
    /// <summary>Kind 0: the طلب التحاق whose form + down-payment invoice + receipt are created when this order is confirmed.</summary>
    public Guid? RegOrder { get; set; }
}

public sealed class ReviewInput { public string? Notes { get; set; } }

public sealed class PayStatusInput { public int Status { get; set; } public string? Reason { get; set; } }

/// <summary>Corrections the reviewer may make to the transfer data before confirming (with a reason).</summary>
public sealed class PayEditInput
{
    public Guid? CashAccountGuid { get; set; }
    public Guid? CostCenterGuid { get; set; }
    public string? Ref { get; set; }
    public DateTime? PayDate { get; set; }
    public Guid? CashAccountGuid2 { get; set; }
    public Guid? CostCenterGuid2 { get; set; }
    public string? Ref2 { get; set; }
    public DateTime? PayDate2 { get; set; }
    public string? Notes { get; set; }
    public string? Reason { get; set; }
}

public sealed class OrderListRow
{
    public Guid Guid { get; set; }
    public int Code { get; set; }
    public DateTime OrderDate { get; set; }
    public int Status { get; set; }
    public Guid AccountGuid { get; set; }
    public Guid StudentGuid { get; set; }
    public string StudentName { get; set; } = "";
    public string? NationalId { get; set; }
    public string? StudentTel { get; set; }
    public string? Branch { get; set; }
    public string? Programs { get; set; }
    public int RegType { get; set; }
    public int Kind { get; set; }
    public string? Batch { get; set; }
    public string? Seller { get; set; }
    public decimal SubTotal { get; set; }
    public decimal PayAmount { get; set; }
    public string? Cash { get; set; }
    public string? PayRef { get; set; }
    public DateTime? PayDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? ReviewNotes { get; set; }
    public string? DocCode { get; set; }
    public Guid? DocGuid { get; set; }
    public Guid? BillGuid { get; set; }
    public string? TaxNo { get; set; }
    public int Files { get; set; }
    public int Duplicates { get; set; }
}
