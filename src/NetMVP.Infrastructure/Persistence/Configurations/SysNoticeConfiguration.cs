using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Infrastructure.Persistence.Converters;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 通知公告实体配置
/// </summary>
public class SysNoticeConfiguration : IEntityTypeConfiguration<SysNotice>
{
    public void Configure(EntityTypeBuilder<SysNotice> builder)
    {
        builder.ToTable("sys_notice");

        builder.HasKey(e => e.NoticeId);
        builder.Property(e => e.NoticeId).HasColumnName("notice_id");

        builder.Property(e => e.NoticeTitle).HasColumnName("notice_title").HasMaxLength(50).IsRequired();
        builder.Property(e => e.NoticeType).HasColumnName("notice_type").HasConversion(EnumConverters.NoticeTypeConverter);
        builder.Property(e => e.NoticeContent).HasColumnName("notice_content");
        builder.Property(e => e.Status).HasColumnName("status").HasConversion(EnumConverters.NoticeStatusConverter);

        // BaseEntity 字段
        builder.Property(e => e.CreateBy).HasColumnName("create_by").HasMaxLength(64);
        builder.Property(e => e.CreateTime).HasColumnName("create_time");
        builder.Property(e => e.UpdateBy).HasColumnName("update_by").HasMaxLength(64);
        builder.Property(e => e.UpdateTime).HasColumnName("update_time");
        builder.Property(e => e.Remark).HasColumnName("remark").HasMaxLength(500);
    }
}
