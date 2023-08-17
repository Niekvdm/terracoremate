namespace TerracoreMate.Extensions;

/// <summary>
/// This static class provides extension methods for working with DateTime data.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Converts the provided DateTimeOffset into hours from now.
    /// </summary>
    /// <param name="date">The date to be converted.</param>
    /// <returns>Hours from now as an integer or -1 if date is null.</returns>
    public static int ToHoursFromNow(this DateTimeOffset? date)
    {
        return date.HasValue ? (int)date.Value.Subtract(DateTimeOffset.UtcNow).TotalHours : -1;
    }
    
    /// <summary>
    /// Converts the provided DateTimeOffset into minutes from now.
    /// </summary>
    /// <param name="date">The date to be converted.</param>
    /// <returns>Minutes from now as an integer or -1 if date is null.</returns>
    public static int ToMinutesFromNow(this DateTimeOffset? date)
    {
        return date.HasValue ? (int)date.Value.Subtract(DateTimeOffset.UtcNow).TotalMinutes : -1;
    }
    
    /// <summary>
    /// Calculates the TimeSpan from now for the provided DateTimeOffset.
    /// </summary>
    /// <param name="date">The date to be used for the calculation.</param>
    /// <returns>A TimeSpan representing the time from now or null if date is null.</returns>
    public static TimeSpan? ToTimeSpanFromNow(this DateTimeOffset? date)
    {
        return date?.Subtract(DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Converts the provided TimeSpan into a string representation of clock time.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan to be converted.</param>
    /// <returns>A string representing the TimeSpan in the format HH:MM:SS, or an empty string if TimeSpan is null.</returns>
    public static string ToClockTime(this TimeSpan? timeSpan)
    {
        return timeSpan.HasValue ? $"{timeSpan.Value.Hours:D2}:{timeSpan.Value.Minutes:D2}:{timeSpan.Value.Seconds:D2}" : string.Empty;
    }
}