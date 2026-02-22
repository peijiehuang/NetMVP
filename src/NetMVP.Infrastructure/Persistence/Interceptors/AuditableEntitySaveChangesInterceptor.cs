using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetMVP.Domain.Common;

namespace NetMVP.Infrastructure.Persistence.Interceptors;

/// <summary>
/// 审计实体保存拦截器
/// 自动填充创建和更新的审计字段
/// </summary>
public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// 更新实体的审计字段
    /// </summary>
    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries<BaseEntity>();
        var currentTime = DateTime.Now;
        var currentUser = GetCurrentUser();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreateTime = currentTime;
                entry.Entity.CreateBy = currentUser;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Entity.UpdateTime = currentTime;
                entry.Entity.UpdateBy = currentUser;
            }
        }
    }

    /// <summary>
    /// 获取当前用户
    /// TODO: 从 HttpContext 或 ICurrentUserService 获取当前登录用户
    /// </summary>
    private string GetCurrentUser()
    {
        // 暂时返回系统用户，后续集成身份认证后从上下文获取
        return "system";
    }
}
