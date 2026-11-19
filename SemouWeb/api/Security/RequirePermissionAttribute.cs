using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Semou.Api.Security;

/// <summary>
/// Server-side permission check, the same as AllFiles.FORSAVE / FORUPDATE / FORFIND / FORVIEW in the desktop app.
/// The React UI also hides buttons, but this attribute is what actually enforces the rules.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class RequirePermissionAttribute(string form, Perm perm) : Attribute, IAsyncActionFilter
{
    public string Form { get; } = form;
    public Perm Perm { get; } = perm;

    private static readonly Dictionary<Perm, string> Messages = new()
    {
        [Perm.Add] = "لا تملك صلاحية إضافة جديد",
        [Perm.Edit] = "لا تملك صلاحية تعديل البيانات",
        [Perm.Delete] = "لا تملك صلاحية حذف البيانات",
        [Perm.Print] = "لا تملك صلاحية الطباعة",
        [Perm.Find] = "لا تملك صلاحية استعراض البيانات",
        [Perm.View] = "لا تملك صلاحية فتح هذه الشاشة",
    };

    public static string Message(Perm p) => Messages[p];

    public async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
    {
        var sp = ctx.HttpContext.RequestServices;
        var user = sp.GetRequiredService<CurrentUser>();
        var perms = sp.GetRequiredService<PermissionService>();
        var p = await perms.GetAsync(user.UserGuid, Form);
        if (!p.Has(Perm))
        {
            ctx.Result = new ObjectResult(new { message = Messages[Perm] }) { StatusCode = StatusCodes.Status403Forbidden };
            return;
        }
        await next();
    }
}
