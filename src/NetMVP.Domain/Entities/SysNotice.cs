using NetMVP.Domain.Common;
using NetMVP.Domain.Enums;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 通知公告实体
/// </summary>
public class SysNotice : BaseEntity
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
    public NoticeType NoticeType { get; set; } = NoticeType.Notice;

    /// <summary>
    /// 公告内容
    /// </summary>
    public byte[]? NoticeContent { get; set; }

    /// <summary>
    /// 公告状态
    /// </summary>
    public NoticeStatus Status { get; set; } = NoticeStatus.Normal;
}
