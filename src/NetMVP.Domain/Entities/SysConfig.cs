using NetMVP.Domain.Common;
using NetMVP.Domain.Enums;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 参数配置实体
/// </summary>
public class SysConfig : BaseEntity
{
    /// <summary>
    /// 参数ID
    /// </summary>
    public int ConfigId { get; set; }

    /// <summary>
    /// 参数名称
    /// </summary>
    public string ConfigName { get; set; } = string.Empty;

    /// <summary>
    /// 参数键名
    /// </summary>
    public string ConfigKey { get; set; } = string.Empty;

    /// <summary>
    /// 参数键值
    /// </summary>
    public string ConfigValue { get; set; } = string.Empty;

    /// <summary>
    /// 系统内置
    /// </summary>
    public YesNo ConfigType { get; set; } = YesNo.No;
}
