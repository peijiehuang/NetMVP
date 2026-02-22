using System.Text.RegularExpressions;

namespace NetMVP.Domain.ValueObjects;

/// <summary>
/// 手机号值对象
/// </summary>
public sealed class PhoneNumber : IEquatable<PhoneNumber>
{
    private static readonly Regex PhoneRegex = new(
        @"^1[3-9]\d{9}$",
        RegexOptions.Compiled);

    /// <summary>
    /// 手机号
    /// </summary>
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// 创建手机号值对象
    /// </summary>
    public static PhoneNumber? Create(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return null;
        }

        if (!IsValid(phoneNumber))
        {
            throw new ArgumentException("手机号格式不正确", nameof(phoneNumber));
        }

        return new PhoneNumber(phoneNumber.Trim());
    }

    /// <summary>
    /// 验证手机号格式
    /// </summary>
    public static bool IsValid(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return false;
        }

        return PhoneRegex.IsMatch(phoneNumber);
    }

    public bool Equals(PhoneNumber? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is PhoneNumber phone && Equals(phone);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(PhoneNumber phone) => phone.Value;
}
