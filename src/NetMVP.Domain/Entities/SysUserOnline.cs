using NetMVP.Domain.ValueObjects;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 在线用户实体
/// </summary>
public class SysUserOnline
{
    /// <summary>
    /// 用户会话ID
    /// </summary>
    public string TokenId { get; set; } = string.Empty;

    /// <summary>
    /// 用户账号
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 登录IP
    /// </summary>
    public string IpAddrValue { get; set; } = string.Empty;

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
    /// 登录时间
    /// </summary>
    public DateTime LoginTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 登录IP（值对象）
    /// </summary>
    public IpAddress? IpAddr => IpAddress.Create(IpAddrValue);
}
