using Microsoft.Extensions.Options;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace NetMVP.Infrastructure.Services.Cache;

/// <summary>
/// Redis 缓存服务实现
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly CacheOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IConnectionMultiplexer redis, IOptions<CacheOptions> options)
    {
        _redis = redis;
        _database = redis.GetDatabase();
        _options = options.Value;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
    }

    private string GetKey(string key) => $"{_options.KeyPrefix}{key}";

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var value = await _database.StringGetAsync(fullKey);
        
        if (value.IsNullOrEmpty)
        {
            return default;
        }

        return DeserializeValue<T>(value!);
    }

    public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var value = await _database.StringGetAsync(fullKey);
        return value.IsNullOrEmpty ? null : value.ToString();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var serializedValue = SerializeValue(value);
        await _database.StringSetAsync(fullKey, serializedValue, expiry ?? _options.DefaultExpiry);
    }

    public async Task SetAsync(string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        await _database.StringSetAsync(fullKey, value, expiry ?? _options.DefaultExpiry);
    }

    public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        return await _database.KeyDeleteAsync(fullKey);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var fullPrefix = GetKey(prefix);
        var endpoints = _redis.GetEndPoints();
        
        foreach (var endpoint in endpoints)
        {
            var server = _redis.GetServer(endpoint);
            var keys = server.Keys(pattern: $"{fullPrefix}*").ToArray();
            
            if (keys.Length > 0)
            {
                await _database.KeyDeleteAsync(keys);
            }
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        return await _database.KeyExistsAsync(fullKey);
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

    // Hash 操作
    public async Task HashSetAsync<T>(string key, string field, T value, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var serializedValue = SerializeValue(value);
        await _database.HashSetAsync(fullKey, field, serializedValue);
    }

    public async Task<T?> HashGetAsync<T>(string key, string field, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var value = await _database.HashGetAsync(fullKey, field);
        
        if (value.IsNullOrEmpty)
        {
            return default;
        }

        return DeserializeValue<T>(value!);
    }

    public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var entries = await _database.HashGetAllAsync(fullKey);
        var result = new Dictionary<string, T>();
        
        foreach (var entry in entries)
        {
            var value = DeserializeValue<T>(entry.Value!);
            if (value != null)
            {
                result[entry.Name!] = value;
            }
        }
        
        return result;
    }

    public async Task<bool> HashDeleteAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        return await _database.HashDeleteAsync(fullKey, field);
    }

    // List 操作
    public async Task<long> ListPushAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var serializedValue = SerializeValue(value);
        return await _database.ListRightPushAsync(fullKey, serializedValue);
    }

    public async Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var values = await _database.ListRangeAsync(fullKey, start, stop);
        var result = new List<T>();
        
        foreach (var value in values)
        {
            if (!value.IsNullOrEmpty)
            {
                var item = DeserializeValue<T>(value!);
                if (item != null)
                {
                    result.Add(item);
                }
            }
        }
        
        return result;
    }

    // Set 操作
    public async Task<bool> SetAddAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var serializedValue = SerializeValue(value);
        return await _database.SetAddAsync(fullKey, serializedValue);
    }

    public async Task<List<T>> SetMembersAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var values = await _database.SetMembersAsync(fullKey);
        var result = new List<T>();
        
        foreach (var value in values)
        {
            if (!value.IsNullOrEmpty)
            {
                var item = DeserializeValue<T>(value!);
                if (item != null)
                {
                    result.Add(item);
                }
            }
        }
        
        return result;
    }

    public Task<List<string>> GetKeysAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var fullPattern = GetKey(pattern);
        var result = new List<string>();
        var endpoints = _redis.GetEndPoints();
        
        foreach (var endpoint in endpoints)
        {
            var server = _redis.GetServer(endpoint);
            var keys = server.Keys(pattern: fullPattern).ToArray();
            result.AddRange(keys.Select(k => k.ToString()));
        }
        
        return Task.FromResult(result);
    }

    /// <summary>
    /// 序列化值
    /// </summary>
    private string SerializeValue<T>(T value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        // 对于字符串类型，直接返回原值，避免双重序列化
        if (typeof(T) == typeof(string))
        {
            return value.ToString()!;
        }

        // 对于基本类型，转换为字符串
        if (IsSimpleType(typeof(T)))
        {
            return value.ToString()!;
        }

        // 对于复杂类型，使用 JSON 序列化
        return JsonSerializer.Serialize(value, _jsonOptions);
    }

    /// <summary>
    /// 反序列化值
    /// </summary>
    private T? DeserializeValue<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return default;
        }

        var targetType = typeof(T);

        // 字符串类型直接返回
        if (targetType == typeof(string))
        {
            return (T)(object)value;
        }

        // 基本类型转换
        if (IsSimpleType(targetType))
        {
            try
            {
                return (T)Convert.ChangeType(value, targetType);
            }
            catch
            {
                // 如果转换失败，尝试 JSON 反序列化
                return JsonSerializer.Deserialize<T>(value, _jsonOptions);
            }
        }

        // 复杂类型使用 JSON 反序列化
        return JsonSerializer.Deserialize<T>(value, _jsonOptions);
    }

    /// <summary>
    /// 判断是否为简单类型
    /// </summary>
    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(Guid);
    }
}
