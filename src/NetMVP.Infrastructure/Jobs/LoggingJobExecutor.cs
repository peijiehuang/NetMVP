using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetMVP.Application.Services;
using NetMVP.Domain.Interfaces;
using NetMVP.Domain.Jobs;

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
        string status = "0";
        string? exceptionInfo = null;
        string jobMessage;
        
        // 创建任务执行上下文
        var context = new JobExecutionContext
        {
            JobName = jobName,
            JobGroup = jobGroup,
            StartTime = startTime
        };
        
        // 设置当前上下文
        JobContext.Current = context;
        
        // 从数据库获取任务信息
        string invokeTarget = jobName;
        using (var scope = _serviceProvider.CreateScope())
        {
            var jobRepository = scope.ServiceProvider.GetRequiredService<ISysJobRepository>();
            var job = await jobRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.JobName == jobName && x.JobGroup == jobGroup, cancellationToken);
            if (job != null)
            {
                invokeTarget = job.InvokeTarget;
                context.InvokeTarget = invokeTarget;
            }
        }

        try
        {
            // 解析并执行任务
            await JobInvokeUtil.InvokeMethodAsync(_serviceProvider, invokeTarget);

            var endTime = DateTime.Now;
            var duration = (endTime - startTime).TotalMilliseconds;
            
            // 获取任务执行过程中的日志
            var logs = context.GetLogs();
            if (!string.IsNullOrEmpty(logs))
            {
                jobMessage = $"{jobName} 总共耗时：{duration:F2}毫秒\n执行详情：\n{logs}";
            }
            else
            {
                jobMessage = $"{jobName} 总共耗时：{duration:F2}毫秒";
            }
        }
        catch (Exception ex)
        {
            status = "1";
            var errorMsg = ex.ToString();
            exceptionInfo = errorMsg.Length > 2000 ? errorMsg.Substring(0, 2000) : errorMsg;
            
            // 获取任务执行过程中的日志
            var logs = context.GetLogs();
            if (!string.IsNullOrEmpty(logs))
            {
                jobMessage = $"{jobName} 执行失败: {ex.Message}\n执行详情：\n{logs}";
            }
            else
            {
                jobMessage = $"{jobName} 执行失败: {ex.Message}";
            }
            
            _logger.LogError(ex, "任务执行失败: {JobName}.{JobGroup}", jobName, jobGroup);
        }
        finally
        {
            // 清除上下文
            JobContext.Current = null;
        }

        // 记录任务日志到数据库
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var jobLogService = scope.ServiceProvider.GetRequiredService<ISysJobLogService>();
            await jobLogService.AddJobLogAsync(jobName, jobGroup, invokeTarget, jobMessage, status, exceptionInfo, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "记录任务日志失败: {JobName}.{JobGroup}", jobName, jobGroup);
        }
    }
}
