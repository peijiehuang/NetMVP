namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 认证服务接口
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 登录
    /// </summary>
    Task<(string accessToken, string refreshToken)> LoginAsync(string userName, string password, string? code, string? uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// 登出
    /// </summary>
    Task LogoutAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取验证码
    /// </summary>
    Task<(string uuid, string image, bool captchaEnabled)> GetCaptchaAsync(CancellationToken cancellationToken = default);
}
