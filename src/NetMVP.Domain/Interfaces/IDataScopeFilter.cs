namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 数据权限过滤接口
/// </summary>
public interface IDataScopeFilter
{
    /// <summary>
    /// 应用数据权限过滤
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">查询</param>
    /// <param name="userId">用户ID</param>
    /// <returns>过滤后的查询</returns>
    Task<IQueryable<T>> ApplyDataScopeAsync<T>(IQueryable<T> query, long userId) where T : class;

    /// <summary>
    /// 获取用户可访问的部门ID列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>部门ID列表</returns>
    Task<IEnumerable<long>> GetAccessibleDeptIdsAsync(long userId);
}
