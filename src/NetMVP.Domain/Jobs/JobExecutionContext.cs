using System.Text;

namespace NetMVP.Domain.Jobs;

/// <summary>
/// 任务执行上下文
/// 用于在任务执行过程中收集日志信息
/// </summary>
public class JobExecutionContext
{
    private readonly StringBuilder _logBuilder = new();
    private readonly object _lock = new();

    /// <summary>
    /// 任务名称
    /// </summary>
    public string JobName { get; set; } = string.Empty;

    /// <summary>
    /// 任务组
    /// </summary>
    public string JobGroup { get; set; } = string.Empty;

    /// <summary>
    /// 调用目标
    /// </summary>
    public string InvokeTarget { get; set; } = string.Empty;

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 添加日志信息
    /// </summary>
    public void AppendLog(string message)
    {
        lock (_lock)
        {
            if (_logBuilder.Length > 0)
            {
                _logBuilder.AppendLine();
            }
            _logBuilder.Append($"[{DateTime.Now:HH:mm:ss}] {message}");
        }
    }

    /// <summary>
    /// 添加格式化日志信息
    /// </summary>
    public void AppendLog(string format, params object[] args)
    {
        AppendLog(string.Format(format, args));
    }

    /// <summary>
    /// 获取所有日志
    /// </summary>
    public string GetLogs()
    {
        lock (_lock)
        {
            return _logBuilder.ToString();
        }
    }

    /// <summary>
    /// 清空日志
    /// </summary>
    public void ClearLogs()
    {
        lock (_lock)
        {
            _logBuilder.Clear();
        }
    }
}
