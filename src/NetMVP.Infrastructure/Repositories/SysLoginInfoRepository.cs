using Microsoft.EntityFrameworkCore;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Persistence;

namespace NetMVP.Infrastructure.Repositories;

/// <summary>
/// 登录日志仓储实现
/// </summary>
public class SysLoginInfoRepository : ISysLoginInfoRepository
{
    private readonly NetMVPDbContext _context;
    private readonly DbSet<SysLoginInfo> _dbSet;

    public SysLoginInfoRepository(NetMVPDbContext context)
    {
        _context = context;
        _dbSet = context.Set<SysLoginInfo>();
    }

    public IQueryable<SysLoginInfo> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public async Task AddAsync(SysLoginInfo entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(SysLoginInfo entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task<int> CleanAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE sys_logininfor",
            cancellationToken);
    }
}
