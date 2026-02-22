namespace NetMVP.Infrastructure.Configuration;

/// <summary>
/// Quartz配置选项
/// </summary>
public class QuartzOptions
{
    /// <summary>
    /// 调度器名称
    /// </summary>
    public string SchedulerName { get; set; } = "NetMVPScheduler";

    /// <summary>
    /// 调度器实例ID
    /// </summary>
    public string InstanceId { get; set; } = "AUTO";

    /// <summary>
    /// 是否启用集群
    /// </summary>
    public bool Clustered { get; set; } = false;

    /// <summary>
    /// 线程池大小
    /// </summary>
    public int ThreadCount { get; set; } = 10;

    /// <summary>
    /// 是否使用持久化存储
    /// </summary>
    public bool UsePersistentStore { get; set; } = false;

    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// 表前缀
    /// </summary>
    public string TablePrefix { get; set; } = "QRTZ_";
}
