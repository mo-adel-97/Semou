using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Semou.Api.Infrastructure;

/// <summary>
/// One error format for the whole API (RFC 7807 ProblemDetails) with an extra "message" field in Arabic
/// that the UI shows directly. Unexpected errors are logged with a traceId the user can quote to support.
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> log, IProblemDetailsService problems)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
    {
        var (status, message) = ex switch
        {
            ApiException a => (a.Status, a.Message),
            UnauthorizedAccessException => (401, "انتهت الجلسة يرجى تسجيل الدخول مرة أخرى"),
            SqlException { Number: 1205 } => (409, "السجل مستخدم حالياً من مستخدم آخر، حاول مرة أخرى"),
            SqlException { Number: 2627 or 2601 } => (409, "البيانات مسجلة من قبل"),
            SqlException { Number: 547 } => (409, "لا يمكن تنفيذ العملية لارتباط البيانات بسجلات أخرى"),
            SqlException => (503, "تعذر الاتصال بقاعدة البيانات، يرجى المحاولة مرة أخرى بعد قليل"),
            OperationCanceledException => (499, "تم إلغاء الطلب"),
            _ => (500, "حدث خطأ غير متوقع، يرجى التواصل مع الدعم الفني")
        };

        if (status >= 500) log.LogError(ex, "Unhandled error {Method} {Path}", ctx.Request.Method, ctx.Request.Path);
        else log.LogInformation("Business error {Status} on {Path}: {Message}", status, ctx.Request.Path, message);

        ctx.Response.StatusCode = status;
        return await problems.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = ctx,
            Exception = ex,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = message,
                Extensions = { ["message"] = message }
            }
        });
    }
}

public static class ErrorHandlingExtensions
{
    public static IServiceCollection AddSemouErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(o => o.CustomizeProblemDetails = c =>
        {
            c.ProblemDetails.Extensions["traceId"] = c.HttpContext.TraceIdentifier;
            c.ProblemDetails.Extensions.TryAdd("message", c.ProblemDetails.Title);
        });
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // Model-binding / validation errors -> same shape, Arabic summary message.
        services.Configure<ApiBehaviorOptions>(o => o.InvalidModelStateResponseFactory = ctx =>
        {
            var errors = ctx.ModelState.Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(e => e.Key, e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray());
            var first = errors.Values.SelectMany(v => v).FirstOrDefault(m => !string.IsNullOrWhiteSpace(m));
            var pd = new ValidationProblemDetails(errors) { Status = 400, Title = first ?? "يرجى مراجعة البيانات المدخلة" };
            pd.Extensions["message"] = pd.Title;
            pd.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
            return new BadRequestObjectResult(pd);
        });
        return services;
    }
}
