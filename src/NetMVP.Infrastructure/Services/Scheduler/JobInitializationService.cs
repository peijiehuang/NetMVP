using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Infrastructure.Services.Scheduler;

/// <summary>
/// 任务初始化服务
/// 项目启动时，从数据库加载所有任务到调度器
/// </summary>
public class JobInitializationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<JobInitializationService> _logger;

    public JobInitializationService(
        IServiceProvider serviceProvider,
        ILogger<JobInitializationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("开始初始化定时任务...");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var schedulerService = scope.ServiceProvider.GetRequiredService<ISchedulerService>();
            var jobRepository = scope.ServiceProvider.GetRequiredService<ISysJobRepository>();

            // 启动调度器
            await schedulerService.StartAsync(cancellationToken);

            // 从数据库加载所有任务
            var jobs = await jobRepository.GetQueryable().ToListAsync(cancellationToken);

            _logger.LogInformation("从数据库加载了 {Count} 个任务", jobs.Count);

            // 将状态为"正常"的任务添加到调度器
            var loadedCount = 0;
            foreach (var job in jobs)
            {
                if (job.Status == "0") // 0表示正常
                {
                    try
                    {
                        await schedulerService.AddJobAsync(
                            job.JobName,
                            job.JobGroup,
                            job.CronExpression,
                            null,
                            cancellationToken);

                        loadedCount++;
                        _logger.LogInformation("已加载任务: {JobName}.{JobGroup}", job.JobName, job.JobGroup);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "加载任务失败: {JobName}.{JobGroup}", job.JobName, job.JobGroup);
                    }
                }
            }

            _logger.LogInformation("定时任务初始化完成，成功加载 {LoadedCount}/{TotalCount} 个任务", loadedCount, jobs.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "定时任务初始化失败");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("停止定时任务调度器...");
        return Task.CompletedTask;
    }
}
