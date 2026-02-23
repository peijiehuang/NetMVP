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
}
