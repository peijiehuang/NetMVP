using NetMVP.Domain.Common;
using NetMVP.Domain.Constants;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 字典数据实体
/// </summary>
public class SysDictData : BaseEntity
{
    /// <summary>
    /// 字典编码
    /// </summary>
    public long DictCode { get; set; }

    /// <summary>
    /// 字典排序
    /// </summary>
    public int DictSort { get; set; }

    /// <summary>
    /// 字典标签
    /// </summary>
    public string DictLabel { get; set; } = string.Empty;

    /// <summary>
    /// 字典键值
    /// </summary>
    public string DictValue { get; set; } = string.Empty;

    /// <summary>
    /// 字典类型
    /// </summary>
    public string DictType { get; set; } = string.Empty;

    /// <summary>
    /// 样式属性
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// 表格回显样式
    /// </summary>
    public string? ListClass { get; set; }

    /// <summary>
    /// 是否默认（Y是 N否）
    /// </summary>
    public string IsDefault { get; set; } = CommonConstants.NO;

    /// <summary>
    /// 状态（0正常 1停用）
    /// </summary>
    public string Status { get; set; } = UserConstants.NORMAL;
}
