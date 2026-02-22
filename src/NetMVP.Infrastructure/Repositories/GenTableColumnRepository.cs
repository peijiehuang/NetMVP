using Microsoft.EntityFrameworkCore;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Persistence;

namespace NetMVP.Infrastructure.Repositories;

/// <summary>
/// 代码生成业务表字段仓储实现
/// </summary>
public class GenTableColumnRepository : Repository<GenTableColumn>, IGenTableColumnRepository
{
    public GenTableColumnRepository(NetMVPDbContext context) : base(context)
    {
    }

    /// <summary>
    /// 根据表名查询数据库表字段信息
    /// </summary>
    public async Task<List<GenTableColumn>> GetDbTableColumnsByNameAsync(string tableName)
    {
        var sql = @"
            SELECT 
                column_name AS ColumnName,
                (CASE WHEN (is_nullable = 'no' AND column_key != 'PRI') THEN '1' ELSE '0' END) AS IsRequired,
                (CASE WHEN column_key = 'PRI' THEN '1' ELSE '0' END) AS IsPk,
                ordinal_position AS Sort,
                column_comment AS ColumnComment,
                (CASE WHEN extra = 'auto_increment' THEN '1' ELSE '0' END) AS IsIncrement,
                column_type AS ColumnType
            FROM information_schema.columns 
            WHERE table_schema = DATABASE() 
            AND table_name = {0}
            ORDER BY ordinal_position";

        return await _context.Database
            .SqlQueryRaw<GenTableColumn>(sql, tableName)
            .ToListAsync();
    }

    /// <summary>
    /// 根据表ID查询字段列表
    /// </summary>
    public async Task<List<GenTableColumn>> GetColumnsByTableIdAsync(long tableId)
    {
        return await _dbSet
            .Where(c => c.TableId == tableId)
            .OrderBy(c => c.Sort)
            .ToListAsync();
    }

    /// <summary>
    /// 根据表ID删除字段
    /// </summary>
    public async Task<int> DeleteByTableIdAsync(long tableId)
    {
        return await _dbSet
            .Where(c => c.TableId == tableId)
            .ExecuteDeleteAsync();
    }
}
