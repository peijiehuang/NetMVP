using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NetMVP.Domain.Interfaces;
using System.Security.Claims;

namespace NetMVP.WebApi.Attributes;

/// <summary>
/// 角色验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireRoleAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _roleKey;

    public RequireRoleAttribute(string roleKey)
    {
        _roleKey = roleKey;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // 获取用户ID
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            context.Result = new UnauthorizedObjectResult(new { code = 401, msg = "未授权" });
            return;
        }

        // 获取权限服务
        var permissionService = context.HttpContext.RequestServices.GetService<IPermissionService>();
        if (permissionService == null)
        {
            context.Result = new StatusCodeResult(500);
            return;
        }

        // 检查角色
        var hasRole = await permissionService.HasRoleAsync(userId, _roleKey);
        if (!hasRole)
        {
            context.Result = new ObjectResult(new { code = 403, msg = "没有权限，请联系管理员授权" })
            {
                StatusCode = 403
            };
        }
    }
}
