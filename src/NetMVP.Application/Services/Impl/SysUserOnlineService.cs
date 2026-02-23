using Microsoft.Extensions.Logging;
using NetMVP.Application.DTOs.UserOnline;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 在线用户服务实现
/// </summary>
public class SysUserOnlineService : ISysUserOnlineService
{
    private readonly ICacheService _cacheService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<SysUserOnlineService> _logger;

    public SysUserOnlineService(
        ICacheService cacheService, 
        IJwtService jwtService,
        ILogger<SysUserOnlineService> logger)
    {
        _cacheService = cacheService;
        _jwtService = jwtService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<(List<OnlineUserDto> users, int total)> GetOnlineUserListAsync(
        OnlineUserQueryDto query, 
        CancellationToken cancellationToken = default)
    {
        // 获取所有在线用户的 Key
        var keys = await _cacheService.GetKeysAsync($"{CacheConstants.ONLINE_USER_KEY}*", cancellationToken);
        if (keys == null || keys.Count == 0)
            return (new List<OnlineUserDto>(), 0);

        // 获取所有在线用户信息
        var onlineUsers = new List<OnlineUserDto>();
        foreach (var key in keys)
        {
            try
            {
                var userInfo = await _cacheService.GetAsync<OnlineUserDto>(key, cancellationToken);
                if (userInfo != null)
                {
                    onlineUsers.Add(userInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取在线用户信息失败，Key: {key}");
            }
        }

        // 过滤
        if (!string.IsNullOrWhiteSpace(query.UserName))
        {
            onlineUsers = onlineUsers
                .Where(u => u.UserName.Contains(query.UserName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.Ipaddr))
        {
            onlineUsers = onlineUsers
                .Where(u => u.Ipaddr.Contains(query.Ipaddr, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var total = onlineUsers.Count;

        // 分页
        var pagedUsers = onlineUsers
            .OrderByDescending(u => u.LoginTime)
            .Skip((query.PageNum - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return (pagedUsers, total);
    }

    /// <inheritdoc/>
    public async Task ForceLogoutAsync(string tokenId, CancellationToken cancellationToken = default)
    {
        // tokenId 就是 JTI
        var onlineUserKey = $"{CacheConstants.ONLINE_USER_KEY}{tokenId}";
        
        try
        {
            // 先获取用户信息
            var userInfo = await _cacheService.GetAsync<OnlineUserDto>(onlineUserKey, cancellationToken);
            
            if (userInfo != null)
            {
                // 删除在线用户信息
                await _cacheService.RemoveAsync(onlineUserKey, cancellationToken);
                
                // 删除用户会话编号（单点登录关键）
                var userSessionKey = $"{CacheConstants.USER_SESSION_KEY}{userInfo.UserId}";
                await _cacheService.RemoveAsync(userSessionKey, cancellationToken);
                
                _logger.LogInformation($"强退用户：{userInfo.UserName}({userInfo.UserId})，JTI: {tokenId}");
            }
            else
            {
                _logger.LogWarning($"强退用户失败，未找到在线用户信息，JTI: {tokenId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"强退用户失败，JTI: {tokenId}");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task BatchForceLogoutAsync(string[] tokenIds, CancellationToken cancellationToken = default)
    {
        foreach (var tokenId in tokenIds)
        {
            await ForceLogoutAsync(tokenId, cancellationToken);
        }
    }
}
