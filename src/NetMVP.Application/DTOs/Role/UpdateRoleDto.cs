using NetMVP.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.Role;

/// <summary>
/// 更新角色 DTO
/// </summary>
public class UpdateRoleDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [Required(ErrorMessage = "角色ID不能为空")]
    public long RoleId { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    [StringLength(30, ErrorMessage = "角色名称长度不能超过30个字符")]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色权限字符串
    /// </summary>
    [Required(ErrorMessage = "权限字符不能为空")]
    [StringLength(100, ErrorMessage = "权限字符长度不能超过100个字符")]
    public string RoleKey { get; set; } = string.Empty;

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int RoleSort { get; set; }

    /// <summary>
    /// 数据范围
    /// </summary>
    public DataScopeType DataScope { get; set; } = DataScopeType.Department;

    /// <summary>
    /// 菜单树选择项是否关联显示
    /// </summary>
    public bool MenuCheckStrictly { get; set; } = true;

    /// <summary>
    /// 部门树选择项是否关联显示
    /// </summary>
    public bool DeptCheckStrictly { get; set; } = true;

    /// <summary>
    /// 角色状态
    /// </summary>
    public UserStatus Status { get; set; } = UserStatus.Normal;

    /// <summary>
    /// 菜单ID列表
    /// </summary>
    public List<long> MenuIds { get; set; } = new();

    /// <summary>
    /// 部门ID列表（数据权限）
    /// </summary>
    public List<long> DeptIds { get; set; } = new();

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}
