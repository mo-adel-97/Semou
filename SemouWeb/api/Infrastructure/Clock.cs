namespace Semou.Api.Infrastructure;

/// <summary>Local business time (Saudi Arabia). The desktop app used the client PC clock.</summary>
public static class Clock
{
    private static readonly TimeZoneInfo Tz = Find();

    private static TimeZoneInfo Find()
    {
        foreach (var id in new[] { "Asia/Riyadh", "Arab Standard Time" })
            if (TimeZoneInfo.TryFindSystemTimeZoneById(id, out var tz)) return tz;
        return TimeZoneInfo.Local;
    }

    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Tz);
}
