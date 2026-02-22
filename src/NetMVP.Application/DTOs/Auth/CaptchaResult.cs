namespace NetMVP.Application.DTOs.Auth;

/// <summary>
/// 验证码结果
/// </summary>
public class CaptchaResult
{
    /// <summary>
    /// 验证码唯一标识
    /// </summary>
    public string Uuid { get; set; } = string.Empty;

    /// <summary>
    /// 验证码图片（Base64）
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用验证码
    /// </summary>
    public bool CaptchaEnabled { get; set; } = true;
}
