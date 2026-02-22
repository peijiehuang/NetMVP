using Microsoft.EntityFrameworkCore;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Job;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 定时任务日志服务实现
/// </summary>
public class SysJobLogService : ISysJobLogService
{
    private readonly ISysJobLogRepository _jobLogRepository;
    private readonly IExcelService _excelService;
    private readonly IUnitOfWork _unitOfWork;

    public SysJobLogService(
        ISysJobLogRepository jobLogRepository,
        IExcelService excelService,
        IUnitOfWork unitOfWork)
    {
        _jobLogRepository = jobLogRepository;
        _excelService = excelService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<JobLogDto>> GetJobLogListAsync(JobLogQueryDto query, CancellationToken cancellationToken = default)
    {
        var queryable = _jobLogRepository.GetQueryable();

        // 条件过滤
        if (!string.IsNullOrWhiteSpace(query.JobName))
        {
            queryable = queryable.Where(x => x.JobName.Contains(query.JobName));
        }

        if (!string.IsNullOrWhiteSpace(query.JobGroup))
        {
            queryable = queryable.Where(x => x.JobGroup == query.JobGroup);
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(x => x.Status == query.Status);
        }

        // 排序
        queryable = queryable.OrderByDescending(x => x.CreateTime);

        // 分页
        var total = await queryable.CountAsync(cancellationToken);
        var items = await queryable
            .Skip((query.PageNum - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(x => new JobLogDto
        {
            JobLogId = x.JobLogId,
            JobName = x.JobName,
            JobGroup = x.JobGroup,
            InvokeTarget = x.InvokeTarget,
            JobMessage = x.JobMessage,
            Status = x.Status,
            ExceptionInfo = x.ExceptionInfo,
            CreateTime = x.CreateTime ?? DateTime.Now
        }).ToList();

        return new PagedResult<JobLogDto>
        {
            Rows = dtos,
            Total = total
        };
    }

    public async Task<JobLogDto?> GetJobLogByIdAsync(long jobLogId, CancellationToken cancellationToken = default)
    {
        var jobLog = await _jobLogRepository.GetByJobLogIdAsync(jobLogId, cancellationToken);
        if (jobLog == null) return null;

        return new JobLogDto
        {
            JobLogId = jobLog.JobLogId,
            JobName = jobLog.JobName,
            JobGroup = jobLog.JobGroup,
            InvokeTarget = jobLog.InvokeTarget,
            JobMessage = jobLog.JobMessage,
            Status = jobLog.Status,
            ExceptionInfo = jobLog.ExceptionInfo,
            CreateTime = jobLog.CreateTime ?? DateTime.Now
        };
    }

    public async Task DeleteJobLogAsync(long jobLogId, CancellationToken cancellationToken = default)
    {
        var jobLog = await _jobLogRepository.GetByJobLogIdAsync(jobLogId, cancellationToken);
        if (jobLog == null)
        {
            throw new InvalidOperationException($"任务日志不存在: {jobLogId}");
        }

        await _jobLogRepository.DeleteAsync(jobLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task CleanJobLogAsync(CancellationToken cancellationToken = default)
    {
        await _jobLogRepository.CleanJobLogAsync(cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<byte[]> ExportJobLogsAsync(JobLogQueryDto query, CancellationToken cancellationToken = default)
    {
        var queryable = _jobLogRepository.GetQueryable();

        // 条件过滤
        if (!string.IsNullOrWhiteSpace(query.JobName))
        {
            queryable = queryable.Where(x => x.JobName.Contains(query.JobName));
        }

        if (!string.IsNullOrWhiteSpace(query.JobGroup))
        {
            queryable = queryable.Where(x => x.JobGroup == query.JobGroup);
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(x => x.Status == query.Status);
        }

        var jobLogs = await queryable.OrderByDescending(x => x.CreateTime).ToListAsync(cancellationToken);

        var data = jobLogs.Select(x => new
        {
            日志ID = x.JobLogId,
            任务名称 = x.JobName,
            任务组名 = x.JobGroup,
            调用目标 = x.InvokeTarget,
            日志信息 = x.JobMessage,
            状态 = x.Status == "0" ? "正常" : "失败",
            异常信息 = x.ExceptionInfo,
            创建时间 = x.CreateTime
        }).ToList();

        using var stream = new MemoryStream();
        await _excelService.ExportAsync(data, stream, cancellationToken);
        return stream.ToArray();
    }

    public async Task AddJobLogAsync(string jobName, string jobGroup, string invokeTarget, string jobMessage, string status, string? exceptionInfo = null, CancellationToken cancellationToken = default)
    {
        var jobLog = new SysJobLog
        {
            JobName = jobName,
            JobGroup = jobGroup,
            InvokeTarget = invokeTarget,
            JobMessage = jobMessage,
            Status = status,
            ExceptionInfo = exceptionInfo,
            CreateTime = DateTime.Now
        };

        await _jobLogRepository.AddAsync(jobLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
