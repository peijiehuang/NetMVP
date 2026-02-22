using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Entities;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 定时任务实体配置
/// </summary>
public class SysJobConfiguration : IEntityTypeConfiguration<SysJob>
{
    public void Configure(EntityTypeBuilder<SysJob> builder)
    {
        builder.ToTable("sys_job");

        builder.HasKey(e => e.JobId);
        builder.Property(e => e.JobId).HasColumnName("job_id");

        builder.Property(e => e.JobName).HasColumnName("job_name").HasMaxLength(64).IsRequired();
        builder.Property(e => e.JobGroup).HasColumnName("job_group").HasMaxLength(64).IsRequired();
        builder.Property(e => e.InvokeTarget).HasColumnName("invoke_target").HasMaxLength(500).IsRequired();
        builder.Property(e => e.CronExpression).HasColumnName("cron_expression").HasMaxLength(255).IsRequired();
        builder.Property(e => e.MisfirePolicy).HasColumnName("misfire_policy").HasMaxLength(20).HasDefaultValue("3");
        builder.Property(e => e.Concurrent).HasColumnName("concurrent").HasMaxLength(1).HasDefaultValue("1");
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(1).HasDefaultValue("0");
        builder.Property(e => e.Remark).HasColumnName("remark").HasMaxLength(500);

        builder.Property(e => e.CreateBy).HasColumnName("create_by").HasMaxLength(64);
        builder.Property(e => e.CreateTime).HasColumnName("create_time");
        builder.Property(e => e.UpdateBy).HasColumnName("update_by").HasMaxLength(64);
        builder.Property(e => e.UpdateTime).HasColumnName("update_time");
    }
}
