using NetMVP.Application.DTOs.Menu;

namespace NetMVP.Application.Services;

/// <summary>
/// 菜单服务接口
/// </summary>
public interface ISysMenuService
{
    /// <summary>
    /// 获取菜单树
    /// </summary>
    Task<List<MenuDto>> GetMenuTreeAsync(MenuQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取菜单列表
    /// </summary>
    Task<List<MenuDto>> GetMenuListAsync(MenuQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取菜单
    /// </summary>
    Task<MenuDto?> GetMenuByIdAsync(long menuId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建菜单
    /// </summary>
    Task<long> CreateMenuAsync(CreateMenuDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新菜单
    /// </summary>
    Task UpdateMenuAsync(UpdateMenuDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除菜单
    /// </summary>
    Task DeleteMenuAsync(long menuId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户菜单树
    /// </summary>
    Task<List<MenuDto>> GetMenuTreeByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取角色菜单树
    /// </summary>
    Task<List<MenuTreeDto>> GetMenuTreeByRoleIdAsync(long roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户路由
    /// </summary>
    Task<List<RouterDto>> GetRoutersByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查菜单名称唯一性
    /// </summary>
    Task<bool> CheckMenuNameUniqueAsync(string menuName, long parentId, long? excludeMenuId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取菜单树选择列表
    /// </summary>
    Task<List<MenuTreeDto>> GetMenuTreeSelectAsync(CancellationToken cancellationToken = default);
}
