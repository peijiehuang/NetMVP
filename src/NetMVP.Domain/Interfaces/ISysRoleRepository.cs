using NetMVP.Domain.Entities;

namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 角色仓储接口
/// </summary>
public interface ISysRoleRepository : IRepository<SysRole>
{
    /// <summary>
    /// 获取角色及其菜单
    /// </summary>
    Task<SysRole?> GetRoleWithMenusAsync(long roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取角色及其部门
    /// </summary>
    Task<SysRole?> GetRoleWithDeptsAsync(long roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查角色名称是否唯一
    /// </summary>
    Task<bool> CheckRoleNameUniqueAsync(string roleName, long? excludeRoleId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查角色权限字符是否唯一
    /// </summary>
    Task<bool> CheckRoleKeyUniqueAsync(string roleKey, long? excludeRoleId = null, CancellationToken cancellationToken = default);
}
