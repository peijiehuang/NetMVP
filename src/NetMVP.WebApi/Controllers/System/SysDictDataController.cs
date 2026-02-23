using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Dict;
using NetMVP.Application.Services;
using NetMVP.Domain.Constants;
using NetMVP.WebApi.Attributes;

namespace NetMVP.WebApi.Controllers.System;

/// <summary>
/// 字典数据管理控制器
/// </summary>
[ApiController]
[Route("system/dict/data")]
[Authorize]
public class SysDictDataController : BaseController
{
    private readonly ISysDictDataService _dictDataService;

    public SysDictDataController(ISysDictDataService dictDataService)
    {
        _dictDataService = dictDataService;
    }

    /// <summary>
    /// 获取字典数据列表
    /// </summary>
    [HttpGet("list")]
    [RequirePermission("system:dict:list")]
    public async Task<TableDataInfo> GetList([FromQuery] DictDataQueryDto query)
    {
        var (dictData, total) = await _dictDataService.GetDictDataListAsync(query);
        return GetTableData(dictData, total);
    }

    /// <summary>
    /// 根据字典类型获取字典数据
    /// </summary>
    [HttpGet("type/{dictType}")]
    public async Task<AjaxResult> GetDictDataByType(string dictType)
    {
        var dictData = await _dictDataService.GetDictDataByTypeAsync(dictType);
        return Success(dictData);
    }

    /// <summary>
    /// 获取字典数据详情
    /// </summary>
    [HttpGet("{dictCode}")]
    [RequirePermission("system:dict:query")]
    public async Task<AjaxResult> GetInfo(long dictCode)
    {
        var dictData = await _dictDataService.GetDictDataByIdAsync(dictCode);
        if (dictData == null)
        {
            return Error("字典数据不存在");
        }

        return Success(dictData);
    }

    /// <summary>
    /// 创建字典数据
    /// </summary>
    [HttpPost]
    [RequirePermission("system:dict:add")]
    [Log(Title = "字典数据", BusinessType = OperLogConstants.BUSINESS_TYPE_INSERT)]
    public async Task<AjaxResult> Add([FromBody] CreateDictDataDto dto)
    {
        try
        {
            var dictCode = await _dictDataService.CreateDictDataAsync(dto);
            return Success(dictCode);
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新字典数据
    /// </summary>
    [HttpPut]
    [RequirePermission("system:dict:edit")]
    [Log(Title = "字典数据", BusinessType = OperLogConstants.BUSINESS_TYPE_UPDATE)]
    public async Task<AjaxResult> Edit([FromBody] UpdateDictDataDto dto)
    {
        try
        {
            await _dictDataService.UpdateDictDataAsync(dto);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 删除字典数据（支持单个和批量）
    /// </summary>
    [HttpDelete("{dictCodes}")]
    [RequirePermission("system:dict:remove")]
    [Log(Title = "字典数据", BusinessType = OperLogConstants.BUSINESS_TYPE_DELETE)]
    public async Task<AjaxResult> Remove(string dictCodes)
    {
        try
        {
            var codes = dictCodes.Split(',').Select(long.Parse).ToArray();
            await _dictDataService.DeleteDictDataAsync(codes);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 导出字典数据
    /// </summary>
    [HttpPost("export")]
    [RequirePermission("system:dict:export")]
    [Log(Title = "字典数据", BusinessType = OperLogConstants.BUSINESS_TYPE_EXPORT)]
    public async Task<IActionResult> Export([FromForm] DictDataQueryDto query)
    {
        var data = await _dictDataService.ExportDictDataAsync(query);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"字典数据_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
}
