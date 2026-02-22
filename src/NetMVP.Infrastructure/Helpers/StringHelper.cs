using System.Text;
using System.Text.RegularExpressions;

namespace NetMVP.Infrastructure.Helpers;

/// <summary>
/// 字符串工具类
/// </summary>
public static partial class StringHelper
{
    /// <summary>
    /// 判断字符串是否为空
    /// </summary>
    public static bool IsNullOrEmpty(string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    /// <summary>
    /// 截断字符串
    /// </summary>
    public static string Truncate(string str, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
            return str;

        return str.Substring(0, maxLength) + suffix;
    }

    /// <summary>
    /// 转换为蛇形命名（snake_case）
    /// </summary>
    public static string ToSnakeCase(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        return SnakeCaseRegex().Replace(str, "$1_$2").ToLower();
    }

    /// <summary>
    /// 转换为帕斯卡命名（PascalCase）
    /// </summary>
    public static string ToPascalCase(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        var words = str.Split(new[] { '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder();

        foreach (var word in words)
        {
            if (word.Length > 0)
            {
                result.Append(char.ToUpper(word[0]));
                if (word.Length > 1)
                    result.Append(word.Substring(1).ToLower());
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// 转换为驼峰命名（camelCase）
    /// </summary>
    public static string ToCamelCase(string str)
    {
        var pascal = ToPascalCase(str);
        if (string.IsNullOrEmpty(pascal))
            return pascal;

        return char.ToLower(pascal[0]) + pascal.Substring(1);
    }

    /// <summary>
    /// 移除特殊字符
    /// </summary>
    public static string RemoveSpecialCharacters(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        return SpecialCharRegex().Replace(str, "");
    }

    /// <summary>
    /// 生成随机字符串
    /// </summary>
    public static string GenerateRandomString(int length, bool includeNumbers = true, bool includeSpecialChars = false)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        if (includeNumbers)
            chars += "0123456789";
        if (includeSpecialChars)
            chars += "!@#$%^&*()_+-=[]{}|;:,.<>?";

        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    [GeneratedRegex("([a-z0-9])([A-Z])")]
    private static partial Regex SnakeCaseRegex();

    [GeneratedRegex("[^a-zA-Z0-9]")]
    private static partial Regex SpecialCharRegex();
}
