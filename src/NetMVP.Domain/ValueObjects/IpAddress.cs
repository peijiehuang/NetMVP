using System.Net;

namespace NetMVP.Domain.ValueObjects;

/// <summary>
/// IP地址值对象
/// </summary>
public sealed class IpAddress : IEquatable<IpAddress>
{
    /// <summary>
    /// IP地址
    /// </summary>
    public string Value { get; }

    private IpAddress(string value)
    {
        Value = value;
    }

    /// <summary>
    /// 创建IP地址值对象
    /// </summary>
    public static IpAddress? Create(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return null;
        }

        if (!IsValid(ipAddress))
        {
            throw new ArgumentException("IP地址格式不正确", nameof(ipAddress));
        }

        return new IpAddress(ipAddress.Trim());
    }

    /// <summary>
    /// 验证IP地址格式
    /// </summary>
    public static bool IsValid(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return false;
        }

        return IPAddress.TryParse(ipAddress, out _);
    }

    public bool Equals(IpAddress? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is IpAddress ip && Equals(ip);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(IpAddress ip) => ip.Value;
}
