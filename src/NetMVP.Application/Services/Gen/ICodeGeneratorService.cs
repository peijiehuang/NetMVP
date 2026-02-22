namespace NetMVP.Application.Services.Gen;

/// <summary>
/// 代码生成服务接口
/// </summary>
public interface ICodeGeneratorService
{
    /// <summary>
    /// 预览代码
    /// </summary>
    /// <param name="tableId">表ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>代码预览结果（文件名 -> 代码内容）</returns>
    Task<Dictionary<string, string>> PreviewCodeAsync(long tableId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成代码（ZIP）
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>ZIP文件字节数组</returns>
    Task<byte[]> GenerateCodeAsync(string tableName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量生成代码
    /// </summary>
    /// <param name="tableNames">表名列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>ZIP文件字节数组</returns>
    Task<byte[]> BatchGenerateCodeAsync(string[] tableNames, CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载代码
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>ZIP文件字节数组</returns>
    Task<byte[]> DownloadCodeAsync(string tableName, CancellationToken cancellationToken = default);
}
