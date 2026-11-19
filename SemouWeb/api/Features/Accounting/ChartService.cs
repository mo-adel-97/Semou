using System.Data;
using System.Data.Common;
using Dapper;
using Semou.Api.Data;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Accounting;

/// <summary>
/// The two hierarchical charts of the desktop app:
///  - Accounts     (EncodingAccount + AccountHome)     table dbo.Account
///  - Cost centers (EncodingCostCenter + CostCenterHome) table dbo.CostCenter
/// Codes follow AllFiles.GETACCOUNTCODE / GETCOSTCENTERCODE: last sibling BasicCode + 1, or parentCode * 100 + 1 for the first child.
/// </summary>
public enum ChartKind { Accounts, CostCenters }

public sealed record TreeNodeDto(string Code, string Name, Guid Guid, Guid? ParentGuid, int? Kind, bool IsUse, int? Type_, int Children);

public sealed class OpenBalanceRow
{
    public Guid BranchGuid { get; set; }
    public decimal Maden { get; set; }
    public decimal Daen { get; set; }
    public string? Notes { get; set; }
}

public sealed class ChartNodeInput
{
    public string Name { get; set; } = "";
    /// <summary>Accounts: 0 رئيسي / 1 فرعي - معاملات. Cost centers: 0 رئيسي / 1 فرعي.</summary>
    public int? Kind { get; set; }
    public Guid? ParentGuid { get; set; }
    public bool IsUse { get; set; } = true;
    /// <summary>Accounts only: 0 إيراد / 1 مصروف / 2 عام.</summary>
    public int? Type_ { get; set; }
    public decimal Maden { get; set; }
    public decimal Daen { get; set; }
    public List<OpenBalanceRow> OpenBalances { get; set; } = [];
    /// <summary>Required on update.</summary>
    public string? Reason { get; set; }
}

public sealed record ChartSaved(string Message, string Code, Guid Guid);

public sealed class ChartService(Db db, UserActionLog audit)
{
    private sealed record Cfg(string Table, string NameCol, string KindCol, string BasicCol, string Form, string Singular, string KindMsg, string ParentMsg, string NameMsg, string SavedNew, string SavedEdit);

    private static Cfg Of(ChartKind k) => k == ChartKind.Accounts
        ? new("Account", "AccountName", "AccountType", "BacicCode", "addaccount", "الحساب",
              "يرجى اختيار نوع الحساب ", "يرجى اختيار الحساب الرئيسي", "يرجى إدخال اسم الحساب", "تم حفظ بيانات الحساب بنجاح", "تم تعديل بيانات الحساب بنجاح")
        : new("CostCenter", "CenterName", "CenterType", "BasicCode", "addcostcenter", "مركز التكلفة",
              "يرجى اختيار نوع مركز التكلفة ", "يرجى اختيار مركز التكلفة الرئيسي", "يرجى إدخال اسم مركز التكلفة", "تم حفظ بيانات مركز التكلفة بنجاح", "تم تعديل بيانات مركز التكلفة بنجاح");

    public static string FormOf(ChartKind k) => Of(k).Form;

    /// <summary>ShowTreeAccount / ShowCenterTree (accounts with IsShow = 0 are hidden, like the desktop).</summary>
    public async Task<IEnumerable<TreeNodeDto>> TreeAsync(ChartKind k, CancellationToken ct)
    {
        var c = Of(k);
        var where = k == ChartKind.Accounts ? "WHERE ISNULL(T.IsShow, 1) = 1" : "";
        await using var cn = await db.OpenAsync(ct);
        return await cn.QueryAsync<TreeNodeDto>(new CommandDefinition($"""
            SELECT CAST(T.Code AS nvarchar(50)) AS Code, T.[{c.NameCol}] AS Name, T.Guid,
                   NULLIF(T.ParentGuid, '00000000-0000-0000-0000-000000000000') AS ParentGuid,
                   T.[{c.KindCol}] AS Kind, CAST(ISNULL(T.IsUse, 0) AS bit) AS IsUse,
                   {(k == ChartKind.Accounts ? "T.Type_" : "CAST(NULL AS int)")} AS Type_,
                   (SELECT COUNT(*) FROM dbo.[{c.Table}] X WHERE X.ParentGuid = T.Guid) AS Children
            FROM dbo.[{c.Table}] T {where}
            ORDER BY T.[{c.BasicCol}]
            """, cancellationToken: ct));
    }

    public async Task<object> GetAsync(ChartKind k, Guid guid, CancellationToken ct)
    {
        var c = Of(k);
        await using var cn = await db.OpenAsync(ct);
        var row = await cn.QueryFirstOrDefaultAsync(new CommandDefinition($"""
            SELECT CAST(T.Code AS nvarchar(50)) AS Code, T.Guid, T.[{c.NameCol}] AS Name, T.[{c.KindCol}] AS Kind, CAST(ISNULL(T.IsUse,0) AS bit) AS IsUse,
                   P.Guid AS ParentGuid, P.[{c.NameCol}] AS ParentName, CAST(P.Code AS nvarchar(50)) AS ParentCode
                   {(k == ChartKind.Accounts ? ", T.Type_, CAST(ISNULL(T.DEBIT,0) AS decimal(18,2)) AS Maden, CAST(ISNULL(T.CREDIT,0) AS decimal(18,2)) AS Daen" : "")}
            FROM dbo.[{c.Table}] T LEFT JOIN dbo.[{c.Table}] P ON P.Guid = T.ParentGuid
            WHERE T.Guid = @guid
            """, new { guid }, cancellationToken: ct)) ?? throw ApiException.NotFound($"{c.Singular} غير موجود");

        IEnumerable<OpenBalanceRow> balances = [];
        if (k == ChartKind.Accounts)
            balances = await cn.QueryAsync<OpenBalanceRow>(new CommandDefinition(
                "SELECT BranchGuid, CAST(ISNULL(OpenMaden,0) AS decimal(18,2)) AS Maden, CAST(ISNULL(OpenDaen,0) AS decimal(18,2)) AS Daen, OpenNotes AS Notes FROM dbo.AccountopenBalance WHERE AccountGuid = @guid ORDER BY ID",
                new { guid }, cancellationToken: ct));
        return new { node = row, openBalances = balances };
    }

    private static void Validate(Cfg c, ChartNodeInput i, bool isNew)
    {
        if (isNew && i.Kind is not (0 or 1)) throw new ApiException(c.KindMsg.Trim());
        if (isNew && i.ParentGuid is null) throw new ApiException(c.ParentMsg);
        if (string.IsNullOrWhiteSpace(i.Name)) throw new ApiException(c.NameMsg);
        if (i.Name.Trim().Length > 150) throw new ApiException("الاسم طويل جداً (الحد الأقصى 150 حرفاً)");
        if (i.Type_ is not null and not (0 or 1 or 2)) throw new ApiException("نوع الحساب غير صحيح");
        foreach (var b in i.OpenBalances)
            if (b.Maden < 0 || b.Daen < 0) throw new ApiException("الأرصدة الافتتاحية لا يمكن أن تكون بالسالب");
    }

    public async Task<ChartSaved> CreateAsync(ChartKind k, ChartNodeInput i, CancellationToken ct)
    {
        var c = Of(k);
        Validate(c, i, isNew: true);
        return await db.InTransactionAsync(async (cn, tx) =>
        {
            var parent = await cn.QueryFirstOrDefaultAsync<(string Code, int? Kind, int? EndAccount)>(
                $"SELECT CAST(Code AS nvarchar(50)), [{c.KindCol}], {(k == ChartKind.Accounts ? "EndAccount" : "CAST(NULL AS int)")} FROM dbo.[{c.Table}] WITH (UPDLOCK) WHERE Guid = @p",
                new { p = i.ParentGuid }, tx);
            if (parent.Code is null) throw new ApiException(c.ParentMsg);
            if (parent.Kind == 1) throw new ApiException($"لا يمكن الإضافة تحت {c.Singular} فرعي، اختر {c.Singular} رئيسي");

            // GETACCOUNTCODE / GETCOSTCENTERCODE
            var max = await cn.ExecuteScalarAsync<long?>($"SELECT MAX(CAST([{c.BasicCol}] AS bigint)) FROM dbo.[{c.Table}] WITH (UPDLOCK, HOLDLOCK) WHERE ParentGuid = @p", new { p = i.ParentGuid }, tx);
            var basic = max.HasValue ? max.Value + 1 : 1 + long.Parse(parent.Code) * 100;
            if (basic > int.MaxValue) throw new ApiException("تم الوصول للحد الأقصى لمستويات الدليل");
            var code = basic.ToString();

            if (k == ChartKind.Accounts)
                await cn.ExecuteAsync("dbo.AddBasicAccount", new
                {
                    BacicCode = (int)basic, Code = code, AccountName = i.Name.Trim(), AccountKind = false, AccountType = i.Kind,
                    EndAccount = parent.EndAccount ?? 0, ParentGuid = i.ParentGuid, IsShow = true, Type_ = i.Type_ ?? 2,
                }, tx, commandType: CommandType.StoredProcedure);
            else
                await cn.ExecuteAsync("dbo.AddCostCenter", new
                {
                    BasicCode = (int)basic, Code = code, CenterName = i.Name.Trim(), ParentGuid = i.ParentGuid, CenterType = i.Kind,
                }, tx, commandType: CommandType.StoredProcedure);

            var guid = await cn.ExecuteScalarAsync<Guid>($"SELECT TOP 1 Guid FROM dbo.[{c.Table}] WHERE CAST(Code AS nvarchar(50)) = @code AND ParentGuid = @p", new { code, p = i.ParentGuid }, tx);
            if (k == ChartKind.Accounts) await SaveBalancesAsync(cn, tx, guid, i.OpenBalances, isNew: true);
            await audit.WriteAsync(cn, tx, c.Form, guid, UserActionLog.New, code);
            return new ChartSaved(c.SavedNew, code, guid);
        }, IsolationLevel.Serializable, ct);
    }

    public async Task<ChartSaved> UpdateAsync(ChartKind k, Guid guid, ChartNodeInput i, CancellationToken ct)
    {
        var c = Of(k);
        Validate(c, i, isNew: false);
        if (string.IsNullOrWhiteSpace(i.Reason)) throw new ApiException("يرجى إدخال سبب الإجراء أولاً");
        return await db.InTransactionAsync(async (cn, tx) =>
        {
            var code = await cn.ExecuteScalarAsync<string?>($"SELECT CAST(Code AS nvarchar(50)) FROM dbo.[{c.Table}] WHERE Guid = @guid", new { guid }, tx)
                       ?? throw ApiException.NotFound($"{c.Singular} غير موجود");
            if (k == ChartKind.Accounts)
            {
                await cn.ExecuteAsync("dbo.UpdateBasicAccount", new
                {
                    Guid = guid, AccountName = i.Name.Trim(), i.IsUse, Maden = (double)i.Maden, Daen = (double)i.Daen, Type_ = i.Type_ ?? 2,
                }, tx, commandType: CommandType.StoredProcedure);
                await SaveBalancesAsync(cn, tx, guid, i.OpenBalances, isNew: false);
            }
            else
                await cn.ExecuteAsync("dbo.UpdateCostCenter", new { Guid = guid, CenterName = i.Name.Trim(), i.IsUse }, tx, commandType: CommandType.StoredProcedure);

            await audit.WriteAsync(cn, tx, c.Form, guid, UserActionLog.Update, code, i.Reason!.Trim());
            return new ChartSaved(c.SavedEdit, code, guid);
        }, ct: ct);
    }

    /// <summary>DELETEACCOUNTOPENBALANCE + ADDACCOUNTOPENBALANCE (rows with both values zero are skipped).</summary>
    private static async Task SaveBalancesAsync(DbConnection cn, DbTransaction tx, Guid account, List<OpenBalanceRow> rows, bool isNew)
    {
        if (!isNew) await cn.ExecuteAsync("dbo.DeleteAccountOpenBalance", new { AccountGuid = account }, tx, commandType: CommandType.StoredProcedure);
        foreach (var r in rows.Where(r => r.Maden != 0 || r.Daen != 0).DistinctBy(r => r.BranchGuid))
            await cn.ExecuteAsync("dbo.AddAccountOpenBalance", new
            {
                AccountGuid = account, r.BranchGuid, OpenMaden = (double)r.Maden, OpenDaen = (double)r.Daen,
                OpenNotes = string.IsNullOrWhiteSpace(r.Notes) ? "لا توجد ملاحظات" : r.Notes.Trim()[..Math.Min(r.Notes.Trim().Length, 250)],
            }, tx, commandType: CommandType.StoredProcedure);
    }
}
