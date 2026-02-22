using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Infrastructure.Jobs;

/// <summary>
/// 带日志记录的任务执行器
/// </summary>
public class LoggingJobExecutor : IJobExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LoggingJobExecutor> _logger;

    public LoggingJobExecutor(IServiceProvider serviceProvider, ILogger<LoggingJobExecutor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ExecuteAsync(string jobName, string jobGroup, Dictionary<string, object>? parameters, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.Now;
        var invokeTarget = $"{jobName}.{jobGroup}";
        string status = "0";
        string? exceptionInfo = null;
        string jobMessage;

        try
        {
            _logger.LogInformation("开始执行任务: {JobName}.{JobGroup}", jobName, jobGroup);

            // 这里可以根据invokeTarget解析并执行具体的任务
            // 示例：简单的日志记录
            if (parameters != null && parameters.Count > 0)
            {
                _logger.LogInformation("任务参数: {Parameters}", string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}")));
            }

            // 模拟任务执行
            await Task.Delay(100, cancellationToken);

            var endTime = DateTime.Now;
            var duration = (endTime - startTime).TotalMilliseconds;
            jobMessage = $"任务执行成功，耗时: {duration}ms";

            _logger.LogInformation("任务执行成功: {JobName}.{JobGroup}, 耗时: {Duration}ms", jobName, jobGroup, duration);
        }
        catch (Exception ex)
        {
            status = "1";
            exceptionInfo = ex.ToString();
            jobMessage = $"任务执行失败: {ex.Message}";
            _logger.LogError(ex, "任务执行失败: {JobName}.{JobGroup}", jobName, jobGroup);
        }

        // 记录任务日志 - 通过事件或其他方式，避免直接依赖Application层
        // 这里暂时只记录到日志，实际的数据库记录可以通过监听器或其他方式实现
        _logger.LogInformation("任务日志: {JobName}.{JobGroup}, 状态: {Status}, 消息: {Message}", 
            jobName, jobGroup, status, jobMessage);
    }
}
