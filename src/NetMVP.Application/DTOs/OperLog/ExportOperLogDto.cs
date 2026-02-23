using MiniExcelLibs.Attributes;

namespace NetMVP.Application.DTOs.OperLog;

/// <summary>
/// 操作日志导出 DTO（带中文列头）
/// </summary>
public class ExportOperLogDto
{
    [ExcelColumnName("日志编号")]
    public long OperId { get; set; }

    [ExcelColumnName("系统模块")]
    public string? Title { get; set; }

    [ExcelColumnName("操作类型")]
    public string BusinessType { get; set; } = string.Empty;

    [ExcelColumnName("请求方式")]
    public string? RequestMethod { get; set; }

    [ExcelColumnName("操作人员")]
    public string? OperName { get; set; }

    [ExcelColumnName("部门名称")]
    public string? DeptName { get; set; }

    [ExcelColumnName("请求地址")]
    public string? OperUrl { get; set; }

    [ExcelColumnName("操作地址")]
    public string? OperIp { get; set; }

    [ExcelColumnName("操作地点")]
    public string? OperLocation { get; set; }

    [ExcelColumnName("操作状态")]
    public string Status { get; set; } = string.Empty;

    [ExcelColumnName("错误消息")]
    public string? ErrorMsg { get; set; }

    [ExcelColumnName("操作时间")]
    public DateTime OperTime { get; set; }

    [ExcelColumnName("消耗时间(毫秒)")]
    public long CostTime { get; set; }
}
