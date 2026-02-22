namespace NetMVP.Application.DTOs.Dict;

/// <summary>
/// 字典类型 DTO
/// </summary>
public class DictTypeDto
{
    public long DictId { get; set; }
    public string DictName { get; set; } = string.Empty;
    public string DictType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? CreateTime { get; set; }
    public string? Remark { get; set; }
}
