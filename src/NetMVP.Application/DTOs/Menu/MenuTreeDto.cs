namespace NetMVP.Application.DTOs.Menu;

/// <summary>
/// 菜单树 DTO
/// </summary>
public class MenuTreeDto
{
    public long Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public List<MenuTreeDto> Children { get; set; } = new();
}
