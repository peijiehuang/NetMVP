using Microsoft.EntityFrameworkCore;
using NetMVP.Infrastructure.Persistence;

namespace NetMVP.Infrastructure.Extensions;

/// <summary>
/// IQueryable 扩展方法
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// 转换为分页列表
    /// </summary>
    public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var totalCount = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<T>(items, totalCount, pageNumber, pageSize);
    }
}
