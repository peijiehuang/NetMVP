using NetMVP.Application.DTOs.Config;

namespace NetMVP.Application.Services;

/// <summary>
/// 参数配置服务接口
/// </summary>
public interface ISysConfigService
{
    /// <summary>
    /// 获取参数列表
    /// </summary>
    Task<(List<ConfigDto> configs, int total)> GetConfigListAsync(ConfigQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取参数
    /// </summary>
    Task<ConfigDto?> GetConfigByIdAsync(int configId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据键名获取参数值
    /// </summary>
    Task<string?> GetConfigByKeyAsync(string configKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建参数
    /// </summary>
    Task<int> CreateConfigAsync(CreateConfigDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新参数
    /// </summary>
    Task UpdateConfigAsync(UpdateConfigDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除参数
    /// </summary>
    Task DeleteConfigAsync(int configId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除参数
    /// </summary>
    Task DeleteConfigsAsync(int[] configIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刷新参数缓存
    /// </summary>
    Task RefreshConfigCacheAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 导出参数
    /// </summary>
    Task<byte[]> ExportConfigsAsync(ConfigQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查参数键名唯一性
    /// </summary>
    Task<bool> CheckConfigKeyUniqueAsync(string configKey, int? excludeConfigId = null, CancellationToken cancellationToken = default);
}
