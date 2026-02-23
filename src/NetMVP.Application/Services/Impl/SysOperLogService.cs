using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetMVP.Application.DTOs.OperLog;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 操作日志服务实现
/// </summary>
public class SysOperLogService : ISysOperLogService
{
    private readonly ISysOperLogRepository _operLogRepository;
    private readonly IExcelService _excelService;
    private readonly IMapper _mapper;

    public SysOperLogService(
        ISysOperLogRepository operLogRepository,
        IExcelService excelService,
        IMapper mapper)
    {
        _operLogRepository = operLogRepository;
        _excelService = excelService;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<(List<OperLogDto> logs, int total)> GetOperLogListAsync(
        OperLogQueryDto query, 
        CancellationToken cancellationToken = default)
    {
        var queryable = _operLogRepository.GetQueryable();

        // 过滤条件
        if (!string.IsNullOrWhiteSpace(query.Title))
        {
            queryable = queryable.Where(x => x.Title != null && x.Title.Contains(query.Title));
        }

        if (!string.IsNullOrWhiteSpace(query.BusinessType))
        {
            queryable = queryable.Where(x => x.BusinessType == query.BusinessType);
        }

        if (!string.IsNullOrWhiteSpace(query.OperName))
        {
            queryable = queryable.Where(x => x.OperName != null && x.OperName.Contains(query.OperName));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(x => x.Status == query.Status);
        }

        if (query.BeginTime.HasValue)
        {
            queryable = queryable.Where(x => x.OperTime >= query.BeginTime.Value);
        }

        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where(x => x.OperTime <= query.EndTime.Value);
        }

        // 总数
        var total = await queryable.CountAsync(cancellationToken);

        // 分页查询
        var logs = await queryable
            .OrderByDescending(x => x.OperTime)
            .Skip((query.PageNum - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var logDtos = _mapper.Map<List<OperLogDto>>(logs);
        return (logDtos, total);
    }

    /// <inheritdoc/>
    public async Task<OperLogDto?> GetOperLogByIdAsync(long operId, CancellationToken cancellationToken = default)
    {
        var log = await Task.Run(() => _operLogRepository.GetQueryable()
            .FirstOrDefault(x => x.OperId == operId), cancellationToken);
        return log == null ? null : _mapper.Map<OperLogDto>(log);
    }

    /// <inheritdoc/>
    public async Task<long> CreateOperLogAsync(CreateOperLogDto dto, CancellationToken cancellationToken = default)
    {
        var log = new SysOperLog
        {
            Title = dto.Title,
            BusinessType = dto.BusinessType,
            Method = dto.Method,
            RequestMethod = dto.RequestMethod,
            OperatorType = dto.OperatorType,
            OperName = dto.OperName,
            DeptName = dto.DeptName,
            OperUrl = dto.OperUrl,
            OperIpValue = dto.OperIp,
            OperLocation = dto.OperLocation,
            OperParam = dto.OperParam,
            JsonResult = dto.JsonResult,
            Status = dto.Status,
            ErrorMsg = dto.ErrorMsg,
            OperTime = DateTime.Now,
            CostTime = dto.CostTime
        };

        await _operLogRepository.AddAsync(log, cancellationToken);
        return log.OperId;
    }

    /// <inheritdoc/>
    public async Task DeleteOperLogAsync(long operId, CancellationToken cancellationToken = default)
    {
        var log = await Task.Run(() => _operLogRepository.GetQueryable()
            .FirstOrDefault(x => x.OperId == operId), cancellationToken);
        if (log != null)
        {
            await _operLogRepository.DeleteAsync(log, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task DeleteOperLogsAsync(long[] operIds, CancellationToken cancellationToken = default)
    {
        foreach (var operId in operIds)
        {
            await DeleteOperLogAsync(operId, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task CleanOperLogAsync(CancellationToken cancellationToken = default)
    {
        await _operLogRepository.CleanAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<byte[]> ExportOperLogsAsync(OperLogQueryDto query, CancellationToken cancellationToken = default)
    {
        // 获取所有数据（不分页）
        var queryable = _operLogRepository.GetQueryable();

        // 应用过滤条件
        if (!string.IsNullOrWhiteSpace(query.Title))
        {
            queryable = queryable.Where(x => x.Title != null && x.Title.Contains(query.Title));
        }

        if (!string.IsNullOrWhiteSpace(query.BusinessType))
        {
            queryable = queryable.Where(x => x.BusinessType == query.BusinessType);
        }

        if (!string.IsNullOrWhiteSpace(query.OperName))
        {
            queryable = queryable.Where(x => x.OperName != null && x.OperName.Contains(query.OperName));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(x => x.Status == query.Status);
        }

        if (query.BeginTime.HasValue)
        {
            queryable = queryable.Where(x => x.OperTime >= query.BeginTime.Value);
        }

        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where(x => x.OperTime <= query.EndTime.Value);
        }

        var logs = await queryable
            .OrderByDescending(x => x.OperTime)
            .ToListAsync(cancellationToken);

        var exportDtos = _mapper.Map<List<ExportOperLogDto>>(logs);

        // 导出Excel
        using var stream = new MemoryStream();
        await _excelService.ExportAsync(exportDtos, stream, cancellationToken);
        return stream.ToArray();
    }
}
