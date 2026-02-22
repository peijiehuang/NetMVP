using NetMVP.Domain.Entities;

namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 定时任务仓储接口
/// </summary>
public interface ISysJobRepository : IRepository<SysJob>
{
    /// <summary>
    /// 根据任务ID获取任务
    /// </summary>
    Task<SysJob?> GetByJobIdAsync(long jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查任务名称是否唯一
    /// </summary>
    Task<bool> IsJobNameUniqueAsync(string jobName, string jobGroup, long? excludeJobId = null, CancellationToken cancellationToken = default);
}
