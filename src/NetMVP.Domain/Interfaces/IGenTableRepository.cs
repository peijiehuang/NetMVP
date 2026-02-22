using NetMVP.Domain.Entities;

namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 代码生成业务表仓储接口
/// </summary>
public interface IGenTableRepository : IRepository<GenTable>
{
    /// <summary>
    /// 查询数据库表列表
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="tableComment">表描述</param>
    /// <returns>数据库表列表</returns>
    Task<List<GenTable>> GetDbTablesAsync(string? tableName = null, string? tableComment = null);

    /// <summary>
    /// 根据表名查询数据库表信息
    /// </summary>
    /// <param name="tableNames">表名数组</param>
    /// <returns>数据库表列表</returns>
    Task<List<GenTable>> GetDbTablesByNamesAsync(string[] tableNames);

    /// <summary>
    /// 根据表名查询生成表信息（包含字段）
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <returns>生成表信息</returns>
    Task<GenTable?> GetGenTableByNameAsync(string tableName);

    /// <summary>
    /// 根据ID查询生成表信息（包含字段）
    /// </summary>
    /// <param name="tableId">表ID</param>
    /// <returns>生成表信息</returns>
    Task<GenTable?> GetGenTableByIdWithColumnsAsync(long tableId);
}
