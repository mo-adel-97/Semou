using System.Data;
using System.Data.Common;
using Dapper;
using Semou.Api.Infrastructure;

namespace Semou.Api.Features.Sales;

/// <summary>A document book (RegSetting) with its posting rules.</summary>
public sealed class DocBook
{
    public Guid Guid { get; set; }
    public int Code { get; set; }
    public string DocName { get; set; } = "";
    public Guid BranchGuid { get; set; }
    public string? BranchName { get; set; }
    public bool ChkVat { get; set; }
    public bool ChkMaden { get; set; }
    public Guid? MadenGuid { get; set; }
    public bool ChkDaen { get; set; }
    public Guid? DaenGuid { get; set; }
    public Guid? CostCenterGuid { get; set; }
    public Guid? VatAccountGuid { get; set; }

    public Guid MadenAccount(Guid student) => ChkMaden ? student : MadenGuid ?? throw new ApiException("حساب المدين غير محدد في الدفتر، يرجى مراجعة مدير النظام");
    public Guid DaenAccount(Guid student) => ChkDaen ? student : DaenGuid ?? throw new ApiException("حساب الدائن غير محدد في الدفتر، يرجى مراجعة مدير النظام");
}

/// <summary>LoadRegSettingForNew: the books of a document type the current user may use (optionally for one branch).</summary>
public static class DocBooks
{
    public static async Task<List<DocBook>> AllowedAsync(DbConnection c, DbTransaction? tx, Guid user, string form, Guid? branch)
    {
        var type = await c.ExecuteScalarAsync<Guid?>("dbo.FormTypeGuid", new { FormName = form }, tx, commandType: CommandType.StoredProcedure);
        return (await c.QueryAsync<DocBook>("""
            SELECT R.Guid, R.Code, R.DocName, R.BranchGuid, B.BrName AS BranchName, CAST(ISNULL(R.ChkVat,0) AS bit) AS ChkVat,
                   CAST(ISNULL(R.ChkMaden,0) AS bit) AS ChkMaden, R.MadenGuid, CAST(ISNULL(R.ChkDaen,0) AS bit) AS ChkDaen, R.DaenGuid, R.CostCenterGuid, R.VatAccountGuid
            FROM dbo.RegSetting R JOIN dbo.Branch B ON B.Guid = R.BranchGuid JOIN dbo.RegSettingUser U ON U.RegSettingGuid = R.Guid
            WHERE U.UserGuid = @user AND R.IsUse = 1 AND R.DocTypeGuid = @type AND (@branch IS NULL OR R.BranchGuid = @branch)
            ORDER BY R.Code
            """, new { user, type, branch }, tx)).ToList();
    }

    public static async Task<DocBook> PickAsync(DbConnection c, DbTransaction tx, Guid user, string form, Guid? branch, Guid book) =>
        (await AllowedAsync(c, tx, user, form, branch)).FirstOrDefault(b => b.Guid == book)
        ?? throw new ApiException("الدفتر المختار غير متاح لك، يرجى اختيار نوع المستند");
}
