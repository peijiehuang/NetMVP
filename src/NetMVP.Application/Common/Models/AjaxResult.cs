using NetMVP.Application.Common.Constants;

namespace NetMVP.Application.Common.Models;

/// <summary>
/// 统一响应结果（模仿若依 AjaxResult）
/// </summary>
public class AjaxResult : Dictionary<string, object>
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int Code
    {
        get => (int)(this.ContainsKey("code") ? this["code"] : HttpStatus.SUCCESS);
        set => this["code"] = value;
    }

    /// <summary>
    /// 消息
    /// </summary>
    public string Msg
    {
        get => (string)(this.ContainsKey("msg") ? this["msg"] : "操作成功");
        set => this["msg"] = value;
    }

    /// <summary>
    /// 默认构造函数
    /// </summary>
    public AjaxResult()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public AjaxResult(int code, string msg)
    {
        Code = code;
        Msg = msg;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public AjaxResult(int code, string msg, object data)
    {
        Code = code;
        Msg = msg;
        this["data"] = data;
    }

    /// <summary>
    /// 成功响应
    /// </summary>
    public static AjaxResult Success()
    {
        return new AjaxResult(HttpStatus.SUCCESS, "操作成功");
    }

    /// <summary>
    /// 成功响应（带数据）
    /// </summary>
    public static AjaxResult Success(object data)
    {
        return new AjaxResult(HttpStatus.SUCCESS, "操作成功", data);
    }

    /// <summary>
    /// 成功响应（自定义消息）
    /// </summary>
    public static AjaxResult Success(string msg)
    {
        return new AjaxResult(HttpStatus.SUCCESS, msg);
    }

    /// <summary>
    /// 成功响应（自定义消息和数据）
    /// </summary>
    public static AjaxResult Success(string msg, object data)
    {
        return new AjaxResult(HttpStatus.SUCCESS, msg, data);
    }

    /// <summary>
    /// 警告响应
    /// </summary>
    public static AjaxResult Warn(string msg)
    {
        return new AjaxResult(HttpStatus.WARN, msg);
    }

    /// <summary>
    /// 警告响应（带数据）
    /// </summary>
    public static AjaxResult Warn(string msg, object data)
    {
        return new AjaxResult(HttpStatus.WARN, msg, data);
    }

    /// <summary>
    /// 错误响应
    /// </summary>
    public static AjaxResult Error()
    {
        return new AjaxResult(HttpStatus.ERROR, "操作失败");
    }

    /// <summary>
    /// 错误响应（自定义消息）
    /// </summary>
    public static AjaxResult Error(string msg)
    {
        return new AjaxResult(HttpStatus.ERROR, msg);
    }

    /// <summary>
    /// 错误响应（自定义消息和数据）
    /// </summary>
    public static AjaxResult Error(string msg, object data)
    {
        return new AjaxResult(HttpStatus.ERROR, msg, data);
    }

    /// <summary>
    /// 错误响应（自定义状态码和消息）
    /// </summary>
    public static AjaxResult Error(int code, string msg)
    {
        return new AjaxResult(code, msg);
    }

    /// <summary>
    /// 添加自定义字段（支持链式调用）
    /// </summary>
    public AjaxResult Put(string key, object value)
    {
        this[key] = value;
        return this;
    }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess()
    {
        return Code == HttpStatus.SUCCESS;
    }

    /// <summary>
    /// 是否警告
    /// </summary>
    public bool IsWarn()
    {
        return Code == HttpStatus.WARN;
    }

    /// <summary>
    /// 是否错误
    /// </summary>
    public bool IsError()
    {
        return Code >= HttpStatus.ERROR;
    }
}
