namespace NetMVP.Application.DTOs.Dict;

/// <summary>
/// 字典数据 DTO
/// </summary>
public class DictDataDto
{
    public long DictCode { get; set; }
    public int DictSort { get; set; }
    public string DictLabel { get; set; } = string.Empty;
    public string DictValue { get; set; } = string.Empty;
    public string DictType { get; set; } = string.Empty;
    public string? CssClass { get; set; }
    public string? ListClass { get; set; }
    public string IsDefault { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? CreateTime { get; set; }
    public string? Remark { get; set; }
}
