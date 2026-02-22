using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetMVP.Domain.Exceptions;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 文件服务实现
/// </summary>
public class FileService : IFileService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileService> _logger;
    private readonly string _uploadPath;
    private readonly long _maxFileSize;
    private readonly string[] _allowedExtensions;

    public FileService(
        IConfiguration configuration,
        ILogger<FileService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // 读取配置
        _uploadPath = configuration["FileUpload:UploadPath"] ?? "uploads";
        var maxFileSizeStr = configuration["FileUpload:MaxFileSize"] ?? "10485760";
        _maxFileSize = long.Parse(maxFileSizeStr); // 默认10MB
        
        // 读取允许的扩展名数组
        var extensions = new List<string>();
        var extensionsSection = configuration.GetSection("FileUpload:AllowedExtensions");
        foreach (var ext in extensionsSection.GetChildren())
        {
            var value = ext.Value;
            if (!string.IsNullOrEmpty(value))
            {
                extensions.Add(value);
            }
        }
        _allowedExtensions = extensions.Count > 0 
            ? extensions.ToArray() 
            : new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".zip" };

        // 确保上传目录存在
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        // 验证文件
        ValidateFile(file);

        // 生成文件名
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_uploadPath, fileName);

        // 保存文件
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        _logger.LogInformation("文件上传成功: {FileName}, 大小: {Size} bytes", fileName, file.Length);

        // 返回访问路径
        return $"/uploads/{fileName}";
    }

    public async Task<(Stream stream, string contentType, string fileName)> DownloadAsync(
        string fileName, 
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var filePath = Path.Combine(_uploadPath, fileName);

            if (!File.Exists(filePath))
            {
                throw new NotFoundException($"文件不存在: {fileName}");
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var contentType = GetContentType(fileName);

            return (stream, contentType, fileName);
        }, cancellationToken);
    }

    public Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var filePath = Path.Combine(_uploadPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("文件删除成功: {FileName}", fileName);
            }
        }, cancellationToken);
    }

    public Task<FileInfo?> GetFileInfoAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_uploadPath, fileName);

        if (!File.Exists(filePath))
        {
            return Task.FromResult<FileInfo?>(null);
        }

        var fileInfo = new FileInfo(filePath);
        return Task.FromResult<FileInfo?>(fileInfo);
    }

    private void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ValidationException("文件不能为空");
        }

        // 验证文件大小
        if (file.Length > _maxFileSize)
        {
            throw new ValidationException($"文件大小不能超过 {_maxFileSize / 1024 / 1024} MB");
        }

        // 验证文件扩展名
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            throw new ValidationException($"不支持的文件类型: {extension}");
        }
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }
}
