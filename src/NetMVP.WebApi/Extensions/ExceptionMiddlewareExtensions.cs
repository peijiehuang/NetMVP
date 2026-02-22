using NetMVP.WebApi.Middleware;

namespace NetMVP.WebApi.Extensions;

/// <summary>
/// 异常中间件扩展
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    /// <summary>
    /// 使用全局异常处理中间件
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
