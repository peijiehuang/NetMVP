using Microsoft.EntityFrameworkCore;
using NetMVP.Domain.Common;
using System.Linq.Expressions;

namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 仓储基础接口
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有实体
    /// </summary>
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件查询
    /// </summary>
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加实体
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新实体
    /// </summary>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除实体
    /// </summary>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID删除实体
    /// </summary>
    Task DeleteByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取可查询对象
    /// </summary>
    IQueryable<TEntity> GetQueryable();

    /// <summary>
    /// 获取数据库上下文
    /// </summary>
    DbContext GetDbContext();
}
