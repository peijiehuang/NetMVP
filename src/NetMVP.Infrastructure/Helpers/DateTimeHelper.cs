namespace NetMVP.Infrastructure.Helpers;

/// <summary>
/// 日期时间工具类
/// </summary>
public static class DateTimeHelper
{
    private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// 转换为 Unix 时间戳（秒）
    /// </summary>
    public static long ToUnixTimestamp(DateTime dateTime)
    {
        return (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds;
    }

    /// <summary>
    /// 从 Unix 时间戳转换为 DateTime
    /// </summary>
    public static DateTime FromUnixTimestamp(long timestamp)
    {
        return UnixEpoch.AddSeconds(timestamp).ToLocalTime();
    }

    /// <summary>
    /// 获取日期范围
    /// </summary>
    /// <param name="type">today, yesterday, week, month, year</param>
    public static (DateTime start, DateTime end) GetDateRange(string type)
    {
        var now = DateTime.Now;
        return type.ToLower() switch
        {
            "today" => (now.Date, now.Date.AddDays(1).AddSeconds(-1)),
            "yesterday" => (now.Date.AddDays(-1), now.Date.AddSeconds(-1)),
            "week" => GetWeekRange(now),
            "month" => GetMonthRange(now),
            "year" => (new DateTime(now.Year, 1, 1), new DateTime(now.Year, 12, 31, 23, 59, 59)),
            _ => (now.Date, now.Date.AddDays(1).AddSeconds(-1))
        };
    }

    /// <summary>
    /// 格式化日期时间
    /// </summary>
    public static string FormatDateTime(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
    {
        return dateTime.ToString(format);
    }

    /// <summary>
    /// 获取周范围（周一到周日）
    /// </summary>
    public static (DateTime start, DateTime end) GetWeekRange(DateTime date)
    {
        var dayOfWeek = (int)date.DayOfWeek;
        if (dayOfWeek == 0) dayOfWeek = 7; // 周日为 7

        var start = date.Date.AddDays(1 - dayOfWeek);
        var end = start.AddDays(7).AddSeconds(-1);

        return (start, end);
    }

    /// <summary>
    /// 获取月范围
    /// </summary>
    public static (DateTime start, DateTime end) GetMonthRange(DateTime date)
    {
        var start = new DateTime(date.Year, date.Month, 1);
        var end = start.AddMonths(1).AddSeconds(-1);

        return (start, end);
    }
}
