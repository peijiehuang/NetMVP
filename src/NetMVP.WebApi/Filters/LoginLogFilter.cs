using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NetMVP.Application.DTOs.LoginInfo;
using NetMVP.Application.Services;
using NetMVP.Domain.Constants;
using NetMVP.Infrastructure.Utils;

namespace NetMVP.WebApi.Filters;

/// <summary>
/// 登录日志过滤器
/// </summary>
public class LoginLogFilter : IAsyncActionFilter
{
    private readonly ISysLoginInfoService _loginInfoService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<LoginLogFilter> _logger;

    public LoginLogFilter(
        ISysLoginInfoService loginInfoService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<LoginLogFilter> logger)
    {
        _loginInfoService = loginInfoService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = _httpContextAccessor.HttpContext!;
        var request = httpContext.Request;

        // 只处理登录和登出请求
        var path = request.Path.Value?.ToLower() ?? "";
        if (path != SystemConstants.LOGIN_PATH && path != SystemConstants.LOGOUT_PATH)
        {
            await next();
            return;
        }

        // 获取客户端信息
        var ipAddress = IpUtils.GetIpAddress(httpContext);
        var browser = GetBrowser(httpContext);
        var os = GetOperatingSystem(httpContext);

        // 获取用户名
        string userName = SystemConstants.UNKNOWN_USER;
        if (path.Contains(SystemConstants.LOGIN_PATH) && context.ActionArguments.TryGetValue("dto", out var loginDto))
        {
            var dto = loginDto as dynamic;
            userName = dto?.UserName ?? SystemConstants.UNKNOWN_USER;
        }
        else if (path.Contains(SystemConstants.LOGOUT_PATH))
        {
            userName = httpContext.User.Identity?.Name ?? SystemConstants.UNKNOWN_USER;
        }

        // 执行操作
        var executedContext = await next();

        // 记录日志
        var logDto = new CreateLoginInfoDto
        {
            UserName = userName,
            IpAddr = ipAddress,
            LoginLocation = SystemConstants.INTERNAL_IP_LOCATION,
            Browser = browser,
            Os = os
        };

        // 判断操作是否成功
        if (executedContext.Exception != null)
        {
            logDto.Status = CommonConstants.FAIL;
            logDto.Msg = executedContext.Exception.Message;
        }
        else if (executedContext.Result is ObjectResult objectResult)
        {
            // 检查返回结果
            if (objectResult.Value is IDictionary<string, object> dict)
            {
                if (dict.TryGetValue("code", out var code) && code?.ToString() == "200")
                {
                    logDto.Status = CommonConstants.SUCCESS;
                    logDto.Msg = path.Contains(SystemConstants.LOGIN_PATH) ? SystemConstants.LOGIN_SUCCESS_MSG : SystemConstants.LOGOUT_SUCCESS_MSG;
                }
                else
                {
                    logDto.Status = CommonConstants.FAIL;
                    logDto.Msg = dict.TryGetValue("msg", out var msg) ? msg?.ToString() ?? SystemConstants.OPERATION_FAILED_MSG : SystemConstants.OPERATION_FAILED_MSG;
                }
            }
            else
            {
                logDto.Status = CommonConstants.SUCCESS;
                logDto.Msg = path.Contains(SystemConstants.LOGIN_PATH) ? SystemConstants.LOGIN_SUCCESS_MSG : SystemConstants.LOGOUT_SUCCESS_MSG;
            }
        }
        else
        {
            logDto.Status = CommonConstants.SUCCESS;
            logDto.Msg = path.Contains(SystemConstants.LOGIN_PATH) ? SystemConstants.LOGIN_SUCCESS_MSG : SystemConstants.LOGOUT_SUCCESS_MSG;
        }

        // 异步保存日志（不影响主流程）
        try
        {
            await _loginInfoService.CreateLoginInfoAsync(logDto);
        }
        catch (Exception ex)
        {
            // 日志记录失败不影响业务，但记录错误日志以便追踪
            _logger.LogError(ex, "保存登录日志失败");
        }
    }

    /// <summary>
    /// 获取浏览器信息
    /// </summary>
    private static string GetBrowser(HttpContext context)
    {
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        if (userAgent.Contains("Chrome"))
            return "Chrome";
        if (userAgent.Contains("Firefox"))
            return "Firefox";
        if (userAgent.Contains("Safari"))
            return "Safari";
        if (userAgent.Contains("Edge"))
            return "Edge";
        return "Unknown";
    }

    /// <summary>
    /// 获取操作系统信息
    /// </summary>
    private static string GetOperatingSystem(HttpContext context)
    {
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        if (userAgent.Contains("Windows"))
            return "Windows";
        if (userAgent.Contains("Mac"))
            return "MacOS";
        if (userAgent.Contains("Linux"))
            return "Linux";
        if (userAgent.Contains("Android"))
            return "Android";
        if (userAgent.Contains("iOS"))
            return "iOS";
        return "Unknown";
    }
}
