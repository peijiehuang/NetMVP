namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 任务调度服务接口
/// </summary>
public interface ISchedulerService
{
    /// <summary>
    /// 启动调度器
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止调度器
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 暂停调度器
    /// </summary>
    Task PauseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 恢复调度器
    /// </summary>
    Task ResumeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加任务
    /// </summary>
    Task AddJobAsync(string jobName, string jobGroup, string cronExpression, Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新任务
    /// </summary>
    Task UpdateJobAsync(string jobName, string jobGroup, string cronExpression, Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除任务
    /// </summary>
    Task DeleteJobAsync(string jobName, string jobGroup, CancellationToken cancellationToken = default);

    /// <summary>
    /// 暂停任务
    /// </summary>
    Task PauseJobAsync(string jobName, string jobGroup, CancellationToken cancellationToken = default);

    /// <summary>
    /// 恢复任务
    /// </summary>
    Task ResumeJobAsync(string jobName, string jobGroup, CancellationToken cancellationToken = default);

    /// <summary>
    /// 立即执行任务
    /// </summary>
    Task TriggerJobAsync(string jobName, string jobGroup, CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证Cron表达式
    /// </summary>
    bool ValidateCronExpression(string cronExpression);

    /// <summary>
    /// 获取下次执行时间
    /// </summary>
    DateTimeOffset? GetNextFireTime(string cronExpression);
}
