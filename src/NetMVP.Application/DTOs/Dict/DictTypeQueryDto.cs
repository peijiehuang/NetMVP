using NetMVP.Application.Common.Models;

namespace NetMVP.Application.DTOs.Dict;

/// <summary>
/// 字典类型查询 DTO
/// </summary>
public class DictTypeQueryDto : PageQueryDto
{
    /// <summary>
    /// 字典名称
    /// </summary>
    public string? DictName { get; set; }

    /// <summary>
    /// 字典类型
    /// </summary>
    public string? DictType { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }
}
