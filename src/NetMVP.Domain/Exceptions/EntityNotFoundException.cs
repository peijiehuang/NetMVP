namespace NetMVP.Domain.Exceptions;

/// <summary>
/// 实体未找到异常
/// </summary>
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object key)
        : base($"实体 '{entityName}' ({key}) 未找到")
    {
    }

    public EntityNotFoundException(string message) : base(message)
    {
    }
}
