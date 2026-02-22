using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Entities;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 角色菜单关联配置
/// </summary>
public class SysRoleMenuConfiguration : IEntityTypeConfiguration<SysRoleMenu>
{
    public void Configure(EntityTypeBuilder<SysRoleMenu> builder)
    {
        builder.ToTable("sys_role_menu");

        builder.HasKey(e => new { e.RoleId, e.MenuId });

        builder.Property(e => e.RoleId).HasColumnName("role_id");
        builder.Property(e => e.MenuId).HasColumnName("menu_id");
    }
}
