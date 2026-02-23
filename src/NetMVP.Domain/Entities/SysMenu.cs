using NetMVP.Domain.Common;
using NetMVP.Domain.Constants;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 菜单实体
/// </summary>
public class SysMenu : BaseEntity
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public long MenuId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 父菜单ID
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int OrderNum { get; set; }

    /// <summary>
    /// 路由地址
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 路由参数
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// 路由名称
    /// </summary>
    public string? RouteName { get; set; }

    /// <summary>
    /// 是否为外链
    /// </summary>
    public bool IsFrame { get; set; }

    /// <summary>
    /// 是否缓存
    /// </summary>
    public bool IsCache { get; set; }

    /// <summary>
    /// 菜单类型（M目录 C菜单 F按钮）
    /// </summary>
    public string MenuType { get; set; } = UserConstants.TYPE_MENU;

    /// <summary>
    /// 显示状态（0显示 1隐藏）
    /// </summary>
    public string Visible { get; set; } = CommonConstants.VISIBLE_SHOW;

    /// <summary>
    /// 状态（0正常 1停用）
    /// </summary>
    public string Status { get; set; } = UserConstants.NORMAL;

    /// <summary>
    /// 权限标识
    /// </summary>
    public string? Perms { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 子菜单
    /// </summary>
    public List<SysMenu> Children { get; set; } = new();
}
