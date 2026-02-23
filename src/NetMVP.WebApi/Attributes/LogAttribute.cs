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
    /// 业务类型（0其它 1新增 2修改 3删除 4授权 5导出 6导入 7强退 8生成代码 9清空数据）
    /// </summary>
    public string BusinessType { get; set; } = "0";

    /// <summary>
    /// 是否保存请求参数
    /// </summary>
    public bool IsSaveRequestData { get; set; } = true;

    /// <summary>
    /// 是否保存响应参数
    /// </summary>
    public bool IsSaveResponseData { get; set; } = true;
}
