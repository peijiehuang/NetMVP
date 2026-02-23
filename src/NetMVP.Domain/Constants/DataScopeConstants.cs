namespace NetMVP.Domain.Constants;

/// <summary>
/// 数据权限常量
/// </summary>
public static class DataScopeConstants
{
    /// <summary>
    /// 全部数据权限
    /// </summary>
    public const string DATA_SCOPE_ALL = "1";

    /// <summary>
    /// 自定义数据权限
    /// </summary>
    public const string DATA_SCOPE_CUSTOM = "2";

    /// <summary>
    /// 部门数据权限
    /// </summary>
    public const string DATA_SCOPE_DEPT = "3";

    /// <summary>
    /// 部门及以下数据权限
    /// </summary>
    public const string DATA_SCOPE_DEPT_AND_CHILD = "4";

    /// <summary>
    /// 仅本人数据权限
    /// </summary>
    public const string DATA_SCOPE_SELF = "5";
}
