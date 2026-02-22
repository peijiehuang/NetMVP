using NetMVP.Application.DTOs.Server;

namespace NetMVP.Application.Services;

/// <summary>
/// 服务器监控服务接口
/// </summary>
public interface IServerMonitorService
{
    /// <summary>
    /// 获取服务器信息
    /// </summary>
    Task<ServerInfoDto> GetServerInfoAsync(CancellationToken cancellationToken = default);
}
