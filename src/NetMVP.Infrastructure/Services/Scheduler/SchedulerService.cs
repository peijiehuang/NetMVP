using Microsoft.Extensions.Logging;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Jobs;
using Quartz;

namespace NetMVP.Infrastructure.Services.Scheduler;

/// <summary>
/// 任务调度服务实现
/// </summary>
public class SchedulerService : ISchedulerService
{
    private readonly IScheduler _scheduler;
    private readonly ILogger<SchedulerService> _logger;

    public SchedulerService(ISchedulerFactory schedulerFactory, ILogger<SchedulerService> logger)
    {
        _scheduler = schedulerFactory.GetScheduler().Result;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (!_scheduler.IsStarted)
        {
            await _scheduler.Start(cancellationToken);
            _logger.LogInformation("调度器已启动");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!_scheduler.IsShutdown)
        {
            await _scheduler.Shutdown(cancellationToken);
            _logger.LogInformation("调度器已停止");
        }
    }

    public async Task PauseAsync(CancellationToken cancellationToken = default)
    {
        await _scheduler.PauseAll(cancellationToken);
        _logger.LogInformation("调度器已暂停");
    }

    public async Task ResumeAsync(CancellationToken cancellationToken = default)
    {
        await _scheduler.ResumeAll(cancellationToken);
        _logger.LogInformation("调度器已恢复");
    }

    public async Task AddJobAsync(string jobName, string jobGroup, string cronExpression, 
        Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default)
    {
        var jobKey = new JobKey(jobName, jobGroup);

        // 检查任务是否已存在
        if (await _scheduler.CheckExists(jobKey, cancellationToken))
        {
            throw new InvalidOperationException($"任务已存在: {jobName}.{jobGroup}");
        }

        // 创建任务
        var jobBuilder = JobBuilder.Create<BaseJob>()
            .WithIdentity(jobKey)
            .WithDescription($"任务: {jobName}");

        // 添加参数
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                jobBuilder.UsingJobData(param.Key, param.Value?.ToString() ?? string.Empty);
            }
        }

        var job = jobBuilder.Build();

        // 创建触发器
        var trigger = TriggerBuilder.Create()
            .WithIdentity($"{jobName}_trigger", jobGroup)
            .WithCronSchedule(cronExpression)
            .Build();

        await _scheduler.ScheduleJob(job, trigger, cancellationToken);
        _logger.LogInformation("任务已添加: {JobName}.{JobGroup}, Cron: {CronExpression}", jobName, jobGroup, cronExpression);
    }

    public async Task UpdateJobAsync(string jobName, string jobGroup, string cronExpression, 
        Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default)
    {
        var jobKey = new JobKey(jobName, jobGroup);

        // 检查任务是否存在
        if (!await _scheduler.CheckExists(jobKey, cancellationToken))
        {
            throw new InvalidOperationException($"任务不存在: {jobName}.{jobGroup}");
        }

        // 删除旧任务
        await DeleteJobAsync(jobName, jobGroup, cancellationToken);

        // 添加新任务
        await AddJobAsync(jobName, jobGroup, cronExpression, parameters, cancellationToken);

        _logger.LogInformation("任务已更新: {JobName}.{JobGroup}", jobName, jobGroup);
    }

    public async Task DeleteJobAsync(string jobName, string jobGroup, CancellationToken cancellationToken = default)
    {
        var jobKey = new JobKey(jobName, jobGroup);
        var deleted = await _scheduler.DeleteJob(jobKey, cancellationToken);

        if (deleted)
        {
            _logger.LogInformation("任务已删除: {JobName}.{JobGroup}", jobName, jobGroup);
        }
        else
        {
            _logger.LogWarning("任务删除失败，任务不存在: {JobName}.{JobGroup}", jobName, jobGroup);
        }
    }

    public async Task PauseJobAsync(string jobName, string jobGroup, CancellationToken cancellationToken = default)
    {
        var jobKey = new JobKey(jobName, jobGroup);
        await _scheduler.PauseJob(jobKey, cancellationToken);
        _logger.LogInformation("任务已暂停: {JobName}.{JobGroup}", jobName, jobGroup);
    }

    public async Task ResumeJobAsync(string jobName, string jobGroup, CancellationToken cancellationToken = default)
    {
        var jobKey = new JobKey(jobName, jobGroup);
        await _scheduler.ResumeJob(jobKey, cancellationToken);
        _logger.LogInformation("任务已恢复: {JobName}.{JobGroup}", jobName, jobGroup);
    }

    public async Task TriggerJobAsync(string jobName, string jobGroup, CancellationToken cancellationToken = default)
    {
        var jobKey = new JobKey(jobName, jobGroup);
        await _scheduler.TriggerJob(jobKey, cancellationToken);
        _logger.LogInformation("任务已触发: {JobName}.{JobGroup}", jobName, jobGroup);
    }

    public bool ValidateCronExpression(string cronExpression)
    {
        try
        {
            CronExpression.ValidateExpression(cronExpression);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public DateTimeOffset? GetNextFireTime(string cronExpression)
    {
        try
        {
            var cron = new CronExpression(cronExpression);
            return cron.GetNextValidTimeAfter(DateTimeOffset.Now);
        }
        catch
        {
            return null;
        }
    }
}
