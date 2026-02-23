using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetMVP.Application.DTOs.Config;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.System;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.System;

public class SysConfigControllerTests : TestBase
{
    private readonly Mock<ISysConfigService> _configServiceMock;
    private readonly SysConfigController _controller;

    public SysConfigControllerTests()
    {
        _configServiceMock = new Mock<ISysConfigService>();
        _controller = new SysConfigController(_configServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetList_ShouldReturnConfigList()
    {
        var query = new ConfigQueryDto();
        _configServiceMock.Setup(x => x.GetConfigListAsync(It.IsAny<ConfigQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<ConfigDto>(), 0));

        var result = await _controller.GetList(query);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task GetInfo_WithValidId_ShouldReturnConfig()
    {
        var configId = 1;
        _configServiceMock.Setup(x => x.GetConfigByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConfigDto { ConfigId = configId });

        var result = await _controller.GetInfo(configId);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task GetConfigKey_WithValidKey_ShouldReturnValue()
    {
        var configKey = "test.key";
        _configServiceMock.Setup(x => x.GetConfigByKeyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("test-value");

        var result = await _controller.GetConfigKey(configKey);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Add_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new CreateConfigDto { ConfigName = "测试配置" };
        _configServiceMock.Setup(x => x.CreateConfigAsync(It.IsAny<CreateConfigDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _controller.Add(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Edit_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new UpdateConfigDto { ConfigId = 1, ConfigName = "更新配置" };
        _configServiceMock.Setup(x => x.UpdateConfigAsync(It.IsAny<UpdateConfigDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Edit(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Remove_WithValidIds_ShouldReturnSuccess()
    {
        var configIds = "1,2,3";
        _configServiceMock.Setup(x => x.DeleteConfigsAsync(It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Remove(configIds);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task RefreshCache_ShouldReturnSuccess()
    {
        _configServiceMock.Setup(x => x.RefreshConfigCacheAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.RefreshCache();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Export_ShouldReturnFile()
    {
        var query = new ConfigQueryDto();
        var fileData = new byte[] { 1, 2, 3 };
        _configServiceMock.Setup(x => x.ExportConfigsAsync(It.IsAny<ConfigQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileData);

        var result = await _controller.Export(query);

        result.Should().BeOfType<FileContentResult>();
    }
}
