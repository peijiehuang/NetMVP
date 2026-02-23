using FluentAssertions;
using Moq;
using NetMVP.Domain.Interfaces;
using NetMVP.WebApi.Controllers.Monitor;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.Monitor;

public class CacheControllerTests : TestBase
{
    private readonly Mock<ICacheMonitorService> _cacheMonitorServiceMock;
    private readonly CacheController _controller;

    public CacheControllerTests()
    {
        _cacheMonitorServiceMock = new Mock<ICacheMonitorService>();
        _controller = new CacheController(_cacheMonitorServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetCacheInfo_ShouldReturnCacheInfo()
    {
        _cacheMonitorServiceMock.Setup(x => x.GetCacheInfoAsync())
            .ReturnsAsync(new NetMVP.Domain.Interfaces.CacheInfoDto());

        var result = await _controller.GetCacheInfo();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task GetCacheNames_ShouldReturnNames()
    {
        _cacheMonitorServiceMock.Setup(x => x.GetCacheNamesAsync())
            .ReturnsAsync(new List<CacheNameDto> { new CacheNameDto { CacheName = "cache1" }, new CacheNameDto { CacheName = "cache2" } });

        var result = await _controller.GetCacheNames();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task GetCacheKeys_WithValidCacheName_ShouldReturnKeys()
    {
        var cacheName = "test-cache";
        _cacheMonitorServiceMock.Setup(x => x.GetCacheKeysAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<string>());

        var result = await _controller.GetCacheKeys(cacheName);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task GetCacheValue_WithValidKey_ShouldReturnValue()
    {
        var cacheName = "test-cache";
        var cacheKey = "test-key";
        _cacheMonitorServiceMock.Setup(x => x.GetCacheValueAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new CacheValueDto { CacheName = cacheName, CacheKey = cacheKey, CacheValue = "test-value" });

        var result = await _controller.GetCacheValue(cacheName, cacheKey);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task ClearCacheName_WithValidName_ShouldReturnSuccess()
    {
        var cacheName = "test-cache";
        _cacheMonitorServiceMock.Setup(x => x.ClearCacheNameAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.ClearCacheName(cacheName);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task ClearCacheKey_WithValidKey_ShouldReturnSuccess()
    {
        var cacheKey = "test-key";
        _cacheMonitorServiceMock.Setup(x => x.ClearCacheKeyAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.ClearCacheKey(cacheKey);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task ClearAllCache_ShouldReturnSuccess()
    {
        _cacheMonitorServiceMock.Setup(x => x.ClearAllCacheAsync())
            .Returns(Task.CompletedTask);

        var result = await _controller.ClearAllCache();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }
}
