using NetMVP.Application.DTOs.Auth;

namespace NetMVP.Application.Services;

/// <summary>
/// 用户注册服务接口
/// </summary>
public interface IRegisterService
{
    /// <summary>
    /// 用户注册
    /// </summary>
    Task RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
}
