using NetMVP.Domain.Common;
using NetMVP.Domain.Constants;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 字典类型实体
/// </summary>
public class SysDictType : BaseEntity
{
    /// <summary>
    /// 字典ID
    /// </summary>
    public long DictId { get; set; }

    /// <summary>
    /// 字典名称
    /// </summary>
    public string DictName { get; set; } = string.Empty;

    /// <summary>
    /// 字典类型
    /// </summary>
    public string DictType { get; set; } = string.Empty;

    /// <summary>
    /// 状态（0正常 1停用）
    /// </summary>
    public string Status { get; set; } = UserConstants.NORMAL;
}
