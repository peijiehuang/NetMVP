using System.ComponentModel.DataAnnotations;

namespace NetMVP.Infrastructure.Configuration;

/// <summary>
/// 文件上传配置
/// </summary>
public class FileUploadSettings
{
    /// <summary>
    /// 最大文件大小（字节）
    /// </summary>
    [Range(1, long.MaxValue, ErrorMessage = "最大文件大小必须大于 0")]
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB

    /// <summary>
    /// 允许的文件扩展名
    /// </summary>
    [Required(ErrorMessage = "允许的文件扩展名不能为空")]
    [MinLength(1, ErrorMessage = "至少需要配置一个允许的文件扩展名")]
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 上传路径
    /// </summary>
    [Required(ErrorMessage = "上传路径不能为空")]
    public string UploadPath { get; set; } = string.Empty;
}
