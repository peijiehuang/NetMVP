using MiniExcelLibs.Attributes;

namespace NetMVP.Application.DTOs.User;

/// <summary>
/// 用户导出 DTO（带中文列头）
/// </summary>
public class ExportUserDto
{
    [ExcelColumnName("用户编号")]
    public long UserId { get; set; }

    [ExcelColumnName("登录名称")]
    public string UserName { get; set; } = string.Empty;

    [ExcelColumnName("用户昵称")]
    public string NickName { get; set; } = string.Empty;

    [ExcelColumnName("用户邮箱")]
    public string Email { get; set; } = string.Empty;

    [ExcelColumnName("手机号码")]
    public string Phonenumber { get; set; } = string.Empty;

    [ExcelColumnName("用户性别")]
    public string Sex { get; set; } = string.Empty;

    [ExcelColumnName("帐号状态")]
    public string Status { get; set; } = string.Empty;

    [ExcelColumnName("部门名称")]
    public string? DeptName { get; set; }

    [ExcelColumnName("创建时间")]
    public DateTime? CreateTime { get; set; }
}
