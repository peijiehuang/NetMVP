using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Infrastructure.Persistence.Converters;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 角色实体配置
/// </summary>
public class SysRoleConfiguration : IEntityTypeConfiguration<SysRole>
{
    public void Configure(EntityTypeBuilder<SysRole> builder)
    {
        builder.ToTable("sys_role");

        builder.HasKey(e => e.RoleId);
        builder.Property(e => e.RoleId).HasColumnName("role_id");

        builder.Property(e => e.RoleName).HasColumnName("role_name").HasMaxLength(30).IsRequired();
        builder.Property(e => e.RoleKey).HasColumnName("role_key").HasMaxLength(100).IsRequired();
        builder.Property(e => e.RoleSort).HasColumnName("role_sort");
        builder.Property(e => e.DataScope).HasColumnName("data_scope").HasConversion(EnumConverters.DataScopeTypeConverter).HasDefaultValue(DataScopeType.All);
        builder.Property(e => e.MenuCheckStrictly).HasColumnName("menu_check_strictly").HasDefaultValue(true);
        builder.Property(e => e.DeptCheckStrictly).HasColumnName("dept_check_strictly").HasDefaultValue(true);
        builder.Property(e => e.Status).HasColumnName("status").HasConversion(EnumConverters.UserStatusConverter);
        builder.Property(e => e.DelFlag).HasColumnName("del_flag").HasConversion(EnumConverters.DelFlagConverter);

        // BaseEntity 字段
        builder.Property(e => e.CreateBy).HasColumnName("create_by").HasMaxLength(64);
        builder.Property(e => e.CreateTime).HasColumnName("create_time");
        builder.Property(e => e.UpdateBy).HasColumnName("update_by").HasMaxLength(64);
        builder.Property(e => e.UpdateTime).HasColumnName("update_time");
        builder.Property(e => e.Remark).HasColumnName("remark").HasMaxLength(500);

        // 配置关系
        builder.HasMany(e => e.RoleMenus)
            .WithOne(e => e.Role)
            .HasForeignKey(e => e.RoleId);

        builder.HasMany(e => e.RoleDepts)
            .WithOne(e => e.Role)
            .HasForeignKey(e => e.RoleId);
    }
}
