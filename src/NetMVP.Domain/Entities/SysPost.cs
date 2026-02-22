using NetMVP.Domain.Common;
using NetMVP.Domain.Enums;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 岗位实体
/// </summary>
public class SysPost : BaseEntity
{
    /// <summary>
    /// 岗位ID
    /// </summary>
    public long PostId { get; set; }

    /// <summary>
    /// 岗位编码
    /// </summary>
    public string PostCode { get; set; } = string.Empty;

    /// <summary>
    /// 岗位名称
    /// </summary>
    public string PostName { get; set; } = string.Empty;

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int PostSort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public UserStatus Status { get; set; } = UserStatus.Normal;
}
