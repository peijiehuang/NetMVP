using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Entities;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 岗位实体配置
/// </summary>
public class SysPostConfiguration : IEntityTypeConfiguration<SysPost>
{
    public void Configure(EntityTypeBuilder<SysPost> builder)
    {
        builder.ToTable("sys_post");

        builder.HasKey(e => e.PostId);
        builder.Property(e => e.PostId).HasColumnName("post_id");

        builder.Property(e => e.PostCode).HasColumnName("post_code").HasMaxLength(64).IsRequired();
        builder.Property(e => e.PostName).HasColumnName("post_name").HasMaxLength(50).IsRequired();
        builder.Property(e => e.PostSort).HasColumnName("post_sort");
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(1).HasDefaultValue(UserConstants.NORMAL);

        // BaseEntity 字段
        builder.Property(e => e.CreateBy).HasColumnName("create_by").HasMaxLength(64);
        builder.Property(e => e.CreateTime).HasColumnName("create_time");
        builder.Property(e => e.UpdateBy).HasColumnName("update_by").HasMaxLength(64);
        builder.Property(e => e.UpdateTime).HasColumnName("update_time");
        builder.Property(e => e.Remark).HasColumnName("remark").HasMaxLength(500);
    }
}
