using NetMVP.Application.DTOs.UserOnline;

namespace NetMVP.Application.Services;

/// <summary>
/// 在线用户服务接口
/// </summary>
public interface ISysUserOnlineService
{
    /// <summary>
    /// 获取在线用户列表
    /// </summary>
    Task<(List<OnlineUserDto> users, int total)> GetOnlineUserListAsync(OnlineUserQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 强制下线
    /// </summary>
    Task ForceLogoutAsync(string tokenId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量强制下线
    /// </summary>
    Task BatchForceLogoutAsync(string[] tokenIds, CancellationToken cancellationToken = default);
}
