namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 缓存监控服务接口
/// </summary>
public interface ICacheMonitorService
{
    /// <summary>
    /// 获取缓存信息
    /// </summary>
    Task<CacheInfoDto> GetCacheInfoAsync();

    /// <summary>
    /// 获取缓存名称列表
    /// </summary>
    Task<List<string>> GetCacheNamesAsync();

    /// <summary>
    /// 获取缓存键列表
    /// </summary>
    Task<List<CacheKeyDto>> GetCacheKeysAsync(string cacheName);

    /// <summary>
    /// 获取缓存值
    /// </summary>
    Task<string?> GetCacheValueAsync(string cacheName, string cacheKey);

    /// <summary>
    /// 清空缓存名称
    /// </summary>
    Task ClearCacheNameAsync(string cacheName);

    /// <summary>
    /// 删除缓存键
    /// </summary>
    Task ClearCacheKeyAsync(string cacheKey);

    /// <summary>
    /// 清空所有缓存
    /// </summary>
    Task ClearAllCacheAsync();
}

/// <summary>
/// 缓存信息DTO
/// </summary>
public class CacheInfoDto
{
    /// <summary>
    /// Redis信息
    /// </summary>
    public Dictionary<string, string> Info { get; set; } = new();

    /// <summary>
    /// 数据库大小
    /// </summary>
    public long DbSize { get; set; }

    /// <summary>
    /// 命令统计
    /// </summary>
    public List<CommandStatDto> CommandStats { get; set; } = new();
}

/// <summary>
/// 命令统计DTO
/// </summary>
public class CommandStatDto
{
    /// <summary>
    /// 命令名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 调用次数
    /// </summary>
    public long Calls { get; set; }

    /// <summary>
    /// 总耗时（微秒）
    /// </summary>
    public long Usec { get; set; }
}

/// <summary>
/// 缓存键DTO
/// </summary>
public class CacheKeyDto
{
    /// <summary>
    /// 缓存名称
    /// </summary>
    public string CacheName { get; set; } = string.Empty;

    /// <summary>
    /// 缓存键
    /// </summary>
    public string CacheKey { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; } = string.Empty;
}
