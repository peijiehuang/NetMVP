using NetMVP.Application.Common.Models;

namespace NetMVP.Application.DTOs.Config;

/// <summary>
/// 参数配置查询 DTO
/// </summary>
public class ConfigQueryDto : PageQueryDto
{
    /// <summary>
    /// 参数名称
    /// </summary>
    public string? ConfigName { get; set; }

    /// <summary>
    /// 参数键名
    /// </summary>
    public string? ConfigKey { get; set; }

    /// <summary>
    /// 系统内置（Y是 N否）
    /// </summary>
    public string? ConfigType { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? BeginTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}
