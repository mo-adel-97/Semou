using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text.Json;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;
using Semou.Api.Security;
using Semou.Api.Features.Sales;

namespace Semou.Api.Features.Registration;

/*
 * استمارة التسجيل (RegisterForm + WhatRegTypeNeed + AttachForm).
 *
 * One save = (same order as the desktop FORSAVE):
 *   RegDoc + RegDocDiplom (diplomas, fees) + journal
 *   [if the book requires a down payment]
 *     Bill (sales invoice) + items + journal (+VAT) + UpdatePayStaut
 *     Receipt voucher (+ a second one for a split bank transfer) + details + journal
 *     BillByJson (ZATCA queue, type 388)
 * all in ONE transaction. Prices are recomputed here; the browser's numbers are never trusted.
 */
public sealed class RegistrationService(Db db, UserActionLog audit, CurrentUser me, AttachmentService attachments, SalesPoster poster)
{
    public const string Form = "addregdoc";
    private const decimal ForeignVat = 15m;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    /* ============================ Reading ============================ */

    private sealed record StudentRow(Guid Guid, Guid AccountGuid, string StudentName, string? StudentTel, string? NationalId, int? StudentNational,
                                     bool? IsUse, bool? LockProfile, Guid? BarnchGuid, string? Email);

    private static async Task<StudentRow> StudentAsync(DbConnection c, DbTransaction? tx, Guid guid)
    {
        var s = await c.QueryFirstOrDefaultAsync<StudentRow>(
            "SELECT Guid, AccountGuid, StudentName, StudentTel, NationalId, StudentNational, IsUse, LockProfile, BarnchGuid, Email FROM dbo.Student WHERE Guid = @guid",
            new { guid }, tx) ?? throw ApiException.NotFound("الطالب غير موجود");
        if (s.IsUse == false) throw new ApiException("الطالب موقوف عن التعامل");
        if (s.LockProfile == true) throw new ApiException("ملف الطالب مغلق، يرجى التواصل مع الإدارة");
        return s;
    }

    /// <summary>AutoGetDocType + ShowRegSetting: the registration book for the study branch and type.</summary>
    private static async Task<RegSettingInfo> SettingAsync(DbConnection c, DbTransaction? tx, Guid branch, int regType)
    {
        var regTypeGuid = await c.ExecuteScalarAsync<Guid>("dbo.FormTypeGuid", new { FormName = Form }, tx, commandType: CommandType.StoredProcedure);
        var s = await c.QueryFirstOrDefaultAsync<RegSettingInfo>("""
            SELECT TOP 1 R.Guid, R.Code, R.DocName, R.BranchGuid, B.BrName AS BranchName, R.PriceListGuid,
                   CAST(ISNULL(R.ChkMaden,0) AS bit) AS ChkMaden, R.MadenGuid, CAST(ISNULL(R.ChkDaen,0) AS bit) AS ChkDaen, R.DaenGuid, R.CostCenterGuid,
                   CAST(ISNULL(R.ChkOtherFees,0) AS bit) AS ChkOtherFees, CAST(ISNULL(R.ChkStartPay,0) AS bit) AS ChkStartPay,
                   CAST(ISNULL(R.ChkAmountPay,0) AS bit) AS ChkAmountPay, CAST(ISNULL(R.AmountPay,0) AS decimal(18,2)) AS AmountPay,
                   R.DocPayGuid, P.DocName AS DocPayName, P.BranchGuid AS PayBranchGuid, P.Code AS PayDocCode,
                   R.VatAccountGuid, CAST(ISNULL(R.ChkVat,0) AS bit) AS ChkVat
            FROM dbo.RegSetting R JOIN dbo.Branch B ON B.Guid = R.BranchGuid LEFT JOIN dbo.RegSetting P ON P.Guid = R.DocPayGuid
            WHERE R.DocTypeGuid = @regTypeGuid AND R.BranchGuid = @branch AND R.RegType = @regType AND R.IsUse = 1
            ORDER BY R.Code
            """, new { regTypeGuid, branch, regType }, tx);
        return s ?? throw new ApiException("لا يتوفر دفتر للتسجيل، يرجى مراجعة مدير النظام");
    }

    private static decimal R3(decimal v) => Math.Round(v, 3, MidpointRounding.AwayFromZero);

    /// <summary>SHOWPRICESALEDIPLOM / SHOWLISTDIPLOM / SHOWPRICESALEONLYCOURSE / SHOWLISTFEES with the nationality rules.</summary>
    private static async Task<(List<PriceRow> Diploms, List<PriceRow> Fees)> PricesAsync(DbConnection c, DbTransaction? tx, RegSettingInfo s, int regType, bool foreign, Guid? courseDiplom)
    {
        var priceSale = await c.ExecuteScalarAsync<Guid?>("dbo.WhatPriceSaleGuidByBranch", new { BranchGuid = s.BranchGuid }, tx, commandType: CommandType.StoredProcedure);
        var diploms = new List<PriceRow>();
        if (priceSale is { } ps)
        {
            var rows = await c.QueryAsync("dbo.ShowPriceSaleDiplom", new { PriceSaleGuid = ps, Type = 0, StudyType = regType, DiplomGuid = regType == 1 ? courseDiplom : null },
                tx, commandType: CommandType.StoredProcedure);
            foreach (IDictionary<string, object?> r in rows)
            {
                decimal cost = D(r["DiplomTotal"]), rate = D(r["DiplomTaxVat"]), tax = D(r["DiplomTaxValue"]), total = D(r["DiplomSubTotal"]);
                if (foreign && regType == 0) { tax = R3(cost * ForeignVat / 100); total = R3(cost + tax); } // the course grid ignores nationality on the desktop
                diploms.Add(new PriceRow(S(r["Code"]), S(r["DiplomName"]), (Guid)r["DiplomGuid"]!, (int)D(r["DiplomType"]), cost, tax, total,
                    regType == 0 && foreign ? ForeignVat : rate));
            }
        }
        else
        {
            var rows = await c.QueryAsync("dbo.ShowPriceListDiplom", new { PriceListGuid = s.PriceListGuid, Type = 0, StudyType = regType }, tx, commandType: CommandType.StoredProcedure);
            foreach (IDictionary<string, object?> r in rows)
            {
                decimal cost = D(r["DiplomCost"]), rate = D(r["DiplomTaxVat"]), tax = D(r["DiplomTaxValue"]), total = D(r["DiplomSubTotal"]);
                if (foreign) { cost = R3(D(r["DiplomCost"]) / (1 + ForeignVat / 100)); tax = R3(cost * ForeignVat / 100); }
                diploms.Add(new PriceRow(S(r["Code"]), S(r["DiplomName"]), (Guid)r["diplomguid"]!, (int)D(r["DiplomType"]), cost, tax, total,
                    regType == 0 && foreign ? ForeignVat : rate));
            }
        }

        var fees = new List<PriceRow>();
        if (s.ChkOtherFees)
        {
            var rows = await c.QueryAsync("dbo.ShowPriceListDiplom", new { PriceListGuid = s.PriceListGuid, Type = 1 }, tx, commandType: CommandType.StoredProcedure);
            foreach (IDictionary<string, object?> r in rows)
                fees.Add(new PriceRow(S(r["Code"]), S(r["DiplomName"]), (Guid)r["diplomguid"]!, (int)D(r["DiplomType"]), D(r["DiplomCost"]), D(r["DiplomTaxValue"]), D(r["DiplomSubTotal"]), D(r["DiplomTaxVat"])));
        }
        return (diploms, fees);
    }

    private static decimal D(object? v) => v is null or DBNull ? 0 : Convert.ToDecimal(v, Inv);
    private static string S(object? v) => v?.ToString() ?? "";

    /// <summary>AskForDiplomSeat for every diploma of a batch: no DiplomCountSeat row = no seats (same as the save check).</summary>
    public async Task<IEnumerable<object>> DiplomSeatsAsync(Guid branch, Guid batch, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var rows = await c.QueryAsync<(Guid DiplomGuid, int Capacity, int Registered)>(new CommandDefinition("""
            SELECT dcs.DiplomGuid, CAST(MAX(ISNULL(dcs.DiplomCount,0)) AS int) AS Capacity,
                   (SELECT COUNT(rd.Code) FROM dbo.RegDoc rd JOIN dbo.RegDocDiplom rdd ON rd.Guid = rdd.RegDocGuid
                     WHERE rdd.DiplomGuid = dcs.DiplomGuid AND rd.RegType = 0 AND rd.DocSubTotal <> 0 AND rd.IsReReg = 0
                       AND rd.BatchGuid = dcs.BatchGuid AND rd.BranchGuid = dcs.BranchGuid) AS Registered
            FROM dbo.DiplomCountSeat dcs
            WHERE dcs.BranchGuid = @branch AND dcs.BatchGuid = @batch
            GROUP BY dcs.DiplomGuid, dcs.BatchGuid, dcs.BranchGuid
            """, new { branch, batch }, cancellationToken: ct));
        return rows.Select(r => (object)new { diplomGuid = r.DiplomGuid, capacity = r.Capacity, registered = r.Registered, available = Math.Max(0, r.Capacity - r.Registered) });
    }

    /// <summary>Everything the wizard needs for one student + study branch + type (WhatRegTypeNeed → RegisterForm load).</summary>
    public async Task<object> PrepareAsync(Guid studentGuid, Guid branch, int regType, CancellationToken ct)
    {
        if (regType is not (0 or 1)) throw new ApiException("يرجى تحديد نوع التسجيل أولاً");
        await using var c = await db.OpenAsync(ct);
        var st = await StudentAsync(c, null, studentGuid);
        if (regType == 0)
        {
            var active = await c.QueryFirstOrDefaultAsync<string?>("dbo.ChkIfStudentRegDiplom", new { StudentGuid = st.AccountGuid }, commandType: CommandType.StoredProcedure);
            if (active != null) throw new ApiException($"لا يمكن تسجيل الطالب في أكثر من دبلوم في الوقت نفسه\nالطالب مسجل حالياً في: {active}");
        }
        var s = await SettingAsync(c, null, branch, regType);
        var foreign = st.StudentNational == 1;
        var (diploms, fees) = await PricesAsync(c, null, s, regType, foreign, null);

        var zero = await c.ExecuteScalarAsync<bool?>("SELECT ChkAmount FROM dbo.User_Info WHERE Guid = @g", new { g = me.UserGuid }) == true;
        return new
        {
            student = new { st.Guid, st.StudentName, st.NationalId, st.StudentTel, foreign },
            setting = new { s.Guid, s.Code, s.DocName, s.BranchGuid, s.BranchName, s.ChkOtherFees, s.ChkStartPay, s.ChkAmountPay, s.AmountPay, s.DocPayName, s.ChkVat },
            zeroAmount = zero,
            diploms, fees,
            // the packages of each diploma (diplomGuid -> packages); empty = sold at the list price
            plans = regType == 0 ? await Semou.Api.Features.Pricing.PricingService.OffersAsync(c, null, diploms.Select(d => d.DiplomGuid)) : [],
            batches = regType == 0 ? await c.QueryAsync("dbo.ShowSeatsOnBatch", new { BranchGuid = s.BranchGuid }, commandType: CommandType.StoredProcedure) : [],
            levels = regType == 0 ? await c.QueryAsync("SELECT Guid, LevelName AS Name, CAST(ISNULL(IsDef,0) AS bit) AS IsDef FROM dbo.Levels WHERE IsUse = 1 ORDER BY TRY_CONVERT(int, Code)") : [],
            courses = regType == 1 ? await c.QueryAsync("""
                SELECT C.Guid, C.Code, D.DiplomName AS Name, C.CourseName AS CourseCode, C.DiplomGuid, D.DiplomName, C.DateStart, C.DateEnd, C.When_ AS Time,
                       ISNULL(C.StudentCount,0) AS Capacity, (SELECT COUNT(*) FROM dbo.RegDoc R WHERE R.CourseGuid = C.Guid AND R.IstUse = 1) AS Registered
                FROM dbo.Course C JOIN dbo.Diplom D ON D.Guid = C.DiplomGuid WHERE C.IsUse = 1 AND C.BranchGuid = @b ORDER BY TRY_CONVERT(int, C.Code) DESC
                """, new { b = s.BranchGuid }) : [],
            sellers = await c.QueryAsync("dbo.ViewSalesManForReg", new { BranchGuid = s.BranchGuid }, commandType: CommandType.StoredProcedure),
            cashBoxes = s.ChkStartPay ? await c.QueryAsync("""
                SELECT CB.AccountGuid AS Guid, CB.CashName AS Name, CB.CashType, CB.CostCenterGuid, CC.CenterName AS CostName
                FROM dbo.CashBox CB LEFT JOIN dbo.CostCenter CC ON CC.Guid = CB.CostCenterGuid
                WHERE CB.IsUse = 1 AND CB.UseInSales = 1 ORDER BY CB.CashType, TRY_CONVERT(int, CB.Code)
                """) : [],
            attachments = await attachments.RequirementsAsync(c, regType),
        };
    }

    /// <summary>Diplomas priced for a chosen course (price offers are filtered by the course's diploma).</summary>
    public async Task<IEnumerable<PriceRow>> CoursePricesAsync(Guid studentGuid, Guid branch, Guid courseGuid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var st = await StudentAsync(c, null, studentGuid);
        var s = await SettingAsync(c, null, branch, 1);
        var diplom = await c.ExecuteScalarAsync<Guid?>("SELECT DiplomGuid FROM dbo.Course WHERE Guid = @courseGuid", new { courseGuid }) ?? throw new ApiException("يرجى اختيار الدورة");
        return (await PricesAsync(c, null, s, 1, st.StudentNational == 1, diplom)).Diploms;
    }

    /* ============================ Saving ============================ */

    private static string Cut(string s, int max) => s.Length <= max ? s : s[..max];
    private static Task<Guid> TypeAsync(DbConnection c, DbTransaction tx, string form) =>
        c.ExecuteScalarAsync<Guid>("dbo.FormTypeGuid", new { FormName = form }, tx, commandType: CommandType.StoredProcedure);

    /// <summary>Direct save (kept for internal use; the web sends a طلب التحاق that is posted on approval).</summary>
    public async Task<RegistrationSaved> SaveAsync(RegistrationInput i, CancellationToken ct)
    {
        Normalize(i);
        return await db.InTransactionAsync(async (c, tx) =>
            await PostAsync(c, tx, i, await PlanAsync(c, tx, i, me.UserGuid, checkAttachments: true, exceptOrder: null), me.UserGuid),
            IsolationLevel.Serializable, ct);
    }

    public static void Normalize(RegistrationInput i)
    {
        if (i.RegType is not (0 or 1)) throw new ApiException("يرجى تحديد نوع التسجيل أولاً");
        if (i.RegDate == default) i.RegDate = Clock.Now;
        else i.RegDate = i.RegDate.Date == Clock.Now.Date ? Clock.Now : i.RegDate.Date;
    }

    /// <summary>Seats held by registration requests still waiting for review (other than <paramref name="exceptOrder"/>).</summary>
    private static Task<int> PendingSeatsAsync(DbConnection c, DbTransaction tx, string column, Guid value, Guid branch, Guid? batch, Guid? exceptOrder) =>
        c.ExecuteScalarAsync<int>($"""
            IF OBJECT_ID('dbo.WebRegOrder') IS NULL SELECT 0 ELSE
            SELECT COUNT(*) FROM dbo.WebRegOrder WITH (UPDLOCK, HOLDLOCK)
            WHERE Status IN (0,3) AND {column} = @value AND BranchGuid = @branch AND (@batch IS NULL OR BatchGuid = @batch) AND (@exceptOrder IS NULL OR Guid <> @exceptOrder)
            """, new { value, branch, batch, exceptOrder }, tx);

    /// <summary>
    /// Every check of the desktop save + server pricing, without writing anything. Used when a request is sent
    /// (by its author) and again when it is approved, so an approved request is exactly what a direct save would be.
    /// </summary>
    public async Task<RegPlan> PlanAsync(DbConnection c, DbTransaction tx, RegistrationInput i, Guid actor, bool checkAttachments, Guid? exceptOrder)
    {
            var st = await StudentAsync(c, tx, i.StudentGuid);
            var s = await SettingAsync(c, tx, i.StudyBranchGuid, i.RegType);
            var zero = await c.ExecuteScalarAsync<bool?>("SELECT ChkAmount FROM dbo.User_Info WHERE Guid = @g", new { g = actor }, tx) == true;

            /* ---------- VALIDATE_DATAFOR_STEP1 ---------- */
            string? batchName = null, courseDiplomName = null;
            Guid? courseDiplom = null;
            if (i.RegType == 0)
            {
                var active = await c.QueryFirstOrDefaultAsync<string?>("dbo.ChkIfStudentRegDiplom", new { StudentGuid = st.AccountGuid }, tx, commandType: CommandType.StoredProcedure);
                if (active != null) throw new ApiException("لا يمكن تسجيل الطالب في أكثر من دبلوم في الوقت نفسه");
                if (i.BatchGuid is null) throw new ApiException("يرجى اختيار الدفعة");
                if (i.LevelGuid is null) throw new ApiException("يرجى اختيار المستوى المراد تسجيل الطالب عليه");
                batchName = await c.ExecuteScalarAsync<string?>("SELECT BatchName FROM dbo.Batch WHERE Guid = @g AND IsUse = 1", new { g = i.BatchGuid }, tx) ?? throw new ApiException("الدفعة غير صالحة");
                if (await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Levels WHERE Guid = @g AND IsUse = 1", new { g = i.LevelGuid }, tx) == 0) throw new ApiException("المستوى غير صالح");
            }
            else
            {
                if (i.CourseGuid is null) throw new ApiException("يرجى اختيار الدورة");
                var course = await c.QueryFirstOrDefaultAsync<(Guid DiplomGuid, string DiplomName, int? Capacity)>(
                    "SELECT C.DiplomGuid, D.DiplomName, C.StudentCount FROM dbo.Course C JOIN dbo.Diplom D ON D.Guid = C.DiplomGuid WHERE C.Guid = @g AND C.IsUse = 1 AND C.BranchGuid = @b",
                    new { g = i.CourseGuid, b = s.BranchGuid }, tx);
                if (course.DiplomName is null) throw new ApiException("الدورة غير صالحة لهذا الفرع");
                var registered = await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.RegDoc WITH (UPDLOCK, HOLDLOCK) WHERE CourseGuid = @g AND IstUse = 1", new { g = i.CourseGuid }, tx);
                var held = await PendingSeatsAsync(c, tx, "CourseGuid", i.CourseGuid.Value, s.BranchGuid, null, exceptOrder);
                if ((course.Capacity ?? 0) - registered - held <= 0)
                    throw new ApiException(held > 0 ? $"لا توجد مقاعد متاحة: المقاعد المتبقية محجوزة لطلبات التحاق بانتظار المراجعة ({held})" : "لا توجد أي مقاعد متاحة للتسجيل");
                courseDiplom = course.DiplomGuid; courseDiplomName = course.DiplomName;
            }
            if (i.SellerGuid is null) throw new ApiException("يرجى اختيار مندوب البيع");
            var sellerOk = (await c.QueryAsync("dbo.ViewSalesManForReg", new { BranchGuid = s.BranchGuid }, tx, commandType: CommandType.StoredProcedure))
                .Any(r => (Guid)((IDictionary<string, object?>)r)["Guid"]! == i.SellerGuid);
            if (!sellerOk) throw new ApiException("مندوب البيع غير مسموح له بالتسجيل في هذا الفرع");

            /* ---------- VALIDATE_DATAFOR_STEP2: prices from the server ---------- */
            var (catalog, feeCatalog) = await PricesAsync(c, tx, s, i.RegType, st.StudentNational == 1, courseDiplom);
            if (i.Diploms.Count == 0) throw new ApiException("لا يمكن التسجيل بدون اختيار الدبلوم - الدورة");
            if (s.ChkOtherFees && i.Fees.Count == 0) throw new ApiException("لا يمكن التسجيل بدون اختيار رسوم الاختبار - الشهادة");
            List<PriceRow> pick(List<RegItem> items, List<PriceRow> from) => items.DistinctBy(x => x.DiplomGuid).Select(x =>
            {
                var p = from.FirstOrDefault(r => r.DiplomGuid == x.DiplomGuid) ?? throw new ApiException("أحد البنود المختارة غير موجود في قائمة الأسعار");
                if (x.Total is { } t && t != p.Total)
                {
                    if (!zero) throw new ApiException("لا تملك صلاحية تعديل قيمة البنود");
                    if (t < 0) throw new ApiException("لا يمكن إدخال قيمة بالسالب");
                    p = p with { Cost = t, Tax = 0, Total = t }; // DiplomGrid_CellEndEdit: the edited total becomes the cost
                }
                return p;
            }).ToList();
            var dips = pick(i.Diploms, catalog);

            /* ---------- باقة التسجيل: the package's price replaces the list price (final amount, VAT inside) ---------- */
            Semou.Api.Features.Pricing.PlanOffer? plan = null;
            if (i.RegType == 0 && dips.Count == 1)
            {
                var offers = await Semou.Api.Features.Pricing.PricingService.OffersAsync(c, tx, [dips[0].DiplomGuid]);
                if (offers.TryGetValue(dips[0].DiplomGuid, out var list) && list.Count > 0)
                {
                    if (i.PlanGuid is null) throw new ApiException("يرجى اختيار باقة التسجيل");
                    plan = list.FirstOrDefault(o => o.PlanGuid == i.PlanGuid) ?? throw new ApiException("الباقة المختارة غير متاحة لهذا الدبلوم، يرجى اختيار باقة أخرى");
                    var d0 = dips[0];
                    var rate = d0.PayTaxRate;
                    var cost = rate != 0 ? R3(plan.Price / (1 + rate / 100)) : plan.Price;
                    dips[0] = d0 with { Cost = cost, Tax = plan.Price - cost, Total = plan.Price };
                }
                else if (i.PlanGuid is not null) throw new ApiException("لا توجد باقات لهذا الدبلوم، يرجى التسجيل بسعر قائمة الأسعار");
            }
            var fees = s.ChkOtherFees ? pick(i.Fees, feeCatalog) : [];
            var all = dips.Concat(fees).ToList();
            decimal fTotal = Math.Round(all.Sum(x => x.Cost), 2), fTax = Math.Round(all.Sum(x => x.Tax), 2), fSub = Math.Round(all.Sum(x => x.Total), 2);
            decimal dSub = Math.Round(dips.Sum(x => x.Total), 2);
            // A diploma form is for ONE diploma: the seat check and the training file are per diploma.
            if (i.RegType == 0 && dips.Count != 1) throw new ApiException("استمارة تسجيل الدبلوم تكون لدبلوم واحد فقط، يرجى اختيار دبلوم واحد");
            var mainDiplom = dips[^1];

            if (i.RegType == 0)
            {
                var seat = await c.QueryFirstOrDefaultAsync<(int DIPLOMCOUNT, int RegCount)>("dbo.AskForDiplomSeat",
                    new { BranchGuid = s.BranchGuid, BatchGuid = i.BatchGuid, DiplomGuid = mainDiplom.DiplomGuid }, tx, commandType: CommandType.StoredProcedure);
                var held = await PendingSeatsAsync(c, tx, "DiplomGuid", mainDiplom.DiplomGuid, s.BranchGuid, i.BatchGuid, exceptOrder);
                if (seat.DIPLOMCOUNT - seat.RegCount - held <= 0)
                    throw new ApiException(held > 0 ? $"عذراً، لا تتوفر مقاعد في هذا التخصص: المقاعد المتبقية محجوزة لطلبات التحاق بانتظار المراجعة ({held})" : "عذراً، التخصص الذي اخترته لا تتوفر فيه أي مقاعد للتسجيل");
            }

            /* ---------- Payment checks (VALIDATE_DATA) ---------- */
            PaymentInput? pay = s.ChkStartPay ? i.Payment ?? throw new ApiException("يرجى إدخال بيانات الدفعة المقدمة") : null;
            List<(PriceRow Item, decimal Cost, decimal Tax, decimal Total)> payLines = [];
            decimal payTotal = 0, payTax = 0, paySub = 0;
            string? cashName = null, cashName2 = null;
            if (pay != null)
            {
                if (s.DocPayGuid is null) throw new ApiException("لا يتوفر نوع مستند السداد، يرجى التواصل مع مدير النظام");
                if (pay.CashAccountGuid is null) throw new ApiException("يرجى تحديد الخزينة / البنك أولاً");
                cashName = await c.ExecuteScalarAsync<string?>("SELECT CashName FROM dbo.CashBox WHERE AccountGuid = @a AND IsUse = 1 AND UseInSales = 1", new { a = pay.CashAccountGuid }, tx)
                           ?? throw new ApiException("الخزينة / البنك غير متاح لفواتير المبيعات");
                if (pay.CostCenterGuid is null) throw new ApiException("يرجى اختيار مركز التكلفة الخاص بعملية السداد");
                if (string.IsNullOrWhiteSpace(pay.Ref)) throw new ApiException("يرجى إدخال رقم المرجع الخاص بعملية السداد");
                if (pay.PayDate is null) throw new ApiException("يرجى إدخال تاريخ الحوالة البنكية / السداد");

                foreach (var l in pay.Lines.Where(l => l.Total != 0).DistinctBy(l => l.DiplomGuid))
                {
                    var item = dips.FirstOrDefault(d => d.DiplomGuid == l.DiplomGuid) ?? throw new ApiException("بند السداد غير موجود في الاستمارة");
                    // PayGrid_CellEndEdit rules
                    var minDown = plan?.DownPayment ?? s.AmountPay;
                    if (l.Total < minDown) throw new ApiException(plan != null
                        ? $"لا يمكن إدخال قيمة أقل من الدفعة المقدمة لباقة {plan.Name} ({minDown:0.00})"
                        : $"لا يمكن إدخال قيمة أقل من القيمة المحددة للدفعة المقدمة ({minDown:0.00})");
                    if (l.Total > dSub) throw new ApiException("لا يمكن إدخال قيمة أكبر من قيمة الدبلوم / الدورة دون الرسوم");
                    var rate = item.PayTaxRate;
                    var cost = rate != 0 ? R3(l.Total / (1 + rate / 100)) : l.Total;
                    var tax = rate != 0 ? R3(cost * rate / 100) : 0;
                    payLines.Add((item, cost, tax, l.Total));
                }
                payTotal = Math.Round(payLines.Sum(x => x.Cost), 2); payTax = Math.Round(payLines.Sum(x => x.Tax), 2); paySub = Math.Round(payLines.Sum(x => x.Total), 2);
                if (paySub == 0 && !zero) throw new ApiException("يرجى إدخال المبلغ الخاص بعملية السداد");
                if (payLines.Count == 0) payLines.Add((mainDiplom, 0, 0, 0)); // zero-amount invoice still lists the item
                if (pay.Split)
                {
                    if (pay.CashAccountGuid2 is null) throw new ApiException("يرجى تحديد الخزينة / البنك الثاني");
                    cashName2 = await c.ExecuteScalarAsync<string?>("SELECT CashName FROM dbo.CashBox WHERE AccountGuid = @a AND IsUse = 1 AND UseInSales = 1", new { a = pay.CashAccountGuid2 }, tx)
                                ?? throw new ApiException("الخزينة / البنك 2 غير متاح لفواتير المبيعات");
                    if (pay.CostCenterGuid2 is null) throw new ApiException("يرجى اختيار مركز التكلفة الثاني الخاص بعملية السداد");
                    if (string.IsNullOrWhiteSpace(pay.Ref2)) throw new ApiException("يرجى إدخال رقم المرجع الثاني الخاص بعملية السداد");
                    if (pay.PayDate2 is null) throw new ApiException("يرجى إدخال تاريخ الحوالة البنكية الثانية / السداد");
                    if (pay.Amount1 <= 0) throw new ApiException("يرجى إدخال قيمة الحوالة الأولى");
                    if (pay.Amount2 <= 0) throw new ApiException("يرجى إدخال قيمة الحوالة الثانية");
                    if (Math.Round(pay.Amount1 + pay.Amount2, 2) != paySub) throw new ApiException("مبالغ الحوالات لا تساوي إجمالي الفاتورة");
                }
            }

            /* ---------- Attachments (ChkAttach) ---------- */
            if (checkAttachments) attachments.EnsureUploaded(st.NationalId ?? "", i.RegType, i.ChkContract, cashName, pay?.Split == true ? cashName2 : null);

            return new RegPlan
            {
                Student = new RegPlanStudent(st.AccountGuid, st.StudentName, st.NationalId), Setting = s, BatchName = batchName, CourseDiplomName = courseDiplomName,
                Diploms = dips, Fees = fees, FTotal = fTotal, FTax = fTax, FSub = fSub, MainDiplom = mainDiplom, Pay = pay,
                PayLines = payLines.Select(l => new RegPlanPayLine(l.Item, l.Cost, l.Tax, l.Total)).ToList(), PaySub = paySub, CashName = cashName, CashName2 = cashName2, Plan = plan,
            };
    }

    /// <summary>Writes the registration form, its journal and (if the book needs it) the down-payment invoice + receipt.</summary>
    public async Task<RegistrationSaved> PostAsync(DbConnection c, DbTransaction tx, RegistrationInput i, RegPlan p, Guid actor)
        => (await PostFormAsync(c, tx, i, p, actor, postPayment: true)).Saved;

    /// <summary>
    /// Writes the registration form and its journal. With <paramref name="postPayment"/> false (web review workflow) the
    /// down payment is NOT posted: it is returned so it can become a طلب سداد, posted later by PostDownPaymentAsync.
    /// </summary>
    public async Task<(RegistrationSaved Saved, DownPayment? Down)> PostFormAsync(DbConnection c, DbTransaction tx, RegistrationInput i, RegPlan p, Guid actor, bool postPayment)
    {
            var st = p.Student; var s = p.Setting; var batchName = p.BatchName; var courseDiplomName = p.CourseDiplomName;
            var dips = p.Diploms; var fees = p.Fees; var mainDiplom = p.MainDiplom; var pay = p.Pay;
            decimal fTotal = p.FTotal, fTax = p.FTax, fSub = p.FSub;
            var payLines = p.PayLines.Select(l => (l.Item, l.Cost, l.Tax, l.Total)).ToList();

            var regTypeGuid = await TypeAsync(c, tx, Form);
            var noteDip = i.RegType == 0 ? mainDiplom.Name : courseDiplomName ?? mainDiplom.Name;

            /* ---------- ADDREGDOC + diplomas + fees ---------- */
            var rp = new DynamicParameters(new
            {
                RegDate = i.RegDate, DocGuid = s.Guid, BatchGuid = i.RegType == 0 ? i.BatchGuid : Guid.Empty, BranchGuid = s.BranchGuid, SellerGuid = i.SellerGuid,
                AccountGuid = st.AccountGuid, DocTotal = (double)fTotal, DocTax = (double)fTax, DocSubTotal = (double)fSub, Notes = Cut(i.Notes?.Trim() ?? "", 250),
                IsPay = false, PayGuid = Guid.Empty, PayCode = "", LevelGuid = i.RegType == 0 ? i.LevelGuid : Guid.Empty, RegType = i.RegType,
                CourseGuid = i.RegType == 1 ? i.CourseGuid : Guid.Empty, i.ChkContract,
                ContractDate = i.ChkContract ? i.RegDate : (DateTime?)null, RemainingDays = i.ChkContract ? (int?)null : 30,
            });
            rp.Add("CodeOut", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await c.ExecuteAsync("dbo.AddRegDoc", rp, tx, commandType: CommandType.StoredProcedure);
            var regCode = rp.Get<int>("CodeOut").ToString();
            var regGuid = await c.ExecuteScalarAsync<Guid>("SELECT Guid FROM dbo.RegDoc WHERE Code = @regCode", new { regCode }, tx);
            foreach (var d in dips)
                await c.ExecuteAsync("dbo.AddRegDocDiplom", new { RegDocGuid = regGuid, d.DiplomGuid, Cost = (double)d.Cost, Tax = (double)d.Tax, SubTotal = (double)d.Total, DiplomType = 0, Type_ = 0 }, tx, commandType: CommandType.StoredProcedure);
            foreach (var f in fees)
                await c.ExecuteAsync("dbo.AddRegDocDiplom", new { RegDocGuid = regGuid, f.DiplomGuid, Cost = (double)f.Cost, Tax = (double)f.Tax, SubTotal = (double)f.Total, DiplomType = 1, Type_ = 0 }, tx, commandType: CommandType.StoredProcedure);

            var regNotes = $"تسجيل الطالب {st.StudentName}\r\n {s.DocName}\r\n {batchName}\r\n {noteDip}" + (p.Plan is { } pl ? $"\r\n باقة {pl.Name}" : "");
            var regCost = s.CostCenterGuid ?? Guid.Empty;
            await SalesPoster.JournalAsync(c, tx, regTypeGuid, regGuid, regCode, s.BranchGuid, i.RegDate, regNotes,
            [
                new SalesPoster.JLine(s.ChkMaden ? st.AccountGuid : s.MadenGuid ?? throw new ApiException("دفتر التسجيل بدون حساب مدين"), fSub, 0, regNotes, regCost),
                new SalesPoster.JLine(s.ChkDaen ? st.AccountGuid : s.DaenGuid ?? throw new ApiException("دفتر التسجيل بدون حساب دائن"), 0, fSub, regNotes, regCost),
            ]);

            string? billCode = null, taxNo = null, receiptCode = null, receiptCode2 = null;
            Guid? billGuid = null;
            DownPayment? down = null;
            if (pay != null)
            {
                down = new DownPayment
                {
                    RegDocGuid = regGuid, RegCode = regCode, StudentAccount = st.AccountGuid, StudentName = st.StudentName, DocName = s.DocName, NoteDip = noteDip,
                    PayDocGuid = s.DocPayGuid!.Value, ChkVat = s.ChkVat, VatAccount = s.VatAccountGuid, RegTax = fTax, Pay = pay,
                    Lines = payLines.Select(l => new InvoiceLine(l.Item.DiplomGuid, l.Item.DiplomType, l.Item.Name, l.Cost, l.Tax, l.Total)).ToList(),
                };
                if (postPayment)
                {
                    var posted = await PostDownPaymentAsync(c, tx, down, actor, i.RegDate);
                    (billCode, taxNo, receiptCode, receiptCode2, billGuid) = (posted.BillCode, posted.TaxNo, posted.ReceiptCode, posted.ReceiptCode2, posted.BillGuid);
                    down = null;
                }
            }

            if (p.Plan is { } plan)
                await c.ExecuteAsync("""
                    INSERT dbo.WebRegPlan (RegDocGuid, PlanGuid, PlanName, Months, Price, DownPayment, Installment, FirstDue)
                    VALUES (@regGuid, @PlanGuid, @Name, @Months, @Price, @DownPayment, @Installment, @firstDue)
                    """, new { regGuid, plan.PlanGuid, plan.Name, plan.Months, plan.Price, plan.DownPayment, plan.Installment,
                               firstDue = plan.Months > 0 ? i.RegDate.Date.AddMonths(1) : (DateTime?)null }, tx);

            await audit.WriteAsync(c, tx, Form, regGuid, UserActionLog.New, regCode);
            return (new RegistrationSaved("تم التسجيل بنجاح", regCode, regGuid, billCode, billGuid, taxNo, receiptCode, receiptCode2), down);
    }

    /// <summary>The checks of the down-payment data (VALIDATE_DATA of the payment part) + the form's state. Returns the box names.</summary>
    public static async Task<(string CashName, string? CashName2)> CheckDownPaymentAsync(DbConnection c, DbTransaction tx, DownPayment d)
    {
        var r = await c.QueryFirstOrDefaultAsync<(string Code, bool IstUse, bool IsReReg, bool IsPay)>(
            "SELECT CAST(Code AS nvarchar(50)), CAST(ISNULL(IstUse,1) AS bit), CAST(ISNULL(IsReReg,0) AS bit), CAST(ISNULL(IsPay,0) AS bit) FROM dbo.RegDoc WITH (UPDLOCK, HOLDLOCK) WHERE Guid = @g",
            new { g = d.RegDocGuid }, tx);
        if (r.Code is null) throw new ApiException("استمارة التسجيل الخاصة بالدفعة المقدمة غير موجودة");
        if (!r.IstUse) throw new ApiException($"استمارة التسجيل رقم {r.Code} ملغاة");
        if (r.IsReReg) throw new ApiException($"استمارة التسجيل رقم {r.Code} عليها مرتجع، لا يمكن تحصيل دفعتها المقدمة");
        if (r.IsPay) throw new ApiException($"الدفعة المقدمة لاستمارة التسجيل رقم {r.Code} مسددة بالفعل");
        var pay = d.Pay;
        if (pay.CashAccountGuid is null) throw new ApiException("يرجى تحديد الخزينة / البنك أولاً");
        var cashName = await c.ExecuteScalarAsync<string?>("SELECT CashName FROM dbo.CashBox WHERE AccountGuid = @a AND IsUse = 1 AND UseInSales = 1", new { a = pay.CashAccountGuid }, tx)
                       ?? throw new ApiException("الخزينة / البنك غير متاح لفواتير المبيعات");
        if (pay.CostCenterGuid is null) throw new ApiException("يرجى اختيار مركز التكلفة الخاص بعملية السداد");
        if (string.IsNullOrWhiteSpace(pay.Ref)) throw new ApiException("يرجى إدخال رقم المرجع الخاص بعملية السداد");
        if (pay.PayDate is null) throw new ApiException("يرجى إدخال تاريخ الحوالة البنكية / السداد");
        string? cashName2 = null;
        if (pay.Split)
        {
            cashName2 = await c.ExecuteScalarAsync<string?>("SELECT CashName FROM dbo.CashBox WHERE AccountGuid = @a AND IsUse = 1 AND UseInSales = 1", new { a = pay.CashAccountGuid2 }, tx)
                        ?? throw new ApiException("الخزينة / البنك 2 غير متاح لفواتير المبيعات");
            if (pay.CostCenterGuid2 is null) throw new ApiException("يرجى اختيار مركز التكلفة الثاني الخاص بعملية السداد");
            if (string.IsNullOrWhiteSpace(pay.Ref2)) throw new ApiException("يرجى إدخال رقم المرجع الثاني الخاص بعملية السداد");
            if (pay.PayDate2 is null) throw new ApiException("يرجى إدخال تاريخ الحوالة البنكية الثانية / السداد");
            if (Math.Round(pay.Amount1 + pay.Amount2, 2) != Math.Round(d.Lines.Sum(l => l.Total), 2)) throw new ApiException("مبالغ الحوالات لا تساوي إجمالي الفاتورة");
        }
        return (cashName, cashName2);
    }

    /// <summary>The down-payment invoice + receipt(s) + journals + ZATCA queue of a registration form, then UpdatePayStaut.</summary>
    public async Task<InvoicePosted> PostDownPaymentAsync(DbConnection c, DbTransaction tx, DownPayment d, Guid actor, DateTime date)
    {
        await CheckDownPaymentAsync(c, tx, d);
        var pay = d.Pay;
        var posted = await poster.PostAsync(c, tx, new InvoicePosting
        {
            UserGuid = actor,
            StudentAccount = d.StudentAccount, Date = date, PayDocGuid = d.PayDocGuid, Lines = d.Lines,
            CashAccount = pay.CashAccountGuid!.Value, CostCenter = pay.CostCenterGuid!.Value, Ref = pay.Ref!, PayDate = pay.PayDate!.Value, Notes = pay.Notes,
            Split = pay.Split, Amount1 = pay.Amount1, Amount2 = pay.Amount2, CashAccount2 = pay.CashAccountGuid2, CostCenter2 = pay.CostCenterGuid2, Ref2 = pay.Ref2, PayDate2 = pay.PayDate2,
            ChkVat = d.ChkVat, VatAccount = d.VatAccount, WriteVatLine = d.ChkVat && d.RegTax != 0, // RegisterForm: ChkVat And TxtFTotal2 <> 0
            BillJournalNotes = $"فاتورة الدفعة المقدمة لحساب الطالب {d.StudentName}\r\n {d.DocName}\r\n {d.NoteDip}",
            ReceiptNotes = $"سداد دفعة مقدمة لحساب الطالب {d.StudentName}\r\n {d.DocName}\r\n {d.NoteDip}",
        });
        await c.ExecuteAsync("dbo.UpdatePayStaut", new { RegDocGuid = d.RegDocGuid, IsPay = true, PayGuid = posted.BillGuid, PayCode = posted.BillCode }, tx, commandType: CommandType.StoredProcedure);
        return posted;
    }

}
