namespace NetMVP.Application.DTOs.Menu;

/// <summary>
/// 菜单查询 DTO
/// </summary>
public class MenuQueryDto
{
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string? MenuName { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }
}
