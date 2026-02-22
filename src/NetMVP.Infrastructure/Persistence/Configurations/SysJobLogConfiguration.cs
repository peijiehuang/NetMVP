using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Entities;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 定时任务日志实体配置
/// </summary>
public class SysJobLogConfiguration : IEntityTypeConfiguration<SysJobLog>
{
    public void Configure(EntityTypeBuilder<SysJobLog> builder)
    {
        builder.ToTable("sys_job_log");

        builder.HasKey(e => e.JobLogId);
        builder.Property(e => e.JobLogId).HasColumnName("job_log_id");

        builder.Property(e => e.JobName).HasColumnName("job_name").HasMaxLength(64).IsRequired();
        builder.Property(e => e.JobGroup).HasColumnName("job_group").HasMaxLength(64).IsRequired();
        builder.Property(e => e.InvokeTarget).HasColumnName("invoke_target").HasMaxLength(500).IsRequired();
        builder.Property(e => e.JobMessage).HasColumnName("job_message").HasMaxLength(500);
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(1).HasDefaultValue("0");
        builder.Property(e => e.ExceptionInfo).HasColumnName("exception_info").HasMaxLength(2000);
        builder.Property(e => e.CreateTime).HasColumnName("create_time");
    }
}
