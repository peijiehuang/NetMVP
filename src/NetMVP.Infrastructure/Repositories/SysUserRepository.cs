using Microsoft.EntityFrameworkCore;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Persistence;

namespace NetMVP.Infrastructure.Repositories;

/// <summary>
/// 用户仓储实现
/// </summary>
public class SysUserRepository : Repository<SysUser>, ISysUserRepository
{
    public SysUserRepository(NetMVPDbContext context) : base(context)
    {
    }

    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    public async Task<SysUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }

    /// <summary>
    /// 根据用户ID获取用户
    /// </summary>
    public async Task<SysUser?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
    }

    /// <summary>
    /// 获取用户及其角色
    /// </summary>
    public async Task<SysUser?> GetUserWithRolesAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
    }

    /// <summary>
    /// 获取用户及其岗位
    /// </summary>
    public async Task<SysUser?> GetUserWithPostsAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.UserPosts)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
    }

    /// <summary>
    /// 检查用户名是否唯一
    /// </summary>
    public async Task<bool> CheckUserNameUniqueAsync(string userName, long? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(u => u.UserName == userName);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.UserId != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 检查手机号是否唯一
    /// </summary>
    public async Task<bool> CheckPhoneUniqueAsync(string phone, long? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(u => u.PhoneNumberValue == phone);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.UserId != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 检查邮箱是否唯一
    /// </summary>
    public async Task<bool> CheckEmailUniqueAsync(string email, long? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(u => u.EmailValue == email);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.UserId != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 获取用户的岗位列表
    /// </summary>
    public async Task<List<SysPost>> GetUserPostsAsync(long userId, CancellationToken cancellationToken = default)
    {
        var userPosts = await _context.Set<SysUserPost>()
            .Where(up => up.UserId == userId)
            .ToListAsync(cancellationToken);

        var postIds = userPosts.Select(up => up.PostId).ToList();

        return await _context.Set<SysPost>()
            .Where(p => postIds.Contains(p.PostId))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 获取用户的角色列表
    /// </summary>
    public async Task<List<SysRole>> GetUserRolesAsync(long userId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _context.Set<SysUserRole>()
            .Where(ur => ur.UserId == userId)
            .ToListAsync(cancellationToken);

        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        return await _context.Set<SysRole>()
            .Where(r => roleIds.Contains(r.RoleId))
            .ToListAsync(cancellationToken);
    }

}
