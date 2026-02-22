namespace NetMVP.Application.DTOs.Dept;

/// <summary>
/// 部门 DTO
/// </summary>
public class DeptDto
{
    public long DeptId { get; set; }
    public long ParentId { get; set; }
    public string? Ancestors { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public int OrderNum { get; set; }
    public string? Leader { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CreateTime { get; set; }
    public List<DeptDto> Children { get; set; } = new();
}
