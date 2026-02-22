using Microsoft.EntityFrameworkCore;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Persistence;

namespace NetMVP.Infrastructure.Repositories;

/// <summary>
/// 部门仓储实现
/// </summary>
public class SysDeptRepository : Repository<SysDept>, ISysDeptRepository
{
    public SysDeptRepository(NetMVPDbContext context) : base(context)
    {
    }

    /// <summary>
    /// 根据部门ID获取部门
    /// </summary>
    public async Task<SysDept?> GetByDeptIdAsync(long deptId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.DeptId == deptId, cancellationToken);
    }

    /// <summary>
    /// 获取所有部门（树形结构）
    /// </summary>
    public async Task<List<SysDept>> GetDeptTreeAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderBy(d => d.ParentId)
            .ThenBy(d => d.OrderNum)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 获取子部门列表
    /// </summary>
    public async Task<List<SysDept>> GetChildrenDeptsAsync(long parentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.ParentId == parentId)
            .OrderBy(d => d.OrderNum)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 检查部门名称是否唯一
    /// </summary>
    public async Task<bool> CheckDeptNameUniqueAsync(string deptName, long parentId, long? excludeDeptId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(d => d.DeptName == deptName && d.ParentId == parentId);
        
        if (excludeDeptId.HasValue)
        {
            query = query.Where(d => d.DeptId != excludeDeptId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 检查是否存在子部门
    /// </summary>
    public async Task<bool> HasChildrenAsync(long deptId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(d => d.ParentId == deptId, cancellationToken);
    }
}
