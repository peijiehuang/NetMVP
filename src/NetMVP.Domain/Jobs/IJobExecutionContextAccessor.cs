namespace NetMVP.Domain.Jobs;

/// <summary>
/// 任务执行上下文访问器
/// 提供静态访问当前任务上下文的能力
/// </summary>
public static class JobContext
{
    private static readonly AsyncLocal<JobExecutionContext?> _current = new();

    /// <summary>
    /// 获取或设置当前任务执行上下文
    /// </summary>
    public static JobExecutionContext? Current
    {
        get => _current.Value;
        set => _current.Value = value;
    }

    /// <summary>
    /// 记录日志到当前任务上下文
    /// </summary>
    public static void Log(string message)
    {
        Current?.AppendLog(message);
    }

    /// <summary>
    /// 记录格式化日志到当前任务上下文
    /// </summary>
    public static void Log(string format, params object[] args)
    {
        Current?.AppendLog(format, args);
    }
}
