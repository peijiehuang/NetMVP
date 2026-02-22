using Microsoft.AspNetCore.Http;

namespace NetMVP.Application.Services;

/// <summary>
/// 文件服务接口
/// </summary>
public interface IFileService
{
    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>文件访问路径</returns>
    Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>文件流和内容类型</returns>
    Task<(Stream stream, string contentType, string fileName)> DownloadAsync(string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task DeleteAsync(string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取文件信息
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>文件信息</returns>
    Task<FileInfo?> GetFileInfoAsync(string fileName, CancellationToken cancellationToken = default);
}
