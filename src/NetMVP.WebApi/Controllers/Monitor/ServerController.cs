using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.Services;
using NetMVP.WebApi.Attributes;

namespace NetMVP.WebApi.Controllers.Monitor;

/// <summary>
/// 服务监控控制器
/// </summary>
[Route("monitor/server")]
[Authorize]
public class ServerController : BaseController
{
    private readonly IServerMonitorService _serverMonitorService;

    public ServerController(IServerMonitorService serverMonitorService)
    {
        _serverMonitorService = serverMonitorService;
    }

    /// <summary>
    /// 获取服务器信息
    /// </summary>
    [HttpGet]
    [RequirePermission("monitor:server:list")]
    public async Task<AjaxResult> GetServerInfo()
    {
        var serverInfo = await _serverMonitorService.GetServerInfoAsync();
        return Success(serverInfo);
    }
}
