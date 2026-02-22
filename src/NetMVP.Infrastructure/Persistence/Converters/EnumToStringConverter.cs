using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetMVP.Domain.Enums;

namespace NetMVP.Infrastructure.Persistence.Converters;

/// <summary>
/// 枚举到字符串的转换器
/// </summary>
public static class EnumConverters
{
    /// <summary>
    /// DelFlag转换器：Exist='0', Deleted='2'
    /// </summary>
    public static readonly ValueConverter<DelFlag, string> DelFlagConverter = new(
        v => v == DelFlag.Exist ? "0" : "2",
        v => v == "0" ? DelFlag.Exist : DelFlag.Deleted);

    /// <summary>
    /// UserStatus转换器：Normal='0', Disabled='1'
    /// </summary>
    public static readonly ValueConverter<UserStatus, string> UserStatusConverter = new(
        v => ((int)v).ToString(),
        v => (UserStatus)int.Parse(v));

    /// <summary>
    /// Gender转换器：Male='0', Female='1', Unknown='2'
    /// </summary>
    public static readonly ValueConverter<Gender, string> GenderConverter = new(
        v => ((int)v).ToString(),
        v => (Gender)int.Parse(v));

    /// <summary>
    /// MenuType转换器：Directory='M', Menu='C', Button='F'
    /// </summary>
    public static readonly ValueConverter<MenuType, string> MenuTypeConverter = new(
        v => v == MenuType.Directory ? "M" : v == MenuType.Menu ? "C" : "F",
        v => v == "M" ? MenuType.Directory : v == "C" ? MenuType.Menu : MenuType.Button);

    /// <summary>
    /// VisibleStatus转换器：Show='0', Hide='1'
    /// </summary>
    public static readonly ValueConverter<VisibleStatus, string> VisibleStatusConverter = new(
        v => ((int)v).ToString(),
        v => (VisibleStatus)int.Parse(v));

    /// <summary>
    /// YesNo转换器：No='N', Yes='Y'
    /// </summary>
    public static readonly ValueConverter<YesNo, string> YesNoConverter = new(
        v => v == YesNo.Yes ? "Y" : "N",
        v => v == "Y" ? YesNo.Yes : YesNo.No);

    /// <summary>
    /// IsFrame转换器（反向布尔）：true='1'(否/不是外链), false='0'(是/是外链)
    /// 注意：若依系统中is_frame字段的含义是反向的
    /// </summary>
    public static readonly ValueConverter<bool, int> IsFrameConverter = new(
        v => v ? 1 : 0,  // true -> 1(不是外链), false -> 0(是外链)
        v => v == 1);     // 1 -> true(不是外链), 0 -> false(是外链)

    /// <summary>
    /// CommonStatus转换器：Success='0', Failure='1'
    /// </summary>
    public static readonly ValueConverter<CommonStatus, string> CommonStatusConverter = new(
        v => ((int)v).ToString(),
        v => (CommonStatus)int.Parse(v));

    /// <summary>
    /// BusinessType转换器
    /// </summary>
    public static readonly ValueConverter<BusinessType, string> BusinessTypeConverter = new(
        v => ((int)v).ToString(),
        v => (BusinessType)int.Parse(v));

    /// <summary>
    /// OperatorType转换器
    /// </summary>
    public static readonly ValueConverter<OperatorType, string> OperatorTypeConverter = new(
        v => ((int)v).ToString(),
        v => (OperatorType)int.Parse(v));

    /// <summary>
    /// NoticeType转换器
    /// </summary>
    public static readonly ValueConverter<NoticeType, string> NoticeTypeConverter = new(
        v => ((int)v).ToString(),
        v => (NoticeType)int.Parse(v));

    /// <summary>
    /// NoticeStatus转换器
    /// </summary>
    public static readonly ValueConverter<NoticeStatus, string> NoticeStatusConverter = new(
        v => ((int)v).ToString(),
        v => (NoticeStatus)int.Parse(v));

    /// <summary>
    /// DataScopeType转换器
    /// </summary>
    public static readonly ValueConverter<DataScopeType, string> DataScopeTypeConverter = new(
        v => ((int)v).ToString(),
        v => (DataScopeType)int.Parse(v));
}
