namespace NetMVP.Domain.Entities;

/// <summary>
/// 用户岗位关联实体
/// </summary>
public class SysUserPost
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 岗位ID
    /// </summary>
    public long PostId { get; set; }

    /// <summary>
    /// 用户
    /// </summary>
    public SysUser? User { get; set; }

    /// <summary>
    /// 岗位
    /// </summary>
    public SysPost? Post { get; set; }
}
