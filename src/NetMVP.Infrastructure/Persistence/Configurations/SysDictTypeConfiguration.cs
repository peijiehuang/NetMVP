using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Infrastructure.Persistence.Converters;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 字典类型实体配置
/// </summary>
public class SysDictTypeConfiguration : IEntityTypeConfiguration<SysDictType>
{
    public void Configure(EntityTypeBuilder<SysDictType> builder)
    {
        builder.ToTable("sys_dict_type");

        builder.HasKey(e => e.DictId);
        builder.Property(e => e.DictId).HasColumnName("dict_id");

        builder.Property(e => e.DictName).HasColumnName("dict_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.DictType).HasColumnName("dict_type").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasConversion(EnumConverters.UserStatusConverter).HasDefaultValue(UserStatus.Normal);

        // BaseEntity 字段
        builder.Property(e => e.CreateBy).HasColumnName("create_by").HasMaxLength(64);
        builder.Property(e => e.CreateTime).HasColumnName("create_time");
        builder.Property(e => e.UpdateBy).HasColumnName("update_by").HasMaxLength(64);
        builder.Property(e => e.UpdateTime).HasColumnName("update_time");
        builder.Property(e => e.Remark).HasColumnName("remark").HasMaxLength(500);

        // 唯一索引
        builder.HasIndex(e => e.DictType).IsUnique();
    }
}
