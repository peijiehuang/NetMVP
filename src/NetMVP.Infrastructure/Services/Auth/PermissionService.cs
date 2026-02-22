using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace NetMVP.Infrastructure.Services.Auth;

/// <summary>
/// 权限服务实现
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IRepository<SysUser> _userRepository;
    private readonly IRepository<SysRole> _roleRepository;
    private readonly IRepository<SysMenu> _menuRepository;
    private readonly NetMVPDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public PermissionService(
        IRepository<SysUser> userRepository,
        IRepository<SysRole> roleRepository,
        IRepository<SysMenu> menuRepository,
        NetMVPDbContext dbContext,
        ICacheService cacheService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _menuRepository = menuRepository;
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    /// <summary>
    /// 检查用户是否拥有指定权限
    /// </summary>
    public async Task<bool> HasPermissionAsync(long userId, string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            return false;
        }

        // 超级管理员拥有所有权限
        if (await IsAdminAsync(userId))
        {
            return true;
        }

        var permissions = await GetUserPermissionsAsync(userId);
        return permissions.Contains(permission);
    }

    /// <summary>
    /// 获取用户的所有权限标识
    /// </summary>
    public async Task<IEnumerable<string>> GetUserPermissionsAsync(long userId)
    {
        var cacheKey = $"user:permissions:{userId}";
        
        // 尝试从缓存获取
        var cachedPermissions = await _cacheService.GetAsync<List<string>>(cacheKey);
        if (cachedPermissions != null)
        {
            return cachedPermissions;
        }

        // 超级管理员拥有所有权限
        if (await IsAdminAsync(userId))
        {
            var allPermissions = await _menuRepository.GetQueryable()
                .Where(m => !string.IsNullOrEmpty(m.Perms) && m.Status == UserStatus.Normal)
                .Select(m => m.Perms!)
                .Distinct()
                .ToListAsync();

            await _cacheService.SetAsync(cacheKey, allPermissions, TimeSpan.FromHours(1));
            return allPermissions;
        }

        // 获取用户角色
        var roleIds = await _dbContext.Set<SysUserRole>()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (!roleIds.Any())
        {
            return Enumerable.Empty<string>();
        }

        // 获取角色对应的菜单权限
        var menuIds = await _dbContext.Set<SysRoleMenu>()
            .Where(rm => roleIds.Contains(rm.RoleId))
            .Select(rm => rm.MenuId)
            .Distinct()
            .ToListAsync();

        if (!menuIds.Any())
        {
            return Enumerable.Empty<string>();
        }

        // 获取菜单的权限标识
        var permissions = await _menuRepository.GetQueryable()
            .Where(m => menuIds.Contains(m.MenuId) 
                && !string.IsNullOrEmpty(m.Perms) 
                && m.Status == UserStatus.Normal)
            .Select(m => m.Perms!)
            .Distinct()
            .ToListAsync();

        // 缓存权限列表
        await _cacheService.SetAsync(cacheKey, permissions, TimeSpan.FromHours(1));

        return permissions;
    }

    /// <summary>
    /// 获取用户的所有角色标识
    /// </summary>
    public async Task<IEnumerable<string>> GetUserRolesAsync(long userId)
    {
        var cacheKey = $"user:roles:{userId}";
        
        // 尝试从缓存获取
        var cachedRoles = await _cacheService.GetAsync<List<string>>(cacheKey);
        if (cachedRoles != null)
        {
            return cachedRoles;
        }

        // 获取用户角色
        var roleIds = await _dbContext.Set<SysUserRole>()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (!roleIds.Any())
        {
            return Enumerable.Empty<string>();
        }

        var roles = await _roleRepository.GetQueryable()
            .Where(r => roleIds.Contains(r.RoleId) && r.Status == UserStatus.Normal)
            .Select(r => r.RoleKey)
            .ToListAsync();

        // 缓存角色列表
        await _cacheService.SetAsync(cacheKey, roles, TimeSpan.FromHours(1));

        return roles;
    }

    /// <summary>
    /// 检查用户是否拥有指定角色
    /// </summary>
    public async Task<bool> HasRoleAsync(long userId, string roleKey)
    {
        if (string.IsNullOrWhiteSpace(roleKey))
        {
            return false;
        }

        // 超级管理员拥有所有角色
        if (await IsAdminAsync(userId))
        {
            return true;
        }

        var roles = await GetUserRolesAsync(userId);
        return roles.Contains(roleKey);
    }

    /// <summary>
    /// 检查用户对指定部门的数据权限
    /// </summary>
    public async Task<bool> CheckDataScopeAsync(long userId, long deptId)
    {
        // 超级管理员拥有所有数据权限
        if (await IsAdminAsync(userId))
        {
            return true;
        }

        var dataScope = await GetUserDataScopeAsync(userId);

        return dataScope switch
        {
            DataScopeType.All => true,
            DataScopeType.Custom => await CheckCustomDataScopeAsync(userId, deptId),
            DataScopeType.Department => await CheckDeptDataScopeAsync(userId, deptId),
            DataScopeType.DepartmentAndBelow => await CheckDeptAndChildDataScopeAsync(userId, deptId),
            DataScopeType.Self => await CheckSelfDataScopeAsync(userId, deptId),
            _ => false
        };
    }

    /// <summary>
    /// 获取用户的数据权限范围
    /// </summary>
    public async Task<DataScopeType> GetUserDataScopeAsync(long userId)
    {
        var cacheKey = $"user:datascope:{userId}";
        
        // 尝试从缓存获取
        var cachedDataScope = await _cacheService.GetAsync<DataScopeType?>(cacheKey);
        if (cachedDataScope.HasValue)
        {
            return cachedDataScope.Value;
        }

        // 超级管理员拥有所有数据权限
        if (await IsAdminAsync(userId))
        {
            await _cacheService.SetAsync(cacheKey, DataScopeType.All, TimeSpan.FromHours(1));
            return DataScopeType.All;
        }

        // 获取用户角色的数据权限范围（取最大权限）
        var roleIds = await _dbContext.Set<SysUserRole>()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (!roleIds.Any())
        {
            return DataScopeType.Self;
        }

        var dataScope = await _roleRepository.GetQueryable()
            .Where(r => roleIds.Contains(r.RoleId) && r.Status == UserStatus.Normal)
            .Select(r => r.DataScope)
            .OrderBy(ds => ds)
            .FirstOrDefaultAsync();

        // 缓存数据权限范围
        await _cacheService.SetAsync(cacheKey, dataScope, TimeSpan.FromHours(1));

        return dataScope;
    }

    /// <summary>
    /// 清除用户权限缓存
    /// </summary>
    public async Task ClearUserPermissionCacheAsync(long userId)
    {
        await _cacheService.RemoveAsync($"user:permissions:{userId}");
        await _cacheService.RemoveAsync($"user:roles:{userId}");
        await _cacheService.RemoveAsync($"user:datascope:{userId}");
    }

    #region 私有方法

    /// <summary>
    /// 检查是否为超级管理员
    /// </summary>
    private async Task<bool> IsAdminAsync(long userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.IsAdmin() ?? false;
    }

    /// <summary>
    /// 检查自定义数据权限
    /// </summary>
    private async Task<bool> CheckCustomDataScopeAsync(long userId, long deptId)
    {
        // 获取用户角色的自定义部门权限
        var roleIds = await _dbContext.Set<SysUserRole>()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (!roleIds.Any())
        {
            return false;
        }

        var hasPermission = await _dbContext.Set<SysRoleDept>()
            .AnyAsync(rd => roleIds.Contains(rd.RoleId) && rd.DeptId == deptId);

        return hasPermission;
    }

    /// <summary>
    /// 检查本部门数据权限
    /// </summary>
    private async Task<bool> CheckDeptDataScopeAsync(long userId, long deptId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.DeptId == deptId;
    }

    /// <summary>
    /// 检查本部门及以下数据权限
    /// </summary>
    private async Task<bool> CheckDeptAndChildDataScopeAsync(long userId, long deptId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.DeptId.HasValue)
        {
            return false;
        }

        // 检查是否为本部门
        if (user.DeptId == deptId)
        {
            return true;
        }

        // 检查是否为子部门
        var dept = await _dbContext.Set<SysDept>().FirstOrDefaultAsync(d => d.DeptId == deptId);
        if (dept == null)
        {
            return false;
        }

        // 检查祖级列表是否包含用户部门
        return dept.Ancestors?.Contains($",{user.DeptId},") ?? false;
    }

    /// <summary>
    /// 检查仅本人数据权限
    /// </summary>
    private async Task<bool> CheckSelfDataScopeAsync(long userId, long deptId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.DeptId == deptId;
    }

    #endregion
}
