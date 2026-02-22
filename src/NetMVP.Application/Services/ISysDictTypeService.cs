using NetMVP.Application.DTOs.Dict;

namespace NetMVP.Application.Services;

/// <summary>
/// 字典类型服务接口
/// </summary>
public interface ISysDictTypeService
{
    /// <summary>
    /// 获取字典类型列表
    /// </summary>
    Task<(List<DictTypeDto> dictTypes, int total)> GetDictTypeListAsync(DictTypeQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取字典类型
    /// </summary>
    Task<DictTypeDto?> GetDictTypeByIdAsync(long dictId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建字典类型
    /// </summary>
    Task<long> CreateDictTypeAsync(CreateDictTypeDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新字典类型
    /// </summary>
    Task UpdateDictTypeAsync(UpdateDictTypeDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除字典类型
    /// </summary>
    Task DeleteDictTypeAsync(long dictId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除字典类型
    /// </summary>
    Task DeleteDictTypesAsync(long[] dictIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刷新字典缓存
    /// </summary>
    Task RefreshDictCacheAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 导出字典类型
    /// </summary>
    Task<byte[]> ExportDictTypesAsync(DictTypeQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查字典类型唯一性
    /// </summary>
    Task<bool> CheckDictTypeUniqueAsync(string dictType, long? excludeDictId = null, CancellationToken cancellationToken = default);
}
