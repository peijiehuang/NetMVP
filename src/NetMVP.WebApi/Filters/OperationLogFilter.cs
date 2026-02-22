using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NetMVP.Application.DTOs.OperLog;
using NetMVP.Application.Services;
using NetMVP.Domain.Enums;
using NetMVP.WebApi.Attributes;
using System.Diagnostics;
using System.Text.Json;

namespace NetMVP.WebApi.Filters;

/// <summary>
/// 操作日志过滤器
/// </summary>
public class OperationLogFilter : IAsyncActionFilter
{
    private readonly ISysOperLogService _operLogService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OperationLogFilter(
        ISysOperLogService operLogService,
        IHttpContextAccessor httpContextAccessor)
    {
        _operLogService = operLogService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 检查是否有Log特性
        var logAttribute = context.ActionDescriptor.EndpointMetadata
            .OfType<LogAttribute>()
            .FirstOrDefault();

        if (logAttribute == null)
        {
            await next();
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var httpContext = _httpContextAccessor.HttpContext!;

        // 获取请求信息
        var request = httpContext.Request;
        var operUrl = $"{request.Path}{request.QueryString}";
        var requestMethod = request.Method;

        // 获取请求参数
        string? operParam = null;
        if (logAttribute.IsSaveRequestData)
        {
            if (context.ActionArguments.Count > 0)
            {
                try
                {
                    operParam = JsonSerializer.Serialize(context.ActionArguments);
                    if (operParam.Length > 2000)
                    {
                        operParam = operParam.Substring(0, 2000);
                    }
                }
                catch
                {
                    operParam = "参数序列化失败";
                }
            }
        }

        // 获取用户信息
        var userName = httpContext.User.Identity?.Name ?? "匿名用户";
        var ipAddress = GetClientIpAddress(httpContext);

        // 执行操作
        var executedContext = await next();
        stopwatch.Stop();

        // 构建日志DTO
        var logDto = new CreateOperLogDto
        {
            Title = logAttribute.Title,
            BusinessType = logAttribute.BusinessType,
            Method = $"{context.ActionDescriptor.RouteValues["controller"]}.{context.ActionDescriptor.RouteValues["action"]}",
            RequestMethod = requestMethod,
            OperatorType = OperatorType.BackendUser,
            OperName = userName,
            OperUrl = operUrl,
            OperIp = ipAddress,
            OperLocation = "内网IP",
            OperParam = operParam,
            CostTime = stopwatch.ElapsedMilliseconds
        };

        // 判断操作是否成功
        if (executedContext.Exception != null)
        {
            logDto.Status = CommonStatus.Failure;
            logDto.ErrorMsg = executedContext.Exception.Message;
            if (logDto.ErrorMsg.Length > 2000)
            {
                logDto.ErrorMsg = logDto.ErrorMsg.Substring(0, 2000);
            }
        }
        else
        {
            logDto.Status = CommonStatus.Success;

            // 保存响应结果
            if (logAttribute.IsSaveResponseData && executedContext.Result is ObjectResult objectResult)
            {
                try
                {
                    var jsonResult = JsonSerializer.Serialize(objectResult.Value);
                    if (jsonResult.Length > 2000)
                    {
                        jsonResult = jsonResult.Substring(0, 2000);
                    }
                    logDto.JsonResult = jsonResult;
                }
                catch
                {
                    logDto.JsonResult = "响应序列化失败";
                }
            }
        }

        // 异步保存日志（不影响主流程）
        _ = Task.Run(async () =>
        {
            try
            {
                await _operLogService.CreateOperLogAsync(logDto);
            }
            catch
            {
                // 日志记录失败不影响业务
            }
        });
    }

    /// <summary>
    /// 获取客户端IP地址
    /// </summary>
    private static string GetClientIpAddress(HttpContext context)
    {
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        }
        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Connection.RemoteIpAddress?.ToString();
        }
        return ip ?? "127.0.0.1";
    }
}
