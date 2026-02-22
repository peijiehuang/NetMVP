namespace NetMVP.Domain.Entities;

/// <summary>
/// 角色部门关联实体
/// </summary>
public class SysRoleDept
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long RoleId { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public long DeptId { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    public SysRole? Role { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public SysDept? Dept { get; set; }
}
