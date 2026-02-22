namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 任务执行器接口
/// </summary>
public interface IJobExecutor
{
    /// <summary>
    /// 执行任务
    /// </summary>
    /// <param name="jobName">任务名称</param>
    /// <param name="jobGroup">任务组</param>
    /// <param name="parameters">任务参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ExecuteAsync(string jobName, string jobGroup, Dictionary<string, object>? parameters, CancellationToken cancellationToken = default);
}
