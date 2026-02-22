using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.Services;

namespace NetMVP.WebApi.Controllers;

/// <summary>
/// 通用控制器
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class CommonController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly ILogger<CommonController> _logger;

    public CommonController(
        IFileService fileService,
        ILogger<CommonController> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    [HttpPost("upload")]
    public async Task<AjaxResult> Upload(IFormFile file)
    {
        var filePath = await _fileService.UploadAsync(file);
        return AjaxResult.Success("上传成功", new { url = filePath });
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> Download(string fileName)
    {
        var (stream, contentType, name) = await _fileService.DownloadAsync(fileName);
        return File(stream, contentType, name);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    [HttpDelete("delete/{fileName}")]
    public async Task<AjaxResult> Delete(string fileName)
    {
        await _fileService.DeleteAsync(fileName);
        return AjaxResult.Success("删除成功");
    }
}
