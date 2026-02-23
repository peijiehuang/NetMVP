using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Entities;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 参数配置实体配置
/// </summary>
public class SysConfigConfiguration : IEntityTypeConfiguration<SysConfig>
{
    public void Configure(EntityTypeBuilder<SysConfig> builder)
    {
        builder.ToTable("sys_config");

        builder.HasKey(e => e.ConfigId);
        builder.Property(e => e.ConfigId).HasColumnName("config_id");

        builder.Property(e => e.ConfigName).HasColumnName("config_name").HasMaxLength(100);
        builder.Property(e => e.ConfigKey).HasColumnName("config_key").HasMaxLength(100);
        builder.Property(e => e.ConfigValue).HasColumnName("config_value").HasMaxLength(500);
        builder.Property(e => e.ConfigType).HasColumnName("config_type").HasMaxLength(1).HasDefaultValue(CommonConstants.NO);

        // BaseEntity 字段
        builder.Property(e => e.CreateBy).HasColumnName("create_by").HasMaxLength(64);
        builder.Property(e => e.CreateTime).HasColumnName("create_time");
        builder.Property(e => e.UpdateBy).HasColumnName("update_by").HasMaxLength(64);
        builder.Property(e => e.UpdateTime).HasColumnName("update_time");
        builder.Property(e => e.Remark).HasColumnName("remark").HasMaxLength(500);

        // 索引
        builder.HasIndex(e => e.ConfigKey).IsUnique();
    }
}
