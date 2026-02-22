using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Job;

namespace NetMVP.Application.Services;

/// <summary>
/// 定时任务日志服务接口
/// </summary>
public interface ISysJobLogService
{
    /// <summary>
    /// 获取任务日志列表
    /// </summary>
    Task<PagedResult<JobLogDto>> GetJobLogListAsync(JobLogQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取任务日志详情
    /// </summary>
    Task<JobLogDto?> GetJobLogByIdAsync(long jobLogId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除任务日志
    /// </summary>
    Task DeleteJobLogAsync(long jobLogId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清空任务日志
    /// </summary>
    Task CleanJobLogAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 导出任务日志
    /// </summary>
    Task<byte[]> ExportJobLogsAsync(JobLogQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加任务日志
    /// </summary>
    Task AddJobLogAsync(string jobName, string jobGroup, string invokeTarget, string jobMessage, string status, string? exceptionInfo = null, CancellationToken cancellationToken = default);
}
