using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.UserOnline;
using NetMVP.Application.Services;

namespace NetMVP.WebApi.Controllers.Monitor;

/// <summary>
/// 在线用户监控
/// </summary>
[ApiController]
[Route("monitor/online")]
[Authorize]
public class SysUserOnlineController : ControllerBase
{
    private readonly ISysUserOnlineService _userOnlineService;

    public SysUserOnlineController(ISysUserOnlineService userOnlineService)
    {
        _userOnlineService = userOnlineService;
    }

    /// <summary>
    /// 获取在线用户列表
    /// </summary>
    [HttpGet("list")]
    public async Task<TableDataInfo> GetList([FromQuery] OnlineUserQueryDto query)
    {
        var (users, total) = await _userOnlineService.GetOnlineUserListAsync(query);
        return TableDataInfo.Build(users, total);
    }

    /// <summary>
    /// 强制下线
    /// </summary>
    [HttpDelete("{tokenId}")]
    public async Task<AjaxResult> ForceLogout(string tokenId)
    {
        await _userOnlineService.ForceLogoutAsync(tokenId);
        return AjaxResult.Success();
    }
}
