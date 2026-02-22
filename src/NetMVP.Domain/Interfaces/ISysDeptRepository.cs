using NetMVP.Domain.Entities;

namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 部门仓储接口
/// </summary>
public interface ISysDeptRepository : IRepository<SysDept>
{
    /// <summary>
    /// 根据部门ID获取部门
    /// </summary>
    Task<SysDept?> GetByDeptIdAsync(long deptId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有部门（树形结构）
    /// </summary>
    Task<List<SysDept>> GetDeptTreeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取子部门列表
    /// </summary>
    Task<List<SysDept>> GetChildrenDeptsAsync(long parentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查部门名称是否唯一
    /// </summary>
    Task<bool> CheckDeptNameUniqueAsync(string deptName, long parentId, long? excludeDeptId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查是否存在子部门
    /// </summary>
    Task<bool> HasChildrenAsync(long deptId, CancellationToken cancellationToken = default);
}
