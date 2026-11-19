using System.Data;
using System.Data.Common;
using Dapper;
using Semou.Api.Security;

namespace Semou.Api.Infrastructure;

/// <summary>
/// Audit trail: writes dbo.User_Action through proc Add_UserAction, the same as the desktop ADD_USERACTION_.
/// Always called inside the same transaction as the change it records.
/// </summary>
public sealed class UserActionLog(CurrentUser user)
{
    public const string New = "جديد";
    public const string Update = "تعديل";
    public const string Delete = "حذف";

    public async Task WriteAsync(DbConnection c, DbTransaction tx, string formUnqName, Guid actionGuid,
                                 string actionType, string? code, string? reason = null)
    {
        var typeGuid = await c.ExecuteScalarAsync<Guid?>("dbo.FormTypeGuid", new { FormName = formUnqName },
            tx, commandType: CommandType.StoredProcedure);
        var now = Clock.Now;
        await c.ExecuteAsync("dbo.Add_UserAction", new
        {
            ActionGuid = actionGuid,
            UserGuid = user.UserGuid,
            Date_ = now,
            Time_ = now.Date.AddHours(now.Hour).AddMinutes(now.Minute),
            Notes_ = actionType,
            ActionTypeGuid = typeGuid,
            ActionReason = reason ?? "",
            ActionCode = code ?? ""
        }, tx, commandType: CommandType.StoredProcedure);
    }
}
