using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Role;
using NetMVP.Application.Services;
using NetMVP.Domain.Constants;
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
    private readonly ISysDeptService _deptService;
    private readonly ISysUserService _userService;

    public SysRoleController(
        ISysRoleService roleService,
        ISysDeptService deptService,
        ISysUserService userService)
    {
        _roleService = roleService;
        _deptService = deptService;
        _userService = userService;
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
    [Log(Title = "角色管理", BusinessType = OperLogConstants.BUSINESS_TYPE_INSERT)]
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
    [Log(Title = "角色管理", BusinessType = OperLogConstants.BUSINESS_TYPE_UPDATE)]
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
    /// 删除角色（支持单个和批量）
    /// </summary>
    [HttpDelete("{roleIds}")]
    [RequirePermission("system:role:remove")]
    [Log(Title = "角色管理", BusinessType = OperLogConstants.BUSINESS_TYPE_DELETE)]
    public async Task<AjaxResult> Remove(string roleIds)
    {
        try
        {
            var ids = roleIds.Split(',').Select(long.Parse).ToArray();
            await _roleService.DeleteRolesAsync(ids);
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
    [Log(Title = "角色管理", BusinessType = OperLogConstants.BUSINESS_TYPE_UPDATE)]
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
    /// 获取角色选择框列表
    /// </summary>
    [HttpGet("optionselect")]
    [RequirePermission("system:role:query")]
    public async Task<AjaxResult> OptionSelect()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Success(roles);
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

    /// <summary>
    /// 获取对应角色部门树列表
    /// </summary>
    [HttpGet("deptTree/{roleId}")]
    [RequirePermission("system:role:query")]
    public async Task<AjaxResult> DeptTree(long roleId)
    {
        var result = Success();
        result.Put("checkedKeys", await _deptService.GetDeptIdsByRoleIdAsync(roleId));
        result.Put("depts", await _deptService.GetDeptTreeSelectAsync());
        return result;
    }

    /// <summary>
    /// 修改保存数据权限
    /// </summary>
    [HttpPut("dataScope")]
    [RequirePermission("system:role:edit")]
    [Log(Title = "角色管理", BusinessType = OperLogConstants.BUSINESS_TYPE_UPDATE)]
    public async Task<AjaxResult> DataScope([FromBody] DataScopeDto dto)
    {
        try
        {
            await _roleService.UpdateDataScopeAsync(dto.RoleId, dto.DataScope, dto.DeptIds);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 查询已分配用户角色列表
    /// </summary>
    [HttpGet("authUser/allocatedList")]
    [RequirePermission("system:role:list")]
    public async Task<TableDataInfo> AllocatedList([FromQuery] AllocatedUserQueryDto query)
    {
        var (users, total) = await _userService.GetAllocatedUserListAsync(
            query.RoleId, query.UserName, query.Phonenumber, query.PageNum, query.PageSize);
        return GetTableData(users, total);
    }

    /// <summary>
    /// 查询未分配用户角色列表
    /// </summary>
    [HttpGet("authUser/unallocatedList")]
    [RequirePermission("system:role:list")]
    public async Task<TableDataInfo> UnallocatedList([FromQuery] UnallocatedUserQueryDto query)
    {
        var (users, total) = await _userService.GetUnallocatedUserListAsync(
            query.RoleId, query.UserName, query.Phonenumber, query.PageNum, query.PageSize);
        return GetTableData(users, total);
    }

    /// <summary>
    /// 取消授权用户
    /// </summary>
    [HttpPut("authUser/cancel")]
    [RequirePermission("system:role:edit")]
    [Log(Title = "角色管理", BusinessType = OperLogConstants.BUSINESS_TYPE_GRANT)]
    public async Task<AjaxResult> CancelAuthUser([FromBody] UserRoleDto dto)
    {
        try
        {
            await _roleService.CancelAuthUserAsync(dto.RoleId, dto.UserId);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 批量取消授权用户
    /// </summary>
    [HttpPut("authUser/cancelAll")]
    [RequirePermission("system:role:edit")]
    [Log(Title = "角色管理", BusinessType = OperLogConstants.BUSINESS_TYPE_GRANT)]
    public async Task<AjaxResult> CancelAuthUserAll([FromQuery] long roleId, [FromQuery] long[] userIds)
    {
        try
        {
            await _roleService.CancelAuthUsersAsync(roleId, userIds);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 批量选择用户授权
    /// </summary>
    [HttpPut("authUser/selectAll")]
    [RequirePermission("system:role:edit")]
    [Log(Title = "角色管理", BusinessType = OperLogConstants.BUSINESS_TYPE_GRANT)]
    public async Task<AjaxResult> SelectAuthUserAll([FromQuery] long roleId, [FromQuery] long[] userIds)
    {
        try
        {
            await _roleService.InsertAuthUsersAsync(roleId, userIds);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 导出角色数据
    /// </summary>
    [HttpPost("export")]
    [RequirePermission("system:role:export")]
    [Log(Title = "角色管理", BusinessType = OperLogConstants.BUSINESS_TYPE_EXPORT)]
    public async Task<IActionResult> Export([FromForm] RoleQueryDto query)
    {
        var data = await _roleService.ExportRolesAsync(query);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"角色数据_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
}

/// <summary>
/// 修改状态 DTO
/// </summary>
public class ChangeStatusDto
{
    public long RoleId { get; set; }
    public string Status { get; set; } = UserConstants.NORMAL;
}

/// <summary>
/// 数据权限 DTO
/// </summary>
public class DataScopeDto
{
    public long RoleId { get; set; }
    public string DataScope { get; set; } = string.Empty;
    public List<long> DeptIds { get; set; } = new();
}

/// <summary>
/// 用户角色 DTO
/// </summary>
public class UserRoleDto
{
    public long UserId { get; set; }
    public long RoleId { get; set; }
}

/// <summary>
/// 已分配用户查询 DTO
/// </summary>
public class AllocatedUserQueryDto
{
    public long RoleId { get; set; }
    public string? UserName { get; set; }
    public string? Phonenumber { get; set; }
    public int PageNum { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// 未分配用户查询 DTO
/// </summary>
public class UnallocatedUserQueryDto
{
    public long RoleId { get; set; }
    public string? UserName { get; set; }
    public string? Phonenumber { get; set; }
    public int PageNum { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
