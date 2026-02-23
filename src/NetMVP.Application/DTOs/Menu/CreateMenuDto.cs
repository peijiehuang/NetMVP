using NetMVP.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.Menu;

/// <summary>
/// 创建菜单 DTO
/// </summary>
public class CreateMenuDto
{
    /// <summary>
    /// 菜单名称
    /// </summary>
    [Required(ErrorMessage = "菜单名称不能为空")]
    [StringLength(50, ErrorMessage = "菜单名称长度不能超过50个字符")]
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 父菜单ID
    /// </summary>
    public long ParentId { get; set; } = 0;

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int OrderNum { get; set; }

    /// <summary>
    /// 路由地址
    /// </summary>
    [StringLength(200, ErrorMessage = "路由地址长度不能超过200个字符")]
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    [StringLength(255, ErrorMessage = "组件路径长度不能超过255个字符")]
    public string? Component { get; set; }

    /// <summary>
    /// 路由参数
    /// </summary>
    [StringLength(255, ErrorMessage = "路由参数长度不能超过255个字符")]
    public string? Query { get; set; }

    /// <summary>
    /// 是否为外链（0否 1是）
    /// </summary>
    public bool IsFrame { get; set; } = false;

    /// <summary>
    /// 是否缓存（0不缓存 1缓存）
    /// </summary>
    public bool IsCache { get; set; } = false;

    /// <summary>
    /// 菜单类型（Directory目录 Menu菜单 Button按钮）
    /// </summary>
    [Required(ErrorMessage = "菜单类型不能为空")]
    public string MenuType { get; set; } = UserConstants.TYPE_MENU;

    /// <summary>
    /// 显示状态（Show显示 Hide隐藏）
    /// </summary>
    public string Visible { get; set; } = CommonConstants.VISIBLE_SHOW;

    /// <summary>
    /// 菜单状态（Normal正常 Disabled停用）
    /// </summary>
    public string Status { get; set; } = UserConstants.NORMAL;

    /// <summary>
    /// 权限标识
    /// </summary>
    [StringLength(100, ErrorMessage = "权限标识长度不能超过100个字符")]
    public string? Perms { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    [StringLength(100, ErrorMessage = "菜单图标长度不能超过100个字符")]
    public string? Icon { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}
