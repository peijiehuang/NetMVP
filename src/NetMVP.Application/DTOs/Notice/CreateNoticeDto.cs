using NetMVP.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.Notice;

/// <summary>
/// 创建通知公告 DTO
/// </summary>
public class CreateNoticeDto
{
    /// <summary>
    /// 公告标题
    /// </summary>
    [Required(ErrorMessage = "公告标题不能为空")]
    [StringLength(50, ErrorMessage = "公告标题长度不能超过50个字符")]
    public string NoticeTitle { get; set; } = string.Empty;

    /// <summary>
    /// 公告类型
    /// </summary>
    [Required(ErrorMessage = "公告类型不能为空")]
    public string NoticeType { get; set; } = NoticeConstants.NOTICE_TYPE_NOTICE;

    /// <summary>
    /// 公告内容
    /// </summary>
    public string? NoticeContent { get; set; }

    /// <summary>
    /// 公告状态
    /// </summary>
    public string Status { get; set; } = NoticeConstants.NOTICE_STATUS_NORMAL;

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}
