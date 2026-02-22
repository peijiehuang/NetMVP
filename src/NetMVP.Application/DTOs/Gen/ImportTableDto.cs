namespace NetMVP.Application.DTOs.Gen;

/// <summary>
/// 导入表DTO
/// </summary>
public class ImportTableDto
{
    /// <summary>
    /// 表名数组
    /// </summary>
    public string[] Tables { get; set; } = Array.Empty<string>();
}
