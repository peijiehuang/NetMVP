namespace NetMVP.Application.DTOs.Post;

/// <summary>
/// 岗位查询 DTO
/// </summary>
public class PostQueryDto
{
    /// <summary>
    /// 岗位编码
    /// </summary>
    public string? PostCode { get; set; }

    /// <summary>
    /// 岗位名称
    /// </summary>
    public string? PostName { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    public int PageNum { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;
}
