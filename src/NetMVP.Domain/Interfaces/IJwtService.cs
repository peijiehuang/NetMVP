namespace NetMVP.Domain.Interfaces;

/// <summary>
/// JWT 服务接口
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// 生成 Token
    /// </summary>
    Task<(string accessToken, string refreshToken)> GenerateTokenAsync(long userId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证 Token
    /// </summary>
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刷新 Token
    /// </summary>
    Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销 Token
    /// </summary>
    Task RevokeTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// 从 Token 获取用户 ID
    /// </summary>
    long? GetUserIdFromToken(string token);

    /// <summary>
    /// 从 Token 获取用户名
    /// </summary>
    string? GetUserNameFromToken(string token);

    /// <summary>
    /// 从 Token 获取 JTI
    /// </summary>
    string? GetJtiFromToken(string token);

    /// <summary>
    /// 从 Token 获取登录时间
    /// </summary>
    DateTime? GetLoginTimeFromToken(string token);
}
