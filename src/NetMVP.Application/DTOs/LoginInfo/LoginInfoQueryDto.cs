using NetMVP.Application.Common.Models;
using NetMVP.Domain.Enums;

namespace NetMVP.Application.DTOs.LoginInfo;

/// <summary>
/// 登录日志查询DTO
/// </summary>
public class LoginInfoQueryDto : PageQueryDto
{
    /// <summary>
    /// 用户账号
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 登录IP
    /// </summary>
    public string? IpAddr { get; set; }

    /// <summary>
    /// 登录状态
    /// </summary>
    public CommonStatus? Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? BeginTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}
