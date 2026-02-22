namespace NetMVP.Application.DTOs.Notice;

/// <summary>
/// 通知公告 DTO
/// </summary>
public class NoticeDto
{
    /// <summary>
    /// 公告ID
    /// </summary>
    public int NoticeId { get; set; }

    /// <summary>
    /// 公告标题
    /// </summary>
    public string NoticeTitle { get; set; } = string.Empty;

    /// <summary>
    /// 公告类型
    /// </summary>
    public string NoticeType { get; set; } = string.Empty;

    /// <summary>
    /// 公告内容
    /// </summary>
    public string? NoticeContent { get; set; }

    /// <summary>
    /// 公告状态
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 创建者
    /// </summary>
    public string? CreateBy { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}
