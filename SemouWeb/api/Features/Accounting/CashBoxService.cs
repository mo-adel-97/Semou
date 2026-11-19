using System.Data;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Accounting;

/// <summary>
/// EncodingCashBox (CashType 0) and EncodingBank (CashType 1).
/// Saving a new one first creates its ledger account under the parent configured in
/// إعدادات الحسابات (DepGuid 30 = خزائن, 40 = بنوك), exactly like the desktop.
/// </summary>
public sealed class CashBoxService(Db db, UserActionLog audit)
{
    public sealed record Kind(int CashType, int SettingDep, string Form, string Singular, string NameMsg, string SavedNew, string SavedEdit);
    public static readonly Kind CashBox = new(0, 30, "addcashbox", "الخزينة", "يرجى إدخال اسم الخزينة", "تم حفظ بيانات الخزينة بنجاح", "تم تعديل بيانات الخزينة بنجاح");
    public static readonly Kind Bank = new(1, 40, "addbank", "البنك", "يرجى إدخال اسم البنك", "تم حفظ بيانات البنك بنجاح", "تم تعديل بيانات البنك بنجاح");

    public sealed class Input
    {
        public string CashName { get; set; } = "";
        public string? AccountNumber { get; set; }
        public Guid? CostCenterGuid { get; set; }
        public bool IsUse { get; set; } = true;
        /// <summary>تستخدم في فواتير المبيعات.</summary>
        public bool UseInSales { get; set; }
        /// <summary>تستخدم في يومية الفرع.</summary>
        public bool IsPublic { get; set; }
        public string? Reason { get; set; }
    }

    public async Task<IEnumerable<dynamic>> ListAsync(Kind k, bool? isUse, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryAsync(new CommandDefinition("""
            SELECT CB.Code, CB.Guid, CB.CashName, CB.AccountNumber, CAST(ISNULL(CB.IsUse,0) AS bit) AS IsUse, CAST(ISNULL(CB.UseInSales,0) AS bit) AS UseInSales,
                   CAST(ISNULL(CB.IsPublic,0) AS bit) AS IsPublic, CC.CenterName, CAST(A.Code AS nvarchar(50)) AS AccountCode
            FROM dbo.CashBox CB LEFT JOIN dbo.CostCenter CC ON CC.Guid = CB.CostCenterGuid LEFT JOIN dbo.Account A ON A.Guid = CB.AccountGuid
            WHERE CB.CashType = @t AND (@isUse IS NULL OR CB.IsUse = @isUse)
            ORDER BY TRY_CONVERT(int, CB.Code)
            """, new { t = k.CashType, isUse }, cancellationToken: ct));
    }

    public async Task<object> ParentAsync(Kind k, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        var p = await c.QueryFirstOrDefaultAsync("""
            SELECT A.Guid, A.AccountName AS Name, CAST(A.Code AS nvarchar(50)) AS Code FROM dbo.AccountSetting S JOIN dbo.Account A ON A.Guid = S.AccountGuid WHERE S.DepGuid = @d ORDER BY S.ID
            """, new { d = k.SettingDep });
        return new { parent = p };
    }

    public async Task<object> GetAsync(Kind k, Guid guid, CancellationToken ct)
    {
        await using var c = await db.OpenAsync(ct);
        return await c.QueryFirstOrDefaultAsync(new CommandDefinition("""
            SELECT CB.Code, CB.Guid, CB.CashName, CB.AccountNumber, CAST(ISNULL(CB.IsUse,0) AS bit) AS IsUse, CAST(ISNULL(CB.UseInSales,0) AS bit) AS UseInSales,
                   CAST(ISNULL(CB.IsPublic,0) AS bit) AS IsPublic, CB.CostCenterGuid, CC.CenterName, CAST(A.Code AS nvarchar(50)) AS AccountCode, P.AccountName AS ParentName
            FROM dbo.CashBox CB LEFT JOIN dbo.CostCenter CC ON CC.Guid = CB.CostCenterGuid
                 LEFT JOIN dbo.Account A ON A.Guid = CB.AccountGuid LEFT JOIN dbo.Account P ON P.Guid = A.ParentGuid
            WHERE CB.Guid = @guid AND CB.CashType = @t
            """, new { guid, t = k.CashType }, cancellationToken: ct)) ?? throw ApiException.NotFound($"{k.Singular} غير موجود");
    }

    private static void Validate(Kind k, Input i)
    {
        if (string.IsNullOrWhiteSpace(i.CashName)) throw new ApiException(k.NameMsg);
        if (i.CashName.Trim().Length > 150) throw new ApiException("الاسم طويل جداً");
        if (k == Bank && string.IsNullOrWhiteSpace(i.AccountNumber)) throw new ApiException("يرجى إدخال الحساب البنكي");
        if (k == Bank && i.CostCenterGuid is null) throw new ApiException("يرجى اختيار مركز التكلفة الافتراضي");
        if ((i.AccountNumber?.Length ?? 0) > 100) throw new ApiException("رقم الحساب طويل جداً");
    }

    public async Task<ChartSaved> CreateAsync(Kind k, Input i, CancellationToken ct)
    {
        Validate(k, i);
        return await db.InTransactionAsync(async (c, tx) =>
        {
            var parent = await c.QueryFirstOrDefaultAsync<(Guid Guid, string Code, int? EndAccount)>("""
                SELECT A.Guid, CAST(A.Code AS nvarchar(50)), A.EndAccount FROM dbo.AccountSetting S JOIN dbo.Account A WITH (UPDLOCK) ON A.Guid = S.AccountGuid WHERE S.DepGuid = @d ORDER BY S.ID
                """, new { d = k.SettingDep }, tx);
            if (parent.Code is null) throw new ApiException($"لم يتم تحديد الحساب الرئيسي لـ{k.Singular} في إعدادات الحسابات");

            // 1) ledger account (ADDACCOUNT): AccountKind = 1, AccountType = 1 (فرعي), Type_ = 2 (عام)
            var max = await c.ExecuteScalarAsync<long?>("SELECT MAX(CAST(BacicCode AS bigint)) FROM dbo.Account WITH (UPDLOCK, HOLDLOCK) WHERE ParentGuid = @p", new { p = parent.Guid }, tx);
            var basic = max.HasValue ? max.Value + 1 : 1 + long.Parse(parent.Code) * 100;
            var accCode = basic.ToString();
            await c.ExecuteAsync("dbo.AddBasicAccount", new
            {
                BacicCode = (int)basic, Code = accCode, AccountName = i.CashName.Trim(), AccountKind = true, AccountType = 1,
                EndAccount = parent.EndAccount ?? 0, ParentGuid = parent.Guid, IsShow = true, Type_ = 2,
            }, tx, commandType: CommandType.StoredProcedure);
            var accGuid = await c.ExecuteScalarAsync<Guid>("SELECT TOP 1 Guid FROM dbo.Account WHERE CAST(Code AS nvarchar(50)) = @accCode AND ParentGuid = @p", new { accCode, p = parent.Guid }, tx);

            // 2) cash box / bank (ADDCASHBOX)
            var id = await c.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(Id), 0) + 1 FROM dbo.CashBox WITH (UPDLOCK, HOLDLOCK)", transaction: tx);
            await c.ExecuteAsync("dbo.AddCashBox", new
            {
                Id = id, Code = id.ToString(), CashName = i.CashName.Trim(), AccountNumber = k == Bank ? i.AccountNumber!.Trim() : "",
                CashType = k.CashType, AccountGuid = accGuid, i.IsUse, CostCenterGuid = i.CostCenterGuid ?? Guid.Empty, i.UseInSales,
            }, tx, commandType: CommandType.StoredProcedure);
            var guid = await c.ExecuteScalarAsync<Guid>("SELECT Guid FROM dbo.CashBox WHERE Id = @id", new { id }, tx);
            // AddCashBox has no IsPublic parameter; apply it with the update proc so the flag isn't lost.
            if (i.IsPublic)
                await c.ExecuteAsync("dbo.UpdateCashBox", new
                {
                    Guid = guid, CashName = i.CashName.Trim(), AccountNumber = k == Bank ? i.AccountNumber!.Trim() : "", i.IsUse,
                    CostCenterGuid = i.CostCenterGuid ?? Guid.Empty, i.UseInSales, i.IsPublic,
                }, tx, commandType: CommandType.StoredProcedure);

            await audit.WriteAsync(c, tx, k.Form, guid, UserActionLog.New, id.ToString());
            return new ChartSaved(k.SavedNew, id.ToString(), guid);
        }, IsolationLevel.Serializable, ct);
    }

    public async Task<ChartSaved> UpdateAsync(Kind k, Guid guid, Input i, CancellationToken ct)
    {
        Validate(k, i);
        if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب الإجراء أولاً");
        return await db.InTransactionAsync(async (c, tx) =>
        {
            var cur = await c.QueryFirstOrDefaultAsync<(string Code, Guid? AccountGuid)>(
                "SELECT CAST(Code AS nvarchar(50)), AccountGuid FROM dbo.CashBox WHERE Guid = @guid AND CashType = @t", new { guid, t = k.CashType }, tx);
            if (cur.Code is null) throw ApiException.NotFound($"{k.Singular} غير موجود");

            await c.ExecuteAsync("dbo.UpdateCashBox", new
            {
                Guid = guid, CashName = i.CashName.Trim(), AccountNumber = k == Bank ? i.AccountNumber!.Trim() : "", i.IsUse,
                CostCenterGuid = i.CostCenterGuid ?? Guid.Empty, i.UseInSales, i.IsPublic,
            }, tx, commandType: CommandType.StoredProcedure);
            if (cur.AccountGuid is { } acc) // UPDATEACOUNT: keep the ledger account name/status in sync
                await c.ExecuteAsync("dbo.UpdateAccount", new { Guid = acc, AccountName = i.CashName.Trim(), i.IsUse }, tx, commandType: CommandType.StoredProcedure);

            await audit.WriteAsync(c, tx, k.Form, guid, UserActionLog.Update, cur.Code, i.Reason!.Trim());
            return new ChartSaved(k.SavedEdit, cur.Code, guid);
        }, ct: ct);
    }
}
