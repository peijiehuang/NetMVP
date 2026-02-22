using Microsoft.AspNetCore.Http;
using NetMVP.Application.DTOs.Profile;

namespace NetMVP.Application.Services;

/// <summary>
/// 个人中心服务接口
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// 获取个人信息
    /// </summary>
    Task<ProfileDto> GetProfileAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新个人信息
    /// </summary>
    Task UpdateProfileAsync(UpdateProfileDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改密码
    /// </summary>
    Task UpdatePasswordAsync(UpdatePasswordDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新头像
    /// </summary>
    Task<string> UpdateAvatarAsync(IFormFile file, CancellationToken cancellationToken = default);
}
