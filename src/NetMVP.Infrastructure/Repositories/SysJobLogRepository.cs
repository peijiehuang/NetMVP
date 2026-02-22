using Microsoft.EntityFrameworkCore;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Persistence;

namespace NetMVP.Infrastructure.Repositories;

/// <summary>
/// 定时任务日志仓储实现
/// </summary>
public class SysJobLogRepository : ISysJobLogRepository
{
    private readonly NetMVPDbContext _context;
    private readonly DbSet<SysJobLog> _dbSet;

    public SysJobLogRepository(NetMVPDbContext context)
    {
        _context = context;
        _dbSet = context.Set<SysJobLog>();
    }

    public IQueryable<SysJobLog> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<SysJobLog?> GetByJobLogIdAsync(long jobLogId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.JobLogId == jobLogId, cancellationToken);
    }

    public async Task AddAsync(SysJobLog entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(SysJobLog entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task CleanJobLogAsync(CancellationToken cancellationToken = default)
    {
        await _dbSet.ExecuteDeleteAsync(cancellationToken);
    }
}
