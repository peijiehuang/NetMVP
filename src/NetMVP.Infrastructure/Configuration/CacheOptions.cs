namespace NetMVP.Infrastructure.Configuration;

/// <summary>
/// 缓存配置选项
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// 缓存类型（Memory/Redis）
    /// </summary>
    public string CacheType { get; set; } = "Memory";

    /// <summary>
    /// 缓存键前缀
    /// </summary>
    public string KeyPrefix { get; set; } = "netmvp:";

    /// <summary>
    /// 默认过期时间（小时）
    /// </summary>
    public int DefaultExpiryHours { get; set; } = 1;

    /// <summary>
    /// Redis 连接字符串
    /// </summary>
    public string? RedisConnection { get; set; }

    /// <summary>
    /// 获取默认过期时间
    /// </summary>
    public TimeSpan DefaultExpiry => TimeSpan.FromHours(DefaultExpiryHours);
}
