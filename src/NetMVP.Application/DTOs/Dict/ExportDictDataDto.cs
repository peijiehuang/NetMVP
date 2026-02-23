using MiniExcelLibs.Attributes;

namespace NetMVP.Application.DTOs.Dict;

/// <summary>
/// 字典数据导出 DTO（带中文列头）
/// </summary>
public class ExportDictDataDto
{
    [ExcelColumnName("字典编码")]
    public long DictCode { get; set; }

    [ExcelColumnName("字典排序")]
    public int DictSort { get; set; }

    [ExcelColumnName("字典标签")]
    public string DictLabel { get; set; } = string.Empty;

    [ExcelColumnName("字典键值")]
    public string DictValue { get; set; } = string.Empty;

    [ExcelColumnName("字典类型")]
    public string DictType { get; set; } = string.Empty;

    [ExcelColumnName("样式属性")]
    public string? CssClass { get; set; }

    [ExcelColumnName("表格回显样式")]
    public string? ListClass { get; set; }

    [ExcelColumnName("是否默认")]
    public string IsDefault { get; set; } = string.Empty;

    [ExcelColumnName("状态")]
    public string Status { get; set; } = string.Empty;

    [ExcelColumnName("创建时间")]
    public DateTime? CreateTime { get; set; }

    [ExcelColumnName("备注")]
    public string? Remark { get; set; }
}
