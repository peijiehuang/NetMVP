using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Entities;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 登录日志实体配置
/// </summary>
public class SysLoginInfoConfiguration : IEntityTypeConfiguration<SysLoginInfo>
{
    public void Configure(EntityTypeBuilder<SysLoginInfo> builder)
    {
        builder.ToTable("sys_logininfor");

        builder.HasKey(e => e.InfoId);
        builder.Property(e => e.InfoId).HasColumnName("info_id");

        builder.Property(e => e.UserName).HasColumnName("user_name").HasMaxLength(50);
        builder.Property(e => e.IpAddrValue).HasColumnName("ipaddr").HasMaxLength(128);
        builder.Property(e => e.LoginLocation).HasColumnName("login_location").HasMaxLength(255);
        builder.Property(e => e.Browser).HasColumnName("browser").HasMaxLength(50);
        builder.Property(e => e.Os).HasColumnName("os").HasMaxLength(50);
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(1).HasDefaultValue(CommonConstants.SUCCESS);
        builder.Property(e => e.Msg).HasColumnName("msg").HasMaxLength(255);
        builder.Property(e => e.LoginTime).HasColumnName("login_time");

        // 忽略导航属性（值对象）
        builder.Ignore(e => e.IpAddr);

        // 索引
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.LoginTime);
    }
}
