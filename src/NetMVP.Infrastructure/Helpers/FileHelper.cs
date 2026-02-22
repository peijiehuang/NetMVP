using Microsoft.AspNetCore.Http;

namespace NetMVP.Infrastructure.Helpers;

/// <summary>
/// 文件工具类
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// 获取文件扩展名
    /// </summary>
    public static string GetFileExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return string.Empty;

        var index = fileName.LastIndexOf('.');
        return index >= 0 ? fileName.Substring(index) : string.Empty;
    }

    /// <summary>
    /// 判断文件扩展名是否允许
    /// </summary>
    public static bool IsAllowedExtension(string extension, string[] allowedExtensions)
    {
        if (string.IsNullOrEmpty(extension) || allowedExtensions == null || allowedExtensions.Length == 0)
            return false;

        return allowedExtensions.Any(e => e.Equals(extension, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 格式化文件大小
    /// </summary>
    public static string GetFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    /// <summary>
    /// 保存文件
    /// </summary>
    public static async Task<string> SaveFile(IFormFile file, string path, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("文件不能为空");

        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        return path;
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    public static bool DeleteFile(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return false;

        try
        {
            File.Delete(path);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
