namespace NetMVP.Application.DTOs.Job;

/// <summary>
/// 任务DTO
/// </summary>
public class JobDto
{
    /// <summary>
    /// 任务ID
    /// </summary>
    public long JobId { get; set; }

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
    /// cron执行表达式
    /// </summary>
    public string CronExpression { get; set; } = string.Empty;

    /// <summary>
    /// 计划执行错误策略（1立即执行 2执行一次 3放弃执行）
    /// </summary>
    public string MisfirePolicy { get; set; } = string.Empty;

    /// <summary>
    /// 是否并发执行（0允许 1禁止）
    /// </summary>
    public string Concurrent { get; set; } = string.Empty;

    /// <summary>
    /// 状态（0正常 1暂停）
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 下次执行时间
    /// </summary>
    public DateTimeOffset? NextValidTime { get; set; }
}
