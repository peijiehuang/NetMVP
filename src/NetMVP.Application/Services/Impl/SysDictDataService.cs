using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetMVP.Application.DTOs.Dict;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 字典数据服务实现
/// </summary>
public class SysDictDataService : ISysDictDataService
{
    private readonly IRepository<SysDictData> _dictDataRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExcelService _excelService;
    private readonly ICacheService _cacheService;
    private const string DictCacheKeyPrefix = "dict:";

    public SysDictDataService(
        IRepository<SysDictData> dictDataRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IExcelService excelService,
        ICacheService cacheService)
    {
        _dictDataRepository = dictDataRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _excelService = excelService;
        _cacheService = cacheService;
    }

    /// <summary>
    /// 获取字典数据列表
    /// </summary>
    public async Task<(List<DictDataDto> dictData, int total)> GetDictDataListAsync(DictDataQueryDto query, CancellationToken cancellationToken = default)
    {
        var queryable = _dictDataRepository.GetQueryable();

        // 字典类型
        if (!string.IsNullOrWhiteSpace(query.DictType))
        {
            queryable = queryable.Where(d => d.DictType == query.DictType);
        }

        // 字典标签
        if (!string.IsNullOrWhiteSpace(query.DictLabel))
        {
            queryable = queryable.Where(d => d.DictLabel.Contains(query.DictLabel));
        }

        // 状态
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(d => d.Status == query.Status);
        }

        // 总数
        var total = await queryable.CountAsync(cancellationToken);

        // 分页
        var dictData = await queryable
            .OrderBy(d => d.DictSort)
            .ThenBy(d => d.DictCode)
            .Skip((query.PageNum - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var dictDataDtos = _mapper.Map<List<DictDataDto>>(dictData);

        return (dictDataDtos, total);
    }

    /// <summary>
    /// 根据字典类型获取字典数据
    /// </summary>
    public async Task<List<DictDataDto>> GetDictDataByTypeAsync(string dictType, CancellationToken cancellationToken = default)
    {
        // 先从缓存获取
        var cacheKey = $"{DictCacheKeyPrefix}{dictType}";
        var cachedData = await _cacheService.GetAsync<List<DictDataDto>>(cacheKey, cancellationToken);
        if (cachedData != null)
        {
            return cachedData;
        }

        // 从数据库获取
        var dictData = await _dictDataRepository.GetQueryable()
            .Where(d => d.DictType == dictType && d.Status == UserConstants.NORMAL)
            .OrderBy(d => d.DictSort)
            .ThenBy(d => d.DictCode)
            .ToListAsync(cancellationToken);

        var dictDataDtos = _mapper.Map<List<DictDataDto>>(dictData);

        // 缓存数据
        await _cacheService.SetAsync(cacheKey, dictDataDtos, TimeSpan.FromHours(1), cancellationToken);

        return dictDataDtos;
    }

    /// <summary>
    /// 根据ID获取字典数据
    /// </summary>
    public async Task<DictDataDto?> GetDictDataByIdAsync(long dictCode, CancellationToken cancellationToken = default)
    {
        var dictData = await _dictDataRepository.GetByIdAsync(dictCode, cancellationToken);
        return dictData == null ? null : _mapper.Map<DictDataDto>(dictData);
    }

    /// <summary>
    /// 创建字典数据
    /// </summary>
    public async Task<long> CreateDictDataAsync(CreateDictDataDto dto, CancellationToken cancellationToken = default)
    {
        var dictData = new SysDictData
        {
            DictSort = dto.DictSort,
            DictLabel = dto.DictLabel,
            DictValue = dto.DictValue,
            DictType = dto.DictType,
            CssClass = dto.CssClass,
            ListClass = dto.ListClass,
            IsDefault = dto.IsDefault,
            Status = dto.Status,
            Remark = dto.Remark
        };

        await _dictDataRepository.AddAsync(dictData, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除缓存
        await _cacheService.RemoveAsync($"{DictCacheKeyPrefix}{dto.DictType}", cancellationToken);

        return dictData.DictCode;
    }

    /// <summary>
    /// 更新字典数据
    /// </summary>
    public async Task UpdateDictDataAsync(UpdateDictDataDto dto, CancellationToken cancellationToken = default)
    {
        var dictData = await _dictDataRepository.GetByIdAsync(dto.DictCode, cancellationToken);
        if (dictData == null)
        {
            throw new InvalidOperationException("字典数据不存在");
        }

        dictData.DictSort = dto.DictSort;
        dictData.DictLabel = dto.DictLabel;
        dictData.DictValue = dto.DictValue;
        dictData.DictType = dto.DictType;
        dictData.CssClass = dto.CssClass;
        dictData.ListClass = dto.ListClass;
        dictData.IsDefault = dto.IsDefault;
        dictData.Status = dto.Status;
        dictData.Remark = dto.Remark;

        await _dictDataRepository.UpdateAsync(dictData, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除缓存
        await _cacheService.RemoveAsync($"{DictCacheKeyPrefix}{dto.DictType}", cancellationToken);
    }

    /// <summary>
    /// 删除字典数据
    /// </summary>
    public async Task DeleteDictDataAsync(long dictCode, CancellationToken cancellationToken = default)
    {
        var dictData = await _dictDataRepository.GetByIdAsync(dictCode, cancellationToken);
        if (dictData == null)
        {
            throw new InvalidOperationException("字典数据不存在");
        }

        await _dictDataRepository.DeleteAsync(dictData, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除缓存
        await _cacheService.RemoveAsync($"{DictCacheKeyPrefix}{dictData.DictType}", cancellationToken);
    }

    /// <summary>
    /// 批量删除字典数据
    /// </summary>
    public async Task DeleteDictDataAsync(long[] dictCodes, CancellationToken cancellationToken = default)
    {
        foreach (var dictCode in dictCodes)
        {
            await DeleteDictDataAsync(dictCode, cancellationToken);
        }
    }

    /// <summary>
    /// 导出字典数据
    /// </summary>
    public async Task<byte[]> ExportDictDataAsync(DictDataQueryDto query, CancellationToken cancellationToken = default)
    {
        // 获取所有数据（不分页）
        var queryable = _dictDataRepository.GetQueryable();

        // 字典类型
        if (!string.IsNullOrWhiteSpace(query.DictType))
        {
            queryable = queryable.Where(d => d.DictType == query.DictType);
        }

        // 字典标签
        if (!string.IsNullOrWhiteSpace(query.DictLabel))
        {
            queryable = queryable.Where(d => d.DictLabel.Contains(query.DictLabel));
        }

        // 状态
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(d => d.Status == query.Status);
        }

        var dictData = await queryable
            .OrderBy(d => d.DictSort)
            .ThenBy(d => d.DictCode)
            .ToListAsync(cancellationToken);

        var dictDataDtos = _mapper.Map<List<DictDataDto>>(dictData);

        // 导出到内存流
        using var stream = new MemoryStream();
        await _excelService.ExportAsync(dictDataDtos, stream, cancellationToken);
        return stream.ToArray();
    }
}
