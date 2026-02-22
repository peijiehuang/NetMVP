using Microsoft.EntityFrameworkCore;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Persistence;

namespace NetMVP.Infrastructure.Repositories;

/// <summary>
/// 定时任务仓储实现
/// </summary>
public class SysJobRepository : Repository<SysJob>, ISysJobRepository
{
    public SysJobRepository(NetMVPDbContext context) : base(context)
    {
    }

    public async Task<SysJob?> GetByJobIdAsync(long jobId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.JobId == jobId, cancellationToken);
    }

    public async Task<bool> IsJobNameUniqueAsync(string jobName, string jobGroup, long? excludeJobId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(x => x.JobName == jobName && x.JobGroup == jobGroup);
        
        if (excludeJobId.HasValue)
        {
            query = query.Where(x => x.JobId != excludeJobId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }
}
