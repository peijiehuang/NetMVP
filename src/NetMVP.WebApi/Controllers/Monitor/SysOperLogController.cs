using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.OperLog;
using NetMVP.Application.Services;

namespace NetMVP.WebApi.Controllers.Monitor;

/// <summary>
/// 操作日志控制器
/// </summary>
[ApiController]
[Route("monitor/operlog")]
[Authorize]
public class SysOperLogController : ControllerBase
{
    private readonly ISysOperLogService _operLogService;

    public SysOperLogController(ISysOperLogService operLogService)
    {
        _operLogService = operLogService;
    }

    /// <summary>
    /// 获取操作日志列表
    /// </summary>
    [HttpGet("list")]
    public async Task<TableDataInfo> GetList([FromQuery] OperLogQueryDto query)
    {
        var (logs, total) = await _operLogService.GetOperLogListAsync(query);
        return TableDataInfo.Build(logs, total);
    }

    /// <summary>
    /// 删除操作日志
    /// </summary>
    [HttpDelete("{operIds}")]
    public async Task<AjaxResult> Delete(string operIds)
    {
        var ids = operIds.Split(',').Select(long.Parse).ToArray();
        await _operLogService.DeleteOperLogsAsync(ids);
        return AjaxResult.Success();
    }

    /// <summary>
    /// 清空操作日志
    /// </summary>
    [HttpDelete("clean")]
    public async Task<AjaxResult> Clean()
    {
        await _operLogService.CleanOperLogAsync();
        return AjaxResult.Success();
    }

    /// <summary>
    /// 导出操作日志
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> Export([FromForm] OperLogQueryDto query)
    {
        var data = await _operLogService.ExportOperLogsAsync(query);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "操作日志.xlsx");
    }
}
