using NetMVP.Application.Common.Models;

namespace NetMVP.Application.DTOs.Dict;

/// <summary>
/// 字典数据查询 DTO
/// </summary>
public class DictDataQueryDto : PageQueryDto
{
    /// <summary>
    /// 字典类型
    /// </summary>
    public string? DictType { get; set; }

    /// <summary>
    /// 字典标签
    /// </summary>
    public string? DictLabel { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }
}
