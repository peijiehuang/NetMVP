using NetMVP.Domain.Entities;

namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 代码生成业务表字段仓储接口
/// </summary>
public interface IGenTableColumnRepository : IRepository<GenTableColumn>
{
    /// <summary>
    /// 根据表名查询数据库表字段信息
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <returns>字段列表</returns>
    Task<List<GenTableColumn>> GetDbTableColumnsByNameAsync(string tableName);

    /// <summary>
    /// 根据表ID查询字段列表
    /// </summary>
    /// <param name="tableId">表ID</param>
    /// <returns>字段列表</returns>
    Task<List<GenTableColumn>> GetColumnsByTableIdAsync(long tableId);

    /// <summary>
    /// 根据表ID删除字段
    /// </summary>
    /// <param name="tableId">表ID</param>
    /// <returns>删除数量</returns>
    Task<int> DeleteByTableIdAsync(long tableId);
}
