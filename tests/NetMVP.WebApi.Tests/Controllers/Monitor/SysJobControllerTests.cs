using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Job;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.Monitor;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.Monitor;

public class SysJobControllerTests : TestBase
{
    private readonly Mock<ISysJobService> _jobServiceMock;
    private readonly SysJobController _controller;

    public SysJobControllerTests()
    {
        _jobServiceMock = new Mock<ISysJobService>();
        _controller = new SysJobController(_jobServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetList_ShouldReturnJobList()
    {
        var query = new JobQueryDto();
        var pagedResult = new PagedResult<JobDto>
        {
            Rows = new List<JobDto>(),
            Total = 0
        };
        _jobServiceMock.Setup(x => x.GetJobListAsync(It.IsAny<JobQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _controller.GetList(query);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnJob()
    {
        var jobId = 1L;
        _jobServiceMock.Setup(x => x.GetJobByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new JobDto { JobId = jobId });

        var result = await _controller.GetById(jobId);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Create_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new CreateJobDto { JobName = "测试任务" };
        _jobServiceMock.Setup(x => x.CreateJobAsync(It.IsAny<CreateJobDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1L);

        var result = await _controller.Create(dto);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Update_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new UpdateJobDto { JobId = 1, JobName = "更新任务" };
        _jobServiceMock.Setup(x => x.UpdateJobAsync(It.IsAny<UpdateJobDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Update(dto);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnSuccess()
    {
        var jobId = 1L;
        _jobServiceMock.Setup(x => x.DeleteJobAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Delete(jobId);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ChangeStatus_WithValidRequest_ShouldReturnSuccess()
    {
        var request = new ChangeJobStatusRequest { JobId = 1, Status = "0" };
        _jobServiceMock.Setup(x => x.ChangeJobStatusAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.ChangeStatus(request);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Run_WithValidRequest_ShouldReturnSuccess()
    {
        var request = new RunJobRequest { JobId = 1, JobGroup = "DEFAULT" };
        _jobServiceMock.Setup(x => x.RunJobAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Run(request);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Export_ShouldReturnFile()
    {
        var query = new JobQueryDto();
        var fileData = new byte[] { 1, 2, 3 };
        _jobServiceMock.Setup(x => x.ExportJobsAsync(It.IsAny<JobQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileData);

        var result = await _controller.Export(query);

        result.Should().BeOfType<FileContentResult>();
    }
}
