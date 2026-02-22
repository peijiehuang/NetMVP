using NetMVP.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.Dict;

/// <summary>
/// 创建字典类型 DTO
/// </summary>
public class CreateDictTypeDto
{
    /// <summary>
    /// 字典名称
    /// </summary>
    [Required(ErrorMessage = "字典名称不能为空")]
    [StringLength(100, ErrorMessage = "字典名称长度不能超过100个字符")]
    public string DictName { get; set; } = string.Empty;

    /// <summary>
    /// 字典类型
    /// </summary>
    [Required(ErrorMessage = "字典类型不能为空")]
    [StringLength(100, ErrorMessage = "字典类型长度不能超过100个字符")]
    public string DictType { get; set; } = string.Empty;

    /// <summary>
    /// 状态（0正常 1停用）
    /// </summary>
    [Required(ErrorMessage = "状态不能为空")]
    public UserStatus Status { get; set; } = UserStatus.Normal;

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}
