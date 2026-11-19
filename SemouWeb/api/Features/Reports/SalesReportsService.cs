using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Reports;

/*
 * قائمة التقارير (report menu):
 *   تقرير التسجيلات (RegReview, reviewdesk)            — RegDocReview: one row per registration (programs joined, not duplicated)
 *   تقرير المبيعات (SalesReport, salesreport)          — ShowSalesReport + the tax invoice number (Bill.TaxNo)
 *   تقرير مبيعات الضرائب (TaxHome, taxreport)          — ShowTaxReport: one row per invoice line; the desktop ignored the
 *                                                        active/cancelled choice, here it is applied
 *   تقرير مرتجع مبيعات الضرائب (ReTaxReport)           — ShowReTaxReport (+ the original invoice number)
 *   تقرير المسوقين (SalesManReport, salesmanreport)    — ShowSalesManReport (+ per-marketer summary on the page)
 *   متابعة السداد (CustomerHome, custbalance)          — AllStudentBalance: the desktop joined every training file and
 *                                                        repeated the student once per file; here one row per student
 *   أرصدة العملاء (CustomerBalanceHome, customerbalance) — ShowAllCustBalance (+ the closing balance)
 *   تحليل المصروفات (ExpenseAnalysisHome, expenseanalysis) — ShowExpenseAnalysis (+ totals per expense account)
 * Student balances follow the statement rule: tax-invoice debits and repay credits are shown on the statement but not counted.
 */
public sealed class RegReportRow
{
    public Guid Guid { get; set; }
    public Guid AccountGuid { get; set; }
    public DateTime Date { get; set; }
    public string Code { get; set; } = "";
    public string? AcadmyId { get; set; }
    public string StudentName { get; set; } = "";
    public string? NationalId { get; set; }
    public string? StudentTel { get; set; }
    public string? Programs { get; set; }
    public int RegType { get; set; }
    public string? Batch { get; set; }
    public string? Seller { get; set; }
    public string? Branch { get; set; }
    public string? Book { get; set; }
    public decimal Total { get; set; }
    public decimal Tax { get; set; }
    public decimal SubTotal { get; set; }
    public bool Accepted { get; set; }
    public bool Returned { get; set; }
    public string? Notes { get; set; }
}

public sealed class SalesRow
{
    public Guid Guid { get; set; }
    public Guid AccountGuid { get; set; }
    public DateTime Date { get; set; }
    public string Code { get; set; } = "";
    public string TaxNo { get; set; } = "";
    public string? Book { get; set; }
    public int InvoiceType { get; set; }
    public string StudentName { get; set; } = "";
    public string? NationalId { get; set; }
    public string? StudentTel { get; set; }
    public string? Notes { get; set; }
    public decimal Total { get; set; }
    public decimal Tax { get; set; }
    public decimal SubTotal { get; set; }
    public string? Branch { get; set; }
    public string? User { get; set; }
    public bool Active { get; set; }
}

public sealed class TaxRow
{
    public Guid Guid { get; set; }
    public DateTime Date { get; set; }
    public string Code { get; set; } = "";
    public string TaxNo { get; set; } = "";
    public string? Book { get; set; }
    public string StudentName { get; set; } = "";
    public string? NationalId { get; set; }
    public string? StudentTel { get; set; }
    public string? Program { get; set; }
    public string? Duration { get; set; }
    public decimal Cost { get; set; }
    public decimal Tax { get; set; }
    public decimal SubTotal { get; set; }
    public string? Branch { get; set; }
    public string? User { get; set; }
    public string? Notes { get; set; }
    public string? InvoiceTaxNo { get; set; }
    public bool Active { get; set; }
}

public sealed class SellerRow
{
    public Guid Guid { get; set; }
    public Guid AccountGuid { get; set; }
    public DateTime Date { get; set; }
    public string Code { get; set; } = "";
    public string? Seller { get; set; }
    public string? Branch { get; set; }
    public string StudentName { get; set; } = "";
    public string? NationalId { get; set; }
    public string? StudentTel { get; set; }
    public string? Programs { get; set; }
    public int RegType { get; set; }
    public decimal SubTotal { get; set; }
    public bool Returned { get; set; }
}

public sealed class BalanceRow
{
    public Guid AccountGuid { get; set; }
    public string Code { get; set; } = "";
    public string? AcadmyId { get; set; }
    public string StudentName { get; set; } = "";
    public string? StudentTel { get; set; }
    public string? NationalId { get; set; }
    public string? Branch { get; set; }
    public decimal Opening { get; set; }
    public decimal Maden { get; set; }
    public decimal Daen { get; set; }
    public decimal Balance { get; set; }
}

public sealed class ExpenseRow
{
    public DateTime Date { get; set; }
    public string Code { get; set; } = "";
    public string? AccountCode { get; set; }
    public string Account { get; set; } = "";
    public decimal Maden { get; set; }
    public decimal Daen { get; set; }
    public decimal Balance { get; set; }
    public string? Notes { get; set; }
    public string? Vendor { get; set; }
    public string? VendorTax { get; set; }
    public string? VendorTel { get; set; }
    public string? CostCenter { get; set; }
}

public sealed class SalesReportsService(Db db)
{
    private static (DateTime f, DateTime t) Range(DateTime? from, DateTime? to)
    {
        var f = (from ?? new DateTime(Clock.Now.Year, Clock.Now.Month, 1)).Date;
        var t = (to ?? Clock.Now).Date;
        if (f > t) throw new ApiException("تاريخ البداية بعد تاريخ النهاية");
        return (f, t.AddDays(1));
    }
    private static string? Text(string? q) => string.IsNullOrWhiteSpace(q) ? null : q.Trim();

    public async Task<object> LookupsAsync(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return new
        {
            sellers = await c.QueryAsync(new CommandDefinition("SELECT Guid AS guid, ManFullName AS name FROM dbo.SalesMan ORDER BY CASE WHEN ISNULL(IsUse,1) = 1 THEN 0 ELSE 1 END, ManFullName", cancellationToken: ct)),
            users = await c.QueryAsync(new CommandDefinition("SELECT Guid AS guid, FullName AS name FROM dbo.User_Info ORDER BY FullName", cancellationToken: ct)),
            batches = await c.QueryAsync(new CommandDefinition("SELECT Guid AS guid, BatchName AS name FROM dbo.Batch ORDER BY TRY_CONVERT(int, Code) DESC", cancellationToken: ct)),
            diploms = await c.QueryAsync(new CommandDefinition("SELECT Guid AS guid, DiplomName AS name FROM dbo.Diplom WHERE ISNULL(DiplomType,0) = 0 AND ISNULL(StudyType,0) = 0 ORDER BY TRY_CONVERT(int, Code)", cancellationToken: ct)),
            courses = await c.QueryAsync(new CommandDefinition("SELECT Guid AS guid, DiplomName AS name FROM dbo.Diplom WHERE ISNULL(DiplomType,0) = 0 AND StudyType = 1 ORDER BY TRY_CONVERT(int, Code)", cancellationToken: ct)),
            books = await c.QueryAsync(new CommandDefinition("SELECT S.Guid AS guid, S.DocName AS name FROM dbo.RegSetting S WHERE EXISTS (SELECT 1 FROM dbo.Bill B WHERE B.DocGuid = S.Guid) ORDER BY S.Code", cancellationToken: ct)),
            cashBoxes = await c.QueryAsync(new CommandDefinition("SELECT AccountGuid AS guid, CashName AS name, CashType AS type FROM dbo.CashBox ORDER BY CashType, TRY_CONVERT(int, Code)", cancellationToken: ct)),
        };
    }

    /* ---------------- تقرير التسجيلات ---------------- */
    public async Task<IEnumerable<RegReportRow>> RegistrationsAsync(DateTime? from, DateTime? to, Guid? branch, Guid? seller, Guid? batch, int? regType, Guid? program, string? q, CancellationToken ct)
    {
        var (f, t) = Range(from, to);
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync<RegReportRow>(new CommandDefinition("""
            SELECT R.Guid, R.AccountGuid, R.RegDate AS Date, CAST(R.Code AS nvarchar(50)) AS Code, S.AcadmyId, S.StudentName, S.NationalId, S.StudentTel,
                   P.Programs, ISNULL(R.RegType,0) AS RegType, BT.BatchName AS Batch, SM.ManFullName AS Seller, BR.BrName AS Branch, RS.DocName AS Book,
                   CAST(ISNULL(R.DocTotal,0) AS decimal(18,2)) AS Total, CAST(ISNULL(R.DocTax,0) AS decimal(18,2)) AS Tax, CAST(ISNULL(R.DocSubTotal,0) AS decimal(18,2)) AS SubTotal,
                   CAST(ISNULL(R.Linked,0) AS bit) AS Accepted, CAST(ISNULL(R.IsReReg,0) AS bit) AS Returned, R.Notes
            FROM dbo.RegDoc R
            JOIN dbo.Student S ON S.AccountGuid = R.AccountGuid
            LEFT JOIN dbo.SalesMan SM ON SM.Guid = R.SellerGuid
            LEFT JOIN dbo.Batch BT ON BT.Guid = R.BatchGuid
            LEFT JOIN dbo.Branch BR ON BR.Guid = R.BranchGuid
            LEFT JOIN dbo.RegSetting RS ON RS.Guid = R.DocGuid
            OUTER APPLY (SELECT STRING_AGG(D.DiplomName, '، ') AS Programs FROM dbo.RegDocDiplom X JOIN dbo.Diplom D ON D.Guid = X.DiplomGuid
                         WHERE X.RegDocGuid = R.Guid AND X.Type_ = 0 AND X.DiplomType = 0) P
            WHERE R.RegDate >= @f AND R.RegDate < @t AND ISNULL(R.IstUse,1) = 1
              AND (@branch IS NULL OR R.BranchGuid = @branch) AND (@seller IS NULL OR R.SellerGuid = @seller)
              AND (@batch IS NULL OR R.BatchGuid = @batch) AND (@regType IS NULL OR R.RegType = @regType)
              AND (@program IS NULL OR EXISTS (SELECT 1 FROM dbo.RegDocDiplom X WHERE X.RegDocGuid = R.Guid AND X.DiplomGuid = @program))
              AND (@q IS NULL OR S.StudentName LIKE '%' + @q + '%' OR S.NationalId LIKE @q + '%' OR S.StudentTel LIKE '%' + @q + '%' OR CAST(R.Code AS nvarchar(50)) = @q)
            ORDER BY R.RegDate DESC, TRY_CONVERT(int, R.Code) DESC
            """, new { f, t, branch, seller, batch, regType, program, q = Text(q) }, commandTimeout: 60, cancellationToken: ct));
    }

    /* ---------------- تقرير المبيعات ---------------- */
    public async Task<IEnumerable<SalesRow>> SalesAsync(DateTime? from, DateTime? to, Guid? branch, Guid? user, Guid? book, bool? active, string? kinds, string? q, CancellationToken ct)
    {
        var (f, t) = Range(from, to);
        var k = (kinds ?? "0,1,2").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => int.TryParse(x, out var n) ? n : -1).Where(n => n is >= 0 and <= 3).ToArray();
        if (k.Length == 0) throw new ApiException("يرجى اختيار نوع فاتورة واحد على الأقل");
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync<SalesRow>(new CommandDefinition("""
            SELECT B.Guid, B.AccountGuid, B.BillDate AS Date, CAST(B.Code AS nvarchar(50)) AS Code,
                   ISNULL(RIGHT('0' + CAST(B.TaxNo AS nvarchar(20)), CASE WHEN B.TaxNo < 10 THEN 2 ELSE 20 END), CAST(B.Code AS nvarchar(50))) AS TaxNo,
                   RS.DocName AS Book, ISNULL(RS.InvoiceType,0) AS InvoiceType, S.StudentName, S.NationalId, S.StudentTel, B.Notes,
                   CAST(ISNULL(B.BillTotal,0) AS decimal(18,2)) AS Total, CAST(ISNULL(B.BillTax,0) AS decimal(18,2)) AS Tax, CAST(ISNULL(B.BillSubTotal,0) AS decimal(18,2)) AS SubTotal,
                   BR.BrName AS Branch, U.FullName AS [User], CAST(ISNULL(B.IsUse,1) AS bit) AS Active
            FROM dbo.Bill B WITH (NOLOCK)
            JOIN dbo.Student S ON S.AccountGuid = B.AccountGuid
            LEFT JOIN dbo.RegSetting RS ON RS.Guid = B.DocGuid
            LEFT JOIN dbo.Branch BR ON BR.Guid = B.BranchGuid
            LEFT JOIN dbo.User_Info U ON U.Guid = B.UserGuid
            WHERE B.BillDate >= @f AND B.BillDate < @t
              AND (@branch IS NULL OR B.BranchGuid = @branch) AND (@user IS NULL OR B.UserGuid = @user) AND (@book IS NULL OR B.DocGuid = @book)
              AND (@active IS NULL OR ISNULL(B.IsUse,1) = @active) AND ISNULL(RS.InvoiceType,0) IN @k
              AND (@q IS NULL OR S.StudentName LIKE '%' + @q + '%' OR S.NationalId LIKE @q + '%' OR CAST(B.Code AS nvarchar(50)) = @q OR CAST(B.TaxNo AS nvarchar(20)) = @q)
            ORDER BY B.BillDate DESC, TRY_CAST(B.Code AS int) DESC
            """, new { f, t, branch, user, book, active, k, q = Text(q) }, commandTimeout: 60, cancellationToken: ct));
    }

    /* ---------------- تقرير مبيعات الضرائب (سطر لكل بند) ---------------- */
    public async Task<IEnumerable<TaxRow>> TaxAsync(DateTime? from, DateTime? to, Guid? branch, bool? active, string? q, CancellationToken ct)
    {
        var (f, t) = Range(from, to);
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync<TaxRow>(new CommandDefinition("""
            SELECT B.Guid, B.BillDate AS Date, CAST(B.Code AS nvarchar(50)) AS Code,
                   ISNULL(RIGHT('0' + CAST(B.TaxNo AS nvarchar(20)), CASE WHEN B.TaxNo < 10 THEN 2 ELSE 20 END), CAST(B.Code AS nvarchar(50))) AS TaxNo,
                   RS.DocName AS Book, S.StudentName, S.NationalId, S.StudentTel, D.DiplomName AS Program,
                   CONVERT(nvarchar(20), D.CountNum) + ' ' + ISNULL(D.CountText,'') AS Duration,
                   CAST(ISNULL(X.Cost,0) AS decimal(18,2)) AS Cost, CAST(ISNULL(X.Tax,0) AS decimal(18,2)) AS Tax, CAST(ISNULL(X.SubTotal,0) AS decimal(18,2)) AS SubTotal,
                   BR.BrName AS Branch, U.FullName AS [User], B.Notes, CAST(ISNULL(B.IsUse,1) AS bit) AS Active
            FROM dbo.Bill B WITH (NOLOCK)
            JOIN dbo.RegDocDiplom X ON X.RegDocGuid = B.Guid
            JOIN dbo.Diplom D ON D.Guid = X.DiplomGuid
            JOIN dbo.Student S ON S.AccountGuid = B.AccountGuid
            LEFT JOIN dbo.RegSetting RS ON RS.Guid = B.DocGuid
            LEFT JOIN dbo.Branch BR ON BR.Guid = B.BranchGuid
            LEFT JOIN dbo.User_Info U ON U.Guid = B.UserGuid
            WHERE B.BillDate >= @f AND B.BillDate < @t AND (@branch IS NULL OR B.BranchGuid = @branch)
              AND (@active IS NULL OR ISNULL(B.IsUse,1) = @active)
              AND (@q IS NULL OR S.StudentName LIKE '%' + @q + '%' OR S.NationalId LIKE @q + '%' OR CAST(B.Code AS nvarchar(50)) = @q OR CAST(B.TaxNo AS nvarchar(20)) = @q)
            ORDER BY B.BillDate DESC, TRY_CAST(B.Code AS int) DESC
            """, new { f, t, branch, active, q = Text(q) }, commandTimeout: 60, cancellationToken: ct));
    }

    /* ---------------- تقرير مرتجع مبيعات الضرائب ---------------- */
    public async Task<IEnumerable<TaxRow>> TaxReturnsAsync(DateTime? from, DateTime? to, Guid? branch, bool? active, string? q, CancellationToken ct)
    {
        var (f, t) = Range(from, to);
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync<TaxRow>(new CommandDefinition("""
            SELECT RB.Guid, RB.ReDate AS Date, CAST(RB.Code AS nvarchar(50)) AS Code, CAST(RB.Code AS nvarchar(50)) AS TaxNo,
                   RS.DocName AS Book, S.StudentName, S.NationalId, S.StudentTel, D.DiplomName AS Program,
                   CONVERT(nvarchar(20), D.CountNum) + ' ' + ISNULL(D.CountText,'') AS Duration,
                   CAST(ISNULL(X.Cost,0) AS decimal(18,2)) AS Cost, CAST(ISNULL(X.Tax,0) AS decimal(18,2)) AS Tax, CAST(ISNULL(X.SubTotal,0) AS decimal(18,2)) AS SubTotal,
                   BR.BrName AS Branch, U.FullName AS [User], RB.ReNotes AS Notes, CAST(ISNULL(RB.IsUse,1) AS bit) AS Active,
                   ISNULL(RIGHT('0' + CAST(OB.TaxNo AS nvarchar(20)), CASE WHEN OB.TaxNo < 10 THEN 2 ELSE 20 END), CAST(OB.Code AS nvarchar(50))) AS InvoiceTaxNo
            FROM dbo.ReBill RB
            JOIN dbo.RegDocDiplom X ON X.RegDocGuid = RB.Guid
            JOIN dbo.Diplom D ON D.Guid = X.DiplomGuid
            JOIN dbo.Student S ON S.AccountGuid = RB.AccountGuid
            LEFT JOIN dbo.Bill OB ON OB.Guid = RB.BillGuid
            LEFT JOIN dbo.RegSetting RS ON RS.Guid = RB.DocGuid
            LEFT JOIN dbo.Branch BR ON BR.Guid = RB.BranchGuid
            LEFT JOIN dbo.User_Info U ON U.Guid = RB.UserGuid
            WHERE RB.ReDate >= @f AND RB.ReDate < @t AND (@branch IS NULL OR RB.BranchGuid = @branch)
              AND (@active IS NULL OR ISNULL(RB.IsUse,1) = @active)
              AND (@q IS NULL OR S.StudentName LIKE '%' + @q + '%' OR S.NationalId LIKE @q + '%' OR CAST(RB.Code AS nvarchar(50)) = @q)
            ORDER BY RB.ReDate DESC, TRY_CAST(RB.Code AS int) DESC
            """, new { f, t, branch, active, q = Text(q) }, commandTimeout: 60, cancellationToken: ct));
    }

    /* ---------------- تقرير المسوقين ---------------- */
    public async Task<IEnumerable<SellerRow>> SellersAsync(DateTime? from, DateTime? to, Guid? branch, Guid? seller, int? regType, CancellationToken ct)
    {
        var (f, t) = Range(from, to);
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync<SellerRow>(new CommandDefinition("""
            SELECT R.Guid, R.AccountGuid, R.RegDate AS Date, CAST(R.Code AS nvarchar(50)) AS Code, SM.ManFullName AS Seller, BR.BrName AS Branch,
                   S.StudentName, S.NationalId, S.StudentTel, P.Programs, ISNULL(R.RegType,0) AS RegType,
                   CAST(ISNULL(R.DocSubTotal,0) AS decimal(18,2)) AS SubTotal, CAST(ISNULL(R.IsReReg,0) AS bit) AS Returned
            FROM dbo.RegDoc R
            JOIN dbo.Student S ON S.AccountGuid = R.AccountGuid
            JOIN dbo.SalesMan SM ON SM.Guid = R.SellerGuid
            LEFT JOIN dbo.Branch BR ON BR.Guid = R.BranchGuid
            OUTER APPLY (SELECT STRING_AGG(D.DiplomName, '، ') AS Programs FROM dbo.RegDocDiplom X JOIN dbo.Diplom D ON D.Guid = X.DiplomGuid
                         WHERE X.RegDocGuid = R.Guid AND X.Type_ = 0 AND X.DiplomType = 0) P
            WHERE R.RegDate >= @f AND R.RegDate < @t AND ISNULL(R.IstUse,1) = 1 AND ISNULL(R.DocSubTotal,0) <> 0
              AND (@branch IS NULL OR R.BranchGuid = @branch) AND (@seller IS NULL OR R.SellerGuid = @seller) AND (@regType IS NULL OR R.RegType = @regType)
            ORDER BY SM.ManFullName, R.RegDate DESC
            """, new { f, t, branch, seller, regType }, commandTimeout: 60, cancellationToken: ct));
    }

    /* ---------------- أرصدة الطلاب (متابعة السداد / أرصدة العملاء) ---------------- */
    public async Task<IEnumerable<BalanceRow>> BalancesAsync(DateTime? from, DateTime? to, Guid? branch, Guid? diplom, Guid? batch, string? q, bool onlyMoved, CancellationToken ct)
    {
        var (f, t) = Range(from, to);
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync<BalanceRow>(new CommandDefinition("""
            WITH M AS (
                SELECT D2.AccountGuid,
                       SUM(CASE WHEN D1.DayDate <  @f AND F.UnqName <> 'salesinvoice' THEN ISNULL(D2.Maden,0) ELSE 0 END)
                     - SUM(CASE WHEN D1.DayDate <  @f AND F.UnqName <> 'repay'        THEN ISNULL(D2.Daen,0)  ELSE 0 END) AS Opening,
                       SUM(CASE WHEN D1.DayDate >= @f AND F.UnqName <> 'salesinvoice' THEN ISNULL(D2.Maden,0) ELSE 0 END) AS Maden,
                       SUM(CASE WHEN D1.DayDate >= @f AND F.UnqName <> 'repay'        THEN ISNULL(D2.Daen,0)  ELSE 0 END) AS Daen
                FROM dbo.Day2 D2 JOIN dbo.Day1 D1 ON D1.Guid = D2.Day1Guid JOIN dbo.Form_Name F ON F.Guid = D1.TypeGuid
                WHERE D1.IsUse = 1 AND D1.DayDate < @t AND D2.AccountGuid IN (SELECT AccountGuid FROM dbo.Student)
                GROUP BY D2.AccountGuid)
            SELECT S.AccountGuid, CAST(S.Code AS nvarchar(50)) AS Code, S.AcadmyId, S.StudentName, S.StudentTel, S.NationalId, BR.BrName AS Branch,
                   CAST(ISNULL(M.Opening,0) AS decimal(18,2)) AS Opening, CAST(ISNULL(M.Maden,0) AS decimal(18,2)) AS Maden, CAST(ISNULL(M.Daen,0) AS decimal(18,2)) AS Daen,
                   CAST(ISNULL(M.Opening,0) + ISNULL(M.Maden,0) - ISNULL(M.Daen,0) AS decimal(18,2)) AS Balance
            FROM dbo.Student S
            LEFT JOIN M ON M.AccountGuid = S.AccountGuid
            LEFT JOIN dbo.Branch BR ON BR.Guid = S.BarnchGuid
            WHERE (@branch IS NULL AND @diplom IS NULL AND @batch IS NULL
                   OR EXISTS (SELECT 1 FROM dbo.StudentLevel L WHERE L.StudentGuid = S.AccountGuid AND ISNULL(L.IsUse,1) = 1
                              AND (@branch IS NULL OR L.BranchGuid = @branch) AND (@diplom IS NULL OR L.DiplomGuid = @diplom) AND (@batch IS NULL OR L.BatchGuid = @batch)))
              AND (@q IS NULL OR S.StudentName LIKE '%' + @q + '%' OR S.NationalId LIKE @q + '%' OR S.StudentTel LIKE '%' + @q + '%')
              AND (@onlyMoved = 0 OR ISNULL(M.Maden,0) <> 0 OR ISNULL(M.Daen,0) <> 0)
            ORDER BY TRY_CONVERT(int, S.Code)
            """, new { f, t, branch, diplom, batch, q = Text(q), onlyMoved }, commandTimeout: 90, cancellationToken: ct));
    }

    /* ---------------- تحليل المصروفات ---------------- */
    public async Task<object> ExpensesAsync(DateTime? from, DateTime? to, Guid cash, Guid? cost, CancellationToken ct)
    {
        var (f, t) = Range(from, to);
        await using var c = await db.OpenAsync(ct);
        var rows = (await c.QueryAsync<ExpenseRow>(new CommandDefinition("""
            SELECT CD.CashDayDate AS Date, CAST(CD.Code AS nvarchar(50)) AS Code, CAST(A.Code AS nvarchar(50)) AS AccountCode, A.AccountName AS Account,
                   CAST(ISNULL(D2.Maden,0) AS decimal(18,2)) AS Maden, CAST(ISNULL(D2.Daen,0) AS decimal(18,2)) AS Daen,
                   D2.Notes, CD.VendorName AS Vendor, CD.VendorTax, CD.VendorTel, CC.CenterName AS CostCenter
            FROM dbo.CashDay CD
            JOIN dbo.Day1 D1 ON D1.DocGuid = CD.Guid
            JOIN dbo.Day2 D2 ON D2.Day1Guid = D1.Guid
            JOIN dbo.Account A ON A.Guid = D2.AccountGuid
            LEFT JOIN dbo.CostCenter CC ON CC.Guid = D2.CostGuid
            WHERE CD.CashDayType = 1 AND CD.FAccountGuid = @cash AND D2.AccountGuid <> @cash AND D1.IsUse = 1
              AND (@cost IS NULL OR D2.CostGuid = @cost) AND CD.CashDayDate >= @f AND CD.CashDayDate < @t
            ORDER BY CD.CashDayDate, TRY_CONVERT(int, CD.Code)
            """, new { f, t, cash, cost }, commandTimeout: 60, cancellationToken: ct))).ToList();
        decimal run = 0;
        foreach (var r in rows) { run += r.Maden - r.Daen; r.Balance = run; }
        var byAccount = rows.GroupBy(r => (r.AccountCode, r.Account))
            .Select(g => new { code = g.Key.AccountCode, account = g.Key.Account, amount = g.Sum(x => x.Maden - x.Daen), count = g.Select(x => x.Code).Distinct().Count() })
            .OrderByDescending(x => x.amount).ToList();
        return new { rows, byAccount, total = run, vouchers = rows.Select(r => r.Code).Distinct().Count() };
    }
}
