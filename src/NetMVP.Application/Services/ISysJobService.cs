using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Job;

namespace NetMVP.Application.Services;

/// <summary>
/// 定时任务服务接口
/// </summary>
public interface ISysJobService
{
    /// <summary>
    /// 获取任务列表
    /// </summary>
    Task<PagedResult<JobDto>> GetJobListAsync(JobQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取任务详情
    /// </summary>
    Task<JobDto?> GetJobByIdAsync(long jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建任务
    /// </summary>
    Task<long> CreateJobAsync(CreateJobDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新任务
    /// </summary>
    Task UpdateJobAsync(UpdateJobDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除任务
    /// </summary>
    Task DeleteJobAsync(long jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改任务状态
    /// </summary>
    Task ChangeJobStatusAsync(long jobId, string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// 立即执行任务
    /// </summary>
    Task RunJobAsync(long jobId, string jobGroup, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导出任务
    /// </summary>
    Task<byte[]> ExportJobsAsync(JobQueryDto query, CancellationToken cancellationToken = default);
}
