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
            
            // 获取Redis基本信息
            var info = await server.InfoAsync();
            foreach (var section in info)
            {
                foreach (var item in section)
                {
                    // 只保留键名，不带section前缀，以匹配前端期望的格式
                    result.Info[item.Key] = item.Value;
                }
            }

            // 单独获取命令统计信息 - 使用 server.Execute 直接调用 INFO commandstats
            try
            {
                var commandStatsResult = await server.ExecuteAsync("INFO", "commandstats");
                if (commandStatsResult != null && !commandStatsResult.IsNull)
                {
                    var commandStatsText = commandStatsResult.ToString();
                    if (!string.IsNullOrEmpty(commandStatsText))
                    {
                        // 解析 INFO commandstats 的文本输出
                        // 格式: cmdstat_get:calls=123,usec=456,usec_per_call=3.78,rejected_calls=0,failed_calls=0
                        var lines = commandStatsText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var line in lines)
                        {
                            if (line.StartsWith("cmdstat_"))
                            {
                                var colonIndex = line.IndexOf(':');
                                if (colonIndex > 0)
                                {
                                    var key = line.Substring(0, colonIndex);
                                    var value = line.Substring(colonIndex + 1);
                                    result.Info[key] = value;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录错误但不中断执行
                // 命令统计可能在某些Redis配置中不可用
                System.Diagnostics.Debug.WriteLine($"Failed to get command stats: {ex.Message}");
            }

            // 获取数据库大小
            var dbSizeResult = await _database.ExecuteAsync("DBSIZE");
            if (dbSizeResult != null && dbSizeResult.Resp2Type == ResultType.Integer)
            {
                result.DbSize = (long)dbSizeResult;
            }

            // 解析命令统计 - 匹配格式: calls=123,usec=456,...
            var commandStats = result.Info
                .Where(kv => kv.Key.StartsWith("cmdstat_"))
                .Select(kv =>
                {
                    var name = kv.Key.Replace("cmdstat_", "");
                    // 匹配 calls= 后面的数字，直到遇到逗号或字符串结束
                    var match = Regex.Match(kv.Value, @"calls=(\d+)(?:,|$)");
                    if (match.Success)
                    {
                        return new CommandStatDto
                        {
                            Name = name,
                            Value = match.Groups[1].Value  // 只取调用次数
                        };
                    }
                    return null;
                })
                .Where(x => x != null)
                .Cast<CommandStatDto>()
                .OrderByDescending(x => long.TryParse(x.Value, out var val) ? val : 0)
                .Take(10)
                .ToList();

            result.CommandStats = commandStats;
        }

        return result;
    }

    public async Task<List<CacheNameDto>> GetCacheNamesAsync()
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
            .Select(name => new CacheNameDto
            {
                CacheName = name,
                Remark = GetCacheRemark(name)
            })
            .ToList();

        return cacheNames;
    }

    private string GetCacheRemark(string cacheName)
    {
        return cacheName switch
        {
            "login_tokens" => "用户信息",
            "sys_config" => "配置信息",
            "sys_dict" => "数据字典",
            "captcha_codes" => "验证码",
            "repeat_submit" => "防重提交",
            "rate_limit" => "限流处理",
            "pwd_err_cnt" => "密码错误次数",
            "online_user" => "在线用户",
            "refresh_token" => "刷新令牌",
            "user" => "用户缓存",
            _ => ""
        };
    }

    public async Task<List<string>> GetCacheKeysAsync(string cacheName)
    {
        var pattern = GetKey($"{cacheName}:*");
        var keys = await GetKeysByPatternAsync(pattern);
        
        var result = keys
            .Select(k => k.Replace(_options.KeyPrefix, ""))
            .OrderBy(k => k)
            .ToList();

        return result;
    }

    public async Task<CacheValueDto?> GetCacheValueAsync(string cacheName, string cacheKey)
    {
        var fullKey = cacheKey;
        var value = await _cacheService.GetAsync(fullKey);
        
        if (value == null)
        {
            return null;
        }

        return new CacheValueDto
        {
            CacheName = cacheName.Replace(":", ""),
            CacheKey = cacheKey.Replace(cacheName, ""),
            CacheValue = value
        };
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
