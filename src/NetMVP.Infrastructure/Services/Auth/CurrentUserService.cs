using Microsoft.AspNetCore.Http;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Infrastructure.Services.Auth;

/// <summary>
/// 当前用户服务实现
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    public long GetUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId") 
            ?? _httpContextAccessor.HttpContext?.User.FindFirst("sub");
        
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            return 1; // 默认返回管理员ID（用于后台任务等场景）
        }
        
        return userId;
    }

    /// <summary>
    /// 获取当前用户名
    /// </summary>
    public string GetUserName()
    {
        var userNameClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userName") 
            ?? _httpContextAccessor.HttpContext?.User.FindFirst("name");
        
        return userNameClaim?.Value ?? "system";
    }

    /// <summary>
    /// 是否已认证
    /// </summary>
    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}
