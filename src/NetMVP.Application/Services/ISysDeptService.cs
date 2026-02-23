using NetMVP.Application.DTOs.Dept;

namespace NetMVP.Application.Services;

/// <summary>
/// 部门服务接口
/// </summary>
public interface ISysDeptService
{
    /// <summary>
    /// 获取部门树
    /// </summary>
    Task<List<DeptDto>> GetDeptTreeAsync(DeptQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取部门列表
    /// </summary>
    Task<List<DeptDto>> GetDeptListAsync(DeptQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取部门
    /// </summary>
    Task<DeptDto?> GetDeptByIdAsync(long deptId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建部门
    /// </summary>
    Task<long> CreateDeptAsync(CreateDeptDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新部门
    /// </summary>
    Task UpdateDeptAsync(UpdateDeptDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除部门
    /// </summary>
    Task DeleteDeptAsync(long deptId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取部门树选择列表
    /// </summary>
    Task<List<DeptTreeDto>> GetDeptTreeSelectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取角色部门树
    /// </summary>
    Task<List<DeptTreeDto>> GetRoleDeptTreeSelectAsync(long roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查部门名称唯一性
    /// </summary>
    Task<bool> CheckDeptNameUniqueAsync(string deptName, long parentId, long? excludeDeptId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取角色已选部门ID列表
    /// </summary>
    Task<List<long>> GetDeptIdsByRoleIdAsync(long roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取部门列表（排除指定节点及其子节点）
    /// </summary>
    Task<List<DeptDto>> GetDeptListExcludeChildAsync(long deptId, CancellationToken cancellationToken = default);
}
