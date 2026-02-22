using NetMVP.Domain.Enums;

namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 权限服务接口
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// 检查用户是否拥有指定权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permission">权限标识</param>
    /// <returns>是否拥有权限</returns>
    Task<bool> HasPermissionAsync(long userId, string permission);

    /// <summary>
    /// 获取用户的所有权限标识
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>权限标识列表</returns>
    Task<IEnumerable<string>> GetUserPermissionsAsync(long userId);

    /// <summary>
    /// 获取用户的所有角色标识
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>角色标识列表</returns>
    Task<IEnumerable<string>> GetUserRolesAsync(long userId);

    /// <summary>
    /// 检查用户是否拥有指定角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="roleKey">角色标识</param>
    /// <returns>是否拥有角色</returns>
    Task<bool> HasRoleAsync(long userId, string roleKey);

    /// <summary>
    /// 检查用户对指定部门的数据权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="deptId">部门ID</param>
    /// <returns>是否有权限</returns>
    Task<bool> CheckDataScopeAsync(long userId, long deptId);

    /// <summary>
    /// 获取用户的数据权限范围
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>数据权限范围</returns>
    Task<DataScopeType> GetUserDataScopeAsync(long userId);

    /// <summary>
    /// 清除用户权限缓存
    /// </summary>
    /// <param name="userId">用户ID</param>
    Task ClearUserPermissionCacheAsync(long userId);
}
