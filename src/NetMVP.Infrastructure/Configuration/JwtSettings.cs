using System.ComponentModel.DataAnnotations;

namespace NetMVP.Infrastructure.Configuration;

/// <summary>
/// JWT 配置
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// 密钥
    /// </summary>
    [Required(ErrorMessage = "JWT 密钥不能为空")]
    [MinLength(32, ErrorMessage = "JWT 密钥长度不能少于 32 位")]
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// 发行者
    /// </summary>
    [Required(ErrorMessage = "JWT 发行者不能为空")]
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// 受众
    /// </summary>
    [Required(ErrorMessage = "JWT 受众不能为空")]
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// 过期时间（分钟）
    /// </summary>
    [Range(1, 1440, ErrorMessage = "JWT 过期时间必须在 1-1440 分钟之间")]
    public int ExpireMinutes { get; set; } = 120;

    /// <summary>
    /// 刷新令牌过期时间（天）
    /// </summary>
    [Range(1, 30, ErrorMessage = "刷新令牌过期时间必须在 1-30 天之间")]
    public int RefreshTokenExpireDays { get; set; } = 7;
}
