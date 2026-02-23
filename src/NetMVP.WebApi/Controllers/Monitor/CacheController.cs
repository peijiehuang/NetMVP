using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Domain.Interfaces;

namespace NetMVP.WebApi.Controllers.Monitor;

/// <summary>
/// 缓存监控控制器
/// </summary>
[ApiController]
[Route("monitor/cache")]
[Authorize]
public class CacheController : BaseController
{
    private readonly ICacheMonitorService _cacheMonitorService;

    public CacheController(ICacheMonitorService cacheMonitorService)
    {
        _cacheMonitorService = cacheMonitorService;
    }

    /// <summary>
    /// 获取缓存信息
    /// </summary>
    [HttpGet]
    public async Task<AjaxResult> GetCacheInfo()
    {
        var info = await _cacheMonitorService.GetCacheInfoAsync();
        return Success(info);
    }

    /// <summary>
    /// 获取缓存名称列表
    /// </summary>
    [HttpGet("getNames")]
    public async Task<AjaxResult> GetCacheNames()
    {
        var names = await _cacheMonitorService.GetCacheNamesAsync();
        return Success(names);
    }

    /// <summary>
    /// 获取缓存键列表
    /// </summary>
    [HttpGet("getKeys/{cacheName}")]
    public async Task<AjaxResult> GetCacheKeys(string cacheName)
    {
        var keys = await _cacheMonitorService.GetCacheKeysAsync(cacheName);
        return Success(keys);
    }

    /// <summary>
    /// 获取缓存值
    /// </summary>
    [HttpGet("getValue/{cacheName}/{cacheKey}")]
    public async Task<AjaxResult> GetCacheValue(string cacheName, string cacheKey)
    {
        var value = await _cacheMonitorService.GetCacheValueAsync(cacheName, cacheKey);
        return Success(value);
    }

    /// <summary>
    /// 清空缓存名称
    /// </summary>
    [HttpDelete("clearCacheName/{cacheName}")]
    public async Task<AjaxResult> ClearCacheName(string cacheName)
    {
        await _cacheMonitorService.ClearCacheNameAsync(cacheName);
        return Success();
    }

    /// <summary>
    /// 删除缓存键
    /// </summary>
    [HttpDelete("clearCacheKey/{cacheKey}")]
    public async Task<AjaxResult> ClearCacheKey(string cacheKey)
    {
        await _cacheMonitorService.ClearCacheKeyAsync(cacheKey);
        return Success();
    }

    /// <summary>
    /// 清空所有缓存
    /// </summary>
    [HttpDelete("clearCacheAll")]
    public async Task<AjaxResult> ClearAllCache()
    {
        await _cacheMonitorService.ClearAllCacheAsync();
        return Success();
    }
}
