using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetMVP.Infrastructure.Cache
{
    /// <summary>
    /// 缓存工具类（静态）
    /// </summary>
    public static class CacheUtil
    {
        private static readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
        private static ICachePersistence? _persistence;
        private static readonly object _lock = new();

        /// <summary>
        /// 初始化持久化实例（可选，如果不调用则使用默认 SQLite）
        /// </summary>
        public static void Initialize(ICachePersistence? persistence = null)
        {
            lock (_lock)
            {
                _persistence = persistence ?? new SqliteCachePersistence("cache.db");
            }
        }

        /// <summary>
        /// 获取持久化实例（延迟初始化）
        /// </summary>
        private static ICachePersistence GetPersistence()
        {
            if (_persistence == null)
            {
                lock (_lock)
                {
                    _persistence ??= new SqliteCachePersistence("cache.db");
                }
            }
            return _persistence;
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiration">过期时间（null表示不过期）</param>
        /// <param name="createBy">创建人</param>
        /// <param name="persist">是否持久化（默认true）</param>
        public static async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null, string? createBy = null, bool persist = true)
        {
            var jsonValue = SerializeValue(value);
            
            var entry = new CacheEntry
            {
                Key = key,
                Value = jsonValue,
                ValueType = typeof(T).FullName ?? typeof(T).Name,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                CreateBy = createBy,
                UpdateBy = createBy,
                OutTime = expiration.HasValue ? DateTime.Now.Add(expiration.Value) : null
            };

            _cache.AddOrUpdate(key, entry, (k, old) =>
            {
                entry.Id = old.Id;
                entry.CreateTime = old.CreateTime;
                entry.CreateBy = old.CreateBy;
                return entry;
            });

            if (persist)
            {
                await GetPersistence().SaveAsync(entry);
            }

            return true;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="loadFromPersist">是否从持久化存储加载（默认true）</param>
        public static async Task<T?> GetAsync<T>(string key, bool loadFromPersist = true)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                if (entry.IsExpired)
                {
                    await RemoveAsync(key);
                    return default;
                }

                return DeserializeValue<T>(entry.Value);
            }

            // 尝试从持久化存储加载
            if (loadFromPersist)
            {
                var persistedEntry = await GetPersistence().GetAsync(key);
                if (persistedEntry != null && !persistedEntry.IsExpired)
                {
                    _cache.TryAdd(key, persistedEntry);
                    return DeserializeValue<T>(persistedEntry.Value);
                }
            }

            return default;
        }

        /// <summary>
        /// 序列化值
        /// </summary>
        private static string SerializeValue<T>(T value)
        {
            if (value == null)
            {
                return "null";
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
            return JsonSerializer.Serialize(value);
        }

        /// <summary>
        /// 反序列化值
        /// </summary>
        private static T? DeserializeValue<T>(string value)
        {
            if (string.IsNullOrEmpty(value) || value == "null")
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
                    return JsonSerializer.Deserialize<T>(value);
                }
            }

            // 复杂类型使用 JSON 反序列化
            return JsonSerializer.Deserialize<T>(value);
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

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="deleteFromPersist">是否从持久化存储删除（默认true）</param>
        public static async Task<bool> RemoveAsync(string key, bool deleteFromPersist = true)
        {
            _cache.TryRemove(key, out _);

            if (deleteFromPersist)
            {
                return await GetPersistence().DeleteAsync(key);
            }

            return true;
        }

        /// <summary>
        /// 检查缓存是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="checkPersist">是否检查持久化存储（默认true）</param>
        public static async Task<bool> ExistsAsync(string key, bool checkPersist = true)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                if (entry.IsExpired)
                {
                    await RemoveAsync(key);
                    return false;
                }
                return true;
            }

            if (checkPersist)
            {
                var persistedEntry = await GetPersistence().GetAsync(key);
                return persistedEntry != null && !persistedEntry.IsExpired;
            }

            return false;
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        /// <param name="clearPersist">是否清空持久化存储（默认true）</param>
        public static async Task ClearAsync(bool clearPersist = true)
        {
            _cache.Clear();

            if (clearPersist)
            {
                await GetPersistence().ClearAsync();
            }
        }

        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        public static List<string> GetAllKeys()
        {
            return _cache.Keys.ToList();
        }

        /// <summary>
        /// 清理过期缓存
        /// </summary>
        /// <param name="cleanPersist">是否清理持久化存储中的过期数据（默认true）</param>
        public static async Task CleanExpiredAsync(bool cleanPersist = true)
        {
            var expiredKeys = _cache.Where(x => x.Value.IsExpired).Select(x => x.Key).ToList();
            
            foreach (var key in expiredKeys)
            {
                await RemoveAsync(key);
            }

            if (cleanPersist)
            {
                await GetPersistence().DeleteExpiredAsync();
            }
        }

        /// <summary>
        /// 从持久化存储加载所有缓存到内存
        /// </summary>
        public static async Task LoadFromPersistAsync()
        {
            var entries = await GetPersistence().GetAllAsync();
            foreach (var entry in entries.Where(e => !e.IsExpired))
            {
                _cache.TryAdd(entry.Key, entry);
            }
        }
    }
}
