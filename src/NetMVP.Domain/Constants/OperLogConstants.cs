namespace NetMVP.Domain.Constants;

/// <summary>
/// 操作日志常量
/// </summary>
public static class OperLogConstants
{
    /// <summary>
    /// 业务操作类型 - 其它
    /// </summary>
    public const string BUSINESS_TYPE_OTHER = "0";

    /// <summary>
    /// 业务操作类型 - 新增
    /// </summary>
    public const string BUSINESS_TYPE_INSERT = "1";

    /// <summary>
    /// 业务操作类型 - 修改
    /// </summary>
    public const string BUSINESS_TYPE_UPDATE = "2";

    /// <summary>
    /// 业务操作类型 - 删除
    /// </summary>
    public const string BUSINESS_TYPE_DELETE = "3";

    /// <summary>
    /// 业务操作类型 - 授权
    /// </summary>
    public const string BUSINESS_TYPE_GRANT = "4";

    /// <summary>
    /// 业务操作类型 - 导出
    /// </summary>
    public const string BUSINESS_TYPE_EXPORT = "5";

    /// <summary>
    /// 业务操作类型 - 导入
    /// </summary>
    public const string BUSINESS_TYPE_IMPORT = "6";

    /// <summary>
    /// 业务操作类型 - 强退
    /// </summary>
    public const string BUSINESS_TYPE_FORCE = "7";

    /// <summary>
    /// 业务操作类型 - 生成代码
    /// </summary>
    public const string BUSINESS_TYPE_GENCODE = "8";

    /// <summary>
    /// 业务操作类型 - 清空数据
    /// </summary>
    public const string BUSINESS_TYPE_CLEAN = "9";

    /// <summary>
    /// 操作人类别 - 其它
    /// </summary>
    public const string OPERATOR_TYPE_OTHER = "0";

    /// <summary>
    /// 操作人类别 - 后台用户
    /// </summary>
    public const string OPERATOR_TYPE_MANAGE = "1";

    /// <summary>
    /// 操作人类别 - 手机端用户
    /// </summary>
    public const string OPERATOR_TYPE_MOBILE = "2";
}
