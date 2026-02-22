namespace NetMVP.Domain.Entities;

/// <summary>
/// 角色菜单关联实体
/// </summary>
public class SysRoleMenu
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long RoleId { get; set; }

    /// <summary>
    /// 菜单ID
    /// </summary>
    public long MenuId { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    public SysRole? Role { get; set; }

    /// <summary>
    /// 菜单
    /// </summary>
    public SysMenu? Menu { get; set; }
}
