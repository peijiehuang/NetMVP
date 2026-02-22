namespace NetMVP.Application.DTOs.Dept;

/// <summary>
/// 部门树 DTO
/// </summary>
public class DeptTreeDto
{
    public long Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public bool Disabled { get; set; }
    public List<DeptTreeDto> Children { get; set; } = new();
}
