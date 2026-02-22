using NetMVP.Domain.Enums;

namespace NetMVP.WebApi.Attributes;

/// <summary>
/// 操作日志记录特性
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class LogAttribute : Attribute
{
    /// <summary>
    /// 模块标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 业务类型
    /// </summary>
    public BusinessType BusinessType { get; set; } = BusinessType.Other;

    /// <summary>
    /// 是否保存请求参数
    /// </summary>
    public bool IsSaveRequestData { get; set; } = true;

    /// <summary>
    /// 是否保存响应参数
    /// </summary>
    public bool IsSaveResponseData { get; set; } = true;
}
