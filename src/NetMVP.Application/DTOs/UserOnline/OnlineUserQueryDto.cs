using NetMVP.Application.Common.Models;

namespace NetMVP.Application.DTOs.UserOnline;

/// <summary>
/// 在线用户查询 DTO
/// </summary>
public class OnlineUserQueryDto : PageQueryDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 登录地址
    /// </summary>
    public string? Ipaddr { get; set; }
}
