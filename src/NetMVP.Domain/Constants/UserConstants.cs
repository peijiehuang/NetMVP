namespace NetMVP.Domain.Constants;

/// <summary>
/// 用户常量信息
/// </summary>
public static class UserConstants
{
    /// <summary>
    /// 平台内系统用户的唯一标志
    /// </summary>
    public const string SYS_USER = "SYS_USER";

    /// <summary>
    /// 正常状态
    /// </summary>
    public const string NORMAL = "0";

    /// <summary>
    /// 异常状态
    /// </summary>
    public const string EXCEPTION = "1";

    /// <summary>
    /// 用户封禁状态
    /// </summary>
    public const string USER_DISABLE = "1";

    /// <summary>
    /// 角色正常状态
    /// </summary>
    public const string ROLE_NORMAL = "0";

    /// <summary>
    /// 角色封禁状态
    /// </summary>
    public const string ROLE_DISABLE = "1";

    /// <summary>
    /// 部门正常状态
    /// </summary>
    public const string DEPT_NORMAL = "0";

    /// <summary>
    /// 部门停用状态
    /// </summary>
    public const string DEPT_DISABLE = "1";

    /// <summary>
    /// 字典正常状态
    /// </summary>
    public const string DICT_NORMAL = "0";

    /// <summary>
    /// 是否为系统默认（是）
    /// </summary>
    public const string YES = "Y";

    /// <summary>
    /// 是否菜单外链（是）
    /// </summary>
    public const string YES_FRAME = "0";

    /// <summary>
    /// 是否菜单外链（否）
    /// </summary>
    public const string NO_FRAME = "1";

    /// <summary>
    /// 菜单类型（目录）
    /// </summary>
    public const string TYPE_DIR = "M";

    /// <summary>
    /// 菜单类型（菜单）
    /// </summary>
    public const string TYPE_MENU = "C";

    /// <summary>
    /// 菜单类型（按钮）
    /// </summary>
    public const string TYPE_BUTTON = "F";

    /// <summary>
    /// Layout组件标识
    /// </summary>
    public const string LAYOUT = "Layout";

    /// <summary>
    /// ParentView组件标识
    /// </summary>
    public const string PARENT_VIEW = "ParentView";

    /// <summary>
    /// InnerLink组件标识
    /// </summary>
    public const string INNER_LINK = "InnerLink";

    /// <summary>
    /// 校验是否唯一的返回标识
    /// </summary>
    public const bool UNIQUE = true;
    public const bool NOT_UNIQUE = false;

    /// <summary>
    /// 用户名长度限制
    /// </summary>
    public const int USERNAME_MIN_LENGTH = 2;
    public const int USERNAME_MAX_LENGTH = 20;

    /// <summary>
    /// 密码长度限制
    /// </summary>
    public const int PASSWORD_MIN_LENGTH = 5;
    public const int PASSWORD_MAX_LENGTH = 20;

    /// <summary>
    /// 性别 - 男
    /// </summary>
    public const string SEX_MALE = "0";

    /// <summary>
    /// 性别 - 女
    /// </summary>
    public const string SEX_FEMALE = "1";

    /// <summary>
    /// 性别 - 未知
    /// </summary>
    public const string SEX_UNKNOWN = "2";

    /// <summary>
    /// 删除标志 - 存在
    /// </summary>
    public const string DEL_FLAG_EXIST = "0";

    /// <summary>
    /// 删除标志 - 删除
    /// </summary>
    public const string DEL_FLAG_DELETED = "2";
}
