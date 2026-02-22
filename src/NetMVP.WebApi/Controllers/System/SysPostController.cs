using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Post;
using NetMVP.Application.Services;
using NetMVP.Domain.Enums;
using NetMVP.WebApi.Attributes;

namespace NetMVP.WebApi.Controllers.System;

/// <summary>
/// 岗位管理控制器
/// </summary>
[ApiController]
[Route("system/post")]
[Authorize]
public class SysPostController : BaseController
{
    private readonly ISysPostService _postService;

    public SysPostController(ISysPostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// 获取岗位列表
    /// </summary>
    [HttpGet("list")]
    [RequirePermission("system:post:list")]
    public async Task<TableDataInfo> GetList([FromQuery] PostQueryDto query)
    {
        var (posts, total) = await _postService.GetPostListAsync(query);
        return GetTableData(posts, total);
    }

    /// <summary>
    /// 获取岗位详情
    /// </summary>
    [HttpGet("{postId}")]
    [RequirePermission("system:post:query")]
    public async Task<AjaxResult> GetInfo(long postId)
    {
        var post = await _postService.GetPostByIdAsync(postId);
        if (post == null)
        {
            return Error("岗位不存在");
        }

        return Success(post);
    }

    /// <summary>
    /// 创建岗位
    /// </summary>
    [HttpPost]
    [RequirePermission("system:post:add")]
    [Log(Title = "岗位管理", BusinessType = BusinessType.Insert)]
    public async Task<AjaxResult> Add([FromBody] CreatePostDto dto)
    {
        try
        {
            var postId = await _postService.CreatePostAsync(dto);
            return Success(postId);
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新岗位
    /// </summary>
    [HttpPut]
    [RequirePermission("system:post:edit")]
    [Log(Title = "岗位管理", BusinessType = BusinessType.Update)]
    public async Task<AjaxResult> Edit([FromBody] UpdatePostDto dto)
    {
        try
        {
            await _postService.UpdatePostAsync(dto);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 删除岗位
    /// </summary>
    [HttpDelete("{postId}")]
    [RequirePermission("system:post:remove")]
    [Log(Title = "岗位管理", BusinessType = BusinessType.Delete)]
    public async Task<AjaxResult> Remove(long postId)
    {
        try
        {
            await _postService.DeletePostAsync(postId);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 批量删除岗位
    /// </summary>
    [HttpDelete]
    [RequirePermission("system:post:remove")]
    [Log(Title = "岗位管理", BusinessType = BusinessType.Delete)]
    public async Task<AjaxResult> RemoveBatch([FromBody] long[] postIds)
    {
        try
        {
            await _postService.DeletePostsAsync(postIds);
            return Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error(ex.Message);
        }
    }

    /// <summary>
    /// 导出岗位
    /// </summary>
    [HttpPost("export")]
    [RequirePermission("system:post:export")]
    [Log(Title = "岗位管理", BusinessType = BusinessType.Export)]
    public async Task<IActionResult> Export([FromBody] PostQueryDto query)
    {
        var data = await _postService.ExportPostsAsync(query);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"岗位数据_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }

    /// <summary>
    /// 检查岗位名称唯一性
    /// </summary>
    [HttpGet("checkPostNameUnique")]
    public async Task<AjaxResult> CheckPostNameUnique([FromQuery] string postName, [FromQuery] long? postId)
    {
        var isUnique = await _postService.CheckPostNameUniqueAsync(postName, postId);
        return Success(isUnique);
    }

    /// <summary>
    /// 检查岗位编码唯一性
    /// </summary>
    [HttpGet("checkPostCodeUnique")]
    public async Task<AjaxResult> CheckPostCodeUnique([FromQuery] string postCode, [FromQuery] long? postId)
    {
        var isUnique = await _postService.CheckPostCodeUniqueAsync(postCode, postId);
        return Success(isUnique);
    }
}
