namespace NetMVP.Domain.Constants;

/// <summary>
/// 缓存键常量
/// </summary>
public static class CacheConstants
{
    /// <summary>
    /// 登录令牌 redis key
    /// </summary>
    public const string LOGIN_TOKEN_KEY = "login_tokens:";

    /// <summary>
    /// 在线用户 redis key
    /// </summary>
    public const string ONLINE_USER_KEY = "online_user:";

    /// <summary>
    /// 用户会话编号 redis key（单点登录）
    /// </summary>
    public const string USER_SESSION_KEY = "user_session:";

    /// <summary>
    /// 刷新令牌 redis key
    /// </summary>
    public const string REFRESH_TOKEN_KEY = "refresh_token:";

    /// <summary>
    /// 令牌黑名单 redis key
    /// </summary>
    public const string TOKEN_BLACKLIST_KEY = "token_blacklist:";

    /// <summary>
    /// 验证码 redis key
    /// </summary>
    public const string CAPTCHA_CODE_KEY = "captcha:";

    /// <summary>
    /// 参数管理 cache key
    /// </summary>
    public const string SYS_CONFIG_KEY = "sys_config:";

    /// <summary>
    /// 字典管理 cache key
    /// </summary>
    public const string SYS_DICT_KEY = "sys_dict:";

    /// <summary>
    /// 防重提交 redis key
    /// </summary>
    public const string REPEAT_SUBMIT_KEY = "repeat_submit:";

    /// <summary>
    /// 限流 redis key
    /// </summary>
    public const string RATE_LIMIT_KEY = "rate_limit:";

    /// <summary>
    /// 登录账户密码错误次数 redis key
    /// </summary>
    public const string PWD_ERR_CNT_KEY = "pwd_err_cnt:";

    /// <summary>
    /// 用户缓存 redis key
    /// </summary>
    public const string USER_KEY = "user:";

    // 缓存名称（不带冒号）
    /// <summary>
    /// 登录令牌缓存名称
    /// </summary>
    public const string CACHE_NAME_LOGIN_TOKENS = "login_tokens";

    /// <summary>
    /// 在线用户缓存名称
    /// </summary>
    public const string CACHE_NAME_ONLINE_USER = "online_user";

    /// <summary>
    /// 刷新令牌缓存名称
    /// </summary>
    public const string CACHE_NAME_REFRESH_TOKEN = "refresh_token";

    /// <summary>
    /// 验证码缓存名称
    /// </summary>
    public const string CACHE_NAME_CAPTCHA = "captcha_codes";

    /// <summary>
    /// 系统配置缓存名称
    /// </summary>
    public const string CACHE_NAME_SYS_CONFIG = "sys_config";

    /// <summary>
    /// 数据字典缓存名称
    /// </summary>
    public const string CACHE_NAME_SYS_DICT = "sys_dict";

    /// <summary>
    /// 防重提交缓存名称
    /// </summary>
    public const string CACHE_NAME_REPEAT_SUBMIT = "repeat_submit";

    /// <summary>
    /// 限流缓存名称
    /// </summary>
    public const string CACHE_NAME_RATE_LIMIT = "rate_limit";

    /// <summary>
    /// 密码错误次数缓存名称
    /// </summary>
    public const string CACHE_NAME_PWD_ERR_CNT = "pwd_err_cnt";

    /// <summary>
    /// 用户缓存名称
    /// </summary>
    public const string CACHE_NAME_USER = "user";

    // 缓存备注
    /// <summary>
    /// 登录令牌缓存备注
    /// </summary>
    public const string CACHE_REMARK_LOGIN_TOKENS = "用户信息";

    /// <summary>
    /// 在线用户缓存备注
    /// </summary>
    public const string CACHE_REMARK_ONLINE_USER = "在线用户";

    /// <summary>
    /// 刷新令牌缓存备注
    /// </summary>
    public const string CACHE_REMARK_REFRESH_TOKEN = "刷新令牌";

    /// <summary>
    /// 验证码缓存备注
    /// </summary>
    public const string CACHE_REMARK_CAPTCHA = "验证码";

    /// <summary>
    /// 系统配置缓存备注
    /// </summary>
    public const string CACHE_REMARK_SYS_CONFIG = "配置信息";

    /// <summary>
    /// 数据字典缓存备注
    /// </summary>
    public const string CACHE_REMARK_SYS_DICT = "数据字典";

    /// <summary>
    /// 防重提交缓存备注
    /// </summary>
    public const string CACHE_REMARK_REPEAT_SUBMIT = "防重提交";

    /// <summary>
    /// 限流缓存备注
    /// </summary>
    public const string CACHE_REMARK_RATE_LIMIT = "限流处理";

    /// <summary>
    /// 密码错误次数缓存备注
    /// </summary>
    public const string CACHE_REMARK_PWD_ERR_CNT = "密码错误次数";

    /// <summary>
    /// 用户缓存备注
    /// </summary>
    public const string CACHE_REMARK_USER = "用户缓存";
}
