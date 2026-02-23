using MiniExcelLibs.Attributes;

namespace NetMVP.Application.DTOs.Post;

/// <summary>
/// 岗位导出 DTO（带中文列头）
/// </summary>
public class ExportPostDto
{
    [ExcelColumnName("岗位编号")]
    public long PostId { get; set; }

    [ExcelColumnName("岗位编码")]
    public string PostCode { get; set; } = string.Empty;

    [ExcelColumnName("岗位名称")]
    public string PostName { get; set; } = string.Empty;

    [ExcelColumnName("显示顺序")]
    public int PostSort { get; set; }

    [ExcelColumnName("状态")]
    public string Status { get; set; } = string.Empty;

    [ExcelColumnName("备注")]
    public string? Remark { get; set; }

    [ExcelColumnName("创建时间")]
    public DateTime? CreateTime { get; set; }
}
