using AutoMapper;
using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.LoginInfo;
using NetMVP.Application.Services.Impl;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Tests.Services;

public class SysLoginInfoServiceTests
{
    private readonly Mock<ISysLoginInfoRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IExcelService> _mockExcelService;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly SysLoginInfoService _service;

    public SysLoginInfoServiceTests()
    {
        _mockRepository = new Mock<ISysLoginInfoRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockExcelService = new Mock<IExcelService>();
        _mockCacheService = new Mock<ICacheService>();
        _service = new SysLoginInfoService(
            _mockRepository.Object,
            _mockMapper.Object,
            _mockExcelService.Object,
            _mockCacheService.Object);
    }

    [Fact]
    public async Task CreateLoginInfoAsync_ShouldCreateLoginInfo()
    {
        // Arrange
        var dto = new CreateLoginInfoDto
        {
            UserName = "admin",
            IpAddr = "127.0.0.1",
            LoginLocation = "内网IP",
            Browser = "Chrome",
            Os = "Windows",
            Status = CommonConstants.SUCCESS,
            Msg = "登录成功"
        };

        long capturedInfoId = 0;
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<SysLoginInfo>(), default))
            .Callback<SysLoginInfo, CancellationToken>((entity, _) =>
            {
                entity.InfoId = 1;
                capturedInfoId = entity.InfoId;
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateLoginInfoAsync(dto);

        // Assert
        result.Should().Be(1);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<SysLoginInfo>(), default), Times.Once);
    }

    [Fact]
    public async Task DeleteLoginInfoAsync_WhenExists_ShouldReturnTrue()
    {
        // Arrange
        var infoId = 1L;
        var entity = new SysLoginInfo { InfoId = infoId };
        
        var mockQueryable = new List<SysLoginInfo> { entity }.AsQueryable();
        _mockRepository.Setup(x => x.GetQueryable())
            .Returns(mockQueryable);

        // Act
        var result = await _service.DeleteLoginInfoAsync(infoId);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<SysLoginInfo>(), default), Times.Once);
    }

    [Fact]
    public async Task DeleteLoginInfoAsync_WhenNotExists_ShouldReturnFalse()
    {
        // Arrange
        var infoId = 1L;
        
        var mockQueryable = new List<SysLoginInfo>().AsQueryable();
        _mockRepository.Setup(x => x.GetQueryable())
            .Returns(mockQueryable);

        // Act
        var result = await _service.DeleteLoginInfoAsync(infoId);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<SysLoginInfo>(), default), Times.Never);
    }

    [Fact]
    public async Task CleanLoginInfoAsync_ShouldCleanAllLogs()
    {
        // Arrange
        _mockRepository.Setup(x => x.CleanAsync(default))
            .ReturnsAsync(100);

        // Act
        var result = await _service.CleanLoginInfoAsync();

        // Assert
        result.Should().Be(100);
        _mockRepository.Verify(x => x.CleanAsync(default), Times.Once);
    }

    [Fact]
    public async Task UnlockUserAsync_ShouldRemovePasswordErrorCache()
    {
        // Arrange
        var userName = "admin";

        // Act
        var result = await _service.UnlockUserAsync(userName);

        // Assert
        result.Should().BeTrue();
        _mockCacheService.Verify(x => x.RemoveAsync($"pwd_err_cnt:{userName}", default), Times.Once);
    }
}
