namespace NetMVP.Domain.Constants;

/// <summary>
/// 任务调度通用常量
/// </summary>
public static class ScheduleConstants
{
    /// <summary>
    /// 任务状态 - 正常
    /// </summary>
    public const string STATUS_NORMAL = "0";

    /// <summary>
    /// 任务状态 - 暂停
    /// </summary>
    public const string STATUS_PAUSE = "1";

    /// <summary>
    /// 任务分组 - 默认
    /// </summary>
    public const string JOB_GROUP_DEFAULT = "DEFAULT";

    /// <summary>
    /// 任务分组 - 系统
    /// </summary>
    public const string JOB_GROUP_SYSTEM = "SYSTEM";

    /// <summary>
    /// 计划执行错误策略 - 立即执行
    /// </summary>
    public const string MISFIRE_DEFAULT = "0";

    /// <summary>
    /// 计划执行错误策略 - 执行一次
    /// </summary>
    public const string MISFIRE_IGNORE_MISFIRES = "1";

    /// <summary>
    /// 计划执行错误策略 - 放弃执行
    /// </summary>
    public const string MISFIRE_FIRE_AND_PROCEED = "2";

    /// <summary>
    /// 计划执行错误策略 - 立即触发执行
    /// </summary>
    public const string MISFIRE_DO_NOTHING = "3";
}
