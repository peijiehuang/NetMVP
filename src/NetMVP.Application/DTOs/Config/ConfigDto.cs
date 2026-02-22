namespace NetMVP.Application.DTOs.Config;

/// <summary>
/// 参数配置 DTO
/// </summary>
public class ConfigDto
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
    /// 系统内置（Y是 N否）
    /// </summary>
    public string ConfigType { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
