namespace NetMVP.Application.DTOs.User;

/// <summary>
/// 用户 DTO
/// </summary>
public class UserDto
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phonenumber { get; set; } = string.Empty;
    public string Sex { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public long? DeptId { get; set; }
    
    /// <summary>
    /// 部门对象（前端期望 dept.deptName 格式）
    /// </summary>
    public DeptInfo? Dept { get; set; }
    
    public string? Avatar { get; set; }
    public DateTime? CreateTime { get; set; }
    public string? Remark { get; set; }
}

/// <summary>
/// 部门信息（用于用户列表显示）
/// </summary>
public class DeptInfo
{
    public string? DeptName { get; set; }
}

