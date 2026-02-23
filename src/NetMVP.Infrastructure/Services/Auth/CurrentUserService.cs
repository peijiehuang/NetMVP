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
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User == null)
        {
            throw new UnauthorizedAccessException("未找到当前用户上下文");
        }

        // 优先查找 sub claim（JWT标准）
        var userIdClaim = httpContext.User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
            ?? httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
            ?? httpContext.User.FindFirst("userId");
        
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("无法获取当前用户ID");
        }
        
        return userId;
    }

    /// <summary>
    /// 获取当前用户名
    /// </summary>
    public string GetUserName()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User == null)
        {
            return "system";
        }

        // 优先查找 unique_name claim（JWT标准）
        var userNameClaim = httpContext.User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName)
            ?? httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Name)
            ?? httpContext.User.FindFirst("userName");
        
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
