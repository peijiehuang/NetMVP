namespace NetMVP.Domain.Enums;

/// <summary>
/// 计划执行错误策略
/// </summary>
public enum MisfirePolicy
{
    /// <summary>
    /// 立即执行
    /// </summary>
    DoNothing = 1,

    /// <summary>
    /// 执行一次
    /// </summary>
    FireOnce = 2,

    /// <summary>
    /// 放弃执行
    /// </summary>
    Ignore = 3
}
