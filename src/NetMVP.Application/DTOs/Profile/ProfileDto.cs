namespace NetMVP.Application.DTOs.Profile;

/// <summary>
/// 个人信息DTO
/// </summary>
public class ProfileDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 昵称
    /// </summary>
    public string NickName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phonenumber { get; set; }

    /// <summary>
    /// 性别（0男 1女 2未知）
    /// </summary>
    public string? Sex { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public long? DeptId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string? DeptName { get; set; }

    /// <summary>
    /// 岗位
    /// </summary>
    public List<string> PostIds { get; set; } = new();

    /// <summary>
    /// 角色
    /// </summary>
    public List<string> RoleIds { get; set; } = new();
}
