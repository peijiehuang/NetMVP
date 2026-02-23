using NetMVP.Application.DTOs.Role;
using NetMVP.Domain.Enums;

namespace NetMVP.Application.Services;

/// <summary>
/// 角色服务接口
/// </summary>
public interface ISysRoleService
{
    /// <summary>
    /// 获取角色列表
    /// </summary>
    Task<(List<RoleDto> roles, int total)> GetRoleListAsync(RoleQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    Task<RoleDto?> GetRoleByIdAsync(long roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建角色
    /// </summary>
    Task<long> CreateRoleAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新角色
    /// </summary>
    Task UpdateRoleAsync(UpdateRoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除角色
    /// </summary>
    Task DeleteRoleAsync(long roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除角色
    /// </summary>
    Task DeleteRolesAsync(long[] roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改角色状态
    /// </summary>
    Task UpdateRoleStatusAsync(long roleId, UserStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查角色名称唯一性
    /// </summary>
    Task<bool> CheckRoleNameUniqueAsync(string roleName, long? excludeRoleId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查角色权限字符串唯一性
    /// </summary>
    Task<bool> CheckRoleKeyUniqueAsync(string roleKey, long? excludeRoleId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新角色数据权限范围
    /// </summary>
    Task UpdateDataScopeAsync(long roleId, string dataScope, List<long> deptIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消单个用户的角色授权
    /// </summary>
    Task CancelAuthUserAsync(long roleId, long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量取消用户的角色授权
    /// </summary>
    Task CancelAuthUsersAsync(long roleId, long[] userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量给用户授予角色
    /// </summary>
    Task InsertAuthUsersAsync(long roleId, long[] userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有角色列表（用于下拉选择）
    /// </summary>
    Task<List<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 导出角色数据
    /// </summary>
    Task<byte[]> ExportRolesAsync(RoleQueryDto query, CancellationToken cancellationToken = default);
}
