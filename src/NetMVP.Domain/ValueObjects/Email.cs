using System.Text.RegularExpressions;

namespace NetMVP.Domain.ValueObjects;

/// <summary>
/// 邮箱值对象
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// 邮箱地址
    /// </summary>
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// 创建邮箱值对象
    /// </summary>
    public static Email? Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        if (!IsValid(email))
        {
            throw new ArgumentException("邮箱格式不正确", nameof(email));
        }

        return new Email(email.Trim());
    }

    /// <summary>
    /// 验证邮箱格式
    /// </summary>
    public static bool IsValid(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return EmailRegex.IsMatch(email);
    }

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return obj is Email email && Equals(email);
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(Email email) => email.Value;
}
