using System.Data;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Control;

/*
 * شئون الطلاب / الإعدادات:
 *   قائمة الأسعار (EncodingPriceList, "addpricelist")   — PriceList + PriceListDiplom
 *   ضبط أعداد الدفعات (EncodingBatchCount, "addbatchcount") — DiplomCountSeat per branch + batch (≤ BatchStudentCount)
 *   قائمة الطلاب الجدد (NewStudentHome, "newstudenthome")  — LoadStudentForAccept; acceptance is AcceptanceService
 *   سندات الخصم (list for the "سند خصم" menu screen)
 */
public sealed class PriceLineIn { public Guid DiplomGuid { get; set; } public decimal Cost { get; set; } public decimal Total { get; set; } }
public sealed class PriceListInput
{
    public string ListName { get; set; } = "";
    public string? Notes { get; set; }
    public bool IsUse { get; set; } = true;
    public bool ChkOfferPrice { get; set; }
    public List<PriceLineIn> Lines { get; set; } = [];
    public string? Reason { get; set; }
}
public sealed class SeatLineIn { public Guid DiplomGuid { get; set; } public int Count { get; set; } }
public sealed class BatchSeatsInput { public Guid BranchGuid { get; set; } public Guid BatchGuid { get; set; } public List<SeatLineIn> Lines { get; set; } = []; public string? Reason { get; set; } }

public sealed class ControlService(Db db, UserActionLog audit)
{
    public const string PriceForm = "addpricelist";
    public const string SeatsForm = "addbatchcount";
    public const string NewStudentsForm = "newstudenthome";

    /* ================= قائمة الأسعار ================= */
    public async Task<object> PriceListsAsync(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync(new CommandDefinition("""
            SELECT P.Guid, P.Code, P.ListName, P.Notes, CAST(ISNULL(P.IsUse,1) AS bit) AS IsUse, CAST(ISNULL(P.ChkOfferPrice,0) AS bit) AS ChkOfferPrice,
                   (SELECT COUNT(*) FROM dbo.PriceListDiplom D WHERE D.PriceListGuid = P.Guid) AS Items,
                   (SELECT COUNT(*) FROM dbo.RegSetting R WHERE R.PriceListGuid = P.Guid) AS Books
            FROM dbo.PriceList P ORDER BY P.Code
            """, cancellationToken: ct));
    }

    /// <summary>LoadDiplom: every active diploma / course / fee with its VAT rate.</summary>
    public async Task<object> PriceItemsAsync(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync(new CommandDefinition("""
            SELECT Guid AS guid, Code AS code, DiplomName AS name, ISNULL(DiplomType,0) AS diplomType, CAST(ISNULL(TaxValue,0) AS decimal(18,2)) AS taxRate
            FROM dbo.Diplom WHERE ISNULL(IsUse,1) = 1 ORDER BY ISNULL(DiplomType,0), TRY_CONVERT(int, Code)
            """, cancellationToken: ct));
    }

    public async Task<object> PriceListAsync(Guid guid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var h = await c.QueryFirstOrDefaultAsync(new CommandDefinition(
            "SELECT Guid, Code, ListName, Notes, CAST(ISNULL(IsUse,1) AS bit) AS IsUse, CAST(ISNULL(ChkOfferPrice,0) AS bit) AS ChkOfferPrice FROM dbo.PriceList WHERE Guid = @guid",
            new { guid }, cancellationToken: ct)) ?? throw ApiException.NotFound("قائمة الأسعار غير موجودة");
        var lines = await c.QueryAsync(new CommandDefinition("""
            SELECT X.DiplomGuid AS diplomGuid, D.Code AS code, D.DiplomName AS name, ISNULL(X.DiplomType,0) AS diplomType,
                   CAST(ISNULL(X.DiplomCost,0) AS decimal(18,3)) AS cost, CAST(ISNULL(X.DiplomTaxVat,0) AS decimal(18,2)) AS taxRate,
                   CAST(ISNULL(X.DiplomTaxValue,0) AS decimal(18,3)) AS tax, CAST(ISNULL(X.DiplomSubTotal,0) AS decimal(18,3)) AS total
            FROM dbo.PriceListDiplom X JOIN dbo.Diplom D ON D.Guid = X.DiplomGuid WHERE X.PriceListGuid = @guid ORDER BY X.ID
            """, new { guid }, cancellationToken: ct));
        return new { head = h, lines };
    }

    public Task<object> SavePriceListAsync(Guid? guid, PriceListInput i, CancellationToken ct) => db.InTransactionAsync<object>(async (c, tx) =>
    {
        var name = (i.ListName ?? "").Trim();
        if (name.Length == 0) throw new ApiException("يرجى إدخال اسم قائمة الأسعار");
        if (name.Length > 100) throw new ApiException("اسم قائمة الأسعار طويل جداً");
        if (i.Lines.Count == 0) throw new ApiException("يرجى إضافة الدبلومات / الدورات إلى القائمة");
        if (i.Lines.Select(l => l.DiplomGuid).Distinct().Count() != i.Lines.Count) throw new ApiException("يوجد بند مكرر في القائمة");
        if (i.ChkOfferPrice)
        {
            var other = await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.PriceList WHERE ChkOfferPrice = 1 AND (@g IS NULL OR Guid <> @g)", new { g = guid }, tx);
            if (other > 0) throw new ApiException("توجد بالفعل قائمة مخصصة لعرض الأسعار");
        }
        var ids = i.Lines.Select(l => l.DiplomGuid).ToList();
        var items = (await c.QueryAsync<(Guid Guid, string DiplomName, int DiplomType, decimal Rate)>(
            "SELECT Guid, DiplomName, ISNULL(DiplomType,0), CAST(ISNULL(TaxValue,0) AS decimal(18,4)) FROM dbo.Diplom WHERE Guid IN @ids", new { ids }, tx)).ToDictionary(x => x.Guid);
        if (items.Count != ids.Count) throw new ApiException("أحد البنود غير موجود");

        string code;
        Guid listGuid;
        if (guid is { } g)
        {
            code = await c.ExecuteScalarAsync<string?>("SELECT CAST(Code AS nvarchar(20)) FROM dbo.PriceList WITH (UPDLOCK) WHERE Guid = @g", new { g }, tx) ?? throw ApiException.NotFound("قائمة الأسعار غير موجودة");
            if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب التعديل");
            listGuid = g;
            await c.ExecuteAsync("dbo.UpdatePriceList", new { Guid = g, ListName = name, Notes = (i.Notes ?? "").Trim(), i.IsUse, i.ChkOfferPrice }, tx, commandType: CommandType.StoredProcedure);
            await c.ExecuteAsync("dbo.DeletePriceListDiplom", new { PriceListGuid = g }, tx, commandType: CommandType.StoredProcedure);
        }
        else
        {
            var id = await c.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(ID),0) + 1 FROM dbo.PriceList WITH (UPDLOCK, HOLDLOCK)", transaction: tx);
            code = id.ToString();
            await c.ExecuteAsync("dbo.AddPriceList", new { ID = id, Code = id, ListName = name, Notes = (i.Notes ?? "").Trim(), i.IsUse, i.ChkOfferPrice }, tx, commandType: CommandType.StoredProcedure);
            listGuid = await c.ExecuteScalarAsync<Guid>("SELECT Guid FROM dbo.PriceList WHERE ID = @id", new { id }, tx);
        }
        foreach (var l in i.Lines)
        {
            var d = items[l.DiplomGuid];
            if (l.Cost < 0 || l.Total < 0) throw new ApiException($"لا يمكن إدخال سعر بالسالب ({d.DiplomName})");
            // DiplomGrid_CellEndEdit: total = cost + cost × rate. Accept either side, but they must agree.
            var tax = Math.Round(l.Total - l.Cost, 3);
            if (Math.Abs(tax - l.Cost * d.Rate / 100) > 0.05m) throw new ApiException($"قيمة الضريبة غير متوافقة مع نسبة ضريبة البند ({d.DiplomName})");
            await c.ExecuteAsync("dbo.AddPriceListDiplom", new
            {
                l.DiplomGuid, DiplomCost = (double)Math.Round(l.Cost, 3), DiplomTaxVat = (double)d.Rate, DiplomTaxValue = (double)tax,
                DiplomSubTotal = (double)Math.Round(l.Total, 3), PriceListGuid = listGuid, DiplomType = d.DiplomType,
            }, tx, commandType: CommandType.StoredProcedure);
        }
        await audit.WriteAsync(c, tx, PriceForm, listGuid, guid is null ? UserActionLog.New : UserActionLog.Update, code, i.Reason?.Trim());
        return new { message = guid is null ? "تم حفظ قائمة الأسعار بنجاح" : "تم تعديل قائمة الأسعار بنجاح", guid = listGuid, code };
    }, IsolationLevel.Serializable, ct);

    /* ================= ضبط أعداد الدفعات ================= */
    public async Task<object> BatchesAsync(Guid? branch, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync(new CommandDefinition("""
            SELECT B.Guid AS guid, B.BatchName AS name, CAST(B.Code AS nvarchar(20)) AS code,
                   ISNULL((SELECT TOP 1 C.StudentCount FROM dbo.BatchStudentCount C WHERE C.BatchGuid = B.Guid AND C.BranchGuid = @branch), 0) AS capacity
            FROM dbo.Batch B WHERE ISNULL(B.IsUse,1) = 1 ORDER BY TRY_CONVERT(int, B.Code) DESC
            """, new { branch }, cancellationToken: ct));
    }

    public async Task<object> BatchSeatsAsync(Guid branch, Guid batch, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var capacity = await c.ExecuteScalarAsync<int?>(new CommandDefinition("dbo.WhatBAtchCount", new { BranchGuid = branch, BatchGuid = batch }, commandType: CommandType.StoredProcedure, cancellationToken: ct)) ?? 0;
        var lines = await c.QueryAsync(new CommandDefinition("""
            SELECT D.Guid AS diplomGuid, D.Code AS code, D.DiplomName AS name, ISNULL(S.DiplomCount, 0) AS count,
                   (SELECT COUNT(rd.Code) FROM dbo.RegDoc rd JOIN dbo.RegDocDiplom rdd ON rd.Guid = rdd.RegDocGuid
                     WHERE rdd.DiplomGuid = D.Guid AND rd.RegType = 0 AND rd.DocSubTotal <> 0 AND rd.IsReReg = 0 AND rd.BatchGuid = @batch AND rd.BranchGuid = @branch) AS registered
            FROM dbo.Diplom D LEFT JOIN dbo.DiplomCountSeat S ON S.DiplomGuid = D.Guid AND S.BranchGuid = @branch AND S.BatchGuid = @batch
            WHERE (ISNULL(D.IsUse,1) = 1 AND ISNULL(D.DiplomType,0) = 0 AND ISNULL(D.StudyType,0) = 0) OR S.DiplomGuid IS NOT NULL
            ORDER BY TRY_CONVERT(int, D.Code)
            """, new { branch, batch }, cancellationToken: ct));
        return new { capacity, lines };
    }

    public Task<object> SaveBatchSeatsAsync(BatchSeatsInput i, CancellationToken ct) => db.InTransactionAsync<object>(async (c, tx) =>
    {
        if (i.BranchGuid == Guid.Empty) throw new ApiException("يرجى اختيار الفرع أولاً");
        if (i.BatchGuid == Guid.Empty) throw new ApiException("يرجى اختيار الدفعة أولاً");
        if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب التعديل");
        var batchCode = await c.ExecuteScalarAsync<string?>("SELECT CAST(Code AS nvarchar(20)) FROM dbo.Batch WHERE Guid = @g", new { g = i.BatchGuid }, tx) ?? throw new ApiException("الدفعة غير موجودة");
        var capacity = await c.ExecuteScalarAsync<int?>("dbo.WhatBAtchCount", new { i.BranchGuid, i.BatchGuid }, tx, commandType: CommandType.StoredProcedure) ?? 0;
        if (i.Lines.Any(l => l.Count < 0)) throw new ApiException("لا يمكن إدخال عدد بالسالب");
        var total = i.Lines.Sum(l => l.Count);
        if (total > capacity) throw new ApiException($"مجموع المقاعد الموزعة ({total}) أكبر من عدد مقاعد الدفعة في الفرع ({capacity})");
        await c.ExecuteAsync("dbo.DeleteDiplomCountSeat", new { i.BranchGuid, i.BatchGuid }, tx, commandType: CommandType.StoredProcedure);
        foreach (var l in i.Lines.Where(l => l.Count > 0).GroupBy(l => l.DiplomGuid).Select(x => x.First()))
            await c.ExecuteAsync("dbo.AddDiplomCountSeat", new { i.BranchGuid, i.BatchGuid, l.DiplomGuid, DiplomCount = l.Count }, tx, commandType: CommandType.StoredProcedure);
        await audit.WriteAsync(c, tx, SeatsForm, i.BatchGuid, UserActionLog.Update, batchCode, i.Reason.Trim());
        return new { message = "تم حفظ توزيع المقاعد على الدبلومات بنجاح" };
    }, IsolationLevel.Serializable, ct);

    /* ================= قائمة الطلاب الجدد ================= */
    public async Task<IEnumerable<object>> NewStudentsAsync(Guid branch, int regType, string? name, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var rows = await c.QueryAsync(new CommandDefinition("dbo.LoadStudentForAccept",
            new { BranchGuid = branch, StudentName = string.IsNullOrWhiteSpace(name) ? null : name.Trim(), RegType = regType },
            commandType: CommandType.StoredProcedure, cancellationToken: ct));
        // One row per form (the proc returns one row per diploma line).
        return rows.Cast<IDictionary<string, object?>>().Select(x => new Dictionary<string, object?>(x, StringComparer.OrdinalIgnoreCase)).GroupBy(r => (Guid)r["RegDocGuid"]!).Select(g =>
        {
            var r = g.First();
            return (object)new
            {
                regDocGuid = g.Key, code = r["Code"]?.ToString(), regDate = r["RegDate"], studentName = r["StudentName"]?.ToString(), tel = r["StudentTel"]?.ToString(),
                nationalId = r["NationalId"]?.ToString(), branchName = r["BreName"]?.ToString(), batchName = r["BatchName"]?.ToString(), levelName = r["LevelName"]?.ToString(),
                courseName = r["CourseName"]?.ToString(), sellerName = r["ManFullName"]?.ToString(), account = r["AccountGuid"],
                diploms = g.Select(x => new { diplomGuid = x["DiplomGuid"], diplomName = x["DiplomName"]?.ToString() }).ToList(),
            };
        }).ToList();
    }

    /* ================= سندات الخصم ================= */
    public async Task<object> DiscountsAsync(DateTime? from, DateTime? to, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync(new CommandDefinition("""
            SELECT X.Guid, CAST(X.Code AS nvarchar(20)) AS Code, X.ActionDate AS Date, S.Guid AS StudentGuid, S.StudentName, S.NationalId, T.DisName AS TypeName,
                   B.BrName AS BranchName, CAST(ISNULL(X.Amount,0) AS decimal(18,2)) AS Amount, X.ActionNotes AS Notes, CAST(ISNULL(X.IsUse,1) AS bit) AS IsUse
            FROM dbo.StudentDiscount X JOIN dbo.Student S ON S.AccountGuid = X.AccountGuid
            LEFT JOIN dbo.DisCountType T ON T.Guid = X.DiscountTypeGuid LEFT JOIN dbo.Branch B ON B.Guid = X.BranchGuid
            WHERE (@from IS NULL OR X.ActionDate >= @from) AND (@to IS NULL OR X.ActionDate < DATEADD(day, 1, @to))
            ORDER BY X.ActionDate DESC, X.Id DESC
            """, new { from, to }, cancellationToken: ct));
    }
}
