using NetMVP.Application.DTOs.LoginInfo;

namespace NetMVP.Application.Services;

/// <summary>
/// 登录日志服务接口
/// </summary>
public interface ISysLoginInfoService
{
    /// <summary>
    /// 获取登录日志列表
    /// </summary>
    Task<(List<LoginInfoDto> list, int total)> GetLoginInfoListAsync(LoginInfoQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建登录日志
    /// </summary>
    Task<long> CreateLoginInfoAsync(CreateLoginInfoDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除登录日志
    /// </summary>
    Task<bool> DeleteLoginInfoAsync(long infoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除登录日志
    /// </summary>
    Task<int> DeleteLoginInfosAsync(long[] infoIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清空登录日志
    /// </summary>
    Task<int> CleanLoginInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 解锁用户
    /// </summary>
    Task<bool> UnlockUserAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导出登录日志
    /// </summary>
    Task<byte[]> ExportLoginInfosAsync(LoginInfoQueryDto query, CancellationToken cancellationToken = default);
}
