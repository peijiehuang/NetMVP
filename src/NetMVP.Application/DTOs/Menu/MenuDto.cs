namespace NetMVP.Application.DTOs.Menu;

/// <summary>
/// 菜单 DTO
/// </summary>
public class MenuDto
{
    public long MenuId { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public long ParentId { get; set; }
    public int OrderNum { get; set; }
    public string? Path { get; set; }
    public string? Component { get; set; }
    public string? Query { get; set; }
    public bool IsFrame { get; set; }
    public bool IsCache { get; set; }
    public string MenuType { get; set; } = string.Empty;
    public string Visible { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Perms { get; set; }
    public string? Icon { get; set; }
    public string? Remark { get; set; }
    public DateTime? CreateTime { get; set; }
    public List<MenuDto> Children { get; set; } = new();
}
