using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Role;
using NetMVP.Application.Services;
using NetMVP.Domain.Enums;
using NetMVP.WebApi.Attributes;

namespace NetMVP.WebApi.Controllers.System;

/// <summary>
/// 角色管理控制器
/// </summary>
[ApiController]
[Route("system/role")]
[Authorize]
public class SysRoleController : BaseController
{
    private readonly ISysRoleService _roleService;

    public SysRoleController(ISysRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// 获取角色列表
    /// </summary>
    [HttpGet("list")]
    [RequirePermission("system:role:list")]
    public async Task<TableDataInfo> GetList([FromQuery] RoleQueryDto query)
    {
        var (roles, total) = await _roleService.GetRoleListAsync(query);
        return GetTableData(roles, total);
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    [HttpGet("{roleId}")]
    [RequirePermission("system:role:query")]
    public async Task<AjaxResult> GetInfo(long roleId)
    {
        var role = await _roleService.GetRoleByIdAsync(roleId);
        if (role == null)
        {
            return Error("角色不存在");
        }

        return Success(role);
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    [HttpPost]
    [RequirePermission("system:role:add")]
    [Log(Title = "角色管理", BusinessType = BusinessType.Insert)]
    public async Task<AjaxResult> Add([FromBody] CreateRoleDto dto)
    {
        try
        {
            var roleId = await _roleService.CreateRoleAsync(dto);
            return Success(roleId);
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    [HttpPut]
    [RequirePermission("system:role:edit")]
    [Log(Title = "角色管理", BusinessType = BusinessType.Update)]
    public async Task<AjaxResult> Edit([FromBody] UpdateRoleDto dto)
    {
        try
        {
            await _roleService.UpdateRoleAsync(dto);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    [HttpDelete("{roleId}")]
    [RequirePermission("system:role:remove")]
    [Log(Title = "角色管理", BusinessType = BusinessType.Delete)]
    public async Task<AjaxResult> Remove(long roleId)
    {
        try
        {
            await _roleService.DeleteRoleAsync(roleId);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 批量删除角色
    /// </summary>
    [HttpDelete]
    [RequirePermission("system:role:remove")]
    [Log(Title = "角色管理", BusinessType = BusinessType.Delete)]
    public async Task<AjaxResult> RemoveBatch([FromBody] long[] roleIds)
    {
        try
        {
            await _roleService.DeleteRolesAsync(roleIds);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 修改角色状态
    /// </summary>
    [HttpPut("changeStatus")]
    [RequirePermission("system:role:edit")]
    [Log(Title = "角色管理", BusinessType = BusinessType.Update)]
    public async Task<AjaxResult> ChangeStatus([FromBody] ChangeStatusDto dto)
    {
        try
        {
            await _roleService.UpdateRoleStatusAsync(dto.RoleId, dto.Status);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 检查角色名称唯一性
    /// </summary>
    [HttpGet("checkRoleNameUnique")]
    public async Task<AjaxResult> CheckRoleNameUnique([FromQuery] string roleName, [FromQuery] long? roleId)
    {
        var isUnique = await _roleService.CheckRoleNameUniqueAsync(roleName, roleId);
        return Success(isUnique);
    }

    /// <summary>
    /// 检查角色权限字符串唯一性
    /// </summary>
    [HttpGet("checkRoleKeyUnique")]
    public async Task<AjaxResult> CheckRoleKeyUnique([FromQuery] string roleKey, [FromQuery] long? roleId)
    {
        var isUnique = await _roleService.CheckRoleKeyUniqueAsync(roleKey, roleId);
        return Success(isUnique);
    }
}

/// <summary>
/// 修改状态 DTO
/// </summary>
public class ChangeStatusDto
{
    public long RoleId { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Normal;
}
