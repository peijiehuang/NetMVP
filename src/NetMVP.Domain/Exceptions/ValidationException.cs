namespace NetMVP.Domain.Exceptions;

/// <summary>
/// 验证异常
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// 验证错误字典
    /// </summary>
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException() : base("一个或多个验证错误")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(Dictionary<string, string[]> errors) : base("一个或多个验证错误")
    {
        Errors = errors;
    }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }
}
