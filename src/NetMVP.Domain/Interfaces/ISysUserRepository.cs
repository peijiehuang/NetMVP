using NetMVP.Domain.Entities;

namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 用户仓储接口
/// </summary>
public interface ISysUserRepository : IRepository<SysUser>
{
    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    Task<SysUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据用户ID获取用户
    /// </summary>
    Task<SysUser?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户及其角色
    /// </summary>
    Task<SysUser?> GetUserWithRolesAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户及其岗位
    /// </summary>
    Task<SysUser?> GetUserWithPostsAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查用户名是否唯一
    /// </summary>
    Task<bool> CheckUserNameUniqueAsync(string userName, long? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查手机号是否唯一
    /// </summary>
    Task<bool> CheckPhoneUniqueAsync(string phone, long? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查邮箱是否唯一
    /// </summary>
    Task<bool> CheckEmailUniqueAsync(string email, long? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户的岗位列表
    /// </summary>
    Task<List<SysPost>> GetUserPostsAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户的角色列表
    /// </summary>
    Task<List<SysRole>> GetUserRolesAsync(long userId, CancellationToken cancellationToken = default);
}
