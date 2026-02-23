using NetMVP.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.User;

/// <summary>
/// 更新用户状态 DTO
/// </summary>
public class UpdateUserStatusDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [Required(ErrorMessage = "用户ID不能为空")]
    public long UserId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [Required(ErrorMessage = "状态不能为空")]
    public string Status { get; set; } = UserConstants.NORMAL;
}
