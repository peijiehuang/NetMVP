using NetMVP.Domain.Constants;

namespace NetMVP.Application.DTOs.LoginInfo;

/// <summary>
/// 创建登录日志DTO
/// </summary>
public class CreateLoginInfoDto
{
    /// <summary>
    /// 用户账号
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 登录IP
    /// </summary>
    public string IpAddr { get; set; } = string.Empty;

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
    /// 登录状态
    /// </summary>
    public string Status { get; set; } = CommonConstants.SUCCESS;

    /// <summary>
    /// 提示消息
    /// </summary>
    public string? Msg { get; set; }
}
