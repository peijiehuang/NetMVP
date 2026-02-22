using Microsoft.EntityFrameworkCore;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Persistence;

namespace NetMVP.Infrastructure.Repositories;

/// <summary>
/// 代码生成业务表仓储实现
/// </summary>
public class GenTableRepository : Repository<GenTable>, IGenTableRepository
{
    private readonly IGenTableColumnRepository _columnRepository;

    public GenTableRepository(NetMVPDbContext context, IGenTableColumnRepository columnRepository) : base(context)
    {
        _columnRepository = columnRepository;
    }

    /// <summary>
    /// 查询数据库表列表
    /// </summary>
    public async Task<List<GenTable>> GetDbTablesAsync(string? tableName = null, string? tableComment = null)
    {
        var sql = @"
            SELECT 
                table_name AS TableName, 
                table_comment AS TableComment, 
                create_time AS CreateTime, 
                update_time AS UpdateTime 
            FROM information_schema.tables
            WHERE table_schema = DATABASE()
            AND table_name NOT LIKE 'qrtz\_%' 
            AND table_name NOT LIKE 'gen\_%'
            AND table_name NOT IN (SELECT table_name FROM gen_table)";

        var parameters = new List<object>();
        
        if (!string.IsNullOrEmpty(tableName))
        {
            sql += " AND LOWER(table_name) LIKE LOWER({0})";
            parameters.Add($"%{tableName}%");
        }

        if (!string.IsNullOrEmpty(tableComment))
        {
            sql += $" AND LOWER(table_comment) LIKE LOWER({{{parameters.Count}}})";
            parameters.Add($"%{tableComment}%");
        }

        sql += " ORDER BY create_time DESC";

        return await _context.Database
            .SqlQueryRaw<GenTable>(sql, parameters.ToArray())
            .ToListAsync();
    }

    /// <summary>
    /// 根据表名查询数据库表信息
    /// </summary>
    public async Task<List<GenTable>> GetDbTablesByNamesAsync(string[] tableNames)
    {
        var sql = @"
            SELECT 
                table_name AS TableName, 
                table_comment AS TableComment, 
                create_time AS CreateTime, 
                update_time AS UpdateTime 
            FROM information_schema.tables
            WHERE table_schema = DATABASE()
            AND table_name NOT LIKE 'qrtz\_%' 
            AND table_name NOT LIKE 'gen\_%'
            AND table_name IN ({0})";

        var tableNameParams = string.Join(",", tableNames.Select((_, i) => $"{{{i}}}"));
        sql = sql.Replace("{0}", tableNameParams);

        return await _context.Database
            .SqlQueryRaw<GenTable>(sql, tableNames.Cast<object>().ToArray())
            .ToListAsync();
    }

    /// <summary>
    /// 根据表名查询生成表信息（包含字段）
    /// </summary>
    public async Task<GenTable?> GetGenTableByNameAsync(string tableName)
    {
        var table = await _dbSet
            .FirstOrDefaultAsync(t => t.TableName == tableName);

        if (table != null)
        {
            table.Columns = await _columnRepository.GetColumnsByTableIdAsync(table.TableId);
            table.PkColumn = table.Columns.FirstOrDefault(c => c.IsPk == "1");
        }

        return table;
    }

    /// <summary>
    /// 根据ID查询生成表信息（包含字段）
    /// </summary>
    public async Task<GenTable?> GetGenTableByIdWithColumnsAsync(long tableId)
    {
        var table = await _dbSet.FindAsync(tableId);

        if (table != null)
        {
            table.Columns = await _columnRepository.GetColumnsByTableIdAsync(tableId);
            table.PkColumn = table.Columns.FirstOrDefault(c => c.IsPk == "1");
        }

        return table;
    }
}
