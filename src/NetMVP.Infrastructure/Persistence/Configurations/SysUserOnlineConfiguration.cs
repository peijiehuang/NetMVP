using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Entities;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 在线用户实体配置
/// </summary>
public class SysUserOnlineConfiguration : IEntityTypeConfiguration<SysUserOnline>
{
    public void Configure(EntityTypeBuilder<SysUserOnline> builder)
    {
        builder.ToTable("sys_user_online");

        builder.HasKey(e => e.TokenId);
        builder.Property(e => e.TokenId).HasColumnName("tokenId").HasMaxLength(32);

        builder.Property(e => e.UserName).HasColumnName("user_name").HasMaxLength(50);
        builder.Property(e => e.IpAddrValue).HasColumnName("ipaddr").HasMaxLength(128);
        builder.Property(e => e.LoginLocation).HasColumnName("login_location").HasMaxLength(255);
        builder.Property(e => e.Browser).HasColumnName("browser").HasMaxLength(50);
        builder.Property(e => e.Os).HasColumnName("os").HasMaxLength(50);
        builder.Property(e => e.LoginTime).HasColumnName("login_time");

        // 忽略导航属性（值对象）
        builder.Ignore(e => e.IpAddr);
    }
}
