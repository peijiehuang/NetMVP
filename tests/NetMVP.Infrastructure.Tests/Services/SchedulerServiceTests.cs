using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Jobs;
using NetMVP.Infrastructure.Services.Scheduler;
using Quartz;
using Quartz.Impl;
using Xunit;

namespace NetMVP.Infrastructure.Tests.Services;

public class SchedulerServiceTests : IAsyncLifetime
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly Mock<ILogger<SchedulerService>> _mockLogger;
    private readonly Mock<IJobExecutor> _mockJobExecutor;
    private readonly SchedulerService _service;

    public SchedulerServiceTests()
    {
        _schedulerFactory = new StdSchedulerFactory();
        _mockLogger = new Mock<ILogger<SchedulerService>>();
        _mockJobExecutor = new Mock<IJobExecutor>();
        _service = new SchedulerService(_schedulerFactory, _mockLogger.Object);
    }

    public async Task InitializeAsync()
    {
        await _service.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _service.StopAsync();
    }

    [Fact]
    public async Task AddJobAsync_ShouldAddJob()
    {
        // Arrange
        var jobName = "TestJob";
        var jobGroup = "TestGroup";
        var cronExpression = "0 0 12 * * ?"; // 每天中午12点

        // Act
        await _service.AddJobAsync(jobName, jobGroup, cronExpression);

        // Assert - 任务应该被添加（不抛出异常即为成功）
    }

    [Fact]
    public async Task AddJobAsync_WithParameters_ShouldAddJobWithParameters()
    {
        // Arrange
        var jobName = "TestJobWithParams";
        var jobGroup = "TestGroup";
        var cronExpression = "0 0 12 * * ?";
        var parameters = new Dictionary<string, object>
        {
            { "param1", "value1" },
            { "param2", 123 }
        };

        // Act
        await _service.AddJobAsync(jobName, jobGroup, cronExpression, parameters);

        // Assert - 任务应该被添加
    }

    [Fact]
    public async Task AddJobAsync_DuplicateJob_ShouldThrowException()
    {
        // Arrange
        var jobName = "DuplicateJob";
        var jobGroup = "TestGroup";
        var cronExpression = "0 0 12 * * ?";

        await _service.AddJobAsync(jobName, jobGroup, cronExpression);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.AddJobAsync(jobName, jobGroup, cronExpression));
    }

    [Fact]
    public async Task DeleteJobAsync_ShouldDeleteJob()
    {
        // Arrange
        var jobName = "JobToDelete";
        var jobGroup = "TestGroup";
        var cronExpression = "0 0 12 * * ?";

        await _service.AddJobAsync(jobName, jobGroup, cronExpression);

        // Act
        await _service.DeleteJobAsync(jobName, jobGroup);

        // Assert - 删除后应该可以再次添加同名任务
        await _service.AddJobAsync(jobName, jobGroup, cronExpression);
    }

    [Fact]
    public async Task UpdateJobAsync_ShouldUpdateJob()
    {
        // Arrange
        var jobName = "JobToUpdate";
        var jobGroup = "TestGroup";
        var oldCronExpression = "0 0 12 * * ?";
        var newCronExpression = "0 0 18 * * ?"; // 改为下午6点

        await _service.AddJobAsync(jobName, jobGroup, oldCronExpression);

        // Act
        await _service.UpdateJobAsync(jobName, jobGroup, newCronExpression);

        // Assert - 更新后任务应该存在
    }

    [Fact]
    public async Task UpdateJobAsync_NonExistentJob_ShouldThrowException()
    {
        // Arrange
        var jobName = "NonExistentJob";
        var jobGroup = "TestGroup";
        var cronExpression = "0 0 12 * * ?";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.UpdateJobAsync(jobName, jobGroup, cronExpression));
    }

    [Fact]
    public async Task PauseJobAsync_ShouldPauseJob()
    {
        // Arrange
        var jobName = "JobToPause";
        var jobGroup = "TestGroup";
        var cronExpression = "0 0 12 * * ?";

        await _service.AddJobAsync(jobName, jobGroup, cronExpression);

        // Act
        await _service.PauseJobAsync(jobName, jobGroup);

        // Assert - 暂停后应该可以恢复
        await _service.ResumeJobAsync(jobName, jobGroup);
    }

    [Fact]
    public async Task ResumeJobAsync_ShouldResumeJob()
    {
        // Arrange
        var jobName = "JobToResume";
        var jobGroup = "TestGroup";
        var cronExpression = "0 0 12 * * ?";

        await _service.AddJobAsync(jobName, jobGroup, cronExpression);
        await _service.PauseJobAsync(jobName, jobGroup);

        // Act
        await _service.ResumeJobAsync(jobName, jobGroup);

        // Assert - 恢复成功（不抛出异常）
    }

    [Fact]
    public async Task TriggerJobAsync_ShouldTriggerJob()
    {
        // Arrange
        var jobName = "JobToTrigger";
        var jobGroup = "TestGroup";
        var cronExpression = "0 0 12 * * ?";

        await _service.AddJobAsync(jobName, jobGroup, cronExpression);

        // Act
        await _service.TriggerJobAsync(jobName, jobGroup);

        // Assert - 触发成功（不抛出异常）
        await Task.Delay(100); // 等待任务执行
    }

    [Fact]
    public void ValidateCronExpression_ValidExpression_ShouldReturnTrue()
    {
        // Arrange
        var cronExpression = "0 0 12 * * ?";

        // Act
        var result = _service.ValidateCronExpression(cronExpression);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateCronExpression_InvalidExpression_ShouldReturnFalse()
    {
        // Arrange
        var cronExpression = "invalid cron";

        // Act
        var result = _service.ValidateCronExpression(cronExpression);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetNextFireTime_ValidExpression_ShouldReturnNextTime()
    {
        // Arrange
        var cronExpression = "0 0 12 * * ?"; // 每天中午12点

        // Act
        var result = _service.GetNextFireTime(cronExpression);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAfter(DateTimeOffset.Now);
    }

    [Fact]
    public void GetNextFireTime_InvalidExpression_ShouldReturnNull()
    {
        // Arrange
        var cronExpression = "invalid cron";

        // Act
        var result = _service.GetNextFireTime(cronExpression);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task PauseAsync_ShouldPauseScheduler()
    {
        // Act
        await _service.PauseAsync();

        // Assert - 暂停后应该可以恢复
        await _service.ResumeAsync();
    }

    [Fact]
    public async Task ResumeAsync_ShouldResumeScheduler()
    {
        // Arrange
        await _service.PauseAsync();

        // Act
        await _service.ResumeAsync();

        // Assert - 恢复成功（不抛出异常）
    }
}
