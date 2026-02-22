using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.Auth;

/// <summary>
/// 用户注册DTO
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "用户名长度必须在2-30个字符之间")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "密码长度必须在5-20个字符之间")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 确认密码
    /// </summary>
    [Required(ErrorMessage = "确认密码不能为空")]
    [Compare("Password", ErrorMessage = "两次输入的密码不一致")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// 验证码
    /// </summary>
    [Required(ErrorMessage = "验证码不能为空")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 验证码唯一标识
    /// </summary>
    [Required(ErrorMessage = "验证码标识不能为空")]
    public string Uuid { get; set; } = string.Empty;
}
