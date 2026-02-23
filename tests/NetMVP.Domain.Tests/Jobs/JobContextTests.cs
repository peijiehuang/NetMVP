using NetMVP.Domain.Jobs;
using Xunit;

namespace NetMVP.Domain.Tests.Jobs;

/// <summary>
/// JobContext 并发测试
/// </summary>
public class JobContextTests
{
    [Fact]
    public async Task JobContext_ShouldBeIsolated_InConcurrentTasks()
    {
        // Arrange
        var task1Logs = new List<string>();
        var task2Logs = new List<string>();
        var task3Logs = new List<string>();

        // Act - 模拟3个任务并发执行
        var tasks = new[]
        {
            Task.Run(async () =>
            {
                var context = new JobExecutionContext { JobName = "Task1" };
                JobContext.Current = context;
                
                JobContext.Log("Task1 - Step 1");
                await Task.Delay(10);
                JobContext.Log("Task1 - Step 2");
                await Task.Delay(10);
                JobContext.Log("Task1 - Step 3");
                
                task1Logs.Add(context.GetLogs());
                JobContext.Current = null;
            }),
            
            Task.Run(async () =>
            {
                var context = new JobExecutionContext { JobName = "Task2" };
                JobContext.Current = context;
                
                JobContext.Log("Task2 - Step 1");
                await Task.Delay(10);
                JobContext.Log("Task2 - Step 2");
                await Task.Delay(10);
                JobContext.Log("Task2 - Step 3");
                
                task2Logs.Add(context.GetLogs());
                JobContext.Current = null;
            }),
            
            Task.Run(async () =>
            {
                var context = new JobExecutionContext { JobName = "Task3" };
                JobContext.Current = context;
                
                JobContext.Log("Task3 - Step 1");
                await Task.Delay(10);
                JobContext.Log("Task3 - Step 2");
                await Task.Delay(10);
                JobContext.Log("Task3 - Step 3");
                
                task3Logs.Add(context.GetLogs());
                JobContext.Current = null;
            })
        };

        await Task.WhenAll(tasks);

        // Assert - 验证每个任务的日志都是独立的
        Assert.Single(task1Logs);
        Assert.Contains("Task1 - Step 1", task1Logs[0]);
        Assert.Contains("Task1 - Step 2", task1Logs[0]);
        Assert.Contains("Task1 - Step 3", task1Logs[0]);
        Assert.DoesNotContain("Task2", task1Logs[0]);
        Assert.DoesNotContain("Task3", task1Logs[0]);

        Assert.Single(task2Logs);
        Assert.Contains("Task2 - Step 1", task2Logs[0]);
        Assert.Contains("Task2 - Step 2", task2Logs[0]);
        Assert.Contains("Task2 - Step 3", task2Logs[0]);
        Assert.DoesNotContain("Task1", task2Logs[0]);
        Assert.DoesNotContain("Task3", task2Logs[0]);

        Assert.Single(task3Logs);
        Assert.Contains("Task3 - Step 1", task3Logs[0]);
        Assert.Contains("Task3 - Step 2", task3Logs[0]);
        Assert.Contains("Task3 - Step 3", task3Logs[0]);
        Assert.DoesNotContain("Task1", task3Logs[0]);
        Assert.DoesNotContain("Task2", task3Logs[0]);
    }

    [Fact]
    public async Task JobContext_ShouldMaintainContext_AcrossAwait()
    {
        // Arrange
        var context = new JobExecutionContext { JobName = "TestTask" };
        JobContext.Current = context;

        // Act
        JobContext.Log("Before await");
        await Task.Delay(50);
        JobContext.Log("After await");

        var logs = context.GetLogs();

        // Assert
        Assert.Contains("Before await", logs);
        Assert.Contains("After await", logs);
    }

    [Fact]
    public void JobContext_ShouldReturnNull_WhenNotSet()
    {
        // Arrange & Act
        var current = JobContext.Current;

        // Assert
        Assert.Null(current);
    }
}
