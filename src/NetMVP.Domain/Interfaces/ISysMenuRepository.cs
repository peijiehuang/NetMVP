using NetMVP.Domain.Entities;

namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 菜单仓储接口
/// </summary>
public interface ISysMenuRepository : IRepository<SysMenu>
{
    /// <summary>
    /// 获取所有菜单（树形结构）
    /// </summary>
    Task<List<SysMenu>> GetMenuTreeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据用户ID获取菜单列表
    /// </summary>
    Task<List<SysMenu>> GetMenusByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据角色ID获取菜单列表
    /// </summary>
    Task<List<SysMenu>> GetMenusByRoleIdAsync(long roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查菜单名称是否唯一
    /// </summary>
    Task<bool> CheckMenuNameUniqueAsync(string menuName, long parentId, long? excludeMenuId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查是否存在子菜单
    /// </summary>
    Task<bool> HasChildrenAsync(long menuId, CancellationToken cancellationToken = default);
}
