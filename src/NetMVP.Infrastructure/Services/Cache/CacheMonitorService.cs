using Microsoft.Extensions.Options;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Configuration;
using StackExchange.Redis;
using System.Text.RegularExpressions;

namespace NetMVP.Infrastructure.Services.Cache;

/// <summary>
/// 缓存监控服务实现
/// </summary>
public class CacheMonitorService : ICacheMonitorService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly ICacheService _cacheService;
    private readonly CacheOptions _options;

    public CacheMonitorService(
        IConnectionMultiplexer redis,
        ICacheService cacheService,
        IOptions<CacheOptions> options)
    {
        _redis = redis;
        _database = redis.GetDatabase();
        _cacheService = cacheService;
        _options = options.Value;
    }

    private string GetKey(string key) => $"{_options.KeyPrefix}{key}";

    public async Task<CacheInfoDto> GetCacheInfoAsync()
    {
        var result = new CacheInfoDto();
        var endpoints = _redis.GetEndPoints();

        if (endpoints.Length > 0)
        {
            var server = _redis.GetServer(endpoints[0]);
            
            // 获取Redis信息
            var info = await server.InfoAsync();
            foreach (var section in info)
            {
                foreach (var item in section)
                {
                    result.Info[$"{section.Key}:{item.Key}"] = item.Value;
                }
            }

            // 获取数据库大小
            var dbSizeResult = await _database.ExecuteAsync("DBSIZE");
            if (dbSizeResult != null && dbSizeResult.Resp2Type == ResultType.Integer)
            {
                result.DbSize = (long)dbSizeResult;
            }

            // 获取命令统计
            var commandStats = result.Info
                .Where(kv => kv.Key.StartsWith("Commandstats:cmdstat_"))
                .Select(kv =>
                {
                    var name = kv.Key.Replace("Commandstats:cmdstat_", "");
                    var match = Regex.Match(kv.Value, @"calls=(\d+),usec=(\d+)");
                    if (match.Success)
                    {
                        return new CommandStatDto
                        {
                            Name = name,
                            Calls = long.Parse(match.Groups[1].Value),
                            Usec = long.Parse(match.Groups[2].Value)
                        };
                    }
                    return null;
                })
                .Where(x => x != null)
                .Cast<CommandStatDto>()
                .OrderByDescending(x => x.Calls)
                .Take(10)
                .ToList();

            result.CommandStats = commandStats;
        }

        return result;
    }

    public async Task<List<string>> GetCacheNamesAsync()
    {
        var keys = await GetAllKeysAsync();
        var cacheNames = keys
            .Select(k => k.Replace(_options.KeyPrefix, ""))
            .Select(k =>
            {
                var index = k.IndexOf(':');
                return index > 0 ? k.Substring(0, index) : k;
            })
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        return cacheNames;
    }

    public async Task<List<CacheKeyDto>> GetCacheKeysAsync(string cacheName)
    {
        var pattern = GetKey($"{cacheName}:*");
        var keys = await GetKeysByPatternAsync(pattern);
        
        var result = keys.Select(k => new CacheKeyDto
        {
            CacheName = cacheName,
            CacheKey = k.Replace(_options.KeyPrefix, ""),
            Remark = ""
        }).ToList();

        return result;
    }

    public async Task<string?> GetCacheValueAsync(string cacheName, string cacheKey)
    {
        var fullKey = $"{cacheName}:{cacheKey}";
        var value = await _cacheService.GetAsync(fullKey);
        return value;
    }

    public async Task ClearCacheNameAsync(string cacheName)
    {
        await _cacheService.RemoveByPrefixAsync($"{cacheName}:");
    }

    public async Task ClearCacheKeyAsync(string cacheKey)
    {
        await _cacheService.RemoveAsync(cacheKey);
    }

    public async Task ClearAllCacheAsync()
    {
        var endpoints = _redis.GetEndPoints();
        foreach (var endpoint in endpoints)
        {
            var server = _redis.GetServer(endpoint);
            await server.FlushDatabaseAsync();
        }
    }

    private async Task<List<string>> GetAllKeysAsync()
    {
        var result = new List<string>();
        var endpoints = _redis.GetEndPoints();

        foreach (var endpoint in endpoints)
        {
            var server = _redis.GetServer(endpoint);
            await foreach (var key in server.KeysAsync(pattern: $"{_options.KeyPrefix}*"))
            {
                result.Add(key.ToString());
            }
        }

        return result;
    }

    private async Task<List<string>> GetKeysByPatternAsync(string pattern)
    {
        var result = new List<string>();
        var endpoints = _redis.GetEndPoints();

        foreach (var endpoint in endpoints)
        {
            var server = _redis.GetServer(endpoint);
            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                result.Add(key.ToString());
            }
        }

        return result;
    }
}
