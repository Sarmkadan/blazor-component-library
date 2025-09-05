// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Globalization;

namespace BlazorComponentLibrary.Utilities;

/// <summary>
/// Utility class for date and time operations.
/// Provides formatting, calculation, and timezone handling.
/// </summary>
public static class DateTimeHelper
{
    /// <summary>
    /// Gets the current date at start of day (00:00:00).
    /// Useful for filtering operations.
    /// </summary>
    public static DateTime GetStartOfDay(DateTime dateTime)
    {
        return dateTime.Date;
    }

    /// <summary>
    /// Gets the end of day (23:59:59).
    /// Useful for date range queries.
    /// </summary>
    public static DateTime GetEndOfDay(DateTime dateTime)
    {
        return dateTime.Date.AddDays(1).AddSeconds(-1);
    }

    /// <summary>
    /// Calculates the start of the current week (Monday).
    /// </summary>
    public static DateTime GetStartOfWeek(DateTime dateTime, DayOfWeek weekStart = DayOfWeek.Monday)
    {
        var diff = dateTime.DayOfWeek - weekStart;
        if (diff < 0) diff += 7;
        return dateTime.AddDays(-diff).Date;
    }

    /// <summary>
    /// Calculates the end of the current week (Sunday).
    /// </summary>
    public static DateTime GetEndOfWeek(DateTime dateTime, DayOfWeek weekEnd = DayOfWeek.Sunday)
    {
        var start = GetStartOfWeek(dateTime);
        return start.AddDays(6).Date.AddDays(1).AddSeconds(-1);
    }

    /// <summary>
    /// Gets the first day of the month.
    /// </summary>
    public static DateTime GetStartOfMonth(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1);
    }

    /// <summary>
    /// Gets the last day of the month.
    /// </summary>
    public static DateTime GetEndOfMonth(DateTime dateTime)
    {
        var startOfMonth = GetStartOfMonth(dateTime);
        return startOfMonth.AddMonths(1).AddSeconds(-1);
    }

    /// <summary>
    /// Gets the first day of the year.
    /// </summary>
    public static DateTime GetStartOfYear(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, 1, 1);
    }

    /// <summary>
    /// Gets the last day of the year.
    /// </summary>
    public static DateTime GetEndOfYear(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, 12, 31, 23, 59, 59);
    }

    /// <summary>
    /// Formats DateTime as relative time (e.g., "2 hours ago", "in 3 days").
    /// Useful for user-friendly UI display.
    /// </summary>
    public static string GetRelativeTimeString(DateTime dateTime, DateTime? baseTime = null)
    {
        baseTime ??= DateTime.UtcNow;
        var timeSpan = baseTime.Value - dateTime;

        return timeSpan.TotalSeconds switch
        {
            < 60 => "just now",
            < 120 => "1 minute ago",
            < 3600 => $"{(int)timeSpan.TotalMinutes} minutes ago",
            < 7200 => "1 hour ago",
            < 86400 => $"{(int)timeSpan.TotalHours} hours ago",
            < 172800 => "1 day ago",
            < 604800 => $"{(int)timeSpan.TotalDays} days ago",
            < 1209600 => "1 week ago",
            _ => $"{(int)timeSpan.TotalDays / 7} weeks ago"
        };
    }

    /// <summary>
    /// Calculates the age in years based on birth date.
    /// </summary>
    public static int GetAgeInYears(DateTime birthDate, DateTime? asOfDate = null)
    {
        asOfDate ??= DateTime.UtcNow;
        var age = asOfDate.Value.Year - birthDate.Year;

        if (birthDate.Date > asOfDate.Value.AddYears(-age))
            age--;

        return age;
    }

    /// <summary>
    /// Checks if a date falls within a specific range.
    /// </summary>
    public static bool IsWithinRange(DateTime dateTime, DateTime startDate, DateTime endDate)
    {
        return dateTime >= startDate && dateTime <= endDate;
    }

    /// <summary>
    /// Gets the number of business days (excluding weekends) between two dates.
    /// </summary>
    public static int GetBusinessDaysBetween(DateTime startDate, DateTime endDate)
    {
        var count = 0;
        var current = startDate;

        while (current <= endDate)
        {
            if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
                count++;

            current = current.AddDays(1);
        }

        return count;
    }

    /// <summary>
    /// Converts DateTime to ISO 8601 format string.
    /// Standard format for API responses.
    /// </summary>
    public static string ToIso8601(DateTime dateTime)
    {
        return dateTime.ToString("O", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts Unix timestamp to DateTime.
    /// </summary>
    public static DateTime UnixTimestampToDateTime(long unixTimestamp)
    {
        return DateTime.UnixEpoch.AddSeconds(unixTimestamp);
    }

    /// <summary>
    /// Converts DateTime to Unix timestamp.
    /// </summary>
    public static long DateTimeToUnixTimestamp(DateTime dateTime)
    {
        return (long)(dateTime - DateTime.UnixEpoch).TotalSeconds;
    }

    /// <summary>
    /// Gets the next occurrence of a specific day of week.
    /// </summary>
    public static DateTime GetNextOccurrenceOfDayOfWeek(DayOfWeek dayOfWeek, DateTime? baseDate = null)
    {
        baseDate ??= DateTime.UtcNow;
        var daysAhead = dayOfWeek - baseDate.Value.DayOfWeek;

        if (daysAhead <= 0)
            daysAhead += 7;

        return baseDate.Value.AddDays(daysAhead);
    }

    /// <summary>
    /// Calculates the difference between two dates in a human-readable format.
    /// </summary>
    public static string GetTimeDifferenceString(DateTime startDate, DateTime endDate)
    {
        var timeSpan = endDate - startDate;

        return timeSpan.TotalDays switch
        {
            >= 365 => $"{(int)(timeSpan.TotalDays / 365)} years",
            >= 30 => $"{(int)(timeSpan.TotalDays / 30)} months",
            >= 7 => $"{(int)(timeSpan.TotalDays / 7)} weeks",
            >= 1 => $"{(int)timeSpan.TotalDays} days",
            _ => $"{(int)timeSpan.TotalHours} hours"
        };
    }

    /// <summary>
    /// Checks if the given year is a leap year.
    /// </summary>
    public static bool IsLeapYear(int year)
    {
        return DateTime.IsLeapYear(year);
    }
}
