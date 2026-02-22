using System.ComponentModel.DataAnnotations;

namespace NetMVP.Infrastructure.Configuration;

/// <summary>
/// 数据库配置
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// 连接字符串
    /// </summary>
    [Required(ErrorMessage = "数据库连接字符串不能为空")]
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 命令超时时间（秒）
    /// </summary>
    [Range(0, 300, ErrorMessage = "命令超时时间必须在 0-300 秒之间")]
    public int CommandTimeout { get; set; } = 30;

    /// <summary>
    /// 是否启用敏感数据日志
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;
}
