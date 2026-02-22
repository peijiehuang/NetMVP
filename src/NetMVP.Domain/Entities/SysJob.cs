using NetMVP.Domain.Common;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 定时任务实体
/// </summary>
public class SysJob : BaseEntity
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
    public string JobGroup { get; set; } = "DEFAULT";

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
    public string MisfirePolicy { get; set; } = "3";

    /// <summary>
    /// 是否并发执行（0允许 1禁止）
    /// </summary>
    public string Concurrent { get; set; } = "1";

    /// <summary>
    /// 状态（0正常 1暂停）
    /// </summary>
    public string Status { get; set; } = "0";

    /// <summary>
    /// 备注
    /// </summary>
    public new string? Remark { get; set; }
}
