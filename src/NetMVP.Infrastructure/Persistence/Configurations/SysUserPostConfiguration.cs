using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Entities;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 用户岗位关联配置
/// </summary>
public class SysUserPostConfiguration : IEntityTypeConfiguration<SysUserPost>
{
    public void Configure(EntityTypeBuilder<SysUserPost> builder)
    {
        builder.ToTable("sys_user_post");

        builder.HasKey(e => new { e.UserId, e.PostId });

        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.PostId).HasColumnName("post_id");
    }
}
