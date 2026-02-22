using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.DTOs.Job;
using NetMVP.Application.Services;

namespace NetMVP.WebApi.Controllers.Monitor;

/// <summary>
/// 定时任务控制器
/// </summary>
[ApiController]
[Route("monitor/job")]
public class SysJobController : BaseController
{
    private readonly ISysJobService _jobService;

    public SysJobController(ISysJobService jobService)
    {
        _jobService = jobService;
    }

    /// <summary>
    /// 获取任务列表
    /// </summary>
    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] JobQueryDto query)
    {
        var result = await _jobService.GetJobListAsync(query);
        return Ok(Success(result));
    }

    /// <summary>
    /// 获取任务详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var job = await _jobService.GetJobByIdAsync(id);
        if (job == null)
        {
            return Ok(Error($"任务不存在: {id}"));
        }
        return Ok(Success(job));
    }

    /// <summary>
    /// 创建任务
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJobDto dto)
    {
        try
        {
            var jobId = await _jobService.CreateJobAsync(dto);
            return Ok(Success(jobId));
        }
        catch (Exception ex)
        {
            return Ok(Error(ex.Message));
        }
    }

    /// <summary>
    /// 更新任务
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateJobDto dto)
    {
        try
        {
            await _jobService.UpdateJobAsync(dto);
            return Ok(Success());
        }
        catch (Exception ex)
        {
            return Ok(Error(ex.Message));
        }
    }

    /// <summary>
    /// 删除任务
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _jobService.DeleteJobAsync(id);
            return Ok(Success());
        }
        catch (Exception ex)
        {
            return Ok(Error(ex.Message));
        }
    }

    /// <summary>
    /// 修改任务状态
    /// </summary>
    [HttpPut("changeStatus")]
    public async Task<IActionResult> ChangeStatus([FromBody] ChangeJobStatusRequest request)
    {
        try
        {
            await _jobService.ChangeJobStatusAsync(request.JobId, request.Status);
            return Ok(Success());
        }
        catch (Exception ex)
        {
            return Ok(Error(ex.Message));
        }
    }

    /// <summary>
    /// 立即执行任务
    /// </summary>
    [HttpPut("run")]
    public async Task<IActionResult> Run([FromBody] RunJobRequest request)
    {
        try
        {
            await _jobService.RunJobAsync(request.JobId, request.JobGroup);
            return Ok(Success());
        }
        catch (Exception ex)
        {
            return Ok(Error(ex.Message));
        }
    }

    /// <summary>
    /// 导出任务
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] JobQueryDto query)
    {
        var data = await _jobService.ExportJobsAsync(query);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "jobs.xlsx");
    }
}

/// <summary>
/// 修改任务状态请求
/// </summary>
public class ChangeJobStatusRequest
{
    public long JobId { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// 立即执行任务请求
/// </summary>
public class RunJobRequest
{
    public long JobId { get; set; }
    public string JobGroup { get; set; } = string.Empty;
}
