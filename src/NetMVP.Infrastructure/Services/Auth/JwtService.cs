using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Configuration;
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

    public JwtService(
        IOptions<JwtSettings> jwtSettings, 
        ICacheService cacheService,
        ISysUserRepository userRepository,
        ISysDeptRepository deptRepository)
    {
        _jwtSettings = jwtSettings.Value;
        _cacheService = cacheService;
        _tokenHandler = new JwtSecurityTokenHandler();
        _userRepository = userRepository;
        _deptRepository = deptRepository;
    }

    /// <inheritdoc/>
    public async Task<(string accessToken, string refreshToken)> GenerateTokenAsync(
        long userId, 
        string userName, 
        CancellationToken cancellationToken = default)
    {
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
        var refreshTokenKey = $"refresh_token:{userId}:{refreshToken}";
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
                var blacklistKey = $"token_blacklist:{jti}";
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
        var keys = await _cacheService.GetKeysAsync($"refresh_token:*:{refreshToken}", cancellationToken);
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
        var blacklistKey = $"token_blacklist:{jti}";
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
        // 构建在线用户信息（简化版本，不查询数据库）
        var onlineUser = new
        {
            tokenId = jti,
            userId = userId,
            userName = userName,
            deptName = (string?)null,
            ipaddr = "127.0.0.1", // 实际应该从 HttpContext 获取
            loginLocation = "内网IP",
            browser = "Unknown",
            os = "Unknown",
            loginTime = DateTime.Now
        };

        // 存储到 Redis，过期时间与 Token 一致
        var onlineUserKey = $"online_user:{jti}";
        await _cacheService.SetAsync(
            onlineUserKey,
            onlineUser,
            TimeSpan.FromMinutes(_jwtSettings.ExpireMinutes),
            cancellationToken);
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
