using System.ComponentModel.DataAnnotations;

namespace NetMVP.Infrastructure.Configuration;

/// <summary>
/// 系统配置
/// </summary>
public class SystemSettings
{
    /// <summary>
    /// 应用名称
    /// </summary>
    [Required(ErrorMessage = "应用名称不能为空")]
    public string AppName { get; set; } = string.Empty;

    /// <summary>
    /// 应用版本
    /// </summary>
    [Required(ErrorMessage = "应用版本不能为空")]
    public string AppVersion { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用验证码
    /// </summary>
    public bool CaptchaEnabled { get; set; } = true;

    /// <summary>
    /// 是否启用注册
    /// </summary>
    public bool RegisterEnabled { get; set; } = false;

    /// <summary>
    /// 是否强制修改初始密码
    /// </summary>
    public bool InitPasswordModify { get; set; } = false;
}
