using NetMVP.Application.DTOs.Dict;

namespace NetMVP.Application.Services;

/// <summary>
/// 字典数据服务接口
/// </summary>
public interface ISysDictDataService
{
    /// <summary>
    /// 获取字典数据列表
    /// </summary>
    Task<(List<DictDataDto> dictData, int total)> GetDictDataListAsync(DictDataQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据字典类型获取字典数据
    /// </summary>
    Task<List<DictDataDto>> GetDictDataByTypeAsync(string dictType, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取字典数据
    /// </summary>
    Task<DictDataDto?> GetDictDataByIdAsync(long dictCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建字典数据
    /// </summary>
    Task<long> CreateDictDataAsync(CreateDictDataDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新字典数据
    /// </summary>
    Task UpdateDictDataAsync(UpdateDictDataDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除字典数据
    /// </summary>
    Task DeleteDictDataAsync(long dictCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除字典数据
    /// </summary>
    Task DeleteDictDataAsync(long[] dictCodes, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导出字典数据
    /// </summary>
    Task<byte[]> ExportDictDataAsync(DictDataQueryDto query, CancellationToken cancellationToken = default);
}
