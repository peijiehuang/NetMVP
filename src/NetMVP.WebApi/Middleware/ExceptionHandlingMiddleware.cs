using System.Net;
using System.Text.Json;
using NetMVP.Application.Common.Models;
using NetMVP.Domain.Exceptions;

namespace NetMVP.WebApi.Middleware;

/// <summary>
/// 全局异常处理中间件
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // 记录异常日志
        LogException(context, exception);

        // 设置响应
        context.Response.ContentType = "application/json";
        var response = CreateErrorResponse(exception);
        context.Response.StatusCode = response.StatusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response.Body, options));
    }

    private void LogException(HttpContext context, Exception exception)
    {
        var request = context.Request;
        var logMessage = $@"
========== 异常信息 ==========
时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
路径: {request.Method} {request.Path}{request.QueryString}
IP: {context.Connection.RemoteIpAddress}
用户: {context.User?.Identity?.Name ?? "匿名"}
异常类型: {exception.GetType().Name}
异常消息: {exception.Message}
堆栈跟踪: {exception.StackTrace}
============================";

        _logger.LogError(exception, logMessage);
    }

    private (int StatusCode, AjaxResult Body) CreateErrorResponse(Exception exception)
    {
        return exception switch
        {
            BusinessException businessEx => (
                StatusCode: businessEx.Code,
                Body: AjaxResult.Error(businessEx.Message)
            ),
            ValidationException validationEx => (
                StatusCode: (int)HttpStatusCode.BadRequest,
                Body: AjaxResult.Error("验证失败", validationEx.Errors)
            ),
            NotFoundException notFoundEx => (
                StatusCode: (int)HttpStatusCode.NotFound,
                Body: AjaxResult.Error(notFoundEx.Message)
            ),
            UnauthorizedException unauthorizedEx => (
                StatusCode: (int)HttpStatusCode.Unauthorized,
                Body: AjaxResult.Error(unauthorizedEx.Message)
            ),
            ForbiddenException forbiddenEx => (
                StatusCode: (int)HttpStatusCode.Forbidden,
                Body: AjaxResult.Error(forbiddenEx.Message)
            ),
            _ => (
                StatusCode: (int)HttpStatusCode.InternalServerError,
                Body: _environment.IsDevelopment()
                    ? AjaxResult.Error(exception.Message, new { stackTrace = exception.StackTrace })
                    : AjaxResult.Error("服务器内部错误，请稍后重试")
            )
        };
    }
}
