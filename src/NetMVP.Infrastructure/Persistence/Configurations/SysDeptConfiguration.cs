using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Infrastructure.Persistence.Converters;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 部门实体配置
/// </summary>
public class SysDeptConfiguration : IEntityTypeConfiguration<SysDept>
{
    public void Configure(EntityTypeBuilder<SysDept> builder)
    {
        builder.ToTable("sys_dept");

        builder.HasKey(e => e.DeptId);
        builder.Property(e => e.DeptId).HasColumnName("dept_id");

        builder.Property(e => e.ParentId).HasColumnName("parent_id").HasDefaultValue(0);
        builder.Property(e => e.Ancestors).HasColumnName("ancestors").HasMaxLength(50);
        builder.Property(e => e.DeptName).HasColumnName("dept_name").HasMaxLength(30).IsRequired();
        builder.Property(e => e.OrderNum).HasColumnName("order_num").HasDefaultValue(0);
        builder.Property(e => e.Leader).HasColumnName("leader").HasMaxLength(20);
        builder.Property(e => e.PhoneValue).HasColumnName("phone").HasMaxLength(11);
        builder.Property(e => e.EmailValue).HasColumnName("email").HasMaxLength(50);
        builder.Property(e => e.Status).HasColumnName("status").HasConversion(EnumConverters.UserStatusConverter).HasDefaultValue(UserStatus.Normal);
        builder.Property(e => e.DelFlag).HasColumnName("del_flag").HasConversion(EnumConverters.DelFlagConverter).HasDefaultValue(DelFlag.Exist);

        // BaseEntity 字段
        builder.Property(e => e.CreateBy).HasColumnName("create_by").HasMaxLength(64);
        builder.Property(e => e.CreateTime).HasColumnName("create_time");
        builder.Property(e => e.UpdateBy).HasColumnName("update_by").HasMaxLength(64);
        builder.Property(e => e.UpdateTime).HasColumnName("update_time");
        // sys_dept表没有remark字段
        builder.Ignore(e => e.Remark);

        // 忽略导航属性（值对象）
        builder.Ignore(e => e.Phone);
        builder.Ignore(e => e.Email);
        builder.Ignore(e => e.Children);
    }
}
