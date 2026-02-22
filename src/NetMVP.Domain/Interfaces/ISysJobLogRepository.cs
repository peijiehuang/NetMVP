using NetMVP.Domain.Entities;

namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 定时任务日志仓储接口
/// </summary>
public interface ISysJobLogRepository
{
    /// <summary>
    /// 获取可查询对象
    /// </summary>
    IQueryable<SysJobLog> GetQueryable();

    /// <summary>
    /// 根据任务日志ID获取日志
    /// </summary>
    Task<SysJobLog?> GetByJobLogIdAsync(long jobLogId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加任务日志
    /// </summary>
    Task AddAsync(SysJobLog entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除任务日志
    /// </summary>
    Task DeleteAsync(SysJobLog entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清空任务日志
    /// </summary>
    Task CleanJobLogAsync(CancellationToken cancellationToken = default);
}
