using NetMVP.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.User;

/// <summary>
/// 更新个人信息 DTO
/// </summary>
public class UpdateProfileDto
{
    /// <summary>
    /// 用户昵称
    /// </summary>
    [Required(ErrorMessage = "用户昵称不能为空")]
    [StringLength(30, ErrorMessage = "用户昵称长度不能超过30个字符")]
    public string NickName { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary>
    [Phone(ErrorMessage = "手机号格式不正确")]
    public string? Phonenumber { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public Gender Gender { get; set; } = Gender.Unknown;
}
