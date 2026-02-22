namespace NetMVP.Application.Common.Models;

/// <summary>
/// 分页结果
/// </summary>
public class PagedResult<T>
{
    /// <summary>
    /// 数据列表
    /// </summary>
    public List<T> Rows { get; set; } = new();

    /// <summary>
    /// 总记录数
    /// </summary>
    public long Total { get; set; }
}
