using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NetMVP.Infrastructure.Configuration;
using NetMVP.Infrastructure.Services.Cache;

namespace NetMVP.Infrastructure.Tests.Services;

/// <summary>
/// 缓存服务测试
/// </summary>
public class CacheServiceTests
{
    private readonly MemoryCacheService _cacheService;

    public CacheServiceTests()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var options = Options.Create(new CacheOptions
        {
            CacheType = "Memory",
            KeyPrefix = "test:",
            DefaultExpiryHours = 1
        });
        _cacheService = new MemoryCacheService(cache, options);
    }

    [Fact]
    public async Task SetAndGet_ShouldWork()
    {
        // Arrange
        var key = "testkey";
        var value = "testvalue";

        // Act
        await _cacheService.SetAsync(key, value);
        var result = await _cacheService.GetAsync(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public async Task SetAndGetGeneric_ShouldWork()
    {
        // Arrange
        var key = "testobj";
        var value = new { Name = "Test", Age = 25 };

        // Act
        await _cacheService.SetAsync(key, value);
        var result = await _cacheService.GetAsync<dynamic>(key);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Remove_ShouldWork()
    {
        // Arrange
        var key = "removekey";
        await _cacheService.SetAsync(key, "value");

        // Act
        var removed = await _cacheService.RemoveAsync(key);
        var exists = await _cacheService.ExistsAsync(key);

        // Assert
        Assert.True(removed);
        Assert.False(exists);
    }

    [Fact]
    public async Task GetOrSet_ShouldWork()
    {
        // Arrange
        var key = "getorsetkey";
        var callCount = 0;

        // Act - 第一次调用，应该执行 factory
        var result1 = await _cacheService.GetOrSetAsync(key, async () =>
        {
            callCount++;
            await Task.Delay(10);
            return "computed value";
        });

        // Act - 第二次调用，应该从缓存获取
        var result2 = await _cacheService.GetOrSetAsync(key, async () =>
        {
            callCount++;
            await Task.Delay(10);
            return "computed value";
        });

        // Assert
        Assert.Equal("computed value", result1);
        Assert.Equal("computed value", result2);
        Assert.Equal(1, callCount); // factory 只应该被调用一次
    }

    [Fact]
    public async Task HashOperations_ShouldWork()
    {
        // Arrange
        var key = "hashkey";
        var field1 = "field1";
        var field2 = "field2";
        var value1 = "value1";
        var value2 = "value2";

        // Act
        await _cacheService.HashSetAsync(key, field1, value1);
        await _cacheService.HashSetAsync(key, field2, value2);
        
        var result1 = await _cacheService.HashGetAsync<string>(key, field1);
        var all = await _cacheService.HashGetAllAsync<string>(key);

        // Assert
        Assert.Equal(value1, result1);
        Assert.Equal(2, all.Count);
    }
}
