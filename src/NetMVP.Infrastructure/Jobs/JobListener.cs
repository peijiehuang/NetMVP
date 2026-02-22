using Microsoft.Extensions.Logging;
using Quartz;

namespace NetMVP.Infrastructure.Jobs;

/// <summary>
/// 任务监听器
/// </summary>
public class JobListener : IJobListener
{
    private readonly ILogger<JobListener> _logger;

    public JobListener(ILogger<JobListener> logger)
    {
        _logger = logger;
    }

    public string Name => "NetMVPJobListener";

    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        var jobKey = context.JobDetail.Key;
        _logger.LogInformation("任务即将执行: {JobName}.{JobGroup}, 触发时间: {FireTime}", 
            jobKey.Name, jobKey.Group, context.FireTimeUtc);
        return Task.CompletedTask;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        var jobKey = context.JobDetail.Key;
        _logger.LogWarning("任务执行被否决: {JobName}.{JobGroup}", jobKey.Name, jobKey.Group);
        return Task.CompletedTask;
    }

    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
    {
        var jobKey = context.JobDetail.Key;
        var runTime = context.JobRunTime;

        if (jobException != null)
        {
            _logger.LogError(jobException, "任务执行异常: {JobName}.{JobGroup}, 运行时间: {RunTime}ms", 
                jobKey.Name, jobKey.Group, runTime.TotalMilliseconds);
        }
        else
        {
            _logger.LogInformation("任务执行完成: {JobName}.{JobGroup}, 运行时间: {RunTime}ms", 
                jobKey.Name, jobKey.Group, runTime.TotalMilliseconds);
        }

        return Task.CompletedTask;
    }
}
