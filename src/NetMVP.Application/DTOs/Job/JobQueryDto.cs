using NetMVP.Application.Common.Models;

namespace NetMVP.Application.DTOs.Job;

/// <summary>
/// 任务查询DTO
/// </summary>
public class JobQueryDto : PageQueryDto
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
    /// 状态（0正常 1暂停）
    /// </summary>
    public string? Status { get; set; }
}
