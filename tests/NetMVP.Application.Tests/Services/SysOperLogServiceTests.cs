using AutoMapper;
using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.OperLog;
using NetMVP.Application.Mappings;
using NetMVP.Application.Services.Impl;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Domain.Interfaces;
using Xunit;

namespace NetMVP.Application.Tests.Services;

/// <summary>
/// 操作日志服务测试
/// </summary>
public class SysOperLogServiceTests
{
    private readonly Mock<ISysOperLogRepository> _operLogRepositoryMock;
    private readonly Mock<IExcelService> _excelServiceMock;
    private readonly IMapper _mapper;
    private readonly SysOperLogService _service;

    public SysOperLogServiceTests()
    {
        _operLogRepositoryMock = new Mock<ISysOperLogRepository>();
        _excelServiceMock = new Mock<IExcelService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _service = new SysOperLogService(
            _operLogRepositoryMock.Object,
            _excelServiceMock.Object,
            _mapper);
    }

    [Fact]
    public async Task GetOperLogByIdAsync_ShouldReturnLog_WhenExists()
    {
        // Arrange
        var log = new SysOperLog
        {
            OperId = 1,
            Title = "用户管理",
            BusinessType = BusinessType.Insert,
            OperTime = DateTime.Now
        };

        var logs = new List<SysOperLog> { log }.AsQueryable();
        _operLogRepositoryMock.Setup(x => x.GetQueryable()).Returns(logs);

        // Act
        var result = await _service.GetOperLogByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.OperId.Should().Be(1);
        result.Title.Should().Be("用户管理");
    }

    [Fact]
    public async Task GetOperLogByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var logs = new List<SysOperLog>().AsQueryable();
        _operLogRepositoryMock.Setup(x => x.GetQueryable()).Returns(logs);

        // Act
        var result = await _service.GetOperLogByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateOperLogAsync_ShouldCreateLog()
    {
        // Arrange
        var dto = new CreateOperLogDto
        {
            Title = "用户管理",
            BusinessType = BusinessType.Insert,
            OperName = "admin",
            Status = CommonStatus.Success
        };

        long capturedOperId = 0;
        _operLogRepositoryMock.Setup(x => x.AddAsync(It.IsAny<SysOperLog>(), It.IsAny<CancellationToken>()))
            .Callback<SysOperLog, CancellationToken>((log, _) =>
            {
                log.OperId = 1;
                capturedOperId = log.OperId;
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateOperLogAsync(dto);

        // Assert
        result.Should().Be(1);
        _operLogRepositoryMock.Verify(x => x.AddAsync(
            It.Is<SysOperLog>(l => l.Title == "用户管理" && l.BusinessType == BusinessType.Insert),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOperLogAsync_ShouldDeleteLog_WhenExists()
    {
        // Arrange
        var log = new SysOperLog { OperId = 1 };
        var logs = new List<SysOperLog> { log }.AsQueryable();
        _operLogRepositoryMock.Setup(x => x.GetQueryable()).Returns(logs);

        // Act
        await _service.DeleteOperLogAsync(1);

        // Assert
        _operLogRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<SysOperLog>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOperLogsAsync_ShouldDeleteMultipleLogs()
    {
        // Arrange
        var log1 = new SysOperLog { OperId = 1 };
        var log2 = new SysOperLog { OperId = 2 };
        var logs = new List<SysOperLog> { log1, log2 };
        
        _operLogRepositoryMock.Setup(x => x.GetQueryable())
            .Returns(() => logs.AsQueryable());

        // Act
        await _service.DeleteOperLogsAsync(new[] { 1L, 2L });

        // Assert
        _operLogRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<SysOperLog>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CleanOperLogAsync_ShouldCleanAllLogs()
    {
        // Act
        await _service.CleanOperLogAsync();

        // Assert
        _operLogRepositoryMock.Verify(x => x.CleanAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
