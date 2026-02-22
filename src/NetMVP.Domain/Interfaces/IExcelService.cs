namespace NetMVP.Domain.Interfaces;

/// <summary>
/// Excel 服务接口
/// </summary>
public interface IExcelService
{
    /// <summary>
    /// 导出到文件
    /// </summary>
    Task ExportAsync<T>(List<T> data, string filePath, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// 导出到流
    /// </summary>
    Task ExportAsync<T>(List<T> data, Stream stream, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// 从文件导入
    /// </summary>
    Task<List<T>> ImportAsync<T>(string filePath, CancellationToken cancellationToken = default) where T : class, new();

    /// <summary>
    /// 从流导入
    /// </summary>
    Task<List<T>> ImportAsync<T>(Stream stream, CancellationToken cancellationToken = default) where T : class, new();
}
