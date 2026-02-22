using NetMVP.Application.DTOs.UserOnline;
using NetMVP.Domain.Interfaces;
using System.Text.Json;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 在线用户服务实现
/// </summary>
public class SysUserOnlineService : ISysUserOnlineService
{
    private readonly ICacheService _cacheService;
    private readonly IJwtService _jwtService;

    public SysUserOnlineService(ICacheService cacheService, IJwtService jwtService)
    {
        _cacheService = cacheService;
        _jwtService = jwtService;
    }

    /// <inheritdoc/>
    public async Task<(List<OnlineUserDto> users, int total)> GetOnlineUserListAsync(
        OnlineUserQueryDto query, 
        CancellationToken cancellationToken = default)
    {
        // 获取所有在线用户的 Key
        var keys = await _cacheService.GetKeysAsync("online_user:*", cancellationToken);
        if (keys == null || keys.Count == 0)
            return (new List<OnlineUserDto>(), 0);

        // 获取所有在线用户信息
        var onlineUsers = new List<OnlineUserDto>();
        foreach (var key in keys)
        {
            var userJson = await _cacheService.GetAsync<string>(key, cancellationToken);
            if (string.IsNullOrEmpty(userJson)) continue;

            try
            {
                var userInfo = JsonSerializer.Deserialize<OnlineUserDto>(userJson);
                if (userInfo != null)
                {
                    onlineUsers.Add(userInfo);
                }
            }
            catch
            {
                // 忽略反序列化失败的数据
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
        // 将 Token 加入黑名单
        var blacklistKey = $"token_blacklist:{tokenId}";
        await _cacheService.SetAsync(blacklistKey, "revoked", TimeSpan.FromHours(24), cancellationToken);

        // 删除在线用户信息
        var onlineUserKey = $"online_user:{tokenId}";
        await _cacheService.RemoveAsync(onlineUserKey, cancellationToken);
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
