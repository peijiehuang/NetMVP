using AutoMapper;
using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.Config;
using NetMVP.Application.Mappings;
using NetMVP.Application.Services.Impl;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Domain.Interfaces;
using Xunit;

namespace NetMVP.Application.Tests.Services;

/// <summary>
/// 参数配置服务测试
/// </summary>
public class SysConfigServiceTests
{
    private readonly Mock<IRepository<SysConfig>> _configRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IExcelService> _excelServiceMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly IMapper _mapper;
    private readonly SysConfigService _configService;

    public SysConfigServiceTests()
    {
        _configRepositoryMock = new Mock<IRepository<SysConfig>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _excelServiceMock = new Mock<IExcelService>();
        _cacheServiceMock = new Mock<ICacheService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _configService = new SysConfigService(
            _configRepositoryMock.Object,
            _mapper,
            _unitOfWorkMock.Object,
            _excelServiceMock.Object,
            _cacheServiceMock.Object
        );
    }

    [Fact]
    public async Task GetConfigByIdAsync_WhenConfigNotExists_ShouldReturnNull()
    {
        // Arrange
        var configId = 999;
        _configRepositoryMock.Setup(x => x.GetByIdAsync(configId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysConfig?)null);

        // Act
        var result = await _configService.GetConfigByIdAsync(configId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetConfigByIdAsync_WhenConfigExists_ShouldReturnConfig()
    {
        // Arrange
        var configId = 1;
        var config = new SysConfig
        {
            ConfigId = configId,
            ConfigName = "测试参数",
            ConfigKey = "test.key",
            ConfigValue = "test value",
            ConfigType = YesNo.No
        };

        _configRepositoryMock.Setup(x => x.GetByIdAsync(configId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        // Act
        var result = await _configService.GetConfigByIdAsync(configId);

        // Assert
        result.Should().NotBeNull();
        result!.ConfigId.Should().Be(configId);
        result.ConfigName.Should().Be("测试参数");
        result.ConfigKey.Should().Be("test.key");
    }

    [Fact]
    public async Task DeleteConfigAsync_WhenConfigNotExists_ShouldThrowException()
    {
        // Arrange
        var configId = 1;
        _configRepositoryMock.Setup(x => x.GetByIdAsync(configId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysConfig?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _configService.DeleteConfigAsync(configId));
    }

    [Fact]
    public async Task DeleteConfigAsync_WhenConfigIsBuiltIn_ShouldThrowException()
    {
        // Arrange
        var configId = 1;
        var config = new SysConfig
        {
            ConfigId = configId,
            ConfigName = "系统内置参数",
            ConfigKey = "sys.builtin",
            ConfigValue = "value",
            ConfigType = YesNo.Yes
        };

        _configRepositoryMock.Setup(x => x.GetByIdAsync(configId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _configService.DeleteConfigAsync(configId));
    }

    [Fact]
    public async Task GetConfigByKeyAsync_WhenCacheExists_ShouldReturnCachedValue()
    {
        // Arrange
        var configKey = "test.key";
        var cachedValue = "cached value";

        _cacheServiceMock.Setup(x => x.GetAsync<string>($"config:{configKey}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedValue);

        // Act
        var result = await _configService.GetConfigByKeyAsync(configKey);

        // Assert
        result.Should().Be(cachedValue);
        _configRepositoryMock.Verify(x => x.GetQueryable(), Times.Never);
    }
}
