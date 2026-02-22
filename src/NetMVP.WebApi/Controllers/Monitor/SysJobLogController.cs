using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.DTOs.Job;
using NetMVP.Application.Services;

namespace NetMVP.WebApi.Controllers.Monitor;

/// <summary>
/// 定时任务日志控制器
/// </summary>
[ApiController]
[Route("monitor/jobLog")]
public class SysJobLogController : BaseController
{
    private readonly ISysJobLogService _jobLogService;

    public SysJobLogController(ISysJobLogService jobLogService)
    {
        _jobLogService = jobLogService;
    }

    /// <summary>
    /// 获取任务日志列表
    /// </summary>
    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] JobLogQueryDto query)
    {
        var result = await _jobLogService.GetJobLogListAsync(query);
        return Ok(Success(result));
    }

    /// <summary>
    /// 获取任务日志详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var jobLog = await _jobLogService.GetJobLogByIdAsync(id);
        if (jobLog == null)
        {
            return Ok(Error($"任务日志不存在: {id}"));
        }
        return Ok(Success(jobLog));
    }

    /// <summary>
    /// 删除任务日志
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _jobLogService.DeleteJobLogAsync(id);
            return Ok(Success());
        }
        catch (Exception ex)
        {
            return Ok(Error(ex.Message));
        }
    }

    /// <summary>
    /// 清空任务日志
    /// </summary>
    [HttpDelete("clean")]
    public async Task<IActionResult> Clean()
    {
        try
        {
            await _jobLogService.CleanJobLogAsync();
            return Ok(Success());
        }
        catch (Exception ex)
        {
            return Ok(Error(ex.Message));
        }
    }

    /// <summary>
    /// 导出任务日志
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] JobLogQueryDto query)
    {
        var data = await _jobLogService.ExportJobLogsAsync(query);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "job_logs.xlsx");
    }
}
