using NetMVP.Domain.Jobs;

namespace NetMVP.Application.Jobs;

/// <summary>
/// 任务基类
/// 提供日志记录功能
/// </summary>
public abstract class JobTaskBase
{
    /// <summary>
    /// 记录日志到任务上下文
    /// </summary>
    protected void Log(string message)
    {
        JobContext.Log(message);
    }

    /// <summary>
    /// 记录格式化日志到任务上下文
    /// </summary>
    protected void Log(string format, params object[] args)
    {
        var message = string.Format(format, args);
        JobContext.Log(message);
    }
}
