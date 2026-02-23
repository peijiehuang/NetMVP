using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Menu;
using NetMVP.Application.Services;
using NetMVP.Domain.Constants;
using NetMVP.WebApi.Attributes;
using System.Security.Claims;

namespace NetMVP.WebApi.Controllers.System;

/// <summary>
/// 菜单管理控制器
/// </summary>
[ApiController]
[Route("system/menu")]
[Authorize]
public class SysMenuController : BaseController
{
    private readonly ISysMenuService _menuService;

    public SysMenuController(ISysMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// 获取菜单列表
    /// </summary>
    [HttpGet("list")]
    [RequirePermission("system:menu:list")]
    public async Task<AjaxResult> GetList([FromQuery] MenuQueryDto query)
    {
        var menus = await _menuService.GetMenuListAsync(query);
        return Success(menus);
    }

    /// <summary>
    /// 获取菜单树
    /// </summary>
    [HttpGet("tree")]
    [RequirePermission("system:menu:list")]
    public async Task<AjaxResult> GetTree([FromQuery] MenuQueryDto query)
    {
        var tree = await _menuService.GetMenuTreeAsync(query);
        return Success(tree);
    }

    /// <summary>
    /// 获取菜单详情
    /// </summary>
    [HttpGet("{menuId}")]
    [RequirePermission("system:menu:query")]
    public async Task<AjaxResult> GetInfo(long menuId)
    {
        var menu = await _menuService.GetMenuByIdAsync(menuId);
        if (menu == null)
        {
            return Error("菜单不存在");
        }

        return Success(menu);
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    [HttpPost]
    [RequirePermission("system:menu:add")]
    [Log(Title = "菜单管理", BusinessType = OperLogConstants.BUSINESS_TYPE_INSERT)]
    public async Task<AjaxResult> Add([FromBody] CreateMenuDto dto)
    {
        try
        {
            var menuId = await _menuService.CreateMenuAsync(dto);
            return Success(menuId);
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    [HttpPut]
    [RequirePermission("system:menu:edit")]
    [Log(Title = "菜单管理", BusinessType = OperLogConstants.BUSINESS_TYPE_UPDATE)]
    public async Task<AjaxResult> Edit([FromBody] UpdateMenuDto dto)
    {
        try
        {
            await _menuService.UpdateMenuAsync(dto);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    [HttpDelete("{menuId}")]
    [RequirePermission("system:menu:remove")]
    [Log(Title = "菜单管理", BusinessType = OperLogConstants.BUSINESS_TYPE_DELETE)]
    public async Task<AjaxResult> Remove(long menuId)
    {
        try
        {
            await _menuService.DeleteMenuAsync(menuId);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 获取菜单树选择列表
    /// </summary>
    [HttpGet("treeselect")]
    public async Task<AjaxResult> TreeSelect()
    {
        var tree = await _menuService.GetMenuTreeSelectAsync();
        return Success(tree);
    }

    /// <summary>
    /// 获取角色菜单树
    /// </summary>
    [HttpGet("roleMenuTreeselect/{roleId}")]
    public async Task<AjaxResult> RoleMenuTreeSelect(long roleId)
    {
        // 获取所有菜单树
        var menuTree = await _menuService.GetMenuTreeSelectAsync();
        
        // 获取角色已选菜单ID列表
        var checkedKeys = await _menuService.GetMenuIdsByRoleIdAsync(roleId);

        // 使用 Put 方法将数据添加到顶层，而不是嵌套在 data 中
        var result = Success();
        result.Put("checkedKeys", checkedKeys);
        result.Put("menus", menuTree);
        return result;
    }

    /// <summary>
    /// 获取路由信息
    /// </summary>
    [HttpGet("getRouters")]
    public async Task<AjaxResult> GetRouters()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            return Error("未授权");
        }

        var routers = await _menuService.GetRoutersByUserIdAsync(userId);
        return Success(routers);
    }

    /// <summary>
    /// 检查菜单名称唯一性
    /// </summary>
    [HttpGet("checkMenuNameUnique")]
    public async Task<AjaxResult> CheckMenuNameUnique([FromQuery] string menuName, [FromQuery] long parentId, [FromQuery] long? menuId)
    {
        var isUnique = await _menuService.CheckMenuNameUniqueAsync(menuName, parentId, menuId);
        return Success(isUnique);
    }
}
