using Microsoft.EntityFrameworkCore;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Persistence;

namespace NetMVP.Infrastructure.Repositories;

/// <summary>
/// 角色仓储实现
/// </summary>
public class SysRoleRepository : Repository<SysRole>, ISysRoleRepository
{
    public SysRoleRepository(NetMVPDbContext context) : base(context)
    {
    }

    /// <summary>
    /// 获取角色及其菜单
    /// </summary>
    public async Task<SysRole?> GetRoleWithMenusAsync(long roleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.RoleMenus)
            .FirstOrDefaultAsync(r => r.RoleId == roleId, cancellationToken);
    }

    /// <summary>
    /// 获取角色及其部门
    /// </summary>
    public async Task<SysRole?> GetRoleWithDeptsAsync(long roleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.RoleDepts)
            .FirstOrDefaultAsync(r => r.RoleId == roleId, cancellationToken);
    }

    /// <summary>
    /// 检查角色名称是否唯一
    /// </summary>
    public async Task<bool> CheckRoleNameUniqueAsync(string roleName, long? excludeRoleId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(r => r.RoleName == roleName);
        
        if (excludeRoleId.HasValue)
        {
            query = query.Where(r => r.RoleId != excludeRoleId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 检查角色权限字符是否唯一
    /// </summary>
    public async Task<bool> CheckRoleKeyUniqueAsync(string roleKey, long? excludeRoleId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(r => r.RoleKey == roleKey);
        
        if (excludeRoleId.HasValue)
        {
            query = query.Where(r => r.RoleId != excludeRoleId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }
}
