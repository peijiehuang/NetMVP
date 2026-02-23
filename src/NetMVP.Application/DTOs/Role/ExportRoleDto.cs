using MiniExcelLibs.Attributes;

namespace NetMVP.Application.DTOs.Role;

/// <summary>
/// 角色导出 DTO（带中文列头）
/// </summary>
public class ExportRoleDto
{
    [ExcelColumnName("角色编号")]
    public long RoleId { get; set; }

    [ExcelColumnName("角色名称")]
    public string RoleName { get; set; } = string.Empty;

    [ExcelColumnName("权限字符")]
    public string RoleKey { get; set; } = string.Empty;

    [ExcelColumnName("显示顺序")]
    public int RoleSort { get; set; }

    [ExcelColumnName("数据范围")]
    public string DataScope { get; set; } = string.Empty;

    [ExcelColumnName("角色状态")]
    public string Status { get; set; } = string.Empty;

    [ExcelColumnName("创建时间")]
    public DateTime? CreateTime { get; set; }

    [ExcelColumnName("备注")]
    public string? Remark { get; set; }
}
