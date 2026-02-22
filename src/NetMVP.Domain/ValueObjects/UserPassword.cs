namespace NetMVP.Domain.ValueObjects;

/// <summary>
/// 用户密码值对象
/// </summary>
public sealed class UserPassword : IEquatable<UserPassword>
{
    /// <summary>
    /// 加密后的密码
    /// </summary>
    public string HashedPassword { get; }

    private UserPassword(string hashedPassword)
    {
        HashedPassword = hashedPassword;
    }

    /// <summary>
    /// 从明文密码创建
    /// </summary>
    public static UserPassword CreateFromPlainText(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
        {
            throw new ArgumentException("密码不能为空", nameof(plainPassword));
        }

        if (plainPassword.Length < 6)
        {
            throw new ArgumentException("密码长度不能少于6位", nameof(plainPassword));
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        return new UserPassword(hashedPassword);
    }

    /// <summary>
    /// 从已加密的密码创建
    /// </summary>
    public static UserPassword CreateFromHash(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            throw new ArgumentException("密码哈希不能为空", nameof(hashedPassword));
        }

        return new UserPassword(hashedPassword);
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    public bool Verify(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, HashedPassword);
        }
        catch
        {
            return false;
        }
    }

    public bool Equals(UserPassword? other)
    {
        if (other is null) return false;
        return HashedPassword == other.HashedPassword;
    }

    public override bool Equals(object? obj)
    {
        return obj is UserPassword password && Equals(password);
    }

    public override int GetHashCode()
    {
        return HashedPassword.GetHashCode();
    }

    public override string ToString()
    {
        return "********";
    }

    public static implicit operator string(UserPassword password) => password.HashedPassword;
}
