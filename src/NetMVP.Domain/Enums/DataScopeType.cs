namespace NetMVP.Domain.Enums;

/// <summary>
/// 数据权限范围
/// </summary>
public enum DataScopeType
{
    /// <summary>
    /// 全部数据权限
    /// </summary>
    All = 1,

    /// <summary>
    /// 自定义数据权限
    /// </summary>
    Custom = 2,

    /// <summary>
    /// 本部门数据权限
    /// </summary>
    Department = 3,

    /// <summary>
    /// 本部门及以下数据权限
    /// </summary>
    DepartmentAndBelow = 4,

    /// <summary>
    /// 仅本人数据权限
    /// </summary>
    Self = 5
}
