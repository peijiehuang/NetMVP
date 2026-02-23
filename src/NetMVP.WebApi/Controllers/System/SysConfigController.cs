using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Config;
using NetMVP.Application.Services;
using NetMVP.Domain.Enums;
using NetMVP.WebApi.Attributes;

namespace NetMVP.WebApi.Controllers.System;

/// <summary>
/// 参数配置控制器
/// </summary>
[ApiController]
[Route("system/config")]
[Authorize]
public class SysConfigController : ControllerBase
{
    private readonly ISysConfigService _configService;

    public SysConfigController(ISysConfigService configService)
    {
        _configService = configService;
    }

    /// <summary>
    /// 获取参数列表
    /// </summary>
    [HttpGet("list")]
    public async Task<AjaxResult> GetList([FromQuery] ConfigQueryDto query)
    {
        var (configs, total) = await _configService.GetConfigListAsync(query);
        return AjaxResult.Success().Put("rows", configs).Put("total", total);
    }

    /// <summary>
    /// 获取参数详情
    /// </summary>
    [HttpGet("{configId}")]
    public async Task<AjaxResult> GetInfo(int configId)
    {
        var config = await _configService.GetConfigByIdAsync(configId);
        if (config == null)
        {
            return AjaxResult.Error("参数不存在");
        }
        return AjaxResult.Success(config);
    }

    /// <summary>
    /// 根据键名获取参数值
    /// </summary>
    [HttpGet("configKey/{configKey}")]
    public async Task<AjaxResult> GetConfigKey(string configKey)
    {
        var configValue = await _configService.GetConfigByKeyAsync(configKey);
        if (configValue == null)
        {
            return AjaxResult.Error("参数不存在");
        }
        return AjaxResult.Success(configValue);
    }

    /// <summary>
    /// 创建参数
    /// </summary>
    [HttpPost]
    [Log(Title = "参数管理", BusinessType = BusinessType.Insert)]
    public async Task<AjaxResult> Add([FromBody] CreateConfigDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return AjaxResult.Error(errors);
        }

        var configId = await _configService.CreateConfigAsync(dto);
        return AjaxResult.Success(configId);
    }

    /// <summary>
    /// 更新参数
    /// </summary>
    [HttpPut]
    [Log(Title = "参数管理", BusinessType = BusinessType.Update)]
    public async Task<AjaxResult> Edit([FromBody] UpdateConfigDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return AjaxResult.Error(errors);
        }

        await _configService.UpdateConfigAsync(dto);
        return AjaxResult.Success();
    }

    /// <summary>
    /// 删除参数
    /// </summary>
    [HttpDelete("{configIds}")]
    [Log(Title = "参数管理", BusinessType = BusinessType.Delete)]
    public async Task<AjaxResult> Remove(string configIds)
    {
        var ids = configIds.Split(',').Select(int.Parse).ToArray();
        await _configService.DeleteConfigsAsync(ids);
        return AjaxResult.Success();
    }

    /// <summary>
    /// 刷新参数缓存
    /// </summary>
    [HttpDelete("refreshCache")]
    [Log(Title = "参数管理", BusinessType = BusinessType.Clean)]
    public async Task<AjaxResult> RefreshCache()
    {
        await _configService.RefreshConfigCacheAsync();
        return AjaxResult.Success();
    }

    /// <summary>
    /// 导出参数
    /// </summary>
    [HttpPost("export")]
    [Log(Title = "参数管理", BusinessType = BusinessType.Export)]
    public async Task<IActionResult> Export([FromForm] ConfigQueryDto query)
    {
        var data = await _configService.ExportConfigsAsync(query);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"参数配置_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
}
