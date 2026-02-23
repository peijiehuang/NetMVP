using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetMVP.Application.DTOs.Config;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 参数配置服务实现
/// </summary>
public class SysConfigService : ISysConfigService
{
    private readonly IRepository<SysConfig> _configRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExcelService _excelService;
    private readonly ICacheService _cacheService;

    public SysConfigService(
        IRepository<SysConfig> configRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IExcelService excelService,
        ICacheService cacheService)
    {
        _configRepository = configRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _excelService = excelService;
        _cacheService = cacheService;
    }

    /// <summary>
    /// 获取参数列表
    /// </summary>
    public async Task<(List<ConfigDto> configs, int total)> GetConfigListAsync(ConfigQueryDto query, CancellationToken cancellationToken = default)
    {
        var queryable = _configRepository.GetQueryable();

        // 参数名称
        if (!string.IsNullOrWhiteSpace(query.ConfigName))
        {
            queryable = queryable.Where(c => c.ConfigName.Contains(query.ConfigName));
        }

        // 参数键名
        if (!string.IsNullOrWhiteSpace(query.ConfigKey))
        {
            queryable = queryable.Where(c => c.ConfigKey.Contains(query.ConfigKey));
        }

        // 系统内置
        if (!string.IsNullOrWhiteSpace(query.ConfigType))
        {
            queryable = queryable.Where(c => c.ConfigType == query.ConfigType);
        }

        // 时间范围
        if (query.BeginTime.HasValue)
        {
            queryable = queryable.Where(c => c.CreateTime >= query.BeginTime.Value);
        }

        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where(c => c.CreateTime <= query.EndTime.Value);
        }

        // 总数
        var total = await queryable.CountAsync(cancellationToken);

        // 分页
        var configs = await queryable
            .OrderBy(c => c.ConfigId)
            .Skip((query.PageNum - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var configDtos = _mapper.Map<List<ConfigDto>>(configs);

        return (configDtos, total);
    }

    /// <summary>
    /// 根据ID获取参数
    /// </summary>
    public async Task<ConfigDto?> GetConfigByIdAsync(int configId, CancellationToken cancellationToken = default)
    {
        var config = await _configRepository.GetByIdAsync(configId, cancellationToken);
        return config == null ? null : _mapper.Map<ConfigDto>(config);
    }

    /// <summary>
    /// 根据键名获取参数值
    /// </summary>
    public async Task<string?> GetConfigByKeyAsync(string configKey, CancellationToken cancellationToken = default)
    {
        // 先从缓存获取
        var cacheKey = $"config:{configKey}";
        var cachedValue = await _cacheService.GetAsync<string>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        // 从数据库获取
        var config = await _configRepository.GetQueryable()
            .FirstOrDefaultAsync(c => c.ConfigKey == configKey, cancellationToken);

        if (config == null)
        {
            return null;
        }

        // 缓存参数值
        await _cacheService.SetAsync(cacheKey, config.ConfigValue, TimeSpan.FromHours(1), cancellationToken);

        return config.ConfigValue;
    }

    /// <summary>
    /// 创建参数
    /// </summary>
    public async Task<int> CreateConfigAsync(CreateConfigDto dto, CancellationToken cancellationToken = default)
    {
        // 检查键名唯一性
        if (!await CheckConfigKeyUniqueAsync(dto.ConfigKey, null, cancellationToken))
        {
            throw new InvalidOperationException($"参数键名'{dto.ConfigKey}'已存在");
        }

        var config = _mapper.Map<SysConfig>(dto);
        config.ConfigType = dto.ConfigType;

        await _configRepository.AddAsync(config, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除缓存
        await ClearConfigCacheAsync(dto.ConfigKey);

        return config.ConfigId;
    }

    /// <summary>
    /// 更新参数
    /// </summary>
    public async Task UpdateConfigAsync(UpdateConfigDto dto, CancellationToken cancellationToken = default)
    {
        var config = await _configRepository.GetByIdAsync(dto.ConfigId, cancellationToken);
        if (config == null)
        {
            throw new InvalidOperationException("参数不存在");
        }

        // 检查键名唯一性
        if (!await CheckConfigKeyUniqueAsync(dto.ConfigKey, dto.ConfigId, cancellationToken))
        {
            throw new InvalidOperationException($"参数键名'{dto.ConfigKey}'已存在");
        }

        var oldConfigKey = config.ConfigKey;

        _mapper.Map(dto, config);
        config.ConfigType = dto.ConfigType;

        await _configRepository.UpdateAsync(config, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除缓存
        await ClearConfigCacheAsync(oldConfigKey);
        if (oldConfigKey != dto.ConfigKey)
        {
            await ClearConfigCacheAsync(dto.ConfigKey);
        }
    }

    /// <summary>
    /// 删除参数
    /// </summary>
    public async Task DeleteConfigAsync(int configId, CancellationToken cancellationToken = default)
    {
        var config = await _configRepository.GetByIdAsync(configId, cancellationToken);
        if (config == null)
        {
            throw new InvalidOperationException("参数不存在");
        }

        // 检查是否为系统内置参数
        if (config.ConfigType == CommonConstants.YES)
        {
            throw new InvalidOperationException("系统内置参数不允许删除");
        }

        await _configRepository.DeleteAsync(config, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除缓存
        await ClearConfigCacheAsync(config.ConfigKey);
    }

    /// <summary>
    /// 批量删除参数
    /// </summary>
    public async Task DeleteConfigsAsync(int[] configIds, CancellationToken cancellationToken = default)
    {
        foreach (var configId in configIds)
        {
            await DeleteConfigAsync(configId, cancellationToken);
        }
    }

    /// <summary>
    /// 刷新参数缓存
    /// </summary>
    public async Task RefreshConfigCacheAsync(CancellationToken cancellationToken = default)
    {
        // 获取所有参数
        var configs = await _configRepository.GetQueryable().ToListAsync(cancellationToken);

        // 清除所有参数缓存
        foreach (var config in configs)
        {
            await ClearConfigCacheAsync(config.ConfigKey);
        }

        // 重新加载到缓存
        foreach (var config in configs)
        {
            var cacheKey = $"config:{config.ConfigKey}";
            await _cacheService.SetAsync(cacheKey, config.ConfigValue, TimeSpan.FromHours(1), cancellationToken);
        }
    }

    /// <summary>
    /// 导出参数
    /// </summary>
    public async Task<byte[]> ExportConfigsAsync(ConfigQueryDto query, CancellationToken cancellationToken = default)
    {
        // 获取所有数据（不分页）
        var queryable = _configRepository.GetQueryable();

        // 参数名称
        if (!string.IsNullOrWhiteSpace(query.ConfigName))
        {
            queryable = queryable.Where(c => c.ConfigName.Contains(query.ConfigName));
        }

        // 参数键名
        if (!string.IsNullOrWhiteSpace(query.ConfigKey))
        {
            queryable = queryable.Where(c => c.ConfigKey.Contains(query.ConfigKey));
        }

        // 系统内置
        if (!string.IsNullOrWhiteSpace(query.ConfigType))
        {
            queryable = queryable.Where(c => c.ConfigType == query.ConfigType);
        }

        // 时间范围
        if (query.BeginTime.HasValue)
        {
            queryable = queryable.Where(c => c.CreateTime >= query.BeginTime.Value);
        }

        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where(c => c.CreateTime <= query.EndTime.Value);
        }

        var configs = await queryable
            .OrderBy(c => c.ConfigId)
            .ToListAsync(cancellationToken);

        var configDtos = _mapper.Map<List<ConfigDto>>(configs);

        // 导出到内存流
        using var stream = new MemoryStream();
        await _excelService.ExportAsync(configDtos, stream, cancellationToken);
        return stream.ToArray();
    }

    /// <summary>
    /// 检查参数键名唯一性
    /// </summary>
    public async Task<bool> CheckConfigKeyUniqueAsync(string configKey, int? excludeConfigId = null, CancellationToken cancellationToken = default)
    {
        var query = _configRepository.GetQueryable().Where(c => c.ConfigKey == configKey);

        if (excludeConfigId.HasValue)
        {
            query = query.Where(c => c.ConfigId != excludeConfigId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 清除参数缓存
    /// </summary>
    private async Task ClearConfigCacheAsync(string configKey)
    {
        var cacheKey = $"config:{configKey}";
        await _cacheService.RemoveAsync(cacheKey);
    }
}
