using System.Data;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Books;

/*
 * إعدادات التسجيل / دفاتر المستندات (EncodingRegSetting, form "regsetting").
 * A book (RegSetting) = document type (Form_Name with D_Seting = 1) + branch + price list + posting rules
 * (ChkDaily: posts a journal; Maden/Daen = the student (Chk) or a fixed account; VAT account; cost center)
 * + registration extras (down payment book, minimum amount, other fees) + the users allowed to use it (RegSettingUser).
 */
public sealed class DocBookInput
{
    public Guid DocTypeGuid { get; set; }
    public string DocName { get; set; } = "";
    public Guid? BranchGuid { get; set; }
    public Guid? PriceListGuid { get; set; }
    public bool IsUse { get; set; } = true;
    public bool ChkDaily { get; set; }
    public bool ChkMaden { get; set; }
    public Guid? MadenGuid { get; set; }
    public bool ChkDaen { get; set; }
    public Guid? DaenGuid { get; set; }
    public bool ChkVat { get; set; }
    public Guid? VatAccountGuid { get; set; }
    public Guid? CostCenterGuid { get; set; }
    public bool ChkOtherFees { get; set; }
    public bool ChkStartPay { get; set; }
    public bool ChkAmountPay { get; set; }
    public decimal AmountPay { get; set; }
    public Guid? DocPayGuid { get; set; }
    /// <summary>addregdoc: 0 دبلوم / 1 دورة.</summary>
    public int? RegType { get; set; }
    /// <summary>salesinvoice: 0 دفعة مقدمة / 1 قسط شهري / 2 سداد رسوم.</summary>
    public int? InvoiceType { get; set; }
    /// <summary>regfess: 1 رسوم دراسية / 2 رسوم إدارية.</summary>
    public int? FessType { get; set; }
    public List<Guid> Users { get; set; } = [];
    public string? Reason { get; set; }
}

public sealed class DocBookService(Db db, UserActionLog audit)
{
    public const string Form = "regsetting";

    public async Task<object> LookupsAsync(CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return new
        {
            types = await c.QueryAsync(new CommandDefinition("SELECT Guid AS guid, Name AS name, UnqName AS unq FROM dbo.Form_Name WHERE D_Seting = 1 AND IsUse = 1 ORDER BY Id", cancellationToken: ct)),
            priceLists = await c.QueryAsync(new CommandDefinition("SELECT Guid AS guid, ListName AS name, CAST(Code AS nvarchar(20)) AS code FROM dbo.PriceList WHERE ISNULL(IsUse,1) = 1 ORDER BY Code", cancellationToken: ct)),
            users = await c.QueryAsync(new CommandDefinition("SELECT Guid AS guid, FullName AS name, UserName AS userName FROM dbo.User_Info WHERE ISNULL(Staut_,1) = 1 ORDER BY FullName", cancellationToken: ct)),
            payBooks = await c.QueryAsync(new CommandDefinition("""
                SELECT R.Guid AS guid, R.DocName AS name, R.BranchGuid AS branchGuid FROM dbo.RegSetting R JOIN dbo.Form_Name F ON F.Guid = R.DocTypeGuid
                WHERE F.UnqName = 'salesinvoice' AND R.IsUse = 1 ORDER BY R.Code
                """, cancellationToken: ct)),
        };
    }

    public async Task<IEnumerable<dynamic>> ListAsync(Guid? type, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync(new CommandDefinition("""
            SELECT R.Guid, R.Code, R.DocName, F.Name AS TypeName, F.UnqName, B.BrName AS BranchName, P.ListName AS PriceListName,
                   CAST(ISNULL(R.IsUse,1) AS bit) AS IsUse, CAST(ISNULL(R.ChkDaily,0) AS bit) AS ChkDaily, CAST(ISNULL(R.ChkVat,0) AS bit) AS ChkVat,
                   (SELECT COUNT(*) FROM dbo.RegSettingUser U WHERE U.RegSettingGuid = R.Guid) AS Users
            FROM dbo.RegSetting R JOIN dbo.Form_Name F ON F.Guid = R.DocTypeGuid
            LEFT JOIN dbo.Branch B ON B.Guid = R.BranchGuid LEFT JOIN dbo.PriceList P ON P.Guid = R.PriceListGuid
            WHERE (@type IS NULL OR R.DocTypeGuid = @type)
            ORDER BY F.Id, R.Code
            """, new { type }, cancellationToken: ct));
    }

    public async Task<object> GetAsync(Guid guid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var h = await c.QueryFirstOrDefaultAsync(new CommandDefinition("""
            SELECT R.Guid, R.Code, R.DocName, R.DocTypeGuid, F.Name AS TypeName, F.UnqName, R.BranchGuid, R.PriceListGuid,
                   CAST(ISNULL(R.IsUse,1) AS bit) AS IsUse, CAST(ISNULL(R.ChkDaily,0) AS bit) AS ChkDaily,
                   CAST(ISNULL(R.ChkMaden,0) AS bit) AS ChkMaden, R.MadenGuid, MA.AccountName AS MadenName, CAST(MA.Code AS nvarchar(50)) AS MadenCode,
                   CAST(ISNULL(R.ChkDaen,0) AS bit) AS ChkDaen, R.DaenGuid, DA.AccountName AS DaenName, CAST(DA.Code AS nvarchar(50)) AS DaenCode,
                   CAST(ISNULL(R.ChkVat,0) AS bit) AS ChkVat, R.VatAccountGuid, VA.AccountName AS VatName, CAST(VA.Code AS nvarchar(50)) AS VatCode,
                   R.CostCenterGuid, CC.CenterName AS CostCenterName,
                   CAST(ISNULL(R.ChkOtherFees,0) AS bit) AS ChkOtherFees, CAST(ISNULL(R.ChkStartPay,0) AS bit) AS ChkStartPay,
                   CAST(ISNULL(R.ChkAmountPay,0) AS bit) AS ChkAmountPay, CAST(ISNULL(R.AmountPay,0) AS decimal(18,2)) AS AmountPay, R.DocPayGuid,
                   R.RegType, R.InvoiceType, R.FessType
            FROM dbo.RegSetting R JOIN dbo.Form_Name F ON F.Guid = R.DocTypeGuid
            LEFT JOIN dbo.Account MA ON MA.Guid = R.MadenGuid LEFT JOIN dbo.Account DA ON DA.Guid = R.DaenGuid LEFT JOIN dbo.Account VA ON VA.Guid = R.VatAccountGuid
            LEFT JOIN dbo.CostCenter CC ON CC.Guid = R.CostCenterGuid
            WHERE R.Guid = @guid
            """, new { guid }, cancellationToken: ct)) ?? throw ApiException.NotFound("الدفتر غير موجود");
        var users = await c.QueryAsync<Guid>(new CommandDefinition("SELECT UserGuid FROM dbo.RegSettingUser WHERE RegSettingGuid = @guid", new { guid }, cancellationToken: ct));
        return new { head = h, users };
    }

    private static Guid G(Guid? g) => g ?? Guid.Empty;

    public Task<object> SaveAsync(Guid? guid, DocBookInput i, CancellationToken ct) => db.InTransactionAsync<object>(async (c, tx) =>
    {
        /* VALIDATE_DATA */
        var name = (i.DocName ?? "").Trim();
        if (name.Length == 0) throw new ApiException("يرجى إدخال اسم الدفتر");
        if (name.Length > 100) throw new ApiException("اسم الدفتر طويل جداً");
        if (i.PriceListGuid is null) throw new ApiException("يرجى اختيار قائمة الأسعار");
        if (i.BranchGuid is null) throw new ApiException("يرجى اختيار الفرع");
        if (i.ChkDaily)
        {
            if (!i.ChkMaden && i.MadenGuid is null) throw new ApiException("يرجى اختيار الجانب المدين");
            if (!i.ChkDaen && i.DaenGuid is null) throw new ApiException("يرجى اختيار الجانب الدائن");
            if (i.ChkVat && i.VatAccountGuid is null) throw new ApiException("يرجى اختيار حساب الضريبة");
            if (i.CostCenterGuid is null) throw new ApiException("يرجى اختيار مركز التكلفة");
        }
        if (i.ChkStartPay && i.DocPayGuid is null) throw new ApiException("يرجى اختيار دفتر فاتورة الدفعة المقدمة");
        if (i.ChkAmountPay && i.AmountPay <= 0) throw new ApiException("يرجى إدخال الحد الأدنى للدفعة المقدمة");

        string unq;
        Guid docType;
        string code = "";
        if (guid is { } g)
        {
            var o = await c.QueryFirstOrDefaultAsync<(Guid DocTypeGuid, int Code)>("SELECT DocTypeGuid, Code FROM dbo.RegSetting WITH (UPDLOCK) WHERE Guid = @g", new { g }, tx);
            if (o.DocTypeGuid == Guid.Empty) throw ApiException.NotFound("الدفتر غير موجود");
            if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب التعديل");
            docType = o.DocTypeGuid; code = o.Code.ToString();
        }
        else docType = i.DocTypeGuid;
        unq = await c.ExecuteScalarAsync<string?>("SELECT UnqName FROM dbo.Form_Name WHERE Guid = @t AND D_Seting = 1", new { t = docType }, tx)
              ?? throw new ApiException("يرجى اختيار نوع الدفتر");

        // CmdFindDocType: the type-specific choices only apply to their own document type.
        int regType = unq == "addregdoc" ? i.RegType is 0 or 1 ? i.RegType.Value : throw new ApiException("يرجى اختيار نوع التسجيل (دبلوم / دورة)") : 2;
        int invoiceType = unq == "salesinvoice" ? i.InvoiceType is >= 0 and <= 2 ? i.InvoiceType.Value : throw new ApiException("يرجى اختيار نوع الفاتورة") : 3;
        int fessType = unq == "regfess" ? i.FessType is 1 or 2 ? i.FessType.Value : throw new ApiException("يرجى اختيار نوع الرسوم") : 0;
        if (i.ChkStartPay)
        {
            var ok = await c.ExecuteScalarAsync<int>("""
                SELECT COUNT(*) FROM dbo.RegSetting R JOIN dbo.Form_Name F ON F.Guid = R.DocTypeGuid
                WHERE R.Guid = @p AND F.UnqName = 'salesinvoice' AND R.BranchGuid = @b
                """, new { p = i.DocPayGuid, b = i.BranchGuid }, tx);
            if (ok == 0) throw new ApiException("دفتر الدفعة المقدمة يجب أن يكون دفتر فاتورة مبيعات في نفس الفرع");
        }
        var users = i.Users.Distinct().ToList();
        if (users.Count > 0)
        {
            var n = await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.User_Info WHERE Guid IN @users", new { users }, tx);
            if (n != users.Count) throw new ApiException("أحد المستخدمين المختارين غير موجود");
        }

        var p = new
        {
            DocName = name, PriceListGuid = i.PriceListGuid, BranchGuid = i.BranchGuid, i.ChkDaily,
            ChkMaden = i.ChkDaily && i.ChkMaden, MadenGuid = i.ChkDaily && !i.ChkMaden ? G(i.MadenGuid) : Guid.Empty,
            ChkDaen = i.ChkDaily && i.ChkDaen, DaenGuid = i.ChkDaily && !i.ChkDaen ? G(i.DaenGuid) : Guid.Empty,
            CostCenterGuid = i.ChkDaily ? G(i.CostCenterGuid) : Guid.Empty, i.IsUse,
            chkotherfees = i.ChkOtherFees, i.ChkStartPay, ChkAmountPay = i.ChkStartPay && i.ChkAmountPay,
            AmountPay = (double)(i.ChkStartPay && i.ChkAmountPay ? i.AmountPay : 0), DocPayGuid = i.ChkStartPay ? G(i.DocPayGuid) : Guid.Empty,
            RegType = regType, VatAccountGuid = i.ChkDaily && i.ChkVat ? G(i.VatAccountGuid) : Guid.Empty, ChkVat = i.ChkDaily && i.ChkVat,
            InvoiceType = invoiceType, FessType = fessType,
        };
        Guid bookGuid;
        if (guid is null)
        {
            // GET___NO: next number within the document type.
            var id = await c.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(ID),0) + 1 FROM dbo.RegSetting WITH (UPDLOCK, HOLDLOCK) WHERE DocTypeGuid = @docType", new { docType }, tx);
            code = id.ToString();
            var dp = new DynamicParameters(p);
            dp.Add("ID", id); dp.Add("Code", id); dp.Add("DocTypeGuid", docType);
            await c.ExecuteAsync("dbo.AddRegSetting", dp, tx, commandType: CommandType.StoredProcedure);
            bookGuid = await c.ExecuteScalarAsync<Guid>("SELECT TOP 1 Guid FROM dbo.RegSetting WHERE DocTypeGuid = @docType AND Code = @id ORDER BY ID DESC", new { docType, id }, tx);
        }
        else
        {
            bookGuid = guid.Value;
            var dp = new DynamicParameters(p);
            dp.Add("Guid", bookGuid);
            await c.ExecuteAsync("dbo.UpdateRegSetting", dp, tx, commandType: CommandType.StoredProcedure);
            await c.ExecuteAsync("dbo.DeleteRegSettingUser", new { regSettingGuid = bookGuid }, tx, commandType: CommandType.StoredProcedure);
        }
        foreach (var u in users)
            await c.ExecuteAsync("dbo.AddRegSettingUser", new { RegSettingGuid = bookGuid, UserGuid = u }, tx, commandType: CommandType.StoredProcedure);
        await audit.WriteAsync(c, tx, Form, bookGuid, guid is null ? UserActionLog.New : UserActionLog.Update, code, i.Reason?.Trim());
        return new { message = guid is null ? $"تم حفظ بيانات الدفتر رقم {code} بنجاح" : "تم تعديل بيانات الدفتر بنجاح", guid = bookGuid, code };
    }, IsolationLevel.Serializable, ct);
}
