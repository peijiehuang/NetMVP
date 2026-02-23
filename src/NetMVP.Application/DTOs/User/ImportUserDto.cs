using MiniExcelLibs.Attributes;

namespace NetMVP.Application.DTOs.User;

/// <summary>
/// 导入用户DTO
/// </summary>
public class ImportUserDto
{
    [ExcelColumnName("用户名")]
    public string UserName { get; set; } = string.Empty;
    
    [ExcelColumnName("用户昵称")]
    public string? NickName { get; set; }
    
    [ExcelColumnName("邮箱")]
    public string? Email { get; set; }
    
    [ExcelColumnName("手机号码")]
    public string? PhoneNumber { get; set; }
    
    [ExcelColumnName("性别")]
    public string? Gender { get; set; }
    
    [ExcelColumnName("状态")]
    public string? Status { get; set; }
    
    [ExcelColumnName("部门ID")]
    public long? DeptId { get; set; }
}
