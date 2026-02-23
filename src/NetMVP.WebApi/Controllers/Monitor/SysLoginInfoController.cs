using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.LoginInfo;
using NetMVP.Application.Services;
using NetMVP.WebApi.Attributes;
using NetMVP.Domain.Enums;

namespace NetMVP.WebApi.Controllers.Monitor;

/// <summary>
/// 登录日志控制器
/// </summary>
[Route("monitor/logininfor")]
public class SysLoginInfoController : BaseController
{
    private readonly ISysLoginInfoService _loginInfoService;

    public SysLoginInfoController(ISysLoginInfoService loginInfoService)
    {
        _loginInfoService = loginInfoService;
    }

    /// <summary>
    /// 获取登录日志列表
    /// </summary>
    [HttpGet("list")]
    public async Task<AjaxResult> GetList([FromQuery] LoginInfoQueryDto query)
    {
        var (list, total) = await _loginInfoService.GetLoginInfoListAsync(query);
        return Success(TableDataInfo.Build(list, total));
    }

    /// <summary>
    /// 删除登录日志
    /// </summary>
    [HttpDelete("{infoIds}")]
    [Log(Title = "登录日志", BusinessType = BusinessType.Delete)]
    public async Task<AjaxResult> Delete(string infoIds)
    {
        var ids = infoIds.Split(',').Select(long.Parse).ToArray();
        var count = await _loginInfoService.DeleteLoginInfosAsync(ids);
        return Success($"删除成功 {count} 条记录");
    }

    /// <summary>
    /// 清空登录日志
    /// </summary>
    [HttpDelete("clean")]
    [Log(Title = "登录日志", BusinessType = BusinessType.Clean)]
    public async Task<AjaxResult> Clean()
    {
        var count = await _loginInfoService.CleanLoginInfoAsync();
        return Success($"清空成功 {count} 条记录");
    }

    /// <summary>
    /// 解锁用户
    /// </summary>
    [HttpPut("unlock/{userName}")]
    [Log(Title = "账户解锁", BusinessType = BusinessType.Other)]
    public async Task<AjaxResult> Unlock(string userName)
    {
        await _loginInfoService.UnlockUserAsync(userName);
        return Success($"用户 {userName} 解锁成功");
    }

    /// <summary>
    /// 导出登录日志
    /// </summary>
    [HttpPost("export")]
    [Log(Title = "登录日志", BusinessType = BusinessType.Export)]
    public async Task<IActionResult> Export([FromForm] LoginInfoQueryDto query)
    {
        var data = await _loginInfoService.ExportLoginInfosAsync(query);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "登录日志.xlsx");
    }
}
