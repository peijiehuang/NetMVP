using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.Auth;

/// <summary>
/// 登录 DTO
/// </summary>
public class LoginDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 验证码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 验证码唯一标识
    /// </summary>
    public string? Uuid { get; set; }
}
