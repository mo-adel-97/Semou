using System.Data;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Reception;

/*
 * كشف حساب الطالب (StusdentStatment):
 *   - movements: StudentStatmentAllTime (every journal line on the student's account), running balance;
 *     like the desktop, "salesinvoice" and "repay" lines are shown but NOT counted (the registration / pay
 *     journals already carry the amount).
 *   - invoices: BillForStudent (+ guid, tax number, cancelled) → reprint the tax invoice.
 *   - returns: ShowReBillStudent.
 *   - registration forms of the student → reprint.
 */
public sealed class StatementRow
{
    public DateTime Date { get; set; }
    public string? TypeName { get; set; }
    public string? Kind { get; set; }
    public string? DocNo { get; set; }
    public string? JournalCode { get; set; }
    public decimal Maden { get; set; }
    public decimal Daen { get; set; }
    public bool Counted { get; set; }
    public decimal Balance { get; set; }
    public string? Notes { get; set; }
    public string? CostCenter { get; set; }
    /// <summary>The source document (RegDoc / Bill / CashDay…), or the journal itself for manual entries.</summary>
    public Guid DocGuid { get; set; }
    public Guid JournalGuid { get; set; }
}

public sealed class StatementBill
{
    public Guid Guid { get; set; }
    public string Code { get; set; } = "";
    public string? TaxNo { get; set; }
    public DateTime BillDate { get; set; }
    public string? Notes { get; set; }
    public decimal Total { get; set; }
    public decimal Tax { get; set; }
    public decimal SubTotal { get; set; }
    public string? UserName { get; set; }
    public bool Cancelled { get; set; }
}

public sealed class StatementReg
{
    public Guid Guid { get; set; }
    public string Code { get; set; } = "";
    public DateTime RegDate { get; set; }
    public int RegType { get; set; }
    public decimal Total { get; set; }
    public bool Accepted { get; set; }
    public bool Returned { get; set; }
    public string? Program { get; set; }
}

/// <summary>One line of a package's schedule: the down payment (No = 0) or a monthly instalment.</summary>
public sealed record PlanLine(int No, DateTime Due, decimal Amount, decimal Paid, string Status);

public sealed class StatementPlan
{
    public Guid RegDocGuid { get; set; }
    public string RegCode { get; set; } = "";
    public DateTime RegDate { get; set; }
    public bool Returned { get; set; }
    public string? Program { get; set; }
    public string PlanName { get; set; } = "";
    public int Months { get; set; }
    public decimal Price { get; set; }
    public decimal DownPayment { get; set; }
    public decimal Installment { get; set; }
    public DateTime? FirstDue { get; set; }
    public decimal Paid { get; set; }
    public decimal Remaining { get; set; }
    public decimal Overdue { get; set; }
    public List<PlanLine> Lines { get; set; } = [];
}

public sealed class StatementService(Db db)
{
    public const string Form = "studentstatment";
    private static readonly HashSet<string> NotCounted = new(StringComparer.OrdinalIgnoreCase) { "salesinvoice", "repay" };

    public async Task<object> GetAsync(Guid accountGuid, string? q, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var st = await c.QueryFirstOrDefaultAsync<(Guid Guid, string Code, string StudentName, string? NationalId, string? StudentTel, string? AcadmyId)>(
            new CommandDefinition("SELECT Guid, Code, StudentName, NationalId, StudentTel, AcadmyId FROM dbo.Student WHERE AccountGuid = @a", new { a = accountGuid }, cancellationToken: ct));
        if (st.StudentName is null) throw ApiException.NotFound("الطالب غير موجود");

        q = string.IsNullOrWhiteSpace(q) ? null : q.Trim();
        var raw = await c.QueryAsync(new CommandDefinition("dbo.StudentStatmentAllTime", new { AccountGuid = accountGuid, Notes = q },
            commandType: CommandType.StoredProcedure, cancellationToken: ct));
        decimal bal = 0, tm = 0, td = 0;
        var rows = new List<StatementRow>();
        foreach (IDictionary<string, object?> r in raw)
        {
            var kind = r["UnqName"]?.ToString() ?? "";
            var counted = !NotCounted.Contains(kind);
            decimal m = Dec(r["Maden"]), d = Dec(r["Daen"]);
            if (counted) bal += m - d;
            // DAILYTOTAL_: invoice debits and return credits are excluded from the totals.
            if (!kind.Equals("salesinvoice", StringComparison.OrdinalIgnoreCase)) tm += m;
            if (!kind.Equals("repay", StringComparison.OrdinalIgnoreCase)) td += d;
            var docGuid = r["DocGuid"] is Guid g && g != Guid.Empty ? g : (Guid)r["Guid"]!;
            rows.Add(new StatementRow
            {
                Date = Convert.ToDateTime(r["DayDate"]), TypeName = r["Name"]?.ToString(), Kind = kind, DocNo = r["ActionCode"]?.ToString(),
                JournalCode = r["code"]?.ToString(), Maden = m, Daen = d, Counted = counted, Balance = Math.Round(bal, 2),
                Notes = r["Notes"]?.ToString(), CostCenter = r["CenterName"]?.ToString(), DocGuid = docGuid, JournalGuid = (Guid)r["Guid"]!,
            });
        }

        var bills = await c.QueryAsync<StatementBill>(new CommandDefinition("""
            SELECT B.Guid, CAST(B.Code AS nvarchar(50)) AS Code, CAST(B.TaxNo AS nvarchar(50)) AS TaxNo, B.BillDate, B.Notes, CAST(ISNULL(B.BillTotal,0) AS decimal(18,2)) AS Total, CAST(ISNULL(B.BillTax,0) AS decimal(18,2)) AS Tax,
                   CAST(ISNULL(B.BillSubTotal,0) AS decimal(18,2)) AS SubTotal, U.FullName AS UserName, CAST(CASE WHEN ISNULL(B.IsUse,1) = 0 THEN 1 ELSE 0 END AS bit) AS Cancelled
            FROM dbo.Bill B LEFT JOIN dbo.User_Info U ON U.Guid = B.UserGuid
            WHERE B.AccountGuid = @a ORDER BY B.BillDate DESC, TRY_CONVERT(int, B.Code) DESC
            """, new { a = accountGuid }, cancellationToken: ct));
        var returns = await c.QueryAsync(new CommandDefinition("dbo.ShowReBillStudent", new { AccountGuid = accountGuid }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        var regs = await c.QueryAsync<StatementReg>(new CommandDefinition("""
            SELECT R.Guid, CAST(R.Code AS nvarchar(50)) AS Code, R.RegDate, ISNULL(R.RegType,0) AS RegType, CAST(ISNULL(R.DocSubTotal,0) AS decimal(18,2)) AS Total,
                   CAST(ISNULL(R.Linked,0) AS bit) AS Accepted, CAST(ISNULL(R.IsReReg,0) AS bit) AS Returned,
                   ISNULL((SELECT TOP 1 D.DiplomName FROM dbo.RegDocDiplom RD JOIN dbo.Diplom D ON D.Guid = RD.DiplomGuid
                                          WHERE RD.RegDocGuid = R.Guid AND RD.DiplomType = '0' AND RD.Type_ = '0' ORDER BY RD.ID), CO.CourseName) AS Program
            FROM dbo.RegDoc R LEFT JOIN dbo.Course CO ON CO.Guid = R.CourseGuid
            WHERE R.AccountGuid = @a ORDER BY R.RegDate DESC
            """, new { a = accountGuid }, cancellationToken: ct));

        var plans = await PlansAsync(c, accountGuid, ct);

        return new
        {
            student = new { st.Guid, accountGuid, st.Code, st.StudentName, st.NationalId, st.StudentTel, st.AcadmyId },
            plans,
            rows,
            totals = new { maden = Math.Round(tm, 2), daen = Math.Round(td, 2), balance = Math.Round(tm - td, 2) },
            bills,
            returns = returns.Select(r => (IDictionary<string, object?>)r).Select(r => new
            {
                date = Convert.ToDateTime(r["ReDate"]), code = r["Code"]?.ToString(), notes = r["ReNotes"]?.ToString(), total = Dec(r["ReSubTotal"]), userName = r["FullName"]?.ToString(),
            }),
            regs,
        };
    }

    /// <summary>
    /// The packages the student registered with (WebRegPlan) and their schedule. What was paid for a package = the
    /// student's tax invoices (every payment issues one) for that diploma since the registration, cancelled ones excluded;
    /// it is spread over the down payment first, then the instalments in order. Payments of other courses or fees never
    /// count. A returned registration keeps its package for reference, without a schedule.
    /// </summary>
    private static async Task<List<StatementPlan>> PlansAsync(System.Data.Common.DbConnection c, Guid accountGuid, CancellationToken ct)
    {
        if (await c.ExecuteScalarAsync<int>(new CommandDefinition("SELECT CASE WHEN OBJECT_ID('dbo.WebRegPlan') IS NULL THEN 0 ELSE 1 END", cancellationToken: ct)) == 0) return [];
        var plans = (await c.QueryAsync<StatementPlan>(new CommandDefinition("""
            SELECT P.RegDocGuid, CAST(R.Code AS nvarchar(50)) AS RegCode, R.RegDate, CAST(ISNULL(R.IsReReg,0) AS bit) AS Returned,
                   (SELECT TOP 1 D.DiplomName FROM dbo.RegDocDiplom RD JOIN dbo.Diplom D ON D.Guid = RD.DiplomGuid
                     WHERE RD.RegDocGuid = R.Guid AND RD.DiplomType = '0' AND RD.Type_ = '0' ORDER BY RD.ID) AS Program,
                   P.PlanName, P.Months, P.Price, P.DownPayment, P.Installment, P.FirstDue,
                   CAST(ISNULL((SELECT SUM(BD.SubTotal) FROM dbo.Bill B JOIN dbo.RegDocDiplom BD ON BD.RegDocGuid = B.Guid AND BD.Type_ = '1'
                         WHERE B.AccountGuid = R.AccountGuid AND ISNULL(B.IsUse,1) = 1 AND CAST(B.BillDate AS date) >= CAST(R.RegDate AS date)
                           AND BD.DiplomGuid = (SELECT TOP 1 RD.DiplomGuid FROM dbo.RegDocDiplom RD WHERE RD.RegDocGuid = R.Guid AND RD.DiplomType = '0' AND RD.Type_ = '0' ORDER BY RD.ID)), 0) AS decimal(18,2)) AS Paid
            FROM dbo.WebRegPlan P JOIN dbo.RegDoc R ON R.Guid = P.RegDocGuid
            WHERE R.AccountGuid = @accountGuid AND ISNULL(R.IstUse,1) = 1
            ORDER BY R.RegDate, R.Code
            """, new { accountGuid }, cancellationToken: ct))).ToList();
        var today = Clock.Now.Date;
        foreach (var p in plans)
        {
            if (p.Returned) continue;
            var left = Math.Max(0, p.Paid);
            var due = new List<(int No, DateTime Due, decimal Amount)>();
            if (p.Months <= 0) due.Add((0, p.RegDate.Date, p.Price));
            else
            {
                if (p.DownPayment > 0) due.Add((0, p.RegDate.Date, p.DownPayment));
                var first = p.FirstDue ?? p.RegDate.Date.AddMonths(1);
                var rest = p.Price - p.DownPayment;
                for (var n = 1; n <= p.Months; n++)
                {
                    // the last instalment absorbs the rounding, so the schedule always adds up to the price
                    var amt = n < p.Months ? p.Installment : rest - p.Installment * (p.Months - 1);
                    due.Add((n, first.AddMonths(n - 1), Math.Max(0, amt)));
                }
            }
            foreach (var (no, date, amount) in due)
            {
                var paid = Math.Min(left, amount);
                left -= paid;
                var status = paid >= amount ? "paid" : date < today ? "overdue" : paid > 0 ? "partial" : date <= today.AddDays(7) ? "soon" : "due";
                p.Lines.Add(new PlanLine(no, date, amount, paid, status));
            }
            p.Paid = p.Lines.Sum(l => l.Paid);
            p.Remaining = p.Price - p.Paid;
            p.Overdue = p.Lines.Where(l => l.Status == "overdue").Sum(l => l.Amount - l.Paid);
        }
        return plans;
    }

    private static decimal Dec(object? v) => v is null or DBNull ? 0 : Math.Round(Convert.ToDecimal(v), 2);
}
