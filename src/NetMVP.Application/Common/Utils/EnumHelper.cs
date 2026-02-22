using NetMVP.Domain.Enums;

namespace NetMVP.Application.Common.Utils;

/// <summary>
/// 枚举转换助手
/// 用于处理枚举与数据库char类型字段的转换
/// </summary>
public static class EnumHelper
{
    /// <summary>
    /// 将Gender枚举转换为字符串（用于API响应）
    /// </summary>
    public static string ToGenderString(Gender gender)
    {
        return ((int)gender).ToString();
    }

    /// <summary>
    /// 将UserStatus枚举转换为字符串（用于API响应）
    /// </summary>
    public static string ToStatusString(UserStatus status)
    {
        return ((int)status).ToString();
    }

    /// <summary>
    /// 将NoticeStatus枚举转换为字符串（用于API响应）
    /// </summary>
    public static string ToNoticeStatusString(NoticeStatus status)
    {
        return ((int)status).ToString();
    }

    /// <summary>
    /// 将DelFlag枚举转换为字符串（用于API响应）
    /// </summary>
    public static string ToDelFlagString(DelFlag delFlag)
    {
        return ((char)delFlag).ToString();
    }

    /// <summary>
    /// 从字符串解析Gender枚举
    /// </summary>
    public static Gender ParseGender(string value)
    {
        if (string.IsNullOrEmpty(value))
            return Gender.Unknown;
        
        if (int.TryParse(value, out int intValue))
            return (Gender)intValue;
        
        return Gender.Unknown;
    }

    /// <summary>
    /// 从字符串解析UserStatus枚举
    /// </summary>
    public static UserStatus ParseStatus(string value)
    {
        if (string.IsNullOrEmpty(value))
            return UserStatus.Normal;
        
        if (int.TryParse(value, out int intValue))
            return (UserStatus)intValue;
        
        return UserStatus.Normal;
    }

    /// <summary>
    /// 从字符串解析DelFlag枚举
    /// </summary>
    public static DelFlag ParseDelFlag(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length == 0)
            return DelFlag.Exist;
        
        return (DelFlag)value[0];
    }
}
