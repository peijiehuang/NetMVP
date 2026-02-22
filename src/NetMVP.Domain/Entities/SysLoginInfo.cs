using NetMVP.Domain.Enums;
using NetMVP.Domain.ValueObjects;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 登录日志实体
/// </summary>
public class SysLoginInfo
{
    /// <summary>
    /// 访问ID
    /// </summary>
    public long InfoId { get; set; }

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
    /// 登录状态
    /// </summary>
    public CommonStatus Status { get; set; } = CommonStatus.Success;

    /// <summary>
    /// 提示消息
    /// </summary>
    public string? Msg { get; set; }

    /// <summary>
    /// 访问时间
    /// </summary>
    public DateTime? LoginTime { get; set; }

    /// <summary>
    /// 登录IP（值对象）
    /// </summary>
    public IpAddress? IpAddr => IpAddress.Create(IpAddrValue);
}
