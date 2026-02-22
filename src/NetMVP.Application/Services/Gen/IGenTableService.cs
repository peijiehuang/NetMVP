using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Gen;

namespace NetMVP.Application.Services.Gen;

/// <summary>
/// 代码生成表服务接口
/// </summary>
public interface IGenTableService
{
    /// <summary>
    /// 获取生成表列表
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页结果</returns>
    Task<PagedResult<GenTableDto>> GetGenTableListAsync(GenTableQueryDto query);

    /// <summary>
    /// 获取数据库表列表
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页结果</returns>
    Task<PagedResult<GenTableDto>> GetDbTableListAsync(GenTableQueryDto query);

    /// <summary>
    /// 根据ID获取生成表详情
    /// </summary>
    /// <param name="tableId">表ID</param>
    /// <returns>生成表信息</returns>
    Task<GenTableDto?> GetGenTableByIdAsync(long tableId);

    /// <summary>
    /// 导入表
    /// </summary>
    /// <param name="dto">导入表DTO</param>
    /// <returns>是否成功</returns>
    Task<bool> ImportGenTableAsync(ImportTableDto dto);

    /// <summary>
    /// 更新生成表
    /// </summary>
    /// <param name="dto">更新DTO</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateGenTableAsync(UpdateGenTableDto dto);

    /// <summary>
    /// 删除生成表
    /// </summary>
    /// <param name="tableIds">表ID数组</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteGenTableAsync(long[] tableIds);

    /// <summary>
    /// 同步数据库
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <returns>是否成功</returns>
    Task<bool> SyncDbAsync(string tableName);

    /// <summary>
    /// 根据ID获取表信息（用于代码生成）
    /// </summary>
    /// <param name="tableId">表ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表信息</returns>
    Task<dynamic?> GetTableByIdAsync(long tableId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据表名获取表信息（用于代码生成）
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表信息</returns>
    Task<dynamic?> GetTableByNameAsync(string tableName, CancellationToken cancellationToken = default);
}
