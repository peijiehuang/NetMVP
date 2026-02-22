using NetMVP.Domain.Enums;
using NetMVP.Domain.ValueObjects;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 操作日志实体
/// </summary>
public class SysOperLog
{
    /// <summary>
    /// 日志ID
    /// </summary>
    public long OperId { get; set; }

    /// <summary>
    /// 模块标题
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 业务类型
    /// </summary>
    public BusinessType BusinessType { get; set; } = BusinessType.Other;

    /// <summary>
    /// 方法名称
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// 请求方式
    /// </summary>
    public string? RequestMethod { get; set; }

    /// <summary>
    /// 操作类别
    /// </summary>
    public OperatorType OperatorType { get; set; } = OperatorType.Other;

    /// <summary>
    /// 操作人员
    /// </summary>
    public string? OperName { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string? DeptName { get; set; }

    /// <summary>
    /// 请求URL
    /// </summary>
    public string? OperUrl { get; set; }

    /// <summary>
    /// 主机地址
    /// </summary>
    public string? OperIpValue { get; set; }

    /// <summary>
    /// 操作地点
    /// </summary>
    public string? OperLocation { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    public string? OperParam { get; set; }

    /// <summary>
    /// 返回参数
    /// </summary>
    public string? JsonResult { get; set; }

    /// <summary>
    /// 操作状态
    /// </summary>
    public CommonStatus Status { get; set; } = CommonStatus.Success;

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMsg { get; set; }

    /// <summary>
    /// 操作时间
    /// </summary>
    public DateTime? OperTime { get; set; }

    /// <summary>
    /// 消耗时间（毫秒）
    /// </summary>
    public long CostTime { get; set; }

    /// <summary>
    /// 主机地址（值对象）
    /// </summary>
    public IpAddress? OperIp => string.IsNullOrWhiteSpace(OperIpValue) ? null : IpAddress.Create(OperIpValue);
}
