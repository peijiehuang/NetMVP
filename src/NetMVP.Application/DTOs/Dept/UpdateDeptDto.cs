using NetMVP.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.Dept;

/// <summary>
/// 更新部门 DTO
/// </summary>
public class UpdateDeptDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    [Required(ErrorMessage = "部门ID不能为空")]
    public long DeptId { get; set; }

    /// <summary>
    /// 父部门ID
    /// </summary>
    public long ParentId { get; set; } = 0;

    /// <summary>
    /// 部门名称
    /// </summary>
    [Required(ErrorMessage = "部门名称不能为空")]
    [StringLength(30, ErrorMessage = "部门名称长度不能超过30个字符")]
    public string DeptName { get; set; } = string.Empty;

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int OrderNum { get; set; }

    /// <summary>
    /// 负责人
    /// </summary>
    [StringLength(20, ErrorMessage = "负责人长度不能超过20个字符")]
    public string? Leader { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [StringLength(11, ErrorMessage = "联系电话长度不能超过11个字符")]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [StringLength(50, ErrorMessage = "邮箱长度不能超过50个字符")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 部门状态
    /// </summary>
    public string Status { get; set; } = UserConstants.NORMAL;
}
