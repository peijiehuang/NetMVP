using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.Profile;

/// <summary>
/// 修改密码DTO
/// </summary>
public class UpdatePasswordDto
{
    /// <summary>
    /// 旧密码
    /// </summary>
    [Required(ErrorMessage = "旧密码不能为空")]
    public string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新密码
    /// </summary>
    [Required(ErrorMessage = "新密码不能为空")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "密码长度必须在5-20个字符之间")]
    public string NewPassword { get; set; } = string.Empty;
}
