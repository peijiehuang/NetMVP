namespace NetMVP.Application.DTOs.Role;

/// <summary>
/// 角色 DTO
/// </summary>
public class RoleDto
{
    public long RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoleKey { get; set; } = string.Empty;
    public int RoleSort { get; set; }
    public string DataScope { get; set; } = string.Empty;
    public bool MenuCheckStrictly { get; set; }
    public bool DeptCheckStrictly { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public DateTime? CreateTime { get; set; }
    public List<long> MenuIds { get; set; } = new();
    public List<long> DeptIds { get; set; } = new();
    
    /// <summary>
    /// 标记该角色是否已分配给用户（用于用户授权角色页面）
    /// </summary>
    public bool Flag { get; set; }
}
