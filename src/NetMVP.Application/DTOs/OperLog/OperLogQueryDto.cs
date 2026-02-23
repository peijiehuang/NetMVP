using NetMVP.Application.Common.Models;
using NetMVP.Domain.Constants;

namespace NetMVP.Application.DTOs.OperLog;

/// <summary>
/// 操作日志查询 DTO
/// </summary>
public class OperLogQueryDto : PageQueryDto
{
    /// <summary>
    /// 模块标题
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 业务类型
    /// </summary>
    public string? BusinessType { get; set; }

    /// <summary>
    /// 操作人员
    /// </summary>
    public string? OperName { get; set; }

    /// <summary>
    /// 操作状态
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? BeginTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}
