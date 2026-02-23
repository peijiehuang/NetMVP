using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NetMVP.Application.Converters;

namespace NetMVP.Application.DTOs.Job;

/// <summary>
/// 创建任务DTO
/// </summary>
public class CreateJobDto
{
    /// <summary>
    /// 任务名称
    /// </summary>
    [Required(ErrorMessage = "任务名称不能为空")]
    [StringLength(64, ErrorMessage = "任务名称长度不能超过64个字符")]
    public string JobName { get; set; } = string.Empty;

    /// <summary>
    /// 任务组名
    /// </summary>
    [Required(ErrorMessage = "任务组名不能为空")]
    [StringLength(64, ErrorMessage = "任务组名长度不能超过64个字符")]
    public string JobGroup { get; set; } = "DEFAULT";

    /// <summary>
    /// 调用目标字符串
    /// </summary>
    [Required(ErrorMessage = "调用目标不能为空")]
    [StringLength(500, ErrorMessage = "调用目标长度不能超过500个字符")]
    public string InvokeTarget { get; set; } = string.Empty;

    /// <summary>
    /// cron执行表达式
    /// </summary>
    [Required(ErrorMessage = "Cron表达式不能为空")]
    [StringLength(255, ErrorMessage = "Cron表达式长度不能超过255个字符")]
    public string CronExpression { get; set; } = string.Empty;

    /// <summary>
    /// 计划执行错误策略（1立即执行 2执行一次 3放弃执行）
    /// </summary>
    [JsonConverter(typeof(FlexibleStringConverter))]
    public string MisfirePolicy { get; set; } = "3";

    /// <summary>
    /// 是否并发执行（0允许 1禁止）
    /// </summary>
    [JsonConverter(typeof(FlexibleStringConverter))]
    public string Concurrent { get; set; } = "1";

    /// <summary>
    /// 状态（0正常 1暂停）
    /// </summary>
    [JsonConverter(typeof(FlexibleStringConverter))]
    public string Status { get; set; } = "0";

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}
