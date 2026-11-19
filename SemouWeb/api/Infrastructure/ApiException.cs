namespace Semou.Api.Infrastructure;

/// <summary>
/// Expected business error. Its Arabic message is shown to the user as-is (it replaces the desktop MessageBox).
/// Anything else that is thrown is logged and returned as a generic 500.
/// </summary>
public sealed class ApiException(string message, int status = StatusCodes.Status400BadRequest) : Exception(message)
{
    public int Status { get; } = status;

    public static ApiException NotFound(string message) => new(message, StatusCodes.Status404NotFound);
    public static ApiException Unauthorized(string message) => new(message, StatusCodes.Status401Unauthorized);
    public static ApiException Forbidden(string message) => new(message, StatusCodes.Status403Forbidden);
}
