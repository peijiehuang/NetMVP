namespace NetMVP.Domain.Exceptions;

/// <summary>
/// 禁止访问异常
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException() : base("禁止访问")
    {
    }

    public ForbiddenException(string message) : base(message)
    {
    }
}
