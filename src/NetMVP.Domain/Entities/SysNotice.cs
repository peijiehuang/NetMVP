using NetMVP.Domain.Common;
using NetMVP.Domain.Constants;

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
    /// 公告类型（1通知 2公告）
    /// </summary>
    public string NoticeType { get; set; } = NoticeConstants.NOTICE_TYPE_NOTICE;

    /// <summary>
    /// 公告内容
    /// </summary>
    public byte[]? NoticeContent { get; set; }

    /// <summary>
    /// 公告状态（0正常 1关闭）
    /// </summary>
    public string Status { get; set; } = NoticeConstants.NOTICE_STATUS_NORMAL;
}
