using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace NetMVP.Infrastructure.Services.Auth;

/// <summary>
/// 数据权限过滤实现
/// </summary>
public class DataScopeFilter : IDataScopeFilter
{
    private readonly IRepository<SysUser> _userRepository;
    private readonly IRepository<SysRole> _roleRepository;
    private readonly IRepository<SysDept> _deptRepository;
    private readonly NetMVPDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public DataScopeFilter(
        IRepository<SysUser> userRepository,
        IRepository<SysRole> roleRepository,
        IRepository<SysDept> deptRepository,
        NetMVPDbContext dbContext,
        ICacheService cacheService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _deptRepository = deptRepository;
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    /// <summary>
    /// 应用数据权限过滤
    /// </summary>
    public async Task<IQueryable<T>> ApplyDataScopeAsync<T>(IQueryable<T> query, long userId) where T : class
    {
        // 超级管理员不过滤
        if (await IsAdminAsync(userId))
        {
            return query;
        }

        // 获取可访问的部门ID列表
        var accessibleDeptIds = await GetAccessibleDeptIdsAsync(userId);
        if (!accessibleDeptIds.Any())
        {
            // 没有任何数据权限，返回空结果
            return query.Where(_ => false);
        }

        // 根据实体类型应用过滤
        if (typeof(T).GetProperty("DeptId") != null)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, "DeptId");
            var deptIdsList = accessibleDeptIds.ToList();
            
            // 构建 x => accessibleDeptIds.Contains(x.DeptId)
            var containsMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(long?));
            
            var containsCall = Expression.Call(
                containsMethod,
                Expression.Constant(deptIdsList.Cast<long?>()),
                property);
            
            var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);
            return query.Where(lambda);
        }

        return query;
    }

    /// <summary>
    /// 获取用户可访问的部门ID列表
    /// </summary>
    public async Task<IEnumerable<long>> GetAccessibleDeptIdsAsync(long userId)
    {
        var cacheKey = $"user:accessible_depts:{userId}";
        
        // 尝试从缓存获取
        var cachedDeptIds = await _cacheService.GetAsync<List<long>>(cacheKey);
        if (cachedDeptIds != null)
        {
            return cachedDeptIds;
        }

        // 超级管理员可访问所有部门
        if (await IsAdminAsync(userId))
        {
            var allDeptIds = await _deptRepository.GetQueryable()
                .Where(d => d.Status == UserStatus.Normal)
                .Select(d => d.DeptId)
                .ToListAsync();

            await _cacheService.SetAsync(cacheKey, allDeptIds, TimeSpan.FromHours(1));
            return allDeptIds;
        }

        // 获取用户角色的数据权限范围
        var roleIds = await _dbContext.Set<SysUserRole>()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (!roleIds.Any())
        {
            return Enumerable.Empty<long>();
        }

        var roles = await _roleRepository.GetQueryable()
            .Where(r => roleIds.Contains(r.RoleId) && r.Status == UserStatus.Normal)
            .ToListAsync();

        if (!roles.Any())
        {
            return Enumerable.Empty<long>();
        }

        // 取最大权限范围
        var maxDataScope = roles.Min(r => r.DataScope);

        var deptIds = maxDataScope switch
        {
            DataScopeType.All => await GetAllDeptIdsAsync(),
            DataScopeType.Custom => await GetCustomDeptIdsAsync(roleIds),
            DataScopeType.Department => await GetUserDeptIdsAsync(userId),
            DataScopeType.DepartmentAndBelow => await GetUserDeptAndChildIdsAsync(userId),
            DataScopeType.Self => await GetUserDeptIdsAsync(userId),
            _ => Enumerable.Empty<long>()
        };

        var deptIdList = deptIds.ToList();
        
        // 缓存部门ID列表
        await _cacheService.SetAsync(cacheKey, deptIdList, TimeSpan.FromHours(1));

        return deptIdList;
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
    /// 获取所有部门ID
    /// </summary>
    private async Task<IEnumerable<long>> GetAllDeptIdsAsync()
    {
        return await _deptRepository.GetQueryable()
            .Where(d => d.Status == UserStatus.Normal)
            .Select(d => d.DeptId)
            .ToListAsync();
    }

    /// <summary>
    /// 获取自定义数据权限的部门ID
    /// </summary>
    private async Task<IEnumerable<long>> GetCustomDeptIdsAsync(List<long> roleIds)
    {
        return await _dbContext.Set<SysRoleDept>()
            .Where(rd => roleIds.Contains(rd.RoleId))
            .Select(rd => rd.DeptId)
            .Distinct()
            .ToListAsync();
    }

    /// <summary>
    /// 获取用户所在部门ID
    /// </summary>
    private async Task<IEnumerable<long>> GetUserDeptIdsAsync(long userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.DeptId == null)
        {
            return Enumerable.Empty<long>();
        }

        return new[] { user.DeptId.Value };
    }

    /// <summary>
    /// 获取用户所在部门及子部门ID
    /// </summary>
    private async Task<IEnumerable<long>> GetUserDeptAndChildIdsAsync(long userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.DeptId == null)
        {
            return Enumerable.Empty<long>();
        }

        var userDeptId = user.DeptId.Value;
        
        // 获取本部门及所有子部门
        var deptIds = await _deptRepository.GetQueryable()
            .Where(d => d.DeptId == userDeptId 
                || (d.Ancestors != null && d.Ancestors.Contains($",{userDeptId},")))
            .Where(d => d.Status == UserStatus.Normal)
            .Select(d => d.DeptId)
            .ToListAsync();

        return deptIds;
    }

    #endregion
}
