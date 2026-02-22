using System.Text.RegularExpressions;

namespace NetMVP.Infrastructure.Helpers;

/// <summary>
/// 验证工具类
/// </summary>
public static partial class ValidationHelper
{
    /// <summary>
    /// 验证邮箱
    /// </summary>
    public static bool IsEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex().IsMatch(email);
    }

    /// <summary>
    /// 验证手机号（中国大陆）
    /// </summary>
    public static bool IsPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        return PhoneRegex().IsMatch(phone);
    }

    /// <summary>
    /// 验证身份证号（中国大陆）
    /// </summary>
    public static bool IsIdCard(string idCard)
    {
        if (string.IsNullOrWhiteSpace(idCard))
            return false;

        return IdCard18Regex().IsMatch(idCard) || IdCard15Regex().IsMatch(idCard);
    }

    /// <summary>
    /// 验证 URL
    /// </summary>
    public static bool IsUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return UrlRegex().IsMatch(url);
    }

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^1[3-9]\d{9}$")]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"^\d{17}[\dXx]$")]
    private static partial Regex IdCard18Regex();

    [GeneratedRegex(@"^\d{15}$")]
    private static partial Regex IdCard15Regex();

    [GeneratedRegex(@"^https?://[^\s]+$")]
    private static partial Regex UrlRegex();
}
