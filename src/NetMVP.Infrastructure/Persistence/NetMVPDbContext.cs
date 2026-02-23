using Microsoft.EntityFrameworkCore;
using NetMVP.Domain.Common;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Entities;

namespace NetMVP.Infrastructure.Persistence;

/// <summary>
/// 数据库上下文
/// </summary>
public class NetMVPDbContext : DbContext
{
    public NetMVPDbContext(DbContextOptions<NetMVPDbContext> options) : base(options)
    {
    }

    // 用户聚合
    public DbSet<SysUser> SysUsers => Set<SysUser>();
    public DbSet<SysUserRole> SysUserRoles => Set<SysUserRole>();
    public DbSet<SysUserPost> SysUserPosts => Set<SysUserPost>();

    // 角色聚合
    public DbSet<SysRole> SysRoles => Set<SysRole>();
    public DbSet<SysRoleMenu> SysRoleMenus => Set<SysRoleMenu>();
    public DbSet<SysRoleDept> SysRoleDepts => Set<SysRoleDept>();

    // 部门聚合
    public DbSet<SysDept> SysDepts => Set<SysDept>();

    // 菜单聚合
    public DbSet<SysMenu> SysMenus => Set<SysMenu>();

    // 岗位聚合
    public DbSet<SysPost> SysPosts => Set<SysPost>();

    // 字典聚合
    public DbSet<SysDictType> SysDictTypes => Set<SysDictType>();
    public DbSet<SysDictData> SysDictData => Set<SysDictData>();

    // 配置聚合
    public DbSet<SysConfig> SysConfigs => Set<SysConfig>();

    // 通知聚合
    public DbSet<SysNotice> SysNotices => Set<SysNotice>();

    // 日志聚合
    public DbSet<SysOperLog> SysOperLogs => Set<SysOperLog>();
    public DbSet<SysLoginInfo> SysLoginInfos => Set<SysLoginInfo>();

    // 在线用户聚合
    public DbSet<SysUserOnline> SysUserOnlines => Set<SysUserOnline>();

    // 定时任务聚合
    public DbSet<SysJob> SysJobs => Set<SysJob>();
    public DbSet<SysJobLog> SysJobLogs => Set<SysJobLog>();

    // 代码生成聚合
    public DbSet<GenTable> GenTables => Set<GenTable>();
    public DbSet<GenTableColumn> GenTableColumns => Set<GenTableColumn>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 应用所有实体配置
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NetMVPDbContext).Assembly);

        // 配置软删除全局过滤器
        ConfigureSoftDeleteFilter(modelBuilder);
    }

    /// <summary>
    /// 配置软删除全局过滤器
    /// </summary>
    private void ConfigureSoftDeleteFilter(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SysUser>().HasQueryFilter(e => e.DelFlag == UserConstants.DEL_FLAG_EXIST);
        modelBuilder.Entity<SysRole>().HasQueryFilter(e => e.DelFlag == UserConstants.DEL_FLAG_EXIST);
        modelBuilder.Entity<SysDept>().HasQueryFilter(e => e.DelFlag == UserConstants.DEL_FLAG_EXIST);
    }
}
