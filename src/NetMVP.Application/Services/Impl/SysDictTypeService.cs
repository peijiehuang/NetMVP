using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetMVP.Application.DTOs.Dict;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 字典类型服务实现
/// </summary>
public class SysDictTypeService : ISysDictTypeService
{
    private readonly IRepository<SysDictType> _dictTypeRepository;
    private readonly IRepository<SysDictData> _dictDataRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExcelService _excelService;
    private readonly ICacheService _cacheService;
    private const string DictCacheKeyPrefix = "dict:";

    public SysDictTypeService(
        IRepository<SysDictType> dictTypeRepository,
        IRepository<SysDictData> dictDataRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IExcelService excelService,
        ICacheService cacheService)
    {
        _dictTypeRepository = dictTypeRepository;
        _dictDataRepository = dictDataRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _excelService = excelService;
        _cacheService = cacheService;
    }

    /// <summary>
    /// 获取字典类型列表
    /// </summary>
    public async Task<(List<DictTypeDto> dictTypes, int total)> GetDictTypeListAsync(DictTypeQueryDto query, CancellationToken cancellationToken = default)
    {
        var queryable = _dictTypeRepository.GetQueryable();

        // 字典名称
        if (!string.IsNullOrWhiteSpace(query.DictName))
        {
            queryable = queryable.Where(d => d.DictName.Contains(query.DictName));
        }

        // 字典类型
        if (!string.IsNullOrWhiteSpace(query.DictType))
        {
            queryable = queryable.Where(d => d.DictType.Contains(query.DictType));
        }

        // 状态
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(d => d.Status == query.Status);
        }

        // 总数
        var total = await queryable.CountAsync(cancellationToken);

        // 分页
        var dictTypes = await queryable
            .OrderBy(d => d.DictId)
            .Skip((query.PageNum - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var dictTypeDtos = _mapper.Map<List<DictTypeDto>>(dictTypes);

        return (dictTypeDtos, total);
    }

    /// <summary>
    /// 根据ID获取字典类型
    /// </summary>
    public async Task<DictTypeDto?> GetDictTypeByIdAsync(long dictId, CancellationToken cancellationToken = default)
    {
        var dictType = await _dictTypeRepository.GetByIdAsync(dictId, cancellationToken);
        return dictType == null ? null : _mapper.Map<DictTypeDto>(dictType);
    }

    /// <summary>
    /// 创建字典类型
    /// </summary>
    public async Task<long> CreateDictTypeAsync(CreateDictTypeDto dto, CancellationToken cancellationToken = default)
    {
        // 检查字典类型唯一性
        if (!await CheckDictTypeUniqueAsync(dto.DictType, null, cancellationToken))
        {
            throw new InvalidOperationException($"字典类型'{dto.DictType}'已存在");
        }

        var dictType = new SysDictType
        {
            DictName = dto.DictName,
            DictType = dto.DictType,
            Status = dto.Status,
            Remark = dto.Remark
        };

        await _dictTypeRepository.AddAsync(dictType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return dictType.DictId;
    }

    /// <summary>
    /// 更新字典类型
    /// </summary>
    public async Task UpdateDictTypeAsync(UpdateDictTypeDto dto, CancellationToken cancellationToken = default)
    {
        var dictType = await _dictTypeRepository.GetByIdAsync(dto.DictId, cancellationToken);
        if (dictType == null)
        {
            throw new InvalidOperationException("字典类型不存在");
        }

        // 检查字典类型唯一性
        if (!await CheckDictTypeUniqueAsync(dto.DictType, dto.DictId, cancellationToken))
        {
            throw new InvalidOperationException($"字典类型'{dto.DictType}'已存在");
        }

        dictType.DictName = dto.DictName;
        dictType.DictType = dto.DictType;
        dictType.Status = dto.Status;
        dictType.Remark = dto.Remark;

        await _dictTypeRepository.UpdateAsync(dictType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除缓存
        await _cacheService.RemoveAsync($"{DictCacheKeyPrefix}{dto.DictType}", cancellationToken);
    }

    /// <summary>
    /// 删除字典类型
    /// </summary>
    public async Task DeleteDictTypeAsync(long dictId, CancellationToken cancellationToken = default)
    {
        var dictType = await _dictTypeRepository.GetByIdAsync(dictId, cancellationToken);
        if (dictType == null)
        {
            throw new InvalidOperationException("字典类型不存在");
        }

        // 检查是否有字典数据使用该类型
        var hasDictData = await _dictDataRepository.GetQueryable()
            .AnyAsync(d => d.DictType == dictType.DictType, cancellationToken);

        if (hasDictData)
        {
            throw new InvalidOperationException("该字典类型下存在字典数据，不能删除");
        }

        await _dictTypeRepository.DeleteAsync(dictType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除缓存
        await _cacheService.RemoveAsync($"{DictCacheKeyPrefix}{dictType.DictType}", cancellationToken);
    }

    /// <summary>
    /// 批量删除字典类型
    /// </summary>
    public async Task DeleteDictTypesAsync(long[] dictIds, CancellationToken cancellationToken = default)
    {
        foreach (var dictId in dictIds)
        {
            await DeleteDictTypeAsync(dictId, cancellationToken);
        }
    }

    /// <summary>
    /// 刷新字典缓存
    /// </summary>
    public async Task RefreshDictCacheAsync(CancellationToken cancellationToken = default)
    {
        // 清除所有字典缓存
        await _cacheService.RemoveByPrefixAsync(DictCacheKeyPrefix, cancellationToken);
    }

    /// <summary>
    /// 导出字典类型
    /// </summary>
    public async Task<byte[]> ExportDictTypesAsync(DictTypeQueryDto query, CancellationToken cancellationToken = default)
    {
        // 获取所有数据（不分页）
        var queryable = _dictTypeRepository.GetQueryable();

        // 字典名称
        if (!string.IsNullOrWhiteSpace(query.DictName))
        {
            queryable = queryable.Where(d => d.DictName.Contains(query.DictName));
        }

        // 字典类型
        if (!string.IsNullOrWhiteSpace(query.DictType))
        {
            queryable = queryable.Where(d => d.DictType.Contains(query.DictType));
        }

        // 状态
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(d => d.Status == query.Status);
        }

        var dictTypes = await queryable
            .OrderBy(d => d.DictId)
            .ToListAsync(cancellationToken);

        var exportDtos = _mapper.Map<List<ExportDictTypeDto>>(dictTypes);

        // 导出到内存流
        using var stream = new MemoryStream();
        await _excelService.ExportAsync(exportDtos, stream, cancellationToken);
        return stream.ToArray();
    }

    /// <summary>
    /// 检查字典类型唯一性
    /// </summary>
    public async Task<bool> CheckDictTypeUniqueAsync(string dictType, long? excludeDictId = null, CancellationToken cancellationToken = default)
    {
        var query = _dictTypeRepository.GetQueryable().Where(d => d.DictType == dictType);

        if (excludeDictId.HasValue)
        {
            query = query.Where(d => d.DictId != excludeDictId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 获取所有字典类型列表（用于下拉选择）
    /// </summary>
    public async Task<List<DictTypeDto>> GetAllDictTypesAsync(CancellationToken cancellationToken = default)
    {
        var dictTypes = await _dictTypeRepository.GetQueryable()
            .Where(d => d.Status == UserConstants.NORMAL)
            .OrderBy(d => d.DictId)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<DictTypeDto>>(dictTypes);
    }
}
