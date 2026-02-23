using MiniExcelLibs.Attributes;

namespace NetMVP.Application.DTOs.Config;

/// <summary>
/// 参数配置导出 DTO（带中文列头）
/// </summary>
public class ExportConfigDto
{
    [ExcelColumnName("参数编号")]
    public int ConfigId { get; set; }

    [ExcelColumnName("参数名称")]
    public string ConfigName { get; set; } = string.Empty;

    [ExcelColumnName("参数键名")]
    public string ConfigKey { get; set; } = string.Empty;

    [ExcelColumnName("参数键值")]
    public string ConfigValue { get; set; } = string.Empty;

    [ExcelColumnName("系统内置")]
    public string ConfigType { get; set; } = string.Empty;

    [ExcelColumnName("备注")]
    public string? Remark { get; set; }

    [ExcelColumnName("创建时间")]
    public DateTime CreateTime { get; set; }
}
