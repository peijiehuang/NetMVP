using NetMVP.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.Notice;

/// <summary>
/// 更新通知公告 DTO
/// </summary>
public class UpdateNoticeDto
{
    /// <summary>
    /// 公告ID
    /// </summary>
    [Required(ErrorMessage = "公告ID不能为空")]
    public int NoticeId { get; set; }

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
    public NoticeType NoticeType { get; set; } = NoticeType.Notice;

    /// <summary>
    /// 公告内容
    /// </summary>
    public string? NoticeContent { get; set; }

    /// <summary>
    /// 公告状态
    /// </summary>
    public NoticeStatus Status { get; set; } = NoticeStatus.Normal;

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}
