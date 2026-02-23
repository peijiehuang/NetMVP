using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetMVP.Application.DTOs.OperLog;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.Monitor;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.Monitor;

public class SysOperLogControllerTests : TestBase
{
    private readonly Mock<ISysOperLogService> _operLogServiceMock;
    private readonly SysOperLogController _controller;

    public SysOperLogControllerTests()
    {
        _operLogServiceMock = new Mock<ISysOperLogService>();
        _controller = new SysOperLogController(_operLogServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetList_ShouldReturnOperLogList()
    {
        var query = new OperLogQueryDto();
        _operLogServiceMock.Setup(x => x.GetOperLogListAsync(It.IsAny<OperLogQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<OperLogDto>(), 0));

        var result = await _controller.GetList(query);

        result.Should().NotBeNull();
        result.Total.Should().Be(0);
    }

    [Fact]
    public async Task Delete_WithValidIds_ShouldReturnSuccess()
    {
        var operIds = "1,2,3";
        _operLogServiceMock.Setup(x => x.DeleteOperLogsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Delete(operIds);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Clean_ShouldReturnSuccess()
    {
        _operLogServiceMock.Setup(x => x.CleanOperLogAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Clean();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Export_ShouldReturnFile()
    {
        var query = new OperLogQueryDto();
        var fileData = new byte[] { 1, 2, 3 };
        _operLogServiceMock.Setup(x => x.ExportOperLogsAsync(It.IsAny<OperLogQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileData);

        var result = await _controller.Export(query);

        result.Should().BeOfType<FileContentResult>();
    }
}
