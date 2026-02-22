using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;

namespace NetMVP.WebApi.Controllers;

/// <summary>
/// 控制器基类
/// </summary>
[ApiController]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// 成功响应
    /// </summary>
    protected AjaxResult Success()
    {
        return AjaxResult.Success();
    }

    /// <summary>
    /// 成功响应（带数据）
    /// </summary>
    protected AjaxResult Success(object data)
    {
        return AjaxResult.Success(data);
    }

    /// <summary>
    /// 成功响应（自定义消息）
    /// </summary>
    protected AjaxResult Success(string msg)
    {
        return AjaxResult.Success(msg);
    }

    /// <summary>
    /// 成功响应（自定义消息和数据）
    /// </summary>
    protected AjaxResult Success(string msg, object data)
    {
        return AjaxResult.Success(msg, data);
    }

    /// <summary>
    /// 错误响应
    /// </summary>
    protected AjaxResult Error()
    {
        return AjaxResult.Error();
    }

    /// <summary>
    /// 错误响应（自定义消息）
    /// </summary>
    protected AjaxResult Error(string msg)
    {
        return AjaxResult.Error(msg);
    }

    /// <summary>
    /// 警告响应
    /// </summary>
    protected AjaxResult Warn(string msg)
    {
        return AjaxResult.Warn(msg);
    }

    /// <summary>
    /// 根据影响行数返回响应
    /// </summary>
    protected AjaxResult ToAjax(int rows)
    {
        return rows > 0 ? Success() : Error("操作失败");
    }

    /// <summary>
    /// 根据布尔值返回响应
    /// </summary>
    protected AjaxResult ToAjax(bool result)
    {
        return result ? Success() : Error("操作失败");
    }

    /// <summary>
    /// 表格数据响应
    /// </summary>
    protected TableDataInfo GetTableData<T>(List<T> list, long total)
    {
        return TableDataInfo.Build(list, total);
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    protected long GetUserId()
    {
        var userIdClaim = User.FindFirst("userId") ?? User.FindFirst("sub");
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("无法获取用户ID");
        }
        return userId;
    }

    /// <summary>
    /// 获取当前用户名
    /// </summary>
    protected string GetUserName()
    {
        var userNameClaim = User.FindFirst("userName") ?? User.FindFirst("name");
        if (userNameClaim == null)
        {
            throw new UnauthorizedAccessException("无法获取用户名");
        }
        return userNameClaim.Value;
    }
}
