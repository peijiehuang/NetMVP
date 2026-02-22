using NetMVP.Domain.Common;
using NetMVP.Domain.Enums;

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
    /// 状态
    /// </summary>
    public UserStatus Status { get; set; } = UserStatus.Normal;
}
