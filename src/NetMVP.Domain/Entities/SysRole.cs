using NetMVP.Domain.Common;
using NetMVP.Domain.Constants;

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
    /// 数据范围（1全部 2自定义 3本部门 4本部门及以下 5仅本人）
    /// </summary>
    public string DataScope { get; set; } = DataScopeConstants.DATA_SCOPE_ALL;

    /// <summary>
    /// 菜单树选择项是否关联显示
    /// </summary>
    public bool MenuCheckStrictly { get; set; } = true;

    /// <summary>
    /// 部门树选择项是否关联显示
    /// </summary>
    public bool DeptCheckStrictly { get; set; } = true;

    /// <summary>
    /// 状态（0正常 1停用）
    /// </summary>
    public string Status { get; set; } = UserConstants.NORMAL;

    /// <summary>
    /// 删除标志（0存在 2删除）
    /// </summary>
    public string DelFlag { get; set; } = UserConstants.DEL_FLAG_EXIST;

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
