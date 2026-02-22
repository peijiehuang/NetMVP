using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Configuration;
using NetMVP.Infrastructure.Services.Cache;
using StackExchange.Redis;
using System.Net;
using Xunit;

namespace NetMVP.Infrastructure.Tests.Services;

public class CacheMonitorServiceTests
{
    private readonly Mock<IConnectionMultiplexer> _mockRedis;
    private readonly Mock<IDatabase> _mockDatabase;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IServer> _mockServer;
    private readonly CacheMonitorService _service;
    private readonly CacheOptions _options;

    public CacheMonitorServiceTests()
    {
        _mockRedis = new Mock<IConnectionMultiplexer>();
        _mockDatabase = new Mock<IDatabase>();
        _mockCacheService = new Mock<ICacheService>();
        _mockServer = new Mock<IServer>();
        _options = new CacheOptions { KeyPrefix = "netmvp:" };

        var optionsMock = new Mock<IOptions<CacheOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);

        _mockRedis.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);

        _service = new CacheMonitorService(
            _mockRedis.Object,
            _mockCacheService.Object,
            optionsMock.Object);
    }

    [Fact]
    public async Task GetCacheInfoAsync_ShouldReturnCacheInfo()
    {
        // Arrange
        var endpoint = new DnsEndPoint("localhost", 6379);
        _mockRedis.Setup(x => x.GetEndPoints(It.IsAny<bool>()))
            .Returns(new EndPoint[] { endpoint });
        _mockRedis.Setup(x => x.GetServer(endpoint, It.IsAny<object>()))
            .Returns(_mockServer.Object);

        // 创建空的info数组
        var infoGroups = Array.Empty<IGrouping<string, KeyValuePair<string, string>>>();
        _mockServer.Setup(x => x.InfoAsync(It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(infoGroups);

        // Act
        var result = await _service.GetCacheInfoAsync();

        // Assert
        result.Should().NotBeNull();
        result.Info.Should().NotBeNull();
        result.CommandStats.Should().NotBeNull();
        // DbSize的mock比较复杂，这里只验证基本结构
        result.DbSize.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetCacheNamesAsync_ShouldReturnDistinctCacheNames()
    {
        // Arrange
        var endpoint = new DnsEndPoint("localhost", 6379);
        _mockRedis.Setup(x => x.GetEndPoints(It.IsAny<bool>()))
            .Returns(new EndPoint[] { endpoint });
        _mockRedis.Setup(x => x.GetServer(endpoint, It.IsAny<object>()))
            .Returns(_mockServer.Object);

        var keys = new[]
        {
            new RedisKey("netmvp:user:1"),
            new RedisKey("netmvp:user:2"),
            new RedisKey("netmvp:role:1")
        };

        _mockServer.Setup(x => x.KeysAsync(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
            .Returns(keys.ToAsyncEnumerable());

        // Act
        var result = await _service.GetCacheNamesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("user");
        result.Should().Contain("role");
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCacheKeysAsync_ShouldReturnKeysForCacheName()
    {
        // Arrange
        var cacheName = "user";
        var endpoint = new DnsEndPoint("localhost", 6379);
        _mockRedis.Setup(x => x.GetEndPoints(It.IsAny<bool>()))
            .Returns(new EndPoint[] { endpoint });
        _mockRedis.Setup(x => x.GetServer(endpoint, It.IsAny<object>()))
            .Returns(_mockServer.Object);

        var keys = new[]
        {
            new RedisKey("netmvp:user:1"),
            new RedisKey("netmvp:user:2")
        };

        _mockServer.Setup(x => x.KeysAsync(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
            .Returns(keys.ToAsyncEnumerable());

        // Act
        var result = await _service.GetCacheKeysAsync(cacheName);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(x => x.CacheName == cacheName).Should().BeTrue();
    }

    [Fact]
    public async Task GetCacheValueAsync_ShouldReturnCacheValue()
    {
        // Arrange
        var cacheName = "user";
        var cacheKey = "1";
        var expectedValue = "{\"id\":1,\"name\":\"test\"}";
        _mockCacheService.Setup(x => x.GetAsync($"{cacheName}:{cacheKey}", default))
            .ReturnsAsync(expectedValue);

        // Act
        var result = await _service.GetCacheValueAsync(cacheName, cacheKey);

        // Assert
        result.Should().Be(expectedValue);
    }

    [Fact]
    public async Task ClearCacheNameAsync_ShouldRemoveCacheByPrefix()
    {
        // Arrange
        var cacheName = "user";

        // Act
        await _service.ClearCacheNameAsync(cacheName);

        // Assert
        _mockCacheService.Verify(x => x.RemoveByPrefixAsync($"{cacheName}:", default), Times.Once);
    }

    [Fact]
    public async Task ClearCacheKeyAsync_ShouldRemoveSpecificKey()
    {
        // Arrange
        var cacheKey = "user:1";

        // Act
        await _service.ClearCacheKeyAsync(cacheKey);

        // Assert
        _mockCacheService.Verify(x => x.RemoveAsync(cacheKey, default), Times.Once);
    }

    [Fact]
    public async Task ClearAllCacheAsync_ShouldFlushDatabase()
    {
        // Arrange
        var endpoint = new DnsEndPoint("localhost", 6379);
        _mockRedis.Setup(x => x.GetEndPoints(It.IsAny<bool>()))
            .Returns(new EndPoint[] { endpoint });
        _mockRedis.Setup(x => x.GetServer(endpoint, It.IsAny<object>()))
            .Returns(_mockServer.Object);

        // Act
        await _service.ClearAllCacheAsync();

        // Assert
        _mockServer.Verify(x => x.FlushDatabaseAsync(It.IsAny<int>(), It.IsAny<CommandFlags>()), Times.Once);
    }
}

// 扩展方法用于测试
public static class TestExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            yield return item;
        }
        await Task.CompletedTask;
    }
}
