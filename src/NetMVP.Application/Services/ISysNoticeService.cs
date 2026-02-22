using NetMVP.Application.DTOs.Notice;

namespace NetMVP.Application.Services;

/// <summary>
/// 通知公告服务接口
/// </summary>
public interface ISysNoticeService
{
    /// <summary>
    /// 获取公告列表
    /// </summary>
    Task<(List<NoticeDto> notices, int total)> GetNoticeListAsync(NoticeQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取公告
    /// </summary>
    Task<NoticeDto?> GetNoticeByIdAsync(int noticeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建公告
    /// </summary>
    Task<int> CreateNoticeAsync(CreateNoticeDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新公告
    /// </summary>
    Task UpdateNoticeAsync(UpdateNoticeDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除公告
    /// </summary>
    Task DeleteNoticeAsync(int noticeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除公告
    /// </summary>
    Task DeleteNoticesAsync(int[] noticeIds, CancellationToken cancellationToken = default);
}
