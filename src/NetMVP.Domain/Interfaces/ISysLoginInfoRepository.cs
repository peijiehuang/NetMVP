using NetMVP.Domain.Entities;

namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 登录日志仓储接口
/// </summary>
public interface ISysLoginInfoRepository
{
    /// <summary>
    /// 获取可查询对象
    /// </summary>
    IQueryable<SysLoginInfo> GetQueryable();

    /// <summary>
    /// 添加登录日志
    /// </summary>
    Task AddAsync(SysLoginInfo entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除登录日志
    /// </summary>
    Task DeleteAsync(SysLoginInfo entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清空登录日志
    /// </summary>
    Task<int> CleanAsync(CancellationToken cancellationToken = default);
}
