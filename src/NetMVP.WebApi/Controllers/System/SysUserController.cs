using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Dept;
using NetMVP.Application.DTOs.Post;
using NetMVP.Application.DTOs.Role;
using NetMVP.Application.DTOs.User;
using NetMVP.Application.Services;
using NetMVP.Domain.Constants;
using NetMVP.WebApi.Attributes;

namespace NetMVP.WebApi.Controllers.System;

/// <summary>
/// 用户管理控制器
/// </summary>
[ApiController]
[Route("system/user")]
[Authorize]
public class SysUserController : BaseController
{
    private readonly ISysUserService _userService;
    private readonly ISysDeptService _deptService;
    private readonly ISysRoleService _roleService;
    private readonly ISysPostService _postService;

    public SysUserController(
        ISysUserService userService, 
        ISysDeptService deptService,
        ISysRoleService roleService,
        ISysPostService postService)
    {
        _userService = userService;
        _deptService = deptService;
        _roleService = roleService;
        _postService = postService;
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    [HttpGet("list")]
    [RequirePermission("system:user:list")]
    public async Task<TableDataInfo> GetList([FromQuery] UserQueryDto query)
    {
        var (users, total) = await _userService.GetUserListAsync(query);
        return GetTableData(users, total);
    }

    /// <summary>
    /// 获取部门树（用于用户管理页面）
    /// </summary>
    [HttpGet("deptTree")]
    [RequirePermission("system:user:list")]
    public async Task<AjaxResult> GetDeptTree([FromQuery] DeptQueryDto query)
    {
        var tree = await _deptService.GetDeptTreeSelectAsync();
        return Success(tree);
    }

    /// <summary>
    /// 获取用户详情或新增/编辑用户所需的初始数据
    /// </summary>
    [HttpGet]
    [HttpGet("{userId:long}")]
    [RequirePermission("system:user:query")]
    public async Task<AjaxResult> GetInfo(long? userId = null)
    {
        // 如果提供了userId，获取用户详细信息
        if (userId.HasValue && userId.Value > 0)
        {
            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                return Error("用户不存在");
            }
            
            // 获取用户的岗位ID列表
            var userPosts = await _userService.GetUserPostIdsAsync(userId.Value);
            
            // 获取用户的角色ID列表
            var userRoles = await _userService.GetUserRoleIdsAsync(userId.Value);
            
            // 获取所有角色列表（用于下拉选择）
            var (roles, _) = await _roleService.GetRoleListAsync(new RoleQueryDto { PageNum = 1, PageSize = 1000 });
            
            // 获取所有岗位列表（用于下拉选择）
            var (posts, _) = await _postService.GetPostListAsync(new PostQueryDto { PageNum = 1, PageSize = 1000 });
            
            // 返回格式与若依Java保持一致
            var result = Success();
            result["data"] = user;
            result["postIds"] = userPosts;
            result["roleIds"] = userRoles;
            result["roles"] = roles;
            result["posts"] = posts;
            return result;
        }
        else
        {
            // 新增用户时，只返回角色和岗位列表
            var (roles, _) = await _roleService.GetRoleListAsync(new RoleQueryDto { PageNum = 1, PageSize = 1000 });
            var (posts, _) = await _postService.GetPostListAsync(new PostQueryDto { PageNum = 1, PageSize = 1000 });
            
            var result = Success();
            result["roles"] = roles;
            result["posts"] = posts;
            return result;
        }
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    [HttpPost]
    [RequirePermission("system:user:add")]
    [Log(Title = "用户管理", BusinessType = OperLogConstants.BUSINESS_TYPE_INSERT)]
    public async Task<AjaxResult> Add([FromBody] CreateUserDto dto)
    {
        try
        {
            var userId = await _userService.CreateUserAsync(dto);
            return Success(userId);
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    [HttpPut]
    [RequirePermission("system:user:edit")]
    [Log(Title = "用户管理", BusinessType = OperLogConstants.BUSINESS_TYPE_UPDATE)]
    public async Task<AjaxResult> Edit([FromBody] UpdateUserDto dto)
    {
        try
        {
            await _userService.UpdateUserAsync(dto);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 删除用户（支持单个和批量）
    /// </summary>
    [HttpDelete("{userIds}")]
    [RequirePermission("system:user:remove")]
    [Log(Title = "用户管理", BusinessType = OperLogConstants.BUSINESS_TYPE_DELETE)]
    public async Task<AjaxResult> Remove(string userIds)
    {
        try
        {
            var ids = userIds.Split(',').Select(long.Parse).ToArray();
            await _userService.DeleteUsersAsync(ids);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    [HttpPut("resetPwd")]
    [RequirePermission("system:user:resetPwd")]
    [Log(Title = "用户管理", BusinessType = OperLogConstants.BUSINESS_TYPE_UPDATE)]
    public async Task<AjaxResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        try
        {
            await _userService.ResetPasswordAsync(dto);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 修改用户状态
    /// </summary>
    [HttpPut("changeStatus")]
    [RequirePermission("system:user:edit")]
    [Log(Title = "用户管理", BusinessType = OperLogConstants.BUSINESS_TYPE_UPDATE)]
    public async Task<AjaxResult> ChangeStatus([FromBody] UpdateUserStatusDto dto)
    {
        try
        {
            await _userService.UpdateUserStatusAsync(dto.UserId, dto.Status);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 导出用户
    /// </summary>
    [HttpPost("export")]
    [RequirePermission("system:user:export")]
    [Log(Title = "用户管理", BusinessType = OperLogConstants.BUSINESS_TYPE_EXPORT)]
    public async Task<IActionResult> Export([FromForm] UserQueryDto query)
    {
        var data = await _userService.ExportUsersAsync(query);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"用户数据_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }

    /// <summary>
    /// 检查用户名唯一性
    /// </summary>
    [HttpGet("checkUserNameUnique")]
    public async Task<AjaxResult> CheckUserNameUnique([FromQuery] string userName, [FromQuery] long? userId)
    {
        var isUnique = await _userService.CheckUserNameUniqueAsync(userName, userId);
        return Success(isUnique);
    }

    /// <summary>
    /// 检查手机号唯一性
    /// </summary>
    [HttpGet("checkPhoneUnique")]
    public async Task<AjaxResult> CheckPhoneUnique([FromQuery] string phone, [FromQuery] long? userId)
    {
        var isUnique = await _userService.CheckPhoneUniqueAsync(phone, userId);
        return Success(isUnique);
    }

    /// <summary>
    /// 检查邮箱唯一性
    /// </summary>
    [HttpGet("checkEmailUnique")]
    public async Task<AjaxResult> CheckEmailUnique([FromQuery] string email, [FromQuery] long? userId)
    {
        var isUnique = await _userService.CheckEmailUniqueAsync(email, userId);
        return Success(isUnique);
    }

    /// <summary>
    /// 获取用户授权角色信息
    /// </summary>
    [HttpGet("authRole/{userId}")]
    [RequirePermission("system:user:query")]
    public async Task<AjaxResult> GetAuthRole(long userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return Error("用户不存在");
        }

        // 获取用户已分配的角色ID列表
        var userRoleIds = await _userService.GetUserRoleIdsAsync(userId);

        // 获取所有角色列表
        var (allRoles, _) = await _roleService.GetRoleListAsync(new RoleQueryDto { PageNum = 1, PageSize = 1000 });
        
        // 如果是超级管理员，返回所有角色；否则过滤掉超级管理员角色
        var roles = userId == 1 ? allRoles : allRoles.Where(r => r.RoleId != 1).ToList();

        // 为每个角色添加flag属性，表示是否已分配给该用户
        foreach (var role in roles)
        {
            role.Flag = userRoleIds.Contains(role.RoleId);
        }

        var result = Success();
        result["user"] = user;
        result["roles"] = roles;
        return result;
    }

    /// <summary>
    /// 用户授权角色
    /// </summary>
    [HttpPut("authRole")]
    [RequirePermission("system:user:edit")]
    [Log(Title = "用户管理", BusinessType = OperLogConstants.BUSINESS_TYPE_GRANT)]
    public async Task<AjaxResult> UpdateAuthRole([FromQuery] long userId, [FromQuery] string? roleIds)
    {
        try
        {
            // 解析roleIds字符串为long数组
            long[] roleIdArray;
            if (string.IsNullOrWhiteSpace(roleIds))
            {
                roleIdArray = Array.Empty<long>();
            }
            else
            {
                roleIdArray = roleIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(long.Parse)
                    .ToArray();
            }

            await _userService.UpdateUserRolesAsync(userId, roleIdArray);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 导入用户数据
    /// </summary>
    [HttpPost("importData")]
    [RequirePermission("system:user:import")]
    [Log(Title = "用户管理", BusinessType = OperLogConstants.BUSINESS_TYPE_IMPORT)]
    public async Task<AjaxResult> ImportData(IFormFile file, [FromQuery] string updateSupport = "0")
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return Error("请选择要导入的文件");
            }

            // 将字符串转换为布尔值（兼容若依前端发送的"0"或"1"）
            var isUpdateSupport = updateSupport == "1" || updateSupport.Equals("true", StringComparison.OrdinalIgnoreCase);

            using var stream = file.OpenReadStream();
            var message = await _userService.ImportUsersAsync(stream, isUpdateSupport);
            return Success(message);
        }
        catch (Exception ex)
        {
            return Error($"导入失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 下载用户导入模板
    /// </summary>
    [HttpPost("importTemplate")]
    public async Task<IActionResult> ImportTemplate()
    {
        var data = await _userService.GetImportTemplateAsync();
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"用户导入模板_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
}
