using NetMVP.Application.DTOs.User;
using NetMVP.Domain.Enums;

namespace NetMVP.Application.Services;

/// <summary>
/// 用户服务接口
/// </summary>
public interface ISysUserService
{
    /// <summary>
    /// 获取用户列表
    /// </summary>
    Task<(List<UserDto> users, int total)> GetUserListAsync(UserQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    Task<UserDto?> GetUserByIdAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建用户
    /// </summary>
    Task<long> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新用户
    /// </summary>
    Task UpdateUserAsync(UpdateUserDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除用户
    /// </summary>
    Task DeleteUserAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除用户
    /// </summary>
    Task DeleteUsersAsync(long[] userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 重置密码
    /// </summary>
    Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改密码
    /// </summary>
    Task ChangePasswordAsync(long userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改用户状态
    /// </summary>
    Task UpdateUserStatusAsync(long userId, UserStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导出用户
    /// </summary>
    Task<byte[]> ExportUsersAsync(UserQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查用户名唯一性
    /// </summary>
    Task<bool> CheckUserNameUniqueAsync(string userName, long? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查手机号唯一性
    /// </summary>
    Task<bool> CheckPhoneUniqueAsync(string phone, long? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查邮箱唯一性
    /// </summary>
    Task<bool> CheckEmailUniqueAsync(string email, long? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户个人信息
    /// </summary>
    Task<UserDto?> GetUserProfileAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新用户个人信息
    /// </summary>
    Task UpdateUserProfileAsync(long userId, UpdateProfileDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新用户头像
    /// </summary>
    Task UpdateUserAvatarAsync(long userId, string avatar, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户的岗位ID列表
    /// </summary>
    Task<List<long>> GetUserPostIdsAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户的角色ID列表
    /// </summary>
    Task<List<long>> GetUserRoleIdsAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新用户角色
    /// </summary>
    Task UpdateUserRolesAsync(long userId, long[] roleIds, CancellationToken cancellationToken = default);
}
