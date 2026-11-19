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

        return new
        {
            student = new { st.Guid, accountGuid, st.Code, st.StudentName, st.NationalId, st.StudentTel, st.AcadmyId },
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

    private static decimal Dec(object? v) => v is null or DBNull ? 0 : Math.Round(Convert.ToDecimal(v), 2);
}
