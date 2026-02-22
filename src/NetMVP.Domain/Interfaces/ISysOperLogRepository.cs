using NetMVP.Domain.Entities;

namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 操作日志仓储接口
/// </summary>
public interface ISysOperLogRepository
{
    /// <summary>
    /// 获取可查询对象
    /// </summary>
    IQueryable<SysOperLog> GetQueryable();

    /// <summary>
    /// 添加操作日志
    /// </summary>
    Task AddAsync(SysOperLog entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除操作日志
    /// </summary>
    Task DeleteAsync(SysOperLog entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清空操作日志
    /// </summary>
    Task CleanAsync(CancellationToken cancellationToken = default);
}
