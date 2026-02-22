using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Gen;
using NetMVP.Application.Services.Gen;

namespace NetMVP.WebApi.Controllers.Tool;

/// <summary>
/// 代码生成控制器
/// </summary>
[Route("tool/gen")]
public class GenController : BaseController
{
    private readonly IGenTableService _genTableService;
    private readonly ICodeGeneratorService _codeGeneratorService;

    public GenController(
        IGenTableService genTableService,
        ICodeGeneratorService codeGeneratorService)
    {
        _genTableService = genTableService;
        _codeGeneratorService = codeGeneratorService;
    }

    /// <summary>
    /// 查询生成表列表
    /// </summary>
    [HttpGet("list")]
    public async Task<AjaxResult> GetGenTableList([FromQuery] GenTableQueryDto query)
    {
        var result = await _genTableService.GetGenTableListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 查询数据库表列表
    /// </summary>
    [HttpGet("db/list")]
    public async Task<AjaxResult> GetDbTableList([FromQuery] GenTableQueryDto query)
    {
        var result = await _genTableService.GetDbTableListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 查询生成表详情
    /// </summary>
    [HttpGet("{tableId}")]
    public async Task<AjaxResult> GetGenTableById(long tableId)
    {
        var result = await _genTableService.GetGenTableByIdAsync(tableId);
        if (result == null)
        {
            return Error("生成表不存在");
        }
        return Success(result);
    }

    /// <summary>
    /// 导入表
    /// </summary>
    [HttpPost("importTable")]
    public async Task<AjaxResult> ImportTable([FromBody] ImportTableDto dto)
    {
        if (dto.Tables == null || dto.Tables.Length == 0)
        {
            return Error("请选择要导入的表");
        }

        var result = await _genTableService.ImportGenTableAsync(dto);
        return result ? Success("导入成功") : Error("导入失败");
    }

    /// <summary>
    /// 修改生成表
    /// </summary>
    [HttpPut]
    public async Task<AjaxResult> UpdateGenTable([FromBody] UpdateGenTableDto dto)
    {
        var result = await _genTableService.UpdateGenTableAsync(dto);
        return result ? Success("修改成功") : Error("修改失败");
    }

    /// <summary>
    /// 删除生成表
    /// </summary>
    [HttpDelete("{tableIds}")]
    public async Task<AjaxResult> DeleteGenTable(string tableIds)
    {
        var ids = tableIds.Split(',').Select(long.Parse).ToArray();
        var result = await _genTableService.DeleteGenTableAsync(ids);
        return result ? Success("删除成功") : Error("删除失败");
    }

    /// <summary>
    /// 同步数据库
    /// </summary>
    [HttpGet("synchDb/{tableName}")]
    public async Task<AjaxResult> SyncDb(string tableName)
    {
        var result = await _genTableService.SyncDbAsync(tableName);
        return result ? Success("同步成功") : Error("同步失败");
    }

    /// <summary>
    /// 预览代码
    /// </summary>
    [HttpGet("preview/{tableId}")]
    public async Task<AjaxResult> PreviewCode(long tableId)
    {
        var result = await _codeGeneratorService.PreviewCodeAsync(tableId);
        return Success(result);
    }

    /// <summary>
    /// 生成代码（下载）
    /// </summary>
    [HttpGet("download/{tableName}")]
    public async Task<IActionResult> DownloadCode(string tableName)
    {
        var data = await _codeGeneratorService.DownloadCodeAsync(tableName);
        return File(data, "application/zip", $"{tableName}.zip");
    }

    /// <summary>
    /// 生成代码
    /// </summary>
    [HttpGet("genCode/{tableName}")]
    public async Task<IActionResult> GenCode(string tableName)
    {
        var data = await _codeGeneratorService.GenerateCodeAsync(tableName);
        return File(data, "application/zip", $"{tableName}.zip");
    }

    /// <summary>
    /// 批量生成代码
    /// </summary>
    [HttpGet("batchGenCode")]
    public async Task<IActionResult> BatchGenCode([FromQuery] string tables)
    {
        var tableNames = tables.Split(',');
        var data = await _codeGeneratorService.BatchGenerateCodeAsync(tableNames);
        return File(data, "application/zip", "code.zip");
    }
}
