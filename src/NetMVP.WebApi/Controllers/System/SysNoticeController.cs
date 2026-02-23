using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Notice;
using NetMVP.Application.Services;
using NetMVP.Domain.Constants;
using NetMVP.WebApi.Attributes;

namespace NetMVP.WebApi.Controllers.System;

/// <summary>
/// 通知公告控制器
/// </summary>
[ApiController]
[Route("system/notice")]
[Authorize]
public class SysNoticeController : ControllerBase
{
    private readonly ISysNoticeService _noticeService;

    public SysNoticeController(ISysNoticeService noticeService)
    {
        _noticeService = noticeService;
    }

    /// <summary>
    /// 获取公告列表
    /// </summary>
    [HttpGet("list")]
    [RequirePermission("system:notice:list")]
    public async Task<AjaxResult> GetList([FromQuery] NoticeQueryDto query)
    {
        var (notices, total) = await _noticeService.GetNoticeListAsync(query);
        return AjaxResult.Success().Put("rows", notices).Put("total", total);
    }

    /// <summary>
    /// 获取公告详情
    /// </summary>
    [HttpGet("{noticeId}")]
    [RequirePermission("system:notice:query")]
    public async Task<AjaxResult> GetInfo(int noticeId)
    {
        var notice = await _noticeService.GetNoticeByIdAsync(noticeId);
        if (notice == null)
        {
            return AjaxResult.Error("公告不存在");
        }
        return AjaxResult.Success(notice);
    }

    /// <summary>
    /// 创建公告
    /// </summary>
    [HttpPost]
    [RequirePermission("system:notice:add")]
    [Log(Title = "通知公告", BusinessType = OperLogConstants.BUSINESS_TYPE_INSERT)]
    public async Task<AjaxResult> Add([FromBody] CreateNoticeDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return AjaxResult.Error(errors);
        }

        var noticeId = await _noticeService.CreateNoticeAsync(dto);
        return AjaxResult.Success(noticeId);
    }

    /// <summary>
    /// 更新公告
    /// </summary>
    [HttpPut]
    [RequirePermission("system:notice:edit")]
    [Log(Title = "通知公告", BusinessType = OperLogConstants.BUSINESS_TYPE_UPDATE)]
    public async Task<AjaxResult> Edit([FromBody] UpdateNoticeDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return AjaxResult.Error(errors);
        }

        try
        {
            await _noticeService.UpdateNoticeAsync(dto);
            return AjaxResult.Success();
        }
        catch (InvalidOperationException ex)
        {
            return AjaxResult.Error(ex.Message);
        }
    }

    /// <summary>
    /// 删除公告
    /// </summary>
    [HttpDelete("{noticeIds}")]
    [RequirePermission("system:notice:remove")]
    [Log(Title = "通知公告", BusinessType = OperLogConstants.BUSINESS_TYPE_DELETE)]
    public async Task<AjaxResult> Remove(string noticeIds)
    {
        try
        {
            var ids = noticeIds.Split(',').Select(int.Parse).ToArray();
            await _noticeService.DeleteNoticesAsync(ids);
            return AjaxResult.Success();
        }
        catch (InvalidOperationException ex)
        {
            return AjaxResult.Error(ex.Message);
        }
    }
}
