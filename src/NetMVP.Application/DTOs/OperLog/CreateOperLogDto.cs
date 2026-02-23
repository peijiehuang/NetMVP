using NetMVP.Domain.Constants;

namespace NetMVP.Application.DTOs.OperLog;

/// <summary>
/// 创建操作日志 DTO
/// </summary>
public class CreateOperLogDto
{
    /// <summary>
    /// 模块标题
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 业务类型
    /// </summary>
    public string BusinessType { get; set; } = OperLogConstants.BUSINESS_TYPE_OTHER;

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
    public string OperatorType { get; set; } = OperLogConstants.OPERATOR_TYPE_OTHER;

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
    public string? OperIp { get; set; }

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
    public string Status { get; set; } = CommonConstants.SUCCESS;

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMsg { get; set; }

    /// <summary>
    /// 消耗时间（毫秒）
    /// </summary>
    public long CostTime { get; set; }
}
