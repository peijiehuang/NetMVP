using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Entities;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 用户实体配置
/// </summary>
public class SysUserConfiguration : IEntityTypeConfiguration<SysUser>
{
    public void Configure(EntityTypeBuilder<SysUser> builder)
    {
        builder.ToTable("sys_user");
        
        builder.HasKey(e => e.UserId);
        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.Property(e => e.DeptId).HasColumnName("dept_id");
        builder.Property(e => e.UserName).HasColumnName("user_name").HasMaxLength(30).IsRequired();
        builder.Property(e => e.NickName).HasColumnName("nick_name").HasMaxLength(30).IsRequired();
        builder.Property(e => e.UserType).HasColumnName("user_type").HasMaxLength(2).HasDefaultValue("00");
        builder.Property(e => e.EmailValue).HasColumnName("email").HasMaxLength(50);
        builder.Property(e => e.PhoneNumberValue).HasColumnName("phonenumber").HasMaxLength(11);
        builder.Property(e => e.Sex).HasColumnName("sex").HasMaxLength(1).HasDefaultValue(UserConstants.SEX_UNKNOWN);
        builder.Property(e => e.Avatar).HasColumnName("avatar").HasMaxLength(100);
        builder.Property(e => e.Password).HasColumnName("password").HasMaxLength(100);
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(1).HasDefaultValue(UserConstants.NORMAL);
        builder.Property(e => e.DelFlag).HasColumnName("del_flag").HasMaxLength(1).HasDefaultValue(UserConstants.DEL_FLAG_EXIST);
        builder.Property(e => e.LoginIpValue).HasColumnName("login_ip").HasMaxLength(128);
        builder.Property(e => e.LoginDate).HasColumnName("login_date");
        builder.Property(e => e.PwdUpdateDate).HasColumnName("pwd_update_date");

        // BaseEntity 字段
        builder.Property(e => e.CreateBy).HasColumnName("create_by").HasMaxLength(64);
        builder.Property(e => e.CreateTime).HasColumnName("create_time");
        builder.Property(e => e.UpdateBy).HasColumnName("update_by").HasMaxLength(64);
        builder.Property(e => e.UpdateTime).HasColumnName("update_time");
        builder.Property(e => e.Remark).HasColumnName("remark").HasMaxLength(500);

        // 忽略导航属性（值对象）
        builder.Ignore(e => e.Email);
        builder.Ignore(e => e.PhoneNumber);
        builder.Ignore(e => e.LoginIp);

        // 配置关系
        builder.HasMany(e => e.UserRoles)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId);

        builder.HasMany(e => e.UserPosts)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId);

        // 索引
        builder.HasIndex(e => e.UserName).IsUnique();
    }
}
