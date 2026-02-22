using Microsoft.EntityFrameworkCore;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Job;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 定时任务服务实现
/// </summary>
public class SysJobService : ISysJobService
{
    private readonly ISysJobRepository _jobRepository;
    private readonly ISchedulerService _schedulerService;
    private readonly IExcelService _excelService;
    private readonly IUnitOfWork _unitOfWork;

    public SysJobService(
        ISysJobRepository jobRepository,
        ISchedulerService schedulerService,
        IExcelService excelService,
        IUnitOfWork unitOfWork)
    {
        _jobRepository = jobRepository;
        _schedulerService = schedulerService;
        _excelService = excelService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<JobDto>> GetJobListAsync(JobQueryDto query, CancellationToken cancellationToken = default)
    {
        var queryable = _jobRepository.GetQueryable();

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

        var dtos = items.Select(x => new JobDto
        {
            JobId = x.JobId,
            JobName = x.JobName,
            JobGroup = x.JobGroup,
            InvokeTarget = x.InvokeTarget,
            CronExpression = x.CronExpression,
            MisfirePolicy = x.MisfirePolicy,
            Concurrent = x.Concurrent,
            Status = x.Status,
            Remark = x.Remark,
            CreateTime = x.CreateTime,
            NextValidTime = _schedulerService.GetNextFireTime(x.CronExpression)
        }).ToList();

        return new PagedResult<JobDto>
        {
            Rows = dtos,
            Total = total
        };
    }

    public async Task<JobDto?> GetJobByIdAsync(long jobId, CancellationToken cancellationToken = default)
    {
        var job = await _jobRepository.GetByJobIdAsync(jobId, cancellationToken);
        if (job == null) return null;

        return new JobDto
        {
            JobId = job.JobId,
            JobName = job.JobName,
            JobGroup = job.JobGroup,
            InvokeTarget = job.InvokeTarget,
            CronExpression = job.CronExpression,
            MisfirePolicy = job.MisfirePolicy,
            Concurrent = job.Concurrent,
            Status = job.Status,
            Remark = job.Remark,
            CreateTime = job.CreateTime,
            NextValidTime = _schedulerService.GetNextFireTime(job.CronExpression)
        };
    }

    public async Task<long> CreateJobAsync(CreateJobDto dto, CancellationToken cancellationToken = default)
    {
        // 验证Cron表达式
        if (!_schedulerService.ValidateCronExpression(dto.CronExpression))
        {
            throw new InvalidOperationException("Cron表达式格式不正确");
        }

        // 检查任务名称是否唯一
        if (!await _jobRepository.IsJobNameUniqueAsync(dto.JobName, dto.JobGroup, null, cancellationToken))
        {
            throw new InvalidOperationException($"任务名称 {dto.JobName} 已存在");
        }

        var job = new SysJob
        {
            JobName = dto.JobName,
            JobGroup = dto.JobGroup,
            InvokeTarget = dto.InvokeTarget,
            CronExpression = dto.CronExpression,
            MisfirePolicy = dto.MisfirePolicy,
            Concurrent = dto.Concurrent,
            Status = dto.Status,
            Remark = dto.Remark
        };

        await _jobRepository.AddAsync(job, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 如果任务状态为正常，则添加到调度器
        if (dto.Status == "0")
        {
            await _schedulerService.AddJobAsync(dto.JobName, dto.JobGroup, dto.CronExpression, null, cancellationToken);
        }

        return job.JobId;
    }

    public async Task UpdateJobAsync(UpdateJobDto dto, CancellationToken cancellationToken = default)
    {
        // 验证Cron表达式
        if (!_schedulerService.ValidateCronExpression(dto.CronExpression))
        {
            throw new InvalidOperationException("Cron表达式格式不正确");
        }

        var job = await _jobRepository.GetByJobIdAsync(dto.JobId, cancellationToken);
        if (job == null)
        {
            throw new InvalidOperationException($"任务不存在: {dto.JobId}");
        }

        // 检查任务名称是否唯一
        if (!await _jobRepository.IsJobNameUniqueAsync(dto.JobName, dto.JobGroup, dto.JobId, cancellationToken))
        {
            throw new InvalidOperationException($"任务名称 {dto.JobName} 已存在");
        }

        // 如果任务名称或组名改变，需要先删除旧任务
        if (job.JobName != dto.JobName || job.JobGroup != dto.JobGroup)
        {
            try
            {
                await _schedulerService.DeleteJobAsync(job.JobName, job.JobGroup, cancellationToken);
            }
            catch
            {
                // 忽略删除失败（任务可能不存在）
            }
        }

        // 更新任务信息
        job.JobName = dto.JobName;
        job.JobGroup = dto.JobGroup;
        job.InvokeTarget = dto.InvokeTarget;
        job.CronExpression = dto.CronExpression;
        job.MisfirePolicy = dto.MisfirePolicy;
        job.Concurrent = dto.Concurrent;
        job.Status = dto.Status;
        job.Remark = dto.Remark;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 更新调度器中的任务
        if (dto.Status == "0")
        {
            try
            {
                await _schedulerService.UpdateJobAsync(dto.JobName, dto.JobGroup, dto.CronExpression, null, cancellationToken);
            }
            catch
            {
                // 如果更新失败，尝试添加
                await _schedulerService.AddJobAsync(dto.JobName, dto.JobGroup, dto.CronExpression, null, cancellationToken);
            }
        }
        else
        {
            // 如果状态为暂停，从调度器中删除
            try
            {
                await _schedulerService.DeleteJobAsync(dto.JobName, dto.JobGroup, cancellationToken);
            }
            catch
            {
                // 忽略删除失败
            }
        }
    }

    public async Task DeleteJobAsync(long jobId, CancellationToken cancellationToken = default)
    {
        var job = await _jobRepository.GetByJobIdAsync(jobId, cancellationToken);
        if (job == null)
        {
            throw new InvalidOperationException($"任务不存在: {jobId}");
        }

        // 从调度器中删除
        try
        {
            await _schedulerService.DeleteJobAsync(job.JobName, job.JobGroup, cancellationToken);
        }
        catch
        {
            // 忽略删除失败
        }

        // 从数据库中删除
        await _jobRepository.DeleteAsync(job, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeJobStatusAsync(long jobId, string status, CancellationToken cancellationToken = default)
    {
        var job = await _jobRepository.GetByJobIdAsync(jobId, cancellationToken);
        if (job == null)
        {
            throw new InvalidOperationException($"任务不存在: {jobId}");
        }

        job.Status = status;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 更新调度器状态
        if (status == "0")
        {
            // 恢复任务
            try
            {
                await _schedulerService.ResumeJobAsync(job.JobName, job.JobGroup, cancellationToken);
            }
            catch
            {
                // 如果恢复失败，尝试添加
                await _schedulerService.AddJobAsync(job.JobName, job.JobGroup, job.CronExpression, null, cancellationToken);
            }
        }
        else
        {
            // 暂停任务
            try
            {
                await _schedulerService.PauseJobAsync(job.JobName, job.JobGroup, cancellationToken);
            }
            catch
            {
                // 忽略暂停失败
            }
        }
    }

    public async Task RunJobAsync(long jobId, string jobGroup, CancellationToken cancellationToken = default)
    {
        var job = await _jobRepository.GetByJobIdAsync(jobId, cancellationToken);
        if (job == null)
        {
            throw new InvalidOperationException($"任务不存在: {jobId}");
        }

        await _schedulerService.TriggerJobAsync(job.JobName, job.JobGroup, cancellationToken);
    }

    public async Task<byte[]> ExportJobsAsync(JobQueryDto query, CancellationToken cancellationToken = default)
    {
        var queryable = _jobRepository.GetQueryable();

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

        var jobs = await queryable.OrderByDescending(x => x.CreateTime).ToListAsync(cancellationToken);

        var data = jobs.Select(x => new
        {
            任务ID = x.JobId,
            任务名称 = x.JobName,
            任务组名 = x.JobGroup,
            调用目标 = x.InvokeTarget,
            Cron表达式 = x.CronExpression,
            执行策略 = x.MisfirePolicy,
            并发执行 = x.Concurrent == "0" ? "允许" : "禁止",
            状态 = x.Status == "0" ? "正常" : "暂停",
            备注 = x.Remark,
            创建时间 = x.CreateTime
        }).ToList();

        using var stream = new MemoryStream();
        await _excelService.ExportAsync(data, stream, cancellationToken);
        return stream.ToArray();
    }
}
