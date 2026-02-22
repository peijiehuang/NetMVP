using NetMVP.Application.Common.Models;

namespace NetMVP.Application.DTOs.Job;

/// <summary>
/// 任务日志查询DTO
/// </summary>
public class JobLogQueryDto : PageQueryDto
{
    /// <summary>
    /// 任务名称
    /// </summary>
    public string? JobName { get; set; }

    /// <summary>
    /// 任务组名
    /// </summary>
    public string? JobGroup { get; set; }

    /// <summary>
    /// 执行状态（0正常 1失败）
    /// </summary>
    public string? Status { get; set; }
}
