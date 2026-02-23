using NetMVP.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.Dict;

/// <summary>
/// 创建字典数据 DTO
/// </summary>
public class CreateDictDataDto
{
    /// <summary>
    /// 字典排序
    /// </summary>
    public int DictSort { get; set; }

    /// <summary>
    /// 字典标签
    /// </summary>
    [Required(ErrorMessage = "字典标签不能为空")]
    [StringLength(100, ErrorMessage = "字典标签长度不能超过100个字符")]
    public string DictLabel { get; set; } = string.Empty;

    /// <summary>
    /// 字典键值
    /// </summary>
    [Required(ErrorMessage = "字典键值不能为空")]
    [StringLength(100, ErrorMessage = "字典键值长度不能超过100个字符")]
    public string DictValue { get; set; } = string.Empty;

    /// <summary>
    /// 字典类型
    /// </summary>
    [Required(ErrorMessage = "字典类型不能为空")]
    [StringLength(100, ErrorMessage = "字典类型长度不能超过100个字符")]
    public string DictType { get; set; } = string.Empty;

    /// <summary>
    /// 样式属性
    /// </summary>
    [StringLength(100, ErrorMessage = "样式属性长度不能超过100个字符")]
    public string? CssClass { get; set; }

    /// <summary>
    /// 表格回显样式
    /// </summary>
    [StringLength(100, ErrorMessage = "表格回显样式长度不能超过100个字符")]
    public string? ListClass { get; set; }

    /// <summary>
    /// 是否默认（Y是 N否）
    /// </summary>
    [Required(ErrorMessage = "是否默认不能为空")]
    public string IsDefault { get; set; } = CommonConstants.NO;

    /// <summary>
    /// 状态（0正常 1停用）
    /// </summary>
    [Required(ErrorMessage = "状态不能为空")]
    public string Status { get; set; } = UserConstants.NORMAL;

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}
