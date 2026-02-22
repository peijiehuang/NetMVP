using Microsoft.Extensions.Options;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Configuration;

namespace NetMVP.Infrastructure.Services.Auth;

/// <summary>
/// 认证服务实现
/// </summary>
public class AuthService : IAuthService
{
    private readonly ISysUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly ICaptchaService _captchaService;
    private readonly SystemSettings _systemSettings;

    public AuthService(
        ISysUserRepository userRepository,
        IJwtService jwtService,
        ICaptchaService captchaService,
        IOptions<SystemSettings> systemSettings)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _captchaService = captchaService;
        _systemSettings = systemSettings.Value;
    }

    /// <inheritdoc/>
    public async Task<(string accessToken, string refreshToken)> LoginAsync(
        string userName, 
        string password, 
        string? code, 
        string? uuid, 
        CancellationToken cancellationToken = default)
    {
        // 验证码校验
        if (_systemSettings.CaptchaEnabled)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(uuid))
                throw new UnauthorizedAccessException("验证码不能为空");

            var isValidCaptcha = await _captchaService.ValidateCaptchaAsync(uuid, code, cancellationToken);
            if (!isValidCaptcha)
                throw new UnauthorizedAccessException("验证码错误");
        }

        // 查询用户
        var user = await _userRepository.GetByUserNameAsync(userName, cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException("用户名或密码错误");

        // 验证密码
        if (!user.VerifyPassword(password))
            throw new UnauthorizedAccessException("用户名或密码错误");

        // 检查用户状态
        if (user.Status == UserStatus.Disabled)
            throw new UnauthorizedAccessException("用户已被停用");

        if (user.DelFlag == DelFlag.Deleted)
            throw new UnauthorizedAccessException("用户已被删除");

        // 生成 Token
        var (accessToken, refreshToken) = await _jwtService.GenerateTokenAsync(
            user.UserId, 
            user.UserName, 
            cancellationToken);

        // 更新登录信息（这里简化处理，实际应该记录 IP 等信息）
        user.UpdateLoginInfo("127.0.0.1");
        await _userRepository.UpdateAsync(user, cancellationToken);

        return (accessToken, refreshToken);
    }

    /// <inheritdoc/>
    public async Task LogoutAsync(string token, CancellationToken cancellationToken = default)
    {
        // 将 Token 加入黑名单
        await _jwtService.RevokeTokenAsync(token, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<(string uuid, string image, bool captchaEnabled)> GetCaptchaAsync(CancellationToken cancellationToken = default)
    {
        if (!_systemSettings.CaptchaEnabled)
        {
            return (string.Empty, string.Empty, false);
        }

        var (uuid, image) = await _captchaService.GenerateCaptchaAsync(cancellationToken);
        return (uuid, image, true);
    }
}
