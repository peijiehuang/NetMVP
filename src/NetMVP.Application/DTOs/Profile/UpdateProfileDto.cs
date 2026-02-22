using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.Profile;

/// <summary>
/// 更新个人信息DTO
/// </summary>
public class UpdateProfileDto
{
    /// <summary>
    /// 昵称
    /// </summary>
    [Required(ErrorMessage = "昵称不能为空")]
    [StringLength(30, ErrorMessage = "昵称长度不能超过30个字符")]
    public string NickName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [StringLength(50, ErrorMessage = "邮箱长度不能超过50个字符")]
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [Phone(ErrorMessage = "手机号格式不正确")]
    [StringLength(11, ErrorMessage = "手机号长度不能超过11个字符")]
    public string? Phonenumber { get; set; }

    /// <summary>
    /// 性别（0男 1女 2未知）
    /// </summary>
    public string? Sex { get; set; }
}
