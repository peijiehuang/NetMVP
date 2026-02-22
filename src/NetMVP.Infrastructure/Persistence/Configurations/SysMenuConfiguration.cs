using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Infrastructure.Persistence.Converters;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 菜单实体配置
/// </summary>
public class SysMenuConfiguration : IEntityTypeConfiguration<SysMenu>
{
    public void Configure(EntityTypeBuilder<SysMenu> builder)
    {
        builder.ToTable("sys_menu");

        builder.HasKey(e => e.MenuId);
        builder.Property(e => e.MenuId).HasColumnName("menu_id");

        builder.Property(e => e.MenuName).HasColumnName("menu_name").HasMaxLength(50).IsRequired();
        builder.Property(e => e.ParentId).HasColumnName("parent_id");
        builder.Property(e => e.OrderNum).HasColumnName("order_num");
        builder.Property(e => e.Path).HasColumnName("path").HasMaxLength(200);
        builder.Property(e => e.Component).HasColumnName("component").HasMaxLength(255);
        builder.Property(e => e.Query).HasColumnName("query").HasMaxLength(255);
        builder.Property(e => e.RouteName).HasColumnName("route_name").HasMaxLength(50);
        builder.Property(e => e.IsFrame).HasColumnName("is_frame");
        builder.Property(e => e.IsCache).HasColumnName("is_cache");
        builder.Property(e => e.MenuType).HasColumnName("menu_type").HasConversion(EnumConverters.MenuTypeConverter);
        builder.Property(e => e.Visible).HasColumnName("visible").HasConversion(EnumConverters.VisibleStatusConverter).HasDefaultValue(VisibleStatus.Show);
        builder.Property(e => e.Status).HasColumnName("status").HasConversion(EnumConverters.UserStatusConverter).HasDefaultValue(UserStatus.Normal);
        builder.Property(e => e.Perms).HasColumnName("perms").HasMaxLength(100);
        builder.Property(e => e.Icon).HasColumnName("icon").HasMaxLength(100);

        // BaseEntity 字段
        builder.Property(e => e.CreateBy).HasColumnName("create_by").HasMaxLength(64);
        builder.Property(e => e.CreateTime).HasColumnName("create_time");
        builder.Property(e => e.UpdateBy).HasColumnName("update_by").HasMaxLength(64);
        builder.Property(e => e.UpdateTime).HasColumnName("update_time");
        builder.Property(e => e.Remark).HasColumnName("remark").HasMaxLength(500);

        // 忽略导航属性
        builder.Ignore(e => e.Children);
    }
}
