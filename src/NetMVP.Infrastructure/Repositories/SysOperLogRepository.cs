using Microsoft.EntityFrameworkCore;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Persistence;

namespace NetMVP.Infrastructure.Repositories;

/// <summary>
/// 操作日志仓储实现
/// </summary>
public class SysOperLogRepository : ISysOperLogRepository
{
    private readonly NetMVPDbContext _context;
    private readonly DbSet<SysOperLog> _dbSet;

    public SysOperLogRepository(NetMVPDbContext context)
    {
        _context = context;
        _dbSet = context.Set<SysOperLog>();
    }

    public IQueryable<SysOperLog> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public async Task AddAsync(SysOperLog entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(SysOperLog entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task CleanAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE sys_oper_log", cancellationToken);
    }
}
