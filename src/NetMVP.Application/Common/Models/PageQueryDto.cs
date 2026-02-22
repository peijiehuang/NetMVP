namespace NetMVP.Application.Common.Models;

/// <summary>
/// 分页查询基类
/// </summary>
public class PageQueryDto
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageNum { get; set; } = 1;

    /// <summary>
    /// 页大小
    /// </summary>
    public int PageSize { get; set; } = 10;
}
