using NetMVP.Application.DTOs.OperLog;

namespace NetMVP.Application.Services;

/// <summary>
/// 操作日志服务接口
/// </summary>
public interface ISysOperLogService
{
    /// <summary>
    /// 获取操作日志列表
    /// </summary>
    Task<(List<OperLogDto> logs, int total)> GetOperLogListAsync(OperLogQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取操作日志
    /// </summary>
    Task<OperLogDto?> GetOperLogByIdAsync(long operId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建操作日志
    /// </summary>
    Task<long> CreateOperLogAsync(CreateOperLogDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除操作日志
    /// </summary>
    Task DeleteOperLogAsync(long operId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除操作日志
    /// </summary>
    Task DeleteOperLogsAsync(long[] operIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清空操作日志
    /// </summary>
    Task CleanOperLogAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 导出操作日志
    /// </summary>
    Task<byte[]> ExportOperLogsAsync(OperLogQueryDto query, CancellationToken cancellationToken = default);
}
