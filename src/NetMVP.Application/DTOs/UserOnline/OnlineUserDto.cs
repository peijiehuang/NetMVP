namespace NetMVP.Application.DTOs.UserOnline;

/// <summary>
/// 在线用户 DTO
/// </summary>
public class OnlineUserDto
{
    /// <summary>
    /// Token ID (JTI)
    /// </summary>
    public string TokenId { get; set; } = string.Empty;

    /// <summary>
    /// 用户 ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 部门名称
    /// </summary>
    public string? DeptName { get; set; }

    /// <summary>
    /// 登录 IP 地址
    /// </summary>
    public string Ipaddr { get; set; } = string.Empty;

    /// <summary>
    /// 登录地点
    /// </summary>
    public string? LoginLocation { get; set; }

    /// <summary>
    /// 浏览器类型
    /// </summary>
    public string? Browser { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    public string? Os { get; set; }

    /// <summary>
    /// 登录时间（时间戳，毫秒）
    /// </summary>
    public long LoginTime { get; set; }
}
