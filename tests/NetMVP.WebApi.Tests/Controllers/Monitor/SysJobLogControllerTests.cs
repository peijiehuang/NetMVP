using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Job;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.Monitor;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.Monitor;

public class SysJobLogControllerTests : TestBase
{
    private readonly Mock<ISysJobLogService> _jobLogServiceMock;
    private readonly SysJobLogController _controller;

    public SysJobLogControllerTests()
    {
        _jobLogServiceMock = new Mock<ISysJobLogService>();
        _controller = new SysJobLogController(_jobLogServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetList_ShouldReturnJobLogList()
    {
        var query = new JobLogQueryDto();
        var pagedResult = new PagedResult<JobLogDto>
        {
            Rows = new List<JobLogDto>(),
            Total = 0
        };
        _jobLogServiceMock.Setup(x => x.GetJobLogListAsync(It.IsAny<JobLogQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _controller.GetList(query);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnJobLog()
    {
        var jobLogId = 1L;
        _jobLogServiceMock.Setup(x => x.GetJobLogByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new JobLogDto { JobLogId = jobLogId });

        var result = await _controller.GetById(jobLogId);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnSuccess()
    {
        var jobLogId = 1L;
        _jobLogServiceMock.Setup(x => x.DeleteJobLogAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Delete(jobLogId);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Clean_ShouldReturnSuccess()
    {
        _jobLogServiceMock.Setup(x => x.CleanJobLogAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Clean();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Export_ShouldReturnFile()
    {
        var query = new JobLogQueryDto();
        var fileData = new byte[] { 1, 2, 3 };
        _jobLogServiceMock.Setup(x => x.ExportJobLogsAsync(It.IsAny<JobLogQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileData);

        var result = await _controller.Export(query);

        result.Should().BeOfType<FileContentResult>();
    }
}
