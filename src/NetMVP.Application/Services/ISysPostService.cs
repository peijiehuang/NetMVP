using NetMVP.Application.DTOs.Post;

namespace NetMVP.Application.Services;

/// <summary>
/// 岗位服务接口
/// </summary>
public interface ISysPostService
{
    /// <summary>
    /// 获取岗位列表
    /// </summary>
    Task<(List<PostDto> posts, int total)> GetPostListAsync(PostQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取岗位
    /// </summary>
    Task<PostDto?> GetPostByIdAsync(long postId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建岗位
    /// </summary>
    Task<long> CreatePostAsync(CreatePostDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新岗位
    /// </summary>
    Task UpdatePostAsync(UpdatePostDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除岗位
    /// </summary>
    Task DeletePostAsync(long postId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除岗位
    /// </summary>
    Task DeletePostsAsync(long[] postIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导出岗位
    /// </summary>
    Task<byte[]> ExportPostsAsync(PostQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查岗位名称唯一性
    /// </summary>
    Task<bool> CheckPostNameUniqueAsync(string postName, long? excludePostId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查岗位编码唯一性
    /// </summary>
    Task<bool> CheckPostCodeUniqueAsync(string postCode, long? excludePostId = null, CancellationToken cancellationToken = default);
}
