using Microsoft.Extensions.Logging;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Infrastructure.Jobs;

/// <summary>
/// 默认任务执行器
/// </summary>
public class DefaultJobExecutor : IJobExecutor
{
    private readonly ILogger<DefaultJobExecutor> _logger;

    public DefaultJobExecutor(ILogger<DefaultJobExecutor> logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync(string jobName, string jobGroup, Dictionary<string, object>? parameters, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("执行任务: {JobName}.{JobGroup}", jobName, jobGroup);
        
        if (parameters != null && parameters.Count > 0)
        {
            _logger.LogInformation("任务参数: {Parameters}", string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}")));
        }

        return Task.CompletedTask;
    }
}
