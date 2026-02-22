using NetMVP.Domain.Common;
using NetMVP.Domain.Enums;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 角色实体
/// </summary>
public class SysRole : BaseEntity
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long RoleId { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色权限字符串
    /// </summary>
    public string RoleKey { get; set; } = string.Empty;

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int RoleSort { get; set; }

    /// <summary>
    /// 数据范围
    /// </summary>
    public DataScopeType DataScope { get; set; } = DataScopeType.All;

    /// <summary>
    /// 菜单树选择项是否关联显示
    /// </summary>
    public bool MenuCheckStrictly { get; set; } = true;

    /// <summary>
    /// 部门树选择项是否关联显示
    /// </summary>
    public bool DeptCheckStrictly { get; set; } = true;

    /// <summary>
    /// 状态
    /// </summary>
    public UserStatus Status { get; set; } = UserStatus.Normal;

    /// <summary>
    /// 删除标志
    /// </summary>
    public DelFlag DelFlag { get; set; } = DelFlag.Exist;

    /// <summary>
    /// 角色菜单关联
    /// </summary>
    public List<SysRoleMenu> RoleMenus { get; set; } = new();

    /// <summary>
    /// 角色部门关联
    /// </summary>
    public List<SysRoleDept> RoleDepts { get; set; } = new();

    /// <summary>
    /// 是否为管理员角色
    /// </summary>
    public bool IsAdmin()
    {
        return RoleId == 1;
    }
}
