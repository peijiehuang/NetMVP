using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Infrastructure.Persistence.Converters;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 字典数据实体配置
/// </summary>
public class SysDictDataConfiguration : IEntityTypeConfiguration<SysDictData>
{
    public void Configure(EntityTypeBuilder<SysDictData> builder)
    {
        builder.ToTable("sys_dict_data");

        builder.HasKey(e => e.DictCode);
        builder.Property(e => e.DictCode).HasColumnName("dict_code");

        builder.Property(e => e.DictSort).HasColumnName("dict_sort");
        builder.Property(e => e.DictLabel).HasColumnName("dict_label").HasMaxLength(100).IsRequired();
        builder.Property(e => e.DictValue).HasColumnName("dict_value").HasMaxLength(100).IsRequired();
        builder.Property(e => e.DictType).HasColumnName("dict_type").HasMaxLength(100).IsRequired();
        builder.Property(e => e.CssClass).HasColumnName("css_class").HasMaxLength(100);
        builder.Property(e => e.ListClass).HasColumnName("list_class").HasMaxLength(100);
        builder.Property(e => e.IsDefault).HasColumnName("is_default").HasConversion(EnumConverters.YesNoConverter).HasDefaultValue(YesNo.No);
        builder.Property(e => e.Status).HasColumnName("status").HasConversion(EnumConverters.UserStatusConverter).HasDefaultValue(UserStatus.Normal);

        // BaseEntity 字段
        builder.Property(e => e.CreateBy).HasColumnName("create_by").HasMaxLength(64);
        builder.Property(e => e.CreateTime).HasColumnName("create_time");
        builder.Property(e => e.UpdateBy).HasColumnName("update_by").HasMaxLength(64);
        builder.Property(e => e.UpdateTime).HasColumnName("update_time");
        builder.Property(e => e.Remark).HasColumnName("remark").HasMaxLength(500);
    }
}
