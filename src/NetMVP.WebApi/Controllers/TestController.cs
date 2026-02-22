using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;

namespace NetMVP.WebApi.Controllers;

/// <summary>
/// 测试控制器（演示 AjaxResult 用法）
/// </summary>
[Route("api/[controller]")]
public class TestController : BaseController
{
    /// <summary>
    /// 简单成功响应
    /// </summary>
    [HttpGet("success")]
    public AjaxResult TestSuccess()
    {
        return Success();
    }

    /// <summary>
    /// 成功响应带数据
    /// </summary>
    [HttpGet("success-with-data")]
    public AjaxResult TestSuccessWithData()
    {
        var user = new { Id = 1, Name = "张三", Age = 25 };
        return Success(user);
    }

    /// <summary>
    /// 表格数据响应
    /// </summary>
    [HttpGet("table-data")]
    public AjaxResult TestTableData()
    {
        var users = new List<object>
        {
            new { Id = 1, Name = "张三", Age = 25 },
            new { Id = 2, Name = "李四", Age = 30 }
        };
        return GetTableData(users, 100);
    }

    /// <summary>
    /// 自定义字段响应
    /// </summary>
    [HttpGet("custom-fields")]
    public AjaxResult TestCustomFields()
    {
        return Success()
            .Put("token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...")
            .Put("userId", 1)
            .Put("userName", "admin");
    }

    /// <summary>
    /// 错误响应
    /// </summary>
    [HttpGet("error")]
    public AjaxResult TestError()
    {
        return Error("操作失败，请稍后重试");
    }

    /// <summary>
    /// 警告响应
    /// </summary>
    [HttpGet("warn")]
    public AjaxResult TestWarn()
    {
        return Warn("数据已存在");
    }
}
