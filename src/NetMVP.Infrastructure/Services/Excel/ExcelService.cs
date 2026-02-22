using Microsoft.AspNetCore.Http;
using MiniExcelLibs;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Infrastructure.Services.Excel;

/// <summary>
/// Excel 服务实现
/// </summary>
public class ExcelService : IExcelService
{
    /// <inheritdoc/>
    public async Task ExportAsync<T>(List<T> data, string filePath, CancellationToken cancellationToken = default) where T : class
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await MiniExcel.SaveAsAsync(filePath, data, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task ExportAsync<T>(List<T> data, Stream stream, CancellationToken cancellationToken = default) where T : class
    {
        await MiniExcel.SaveAsAsync(stream, data, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<T>> ImportAsync<T>(string filePath, CancellationToken cancellationToken = default) where T : class, new()
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("文件不存在", filePath);

        var rows = await MiniExcel.QueryAsync<T>(filePath, cancellationToken: cancellationToken);
        return rows.ToList();
    }

    /// <inheritdoc/>
    public async Task<List<T>> ImportAsync<T>(Stream stream, CancellationToken cancellationToken = default) where T : class, new()
    {
        var rows = await MiniExcel.QueryAsync<T>(stream, cancellationToken: cancellationToken);
        return rows.ToList();
    }

    /// <inheritdoc/>
    public async Task<List<T>> ImportAsync<T>(IFormFile file, CancellationToken cancellationToken = default) where T : class, new()
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("文件不能为空");

        using var stream = file.OpenReadStream();
        return await ImportAsync<T>(stream, cancellationToken);
    }
}
