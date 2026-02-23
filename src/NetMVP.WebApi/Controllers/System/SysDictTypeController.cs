using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Dict;
using NetMVP.Application.Services;
using NetMVP.Domain.Enums;
using NetMVP.WebApi.Attributes;

namespace NetMVP.WebApi.Controllers.System;

/// <summary>
/// 字典类型管理控制器
/// </summary>
[ApiController]
[Route("system/dict/type")]
[Authorize]
public class SysDictTypeController : BaseController
{
    private readonly ISysDictTypeService _dictTypeService;

    public SysDictTypeController(ISysDictTypeService dictTypeService)
    {
        _dictTypeService = dictTypeService;
    }

    /// <summary>
    /// 获取字典类型列表
    /// </summary>
    [HttpGet("list")]
    [RequirePermission("system:dict:list")]
    public async Task<TableDataInfo> GetList([FromQuery] DictTypeQueryDto query)
    {
        var (dictTypes, total) = await _dictTypeService.GetDictTypeListAsync(query);
        return GetTableData(dictTypes, total);
    }

    /// <summary>
    /// 获取字典类型详情
    /// </summary>
    [HttpGet("{dictId}")]
    [RequirePermission("system:dict:query")]
    public async Task<AjaxResult> GetInfo(long dictId)
    {
        var dictType = await _dictTypeService.GetDictTypeByIdAsync(dictId);
        if (dictType == null)
        {
            return Error("字典类型不存在");
        }

        return Success(dictType);
    }

    /// <summary>
    /// 创建字典类型
    /// </summary>
    [HttpPost]
    [RequirePermission("system:dict:add")]
    [Log(Title = "字典类型", BusinessType = BusinessType.Insert)]
    public async Task<AjaxResult> Add([FromBody] CreateDictTypeDto dto)
    {
        try
        {
            var dictId = await _dictTypeService.CreateDictTypeAsync(dto);
            return Success(dictId);
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新字典类型
    /// </summary>
    [HttpPut]
    [RequirePermission("system:dict:edit")]
    [Log(Title = "字典类型", BusinessType = BusinessType.Update)]
    public async Task<AjaxResult> Edit([FromBody] UpdateDictTypeDto dto)
    {
        try
        {
            await _dictTypeService.UpdateDictTypeAsync(dto);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 删除字典类型（支持单个和批量）
    /// </summary>
    [HttpDelete("{dictIds}")]
    [RequirePermission("system:dict:remove")]
    [Log(Title = "字典类型", BusinessType = BusinessType.Delete)]
    public async Task<AjaxResult> Remove(string dictIds)
    {
        try
        {
            var ids = dictIds.Split(',').Select(long.Parse).ToArray();
            await _dictTypeService.DeleteDictTypesAsync(ids);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 刷新字典缓存
    /// </summary>
    [HttpDelete("refreshCache")]
    [RequirePermission("system:dict:remove")]
    [Log(Title = "字典类型", BusinessType = BusinessType.Clean)]
    public async Task<AjaxResult> RefreshCache()
    {
        await _dictTypeService.RefreshDictCacheAsync();
        return Success();
    }

    /// <summary>
    /// 导出字典类型
    /// </summary>
    [HttpPost("export")]
    [RequirePermission("system:dict:export")]
    [Log(Title = "字典类型", BusinessType = BusinessType.Export)]
    public async Task<IActionResult> Export([FromForm] DictTypeQueryDto query)
    {
        var data = await _dictTypeService.ExportDictTypesAsync(query);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"字典类型_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }

    /// <summary>
    /// 获取字典选择框列表
    /// </summary>
    [HttpGet("optionselect")]
    public async Task<AjaxResult> OptionSelect()
    {
        var dictTypes = await _dictTypeService.GetAllDictTypesAsync();
        return Success(dictTypes);
    }

    /// <summary>
    /// 检查字典类型唯一性
    /// </summary>
    [HttpGet("checkDictTypeUnique")]
    public async Task<AjaxResult> CheckDictTypeUnique([FromQuery] string dictType, [FromQuery] long? dictId)
    {
        var isUnique = await _dictTypeService.CheckDictTypeUniqueAsync(dictType, dictId);
        return Success(isUnique);
    }
}
