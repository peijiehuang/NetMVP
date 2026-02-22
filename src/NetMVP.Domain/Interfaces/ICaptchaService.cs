namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 验证码服务接口
/// </summary>
public interface ICaptchaService
{
    /// <summary>
    /// 生成验证码
    /// </summary>
    Task<(string uuid, string image)> GenerateCaptchaAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证验证码
    /// </summary>
    Task<bool> ValidateCaptchaAsync(string uuid, string code, CancellationToken cancellationToken = default);
}
