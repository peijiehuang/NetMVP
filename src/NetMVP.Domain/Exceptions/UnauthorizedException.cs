namespace NetMVP.Domain.Exceptions;

/// <summary>
/// 未授权异常
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException() : base("未授权访问")
    {
    }

    public UnauthorizedException(string message) : base(message)
    {
    }
}
