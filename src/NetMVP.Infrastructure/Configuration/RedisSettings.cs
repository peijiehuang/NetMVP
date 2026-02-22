using System.ComponentModel.DataAnnotations;

namespace NetMVP.Infrastructure.Configuration;

/// <summary>
/// Redis 配置
/// </summary>
public class RedisSettings
{
    /// <summary>
    /// 连接字符串
    /// </summary>
    [Required(ErrorMessage = "Redis 连接字符串不能为空")]
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 实例名称
    /// </summary>
    public string InstanceName { get; set; } = "NetMVP:";

    /// <summary>
    /// 默认数据库
    /// </summary>
    [Range(0, 15, ErrorMessage = "Redis 数据库索引必须在 0-15 之间")]
    public int DefaultDatabase { get; set; } = 0;
}
