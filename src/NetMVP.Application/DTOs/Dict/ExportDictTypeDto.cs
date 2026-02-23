using MiniExcelLibs.Attributes;

namespace NetMVP.Application.DTOs.Dict;

/// <summary>
/// 字典类型导出 DTO（带中文列头）
/// </summary>
public class ExportDictTypeDto
{
    [ExcelColumnName("字典编号")]
    public long DictId { get; set; }

    [ExcelColumnName("字典名称")]
    public string DictName { get; set; } = string.Empty;

    [ExcelColumnName("字典类型")]
    public string DictType { get; set; } = string.Empty;

    [ExcelColumnName("状态")]
    public string Status { get; set; } = string.Empty;

    [ExcelColumnName("创建时间")]
    public DateTime? CreateTime { get; set; }

    [ExcelColumnName("备注")]
    public string? Remark { get; set; }
}
