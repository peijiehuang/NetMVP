namespace NetMVP.Application.DTOs.Dept;

/// <summary>
/// 部门查询 DTO
/// </summary>
public class DeptQueryDto
{
    /// <summary>
    /// 部门名称
    /// </summary>
    public string? DeptName { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }
}
