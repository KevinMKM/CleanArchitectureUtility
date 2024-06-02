namespace CleanArchitectureUtility.Utilities.Utilities.Extensions;

public static class DateTimeX
{
    #region Unix Time Milisecond

    public static long ToUnixTimeMillisecond(this DateTime dateTime, TimeZoneInfo timeZoneInfo) =>
        TimeZoneInfo.ConvertTime(new DateTimeOffset(dateTime), timeZoneInfo).ToUnixTimeMilliseconds();

    public static long? ToUnixTimeMillisecond(this DateTime? dateTime, TimeZoneInfo timeZoneInfo) =>
        dateTime is not null ? ToUnixTimeMillisecond(dateTime.Value, timeZoneInfo) : null;

    public static DateTime UnixMillisecondToDateTime(this long unixTimeMilliseconds, TimeZoneInfo timeZoneInfo) =>
        TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds), timeZoneInfo).DateTime;

    public static DateTime? UnixMillisecondToDateTime(this long? unixTimeMilliseconds, TimeZoneInfo timeZoneInfo) =>
        unixTimeMilliseconds is not null ? UnixMillisecondToDateTime(unixTimeMilliseconds.Value, timeZoneInfo) : null;

    public static long ToLocalUnixTimeMillisecond(this DateTime dateTime) => dateTime.ToUnixTimeMillisecond(TimeZoneInfo.Local);

    public static long? ToLocalUnixTimeMillisecond(this DateTime? dateTime) => dateTime.ToUnixTimeMillisecond(TimeZoneInfo.Local);

    public static DateTime UnixMillisecondToLocalDateTime(this long unixTimeMilliseconds) => unixTimeMilliseconds.UnixMillisecondToDateTime(TimeZoneInfo.Local);

    public static DateTime? UnixMillisecondToLocalDateTime(this long? unixTimeMilliseconds) => unixTimeMilliseconds.UnixMillisecondToDateTime(TimeZoneInfo.Local);

    public static long ToUtcUnixTimeMillisecond(this DateTime dateTime) => dateTime.ToUnixTimeMillisecond(TimeZoneInfo.Utc);

    public static long? ToUtcUnixTimeMillisecond(this DateTime? dateTime) => dateTime.ToUnixTimeMillisecond(TimeZoneInfo.Utc);

    public static DateTime UnixMillisecondToUtcDateTime(this long unixTimeMilliseconds) => unixTimeMilliseconds.UnixMillisecondToDateTime(TimeZoneInfo.Utc);

    public static DateTime? UnixMillisecondToUtcDateTime(this long? unixTimeMilliseconds) => unixTimeMilliseconds.UnixMillisecondToDateTime(TimeZoneInfo.Utc);

    #endregion

    #region Unix Time Second

    public static long ToUnixTimeSecond(this DateTime dateTime, TimeZoneInfo timeZoneInfo) => TimeZoneInfo.ConvertTime(new DateTimeOffset(dateTime), timeZoneInfo).ToUnixTimeSeconds();

    public static long? ToUnixTimeSecond(this DateTime? dateTime, TimeZoneInfo timeZoneInfo) => dateTime is not null ? ToUnixTimeSecond(dateTime.Value, timeZoneInfo) : null;

    public static DateTime UnixSecondsToDateTime(this long unixTimeSeconds, TimeZoneInfo timeZoneInfo) =>
        TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds), timeZoneInfo).DateTime;

    public static DateTime? UnixSecondsToDateTime(this long? unixTimeSeconds, TimeZoneInfo timeZoneInfo) =>
        unixTimeSeconds is not null ? UnixSecondsToDateTime(unixTimeSeconds.Value, timeZoneInfo) : null;

    public static long ToLocalUnixTimeSecond(this DateTime dateTime) => dateTime.ToUnixTimeSecond(TimeZoneInfo.Local);

    public static long? ToLocalUnixTimeSecond(this DateTime? dateTime) => dateTime.ToUnixTimeSecond(TimeZoneInfo.Local);

    public static DateTime UnixSecondsToLocalDateTime(this long unixTimeSeconds) => unixTimeSeconds.UnixSecondsToDateTime(TimeZoneInfo.Local);

    public static DateTime? UnixSecondsToLocalDateTime(this long? unixTimeSeconds) => unixTimeSeconds.UnixSecondsToDateTime(TimeZoneInfo.Local);

    public static long ToUtcUnixTimeSecond(this DateTime dateTime) => dateTime.ToUnixTimeSecond(TimeZoneInfo.Utc);

    public static long? ToUtcUnixTimeSecond(this DateTime? dateTime) => dateTime.ToUnixTimeSecond(TimeZoneInfo.Utc);

    public static DateTime UnixSecondsToUtcDateTime(this long unixTimeSeconds) => unixTimeSeconds.UnixSecondsToDateTime(TimeZoneInfo.Utc);

    public static DateTime? UnixSecondsToUtcDateTime(this long? unixTimeSeconds) => unixTimeSeconds.UnixSecondsToDateTime(TimeZoneInfo.Utc);

    #endregion

    public static DateTime SetDateTimeKind(this DateTime dateTime, DateTimeKind kind)
        => new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, kind);

    public static DateTime? SetDateTimeKind(this DateTime? dateTime, DateTimeKind kind) => dateTime?.SetDateTimeKind(kind);

    public static DateTime ToUtcFormat(this DateTime dateTime) => SetDateTimeKind(dateTime, DateTimeKind.Utc);

    public static DateTime? ToUtcFormat(this DateTime? dateTime) => dateTime?.ToUtcFormat();

    public static string TimeSpanToHourMinute(this TimeSpan timeSpan) => $"{(int)timeSpan.TotalHours:00}:{timeSpan.Minutes:00}";

    public static string TotalMinuteToHourMinute(this int minutes)
    {
        var hour = minutes / 60;
        return $"{hour:00}:{minutes - (hour * 60):00}";
    }

    public static string DateTimeToString(this DateTime dateTime) =>
        $"{dateTime:dd/MM/yyyy} - {(int)dateTime.TimeOfDay.TotalHours:00}:{dateTime.TimeOfDay.Minutes:00}";

    public static List<DateTime> GetDaysTo(this DateTime from, DateTime to)
    {
        var date = from;
        var ret = new List<DateTime>();
        while (date.Date <= to.Date)
        {
            ret.Add(date);
            date = date.AddDays(1);
        }

        return ret;
    }

    public static DateTime MinWith(this DateTime dateTime, List<DateTime> otherDateTimes)
    {
        var min = otherDateTimes.Min();
        return dateTime < min ? dateTime : min;
    }

    public static DateTime MinWith(this DateTime dateTime, params DateTime[] otherDateTimes) => MinWith(dateTime, otherDateTimes.ToList());

    public static TimeSpan MinWith(this TimeSpan dateTime, List<TimeSpan> otherDateTimes)
    {
        var min = otherDateTimes.Min();
        return dateTime < min ? dateTime : min;
    }

    public static TimeSpan MinWith(this TimeSpan dateTime, params TimeSpan[] otherDateTimes) => MinWith(dateTime, otherDateTimes.ToList());

    public static DateTime MaxWith(this DateTime dateTime, List<DateTime> otherDateTimes)
    {
        var min = otherDateTimes.Min();
        return dateTime < min ? dateTime : min;
    }

    public static DateTime MaxWith(this DateTime dateTime, params DateTime[] otherDateTimes) => MaxWith(dateTime, otherDateTimes.ToList());

    public static TimeSpan MaxWith(this TimeSpan time, List<TimeSpan> otherTimes)
    {
        var min = otherTimes.Min();
        return time < min ? time : min;
    }

    public static TimeSpan MaxWith(this TimeSpan time, params TimeSpan[] otherTimes) => MaxWith(time, otherTimes.ToList());
}