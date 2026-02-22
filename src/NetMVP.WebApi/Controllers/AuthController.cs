using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Auth;
using NetMVP.Application.Services;
using NetMVP.Domain.Interfaces;

namespace NetMVP.WebApi.Controllers;

/// <summary>
/// 认证控制器
/// </summary>
[ApiController]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;
    private readonly ISysUserService _userService;
    private readonly ISysMenuService _menuService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionService _permissionService;
    private readonly ISysUserRepository _userRepository;

    public AuthController(
        IAuthService authService, 
        IJwtService jwtService,
        ISysUserService userService,
        ISysMenuService menuService,
        ICurrentUserService currentUserService,
        IPermissionService permissionService,
        ISysUserRepository userRepository)
    {
        _authService = authService;
        _jwtService = jwtService;
        _userService = userService;
        _menuService = menuService;
        _currentUserService = currentUserService;
        _permissionService = permissionService;
        _userRepository = userRepository;
    }

    /// <summary>
    /// 获取验证码
    /// </summary>
    [HttpGet("/captchaImage")]
    [AllowAnonymous]
    public async Task<AjaxResult> GetCaptcha()
    {
        var (uuid, image, captchaEnabled) = await _authService.GetCaptchaAsync();

        return Success()
            .Put("uuid", uuid)
            .Put("img", image)
            .Put("captchaEnabled", captchaEnabled);
    }

    /// <summary>
    /// 登录
    /// </summary>
    [HttpPost("/login")]
    [AllowAnonymous]
    public async Task<AjaxResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return Error("请求参数错误");

        try
        {
            var (accessToken, refreshToken) = await _authService.LoginAsync(
                dto.UserName,
                dto.Password,
                dto.Code,
                dto.Uuid);

            // 若依前端期望返回token字段，而不是access_token
            return Success()
                .Put("token", accessToken);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 登出
    /// </summary>
    [HttpPost("/logout")]
    public async Task<AjaxResult> Logout()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        
        if (!string.IsNullOrEmpty(token))
        {
            await _authService.LogoutAsync(token);
        }

        return Success("退出成功");
    }

    /// <summary>
    /// 刷新 Token
    /// </summary>
    [HttpPost("/refresh")]
    [AllowAnonymous]
    public async Task<AjaxResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        try
        {
            var (accessToken, refreshToken) = await _jwtService.RefreshTokenAsync(dto.RefreshToken);

            return Success()
                .Put("access_token", accessToken)
                .Put("refresh_token", refreshToken)
                .Put("expires_in", 7200);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 测试用户查询（临时调试接口）
    /// </summary>
    [HttpGet("/debug/user/{username}")]
    [AllowAnonymous]
    public async Task<AjaxResult> DebugGetUser(string username)
    {
        var user = await _userRepository.GetByUserNameAsync(username);
        if (user == null)
            return Error("用户不存在");

        return Success()
            .Put("userId", user.UserId)
            .Put("userName", user.UserName)
            .Put("nickName", user.NickName)
            .Put("status", user.Status)
            .Put("delFlag", user.DelFlag)
            .Put("passwordHash", user.Password.Substring(0, 20) + "...")
            .Put("hasPassword", !string.IsNullOrEmpty(user.Password));
    }

    /// <summary>
    /// 测试数据库连接和数据（临时调试接口）
    /// </summary>
    [HttpGet("/debug/db-check")]
    [AllowAnonymous]
    public async Task<AjaxResult> DebugDbCheck()
    {
        try
        {
            var allUsers = await _userRepository.GetAllAsync();
            return Success()
                .Put("totalUsers", allUsers.Count)
                .Put("users", allUsers.Select(u => new { u.UserId, u.UserName, u.NickName, u.DelFlag }).ToList());
        }
        catch (Exception ex)
        {
            return Error($"数据库错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    [HttpGet("/getInfo")]
    public async Task<AjaxResult> GetInfo()
    {
        var userId = _currentUserService.GetUserId();
        
        // 获取用户详细信息
        var userDto = await _userService.GetUserByIdAsync(userId);
        if (userDto == null)
            return Error("获取用户信息失败");

        // 获取角色键列表
        var roles = await _permissionService.GetUserRolesAsync(userId);
        
        // 获取菜单权限
        var permissions = await _permissionService.GetUserPermissionsAsync(userId);

        return Success()
            .Put("user", userDto)
            .Put("roles", roles)
            .Put("permissions", permissions);
    }

    /// <summary>
    /// 获取路由信息
    /// </summary>
    [HttpGet("/getRouters")]
    public async Task<AjaxResult> GetRouters()
    {
        var userId = _currentUserService.GetUserId();
        
        // 获取用户路由
        var routers = await _menuService.GetRoutersByUserIdAsync(userId);

        return Success(routers);
    }

    /// <summary>
    /// 调试：获取原始菜单数据
    /// </summary>
    [HttpGet("/debug/menus")]
    [AllowAnonymous]
    public async Task<AjaxResult> DebugGetMenus()
    {
        var userId = 1L; // admin用户
        var menus = await _menuService.GetMenuTreeByUserIdAsync(userId);
        
        return Success()
            .Put("total", menus.Count)
            .Put("menus", menus.Where(m => m.MenuId >= 100 && m.MenuId <= 102).ToList());
    }
}

/// <summary>
/// 刷新 Token DTO
/// </summary>
public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
