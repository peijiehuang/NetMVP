using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Configuration;
using System.Collections.Concurrent;
using System.Text.Json;

namespace NetMVP.Infrastructure.Services.Cache;

/// <summary>
/// 内存缓存服务实现
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly CacheOptions _options;
    private readonly ConcurrentDictionary<string, byte> _keys = new();

    public MemoryCacheService(IMemoryCache cache, IOptions<CacheOptions> options)
    {
        _cache = cache;
        _options = options.Value;
    }

    private string GetKey(string key) => $"{_options.KeyPrefix}{key}";

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var value = _cache.Get<T>(fullKey);
        return Task.FromResult(value);
    }

    public Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var value = _cache.Get<string>(fullKey);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? _options.DefaultExpiry
        };
        
        _cache.Set(fullKey, value, options);
        _keys.TryAdd(fullKey, 0);
        
        return Task.CompletedTask;
    }

    public Task SetAsync(string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        return SetAsync<string>(key, value, expiry, cancellationToken);
    }

    public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        _cache.Remove(fullKey);
        _keys.TryRemove(fullKey, out _);
        return Task.FromResult(true);
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var fullPrefix = GetKey(prefix);
        var keysToRemove = _keys.Keys.Where(k => k.StartsWith(fullPrefix)).ToList();
        
        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
            _keys.TryRemove(key, out _);
        }
        
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var exists = _cache.TryGetValue(fullKey, out _);
        return Task.FromResult(exists);
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var value = await GetAsync<T>(key, cancellationToken);
        
        if (value != null)
        {
            return value;
        }

        value = await factory();
        await SetAsync(key, value, expiry, cancellationToken);
        
        return value;
    }

    // Hash 操作（内存缓存使用 Dictionary 模拟）
    public Task HashSetAsync<T>(string key, string field, T value, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var hash = _cache.Get<Dictionary<string, T>>(fullKey) ?? new Dictionary<string, T>();
        hash[field] = value;
        _cache.Set(fullKey, hash);
        _keys.TryAdd(fullKey, 0);
        return Task.CompletedTask;
    }

    public Task<T?> HashGetAsync<T>(string key, string field, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var hash = _cache.Get<Dictionary<string, T>>(fullKey);
        
        if (hash != null && hash.TryGetValue(field, out var value))
        {
            return Task.FromResult<T?>(value);
        }
        
        return Task.FromResult<T?>(default);
    }

    public Task<Dictionary<string, T>> HashGetAllAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var hash = _cache.Get<Dictionary<string, T>>(fullKey) ?? new Dictionary<string, T>();
        return Task.FromResult(hash);
    }

    public Task<bool> HashDeleteAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var hash = _cache.Get<Dictionary<string, string>>(fullKey);
        
        if (hash != null)
        {
            var removed = hash.Remove(field);
            _cache.Set(fullKey, hash);
            return Task.FromResult(removed);
        }
        
        return Task.FromResult(false);
    }

    // List 操作（内存缓存使用 List 模拟）
    public Task<long> ListPushAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var list = _cache.Get<List<T>>(fullKey) ?? new List<T>();
        list.Add(value);
        _cache.Set(fullKey, list);
        _keys.TryAdd(fullKey, 0);
        return Task.FromResult((long)list.Count);
    }

    public Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var list = _cache.Get<List<T>>(fullKey) ?? new List<T>();
        
        if (stop == -1)
        {
            stop = list.Count - 1;
        }
        
        var result = list.Skip((int)start).Take((int)(stop - start + 1)).ToList();
        return Task.FromResult(result);
    }

    // Set 操作（内存缓存使用 HashSet 模拟）
    public Task<bool> SetAddAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var set = _cache.Get<HashSet<T>>(fullKey) ?? new HashSet<T>();
        var added = set.Add(value);
        _cache.Set(fullKey, set);
        _keys.TryAdd(fullKey, 0);
        return Task.FromResult(added);
    }

    public Task<List<T>> SetMembersAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var set = _cache.Get<HashSet<T>>(fullKey) ?? new HashSet<T>();
        return Task.FromResult(set.ToList());
    }

    public Task<List<string>> GetKeysAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var fullPattern = GetKey(pattern.Replace("*", ""));
        var matchingKeys = _keys.Keys
            .Where(k => k.StartsWith(fullPattern))
            .ToList();
        return Task.FromResult(matchingKeys);
    }
}
