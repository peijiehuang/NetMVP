using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Configuration;
using NetMVP.Infrastructure.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NetMVP.Infrastructure.Services.Auth;

/// <summary>
/// JWT 服务实现
/// </summary>
public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ICacheService _cacheService;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly ISysUserRepository _userRepository;
    private readonly ISysDeptRepository _deptRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<JwtService> _logger;

    public JwtService(
        IOptions<JwtSettings> jwtSettings, 
        ICacheService cacheService,
        ISysUserRepository userRepository,
        ISysDeptRepository deptRepository,
        IHttpContextAccessor httpContextAccessor,
        ILogger<JwtService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _cacheService = cacheService;
        _tokenHandler = new JwtSecurityTokenHandler();
        _userRepository = userRepository;
        _deptRepository = deptRepository;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<(string accessToken, string refreshToken)> GenerateTokenAsync(
        long userId, 
        string userName, 
        CancellationToken cancellationToken = default)
    {
        // 单点登录：先删除该用户的旧会话
        await RemoveUserOldSessionAsync(userId, cancellationToken);

        // 生成访问令牌
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
            signingCredentials: credentials
        );

        var accessToken = _tokenHandler.WriteToken(token);

        // 生成刷新令牌
        var refreshToken = GenerateRefreshToken();

        // 存储刷新令牌到 Redis
        var refreshTokenKey = $"{CacheConstants.REFRESH_TOKEN_KEY}{userId}:{refreshToken}";
        await _cacheService.SetAsync(
            refreshTokenKey, 
            userId.ToString(), 
            TimeSpan.FromDays(_jwtSettings.RefreshTokenExpireDays),
            cancellationToken);

        // 存储在线用户信息到 Redis
        var jti = claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        await StoreOnlineUserAsync(userId, userName, jti, cancellationToken);

        return (accessToken, refreshToken);
    }

    /// <inheritdoc/>
    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            };

            _tokenHandler.ValidateToken(token, validationParameters, out _);

            // 检查 Token 是否在黑名单中
            var jti = GetJtiFromToken(token);
            if (!string.IsNullOrEmpty(jti))
            {
                var blacklistKey = $"{CacheConstants.TOKEN_BLACKLIST_KEY}{jti}";
                var isBlacklisted = await _cacheService.ExistsAsync(blacklistKey, cancellationToken);
                if (isBlacklisted)
                    return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(
        string refreshToken, 
        CancellationToken cancellationToken = default)
    {
        // 查找刷新令牌
        var keys = await _cacheService.GetKeysAsync($"{CacheConstants.REFRESH_TOKEN_KEY}*:{refreshToken}", cancellationToken);
        if (keys == null || keys.Count == 0)
            throw new UnauthorizedAccessException("无效的刷新令牌");

        var refreshTokenKey = keys[0];
        var userIdStr = await _cacheService.GetAsync<string>(refreshTokenKey, cancellationToken);
        if (string.IsNullOrEmpty(userIdStr))
            throw new UnauthorizedAccessException("刷新令牌已过期");

        var userId = long.Parse(userIdStr);

        // 删除旧的刷新令牌
        await _cacheService.RemoveAsync(refreshTokenKey, cancellationToken);

        // 生成新的令牌对
        // 注意：这里需要从数据库获取用户名，暂时使用占位符
        return await GenerateTokenAsync(userId, $"user_{userId}", cancellationToken);
    }

    /// <inheritdoc/>
    public async Task RevokeTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var jti = GetJtiFromToken(token);
        if (string.IsNullOrEmpty(jti))
            return;

        // 将 Token 加入黑名单
        var blacklistKey = $"{CacheConstants.TOKEN_BLACKLIST_KEY}{jti}";
        var expirationTime = GetTokenExpirationTime(token);
        var ttl = expirationTime - DateTime.UtcNow;
        
        if (ttl > TimeSpan.Zero)
        {
            await _cacheService.SetAsync(blacklistKey, "revoked", ttl, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public long? GetUserIdFromToken(string token)
    {
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
                return userId;

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public string? GetUserNameFromToken(string token)
    {
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            var userNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName);
            return userNameClaim?.Value;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public string? GetJtiFromToken(string token)
    {
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            var jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
            return jtiClaim?.Value;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public DateTime? GetLoginTimeFromToken(string token)
    {
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            var iatClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat);
            if (iatClaim != null && long.TryParse(iatClaim.Value, out var iat))
            {
                return DateTimeOffset.FromUnixTimeSeconds(iat).UtcDateTime;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// 存储在线用户信息
    /// </summary>
    private async Task StoreOnlineUserAsync(long userId, string userName, string jti, CancellationToken cancellationToken)
    {
        // 查询用户部门信息
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        string? deptName = null;
        if (user?.DeptId != null)
        {
            var dept = await _deptRepository.GetByIdAsync(user.DeptId.Value, cancellationToken);
            deptName = dept?.DeptName;
        }

        // 获取真实IP地址
        var httpContext = _httpContextAccessor.HttpContext;
        var ipAddress = IpUtils.GetIpAddress(httpContext);
        var isInternalIp = IpUtils.IsInternalIp(ipAddress);
        var loginLocation = isInternalIp ? "内网IP" : "外网IP";

        // 获取User-Agent信息
        var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
        var browser = ParseBrowser(userAgent);
        var os = ParseOperatingSystem(userAgent);

        // 构建在线用户信息
        var onlineUser = new NetMVP.Application.DTOs.UserOnline.OnlineUserDto
        {
            TokenId = jti,
            UserId = userId,
            UserName = userName,
            DeptName = deptName,
            Ipaddr = ipAddress,
            LoginLocation = loginLocation,
            Browser = browser,
            Os = os,
            LoginTime = DateTimeOffset.Now.ToUnixTimeMilliseconds()
        };

        // 存储在线用户信息，使用 JTI 作为 Key
        var onlineUserKey = $"{CacheConstants.ONLINE_USER_KEY}{jti}";
        await _cacheService.SetAsync(
            onlineUserKey,
            onlineUser,
            TimeSpan.FromMinutes(_jwtSettings.ExpireMinutes),
            cancellationToken);

        // 存储用户当前有效的会话编号（单点登录关键）
        var userSessionKey = $"{CacheConstants.USER_SESSION_KEY}{userId}";
        await _cacheService.SetAsync(
            userSessionKey,
            jti,
            TimeSpan.FromMinutes(_jwtSettings.ExpireMinutes),
            cancellationToken);
        
        _logger.LogInformation($"用户 {userName}({userId}) 登录，JTI: {jti}");
    }

    /// <summary>
    /// 删除用户的旧会话（单点登录）
    /// </summary>
    private async Task RemoveUserOldSessionAsync(long userId, CancellationToken cancellationToken)
    {
        try
        {
            // 获取用户当前的会话编号
            var userSessionKey = $"{CacheConstants.USER_SESSION_KEY}{userId}";
            var oldJti = await _cacheService.GetAsync<string>(userSessionKey, cancellationToken);
            
            if (!string.IsNullOrEmpty(oldJti))
            {
                // 删除旧的在线用户信息
                var oldOnlineUserKey = $"{CacheConstants.ONLINE_USER_KEY}{oldJti}";
                await _cacheService.RemoveAsync(oldOnlineUserKey, cancellationToken);
                _logger.LogInformation($"删除用户 {userId} 的旧会话，JTI: {oldJti}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"删除用户 {userId} 的旧会话失败");
        }
    }

    /// <summary>
    /// 解析浏览器类型
    /// </summary>
    private static string ParseBrowser(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return "Unknown";

        if (userAgent.Contains("Edg/"))
            return "Edge";
        if (userAgent.Contains("Chrome/"))
            return "Chrome";
        if (userAgent.Contains("Firefox/"))
            return "Firefox";
        if (userAgent.Contains("Safari/") && !userAgent.Contains("Chrome"))
            return "Safari";
        if (userAgent.Contains("MSIE") || userAgent.Contains("Trident/"))
            return "IE";

        return "Unknown";
    }

    /// <summary>
    /// 解析操作系统
    /// </summary>
    private static string ParseOperatingSystem(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return "Unknown";

        if (userAgent.Contains("Windows NT 10.0"))
            return "Windows 10";
        if (userAgent.Contains("Windows NT 6.3"))
            return "Windows 8.1";
        if (userAgent.Contains("Windows NT 6.2"))
            return "Windows 8";
        if (userAgent.Contains("Windows NT 6.1"))
            return "Windows 7";
        if (userAgent.Contains("Windows"))
            return "Windows";
        if (userAgent.Contains("Mac OS X"))
            return "macOS";
        if (userAgent.Contains("Linux"))
            return "Linux";
        if (userAgent.Contains("Android"))
            return "Android";
        if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
            return "iOS";

        return "Unknown";
    }

    /// <summary>
    /// 获取 Token 过期时间
    /// </summary>
    private DateTime GetTokenExpirationTime(string token)
    {
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo;
        }
        catch
        {
            return DateTime.UtcNow;
        }
    }
}
