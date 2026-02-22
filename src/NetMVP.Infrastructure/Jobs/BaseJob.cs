using Microsoft.Extensions.Logging;
using NetMVP.Domain.Interfaces;
using Quartz;

namespace NetMVP.Infrastructure.Jobs;

/// <summary>
/// 任务基类
/// </summary>
public class BaseJob : IJob
{
    private readonly IJobExecutor _jobExecutor;
    private readonly ILogger<BaseJob> _logger;

    public BaseJob(IJobExecutor jobExecutor, ILogger<BaseJob> logger)
    {
        _jobExecutor = jobExecutor;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobKey = context.JobDetail.Key;
        var jobName = jobKey.Name;
        var jobGroup = jobKey.Group;

        try
        {
            _logger.LogInformation("任务开始执行: {JobName}.{JobGroup}", jobName, jobGroup);

            // 获取任务参数
            var parameters = context.JobDetail.JobDataMap
                .ToDictionary(x => x.Key, x => x.Value);

            // 执行任务
            await _jobExecutor.ExecuteAsync(jobName, jobGroup, parameters, context.CancellationToken);

            _logger.LogInformation("任务执行成功: {JobName}.{JobGroup}", jobName, jobGroup);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "任务执行失败: {JobName}.{JobGroup}", jobName, jobGroup);
            throw;
        }
    }
}
