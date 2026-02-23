using NetMVP.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.User;

/// <summary>
/// 更新用户 DTO
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [Required(ErrorMessage = "用户ID不能为空")]
    public long UserId { get; set; }

    /// <summary>
    /// 用户名（前端会发送，但不允许修改）
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    [Required(ErrorMessage = "用户昵称不能为空")]
    [StringLength(30, ErrorMessage = "用户昵称长度不能超过30个字符")]
    public string NickName { get; set; } = string.Empty;

    /// <summary>
    /// 部门ID
    /// </summary>
    public long? DeptId { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phonenumber { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 性别（0男 1女 2未知）
    /// </summary>
    public string Sex { get; set; } = UserConstants.SEX_UNKNOWN;

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; } = UserConstants.NORMAL;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public long[]? RoleIds { get; set; }

    /// <summary>
    /// 岗位ID列表
    /// </summary>
    public long[]? PostIds { get; set; }

    /// <summary>
    /// 密码（前端会发送空字符串，忽略）
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 创建时间（前端会发送，忽略）
    /// </summary>
    public DateTime? CreateTime { get; set; }
}
