namespace NetMVP.Application.Common.Constants;

/// <summary>
/// HTTP 状态码常量
/// </summary>
public static class HttpStatus
{
    /// <summary>
    /// 成功
    /// </summary>
    public const int SUCCESS = 200;

    /// <summary>
    /// 警告
    /// </summary>
    public const int WARN = 601;

    /// <summary>
    /// 错误
    /// </summary>
    public const int ERROR = 500;

    /// <summary>
    /// 未授权
    /// </summary>
    public const int UNAUTHORIZED = 401;

    /// <summary>
    /// 禁止访问
    /// </summary>
    public const int FORBIDDEN = 403;

    /// <summary>
    /// 未找到
    /// </summary>
    public const int NOT_FOUND = 404;

    /// <summary>
    /// 请求错误
    /// </summary>
    public const int BAD_REQUEST = 400;
}
