using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Dept;
using NetMVP.Application.Services;
using NetMVP.Domain.Constants;
using NetMVP.WebApi.Attributes;

namespace NetMVP.WebApi.Controllers.System;

/// <summary>
/// 部门管理控制器
/// </summary>
[ApiController]
[Route("system/dept")]
[Authorize]
public class SysDeptController : BaseController
{
    private readonly ISysDeptService _deptService;

    public SysDeptController(ISysDeptService deptService)
    {
        _deptService = deptService;
    }

    /// <summary>
    /// 获取部门列表
    /// </summary>
    [HttpGet("list")]
    [RequirePermission("system:dept:list")]
    public async Task<AjaxResult> GetList([FromQuery] DeptQueryDto query)
    {
        var depts = await _deptService.GetDeptListAsync(query);
        return Success(depts);
    }

    /// <summary>
    /// 查询部门列表（排除节点）
    /// </summary>
    [HttpGet("list/exclude/{deptId}")]
    [RequirePermission("system:dept:list")]
    public async Task<AjaxResult> ExcludeChild(long deptId)
    {
        var depts = await _deptService.GetDeptListExcludeChildAsync(deptId);
        return Success(depts);
    }

    /// <summary>
    /// 获取部门树
    /// </summary>
    [HttpGet("tree")]
    [RequirePermission("system:dept:list")]
    public async Task<AjaxResult> GetTree([FromQuery] DeptQueryDto query)
    {
        var tree = await _deptService.GetDeptTreeAsync(query);
        return Success(tree);
    }

    /// <summary>
    /// 获取部门详情
    /// </summary>
    [HttpGet("{deptId}")]
    [RequirePermission("system:dept:query")]
    public async Task<AjaxResult> GetInfo(long deptId)
    {
        var dept = await _deptService.GetDeptByIdAsync(deptId);
        if (dept == null)
        {
            return Error("部门不存在");
        }

        return Success(dept);
    }

    /// <summary>
    /// 创建部门
    /// </summary>
    [HttpPost]
    [RequirePermission("system:dept:add")]
    [Log(Title = "部门管理", BusinessType = OperLogConstants.BUSINESS_TYPE_INSERT)]
    public async Task<AjaxResult> Add([FromBody] CreateDeptDto dto)
    {
        try
        {
            var deptId = await _deptService.CreateDeptAsync(dto);
            return Success(deptId);
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新部门
    /// </summary>
    [HttpPut]
    [RequirePermission("system:dept:edit")]
    [Log(Title = "部门管理", BusinessType = OperLogConstants.BUSINESS_TYPE_UPDATE)]
    public async Task<AjaxResult> Edit([FromBody] UpdateDeptDto dto)
    {
        try
        {
            await _deptService.UpdateDeptAsync(dto);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    [HttpDelete("{deptId}")]
    [RequirePermission("system:dept:remove")]
    [Log(Title = "部门管理", BusinessType = OperLogConstants.BUSINESS_TYPE_DELETE)]
    public async Task<AjaxResult> Remove(long deptId)
    {
        try
        {
            await _deptService.DeleteDeptAsync(deptId);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 获取部门树选择列表
    /// </summary>
    [HttpGet("treeselect")]
    public async Task<AjaxResult> TreeSelect()
    {
        var tree = await _deptService.GetDeptTreeSelectAsync();
        return Success(tree);
    }

    /// <summary>
    /// 获取角色部门树
    /// </summary>
    [HttpGet("roleDeptTreeselect/{roleId}")]
    public async Task<AjaxResult> RoleDeptTreeSelect(long roleId)
    {
        var tree = await _deptService.GetRoleDeptTreeSelectAsync(roleId);
        return Success(new { checkedKeys = new List<long>(), depts = tree });
    }

    /// <summary>
    /// 检查部门名称唯一性
    /// </summary>
    [HttpGet("checkDeptNameUnique")]
    public async Task<AjaxResult> CheckDeptNameUnique([FromQuery] string deptName, [FromQuery] long parentId, [FromQuery] long? deptId)
    {
        var isUnique = await _deptService.CheckDeptNameUniqueAsync(deptName, parentId, deptId);
        return Success(isUnique);
    }
}
