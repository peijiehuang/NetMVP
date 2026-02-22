using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.User;

/// <summary>
/// 更新头像 DTO
/// </summary>
public class UpdateAvatarDto
{
    /// <summary>
    /// 头像地址
    /// </summary>
    [Required(ErrorMessage = "头像地址不能为空")]
    public string Avatar { get; set; } = string.Empty;
}
