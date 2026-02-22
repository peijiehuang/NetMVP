using Microsoft.EntityFrameworkCore;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Persistence;

namespace NetMVP.Infrastructure.Repositories;

/// <summary>
/// 菜单仓储实现
/// </summary>
public class SysMenuRepository : Repository<SysMenu>, ISysMenuRepository
{
    public SysMenuRepository(NetMVPDbContext context) : base(context)
    {
    }

    /// <summary>
    /// 获取所有菜单（树形结构）
    /// </summary>
    public async Task<List<SysMenu>> GetMenuTreeAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderBy(m => m.ParentId)
            .ThenBy(m => m.OrderNum)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 根据用户ID获取菜单列表
    /// </summary>
    public async Task<List<SysMenu>> GetMenusByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await (from menu in _context.SysMenus
                      join roleMenu in _context.SysRoleMenus on menu.MenuId equals roleMenu.MenuId
                      join userRole in _context.SysUserRoles on roleMenu.RoleId equals userRole.RoleId
                      where userRole.UserId == userId
                      orderby menu.ParentId, menu.OrderNum
                      select menu)
                      .Distinct()
                      .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 根据角色ID获取菜单列表
    /// </summary>
    public async Task<List<SysMenu>> GetMenusByRoleIdAsync(long roleId, CancellationToken cancellationToken = default)
    {
        return await (from menu in _context.SysMenus
                      join roleMenu in _context.SysRoleMenus on menu.MenuId equals roleMenu.MenuId
                      where roleMenu.RoleId == roleId
                      orderby menu.ParentId, menu.OrderNum
                      select menu)
                      .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 检查菜单名称是否唯一
    /// </summary>
    public async Task<bool> CheckMenuNameUniqueAsync(string menuName, long parentId, long? excludeMenuId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(m => m.MenuName == menuName && m.ParentId == parentId);
        
        if (excludeMenuId.HasValue)
        {
            query = query.Where(m => m.MenuId != excludeMenuId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 检查是否存在子菜单
    /// </summary>
    public async Task<bool> HasChildrenAsync(long menuId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(m => m.ParentId == menuId, cancellationToken);
    }
}
