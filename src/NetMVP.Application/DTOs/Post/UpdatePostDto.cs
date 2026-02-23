using NetMVP.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.Post;

/// <summary>
/// 更新岗位 DTO
/// </summary>
public class UpdatePostDto
{
    /// <summary>
    /// 岗位ID
    /// </summary>
    [Required(ErrorMessage = "岗位ID不能为空")]
    public long PostId { get; set; }

    /// <summary>
    /// 岗位编码
    /// </summary>
    [Required(ErrorMessage = "岗位编码不能为空")]
    [StringLength(64, ErrorMessage = "岗位编码长度不能超过64个字符")]
    public string PostCode { get; set; } = string.Empty;

    /// <summary>
    /// 岗位名称
    /// </summary>
    [Required(ErrorMessage = "岗位名称不能为空")]
    [StringLength(50, ErrorMessage = "岗位名称长度不能超过50个字符")]
    public string PostName { get; set; } = string.Empty;

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int PostSort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; } = UserConstants.NORMAL;

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}
