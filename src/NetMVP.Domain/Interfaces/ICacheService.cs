namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 缓存服务接口
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// 获取缓存（泛型）
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取缓存（字符串）
    /// </summary>
    Task<string?> GetAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置缓存（泛型）
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置缓存（字符串）
    /// </summary>
    Task SetAsync(string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除缓存
    /// </summary>
    Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据前缀删除缓存
    /// </summary>
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断缓存是否存在
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取或设置缓存
    /// </summary>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置哈希字段
    /// </summary>
    Task HashSetAsync<T>(string key, string field, T value, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取哈希字段
    /// </summary>
    Task<T?> HashGetAsync<T>(string key, string field, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有哈希字段
    /// </summary>
    Task<Dictionary<string, T>> HashGetAllAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除哈希字段
    /// </summary>
    Task<bool> HashDeleteAsync(string key, string field, CancellationToken cancellationToken = default);

    /// <summary>
    /// 列表添加元素
    /// </summary>
    Task<long> ListPushAsync<T>(string key, T value, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取列表范围
    /// </summary>
    Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1, CancellationToken cancellationToken = default);

    /// <summary>
    /// 集合添加元素
    /// </summary>
    Task<bool> SetAddAsync<T>(string key, T value, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取集合所有成员
    /// </summary>
    Task<List<T>> SetMembersAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据模式获取键列表
    /// </summary>
    Task<List<string>> GetKeysAsync(string pattern, CancellationToken cancellationToken = default);
}
