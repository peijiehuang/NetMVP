using NetMVP.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.Config;

/// <summary>
/// 创建参数配置 DTO
/// </summary>
public class CreateConfigDto
{
    /// <summary>
    /// 参数名称
    /// </summary>
    [Required(ErrorMessage = "参数名称不能为空")]
    [StringLength(100, ErrorMessage = "参数名称长度不能超过100个字符")]
    public string ConfigName { get; set; } = string.Empty;

    /// <summary>
    /// 参数键名
    /// </summary>
    [Required(ErrorMessage = "参数键名不能为空")]
    [StringLength(100, ErrorMessage = "参数键名长度不能超过100个字符")]
    public string ConfigKey { get; set; } = string.Empty;

    /// <summary>
    /// 参数键值
    /// </summary>
    [Required(ErrorMessage = "参数键值不能为空")]
    [StringLength(500, ErrorMessage = "参数键值长度不能超过500个字符")]
    public string ConfigValue { get; set; } = string.Empty;

    /// <summary>
    /// 系统内置（Y是 N否）
    /// </summary>
    [Required(ErrorMessage = "系统内置标识不能为空")]
    [RegularExpression("^[YN]$", ErrorMessage = "系统内置标识格式不正确")]
    public string ConfigType { get; set; } = CommonConstants.NO;

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}
