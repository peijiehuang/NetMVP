namespace NetMVP.Application.DTOs.Job;

/// <summary>
/// 任务日志DTO
/// </summary>
public class JobLogDto
{
    /// <summary>
    /// 任务日志ID
    /// </summary>
    public long JobLogId { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    public string JobName { get; set; } = string.Empty;

    /// <summary>
    /// 任务组名
    /// </summary>
    public string JobGroup { get; set; } = string.Empty;

    /// <summary>
    /// 调用目标字符串
    /// </summary>
    public string InvokeTarget { get; set; } = string.Empty;

    /// <summary>
    /// 日志信息
    /// </summary>
    public string? JobMessage { get; set; }

    /// <summary>
    /// 执行状态（0正常 1失败）
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 异常信息
    /// </summary>
    public string? ExceptionInfo { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
