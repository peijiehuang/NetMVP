using NetMVP.Application.Common.Models;

namespace NetMVP.Application.DTOs.Notice;

/// <summary>
/// 通知公告查询 DTO
/// </summary>
public class NoticeQueryDto : PageQueryDto
{
    /// <summary>
    /// 公告标题
    /// </summary>
    public string? NoticeTitle { get; set; }

    /// <summary>
    /// 公告类型（1通知 2公告）
    /// </summary>
    public string? NoticeType { get; set; }

    /// <summary>
    /// 创建者
    /// </summary>
    public string? CreateBy { get; set; }
}
