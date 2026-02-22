using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Infrastructure.Persistence.Converters;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 操作日志实体配置
/// </summary>
public class SysOperLogConfiguration : IEntityTypeConfiguration<SysOperLog>
{
    public void Configure(EntityTypeBuilder<SysOperLog> builder)
    {
        builder.ToTable("sys_oper_log");

        builder.HasKey(e => e.OperId);
        builder.Property(e => e.OperId).HasColumnName("oper_id");

        builder.Property(e => e.Title).HasColumnName("title").HasMaxLength(50);
        builder.Property(e => e.BusinessType).HasColumnName("business_type").HasConversion(EnumConverters.BusinessTypeConverter);
        builder.Property(e => e.Method).HasColumnName("method").HasMaxLength(100);
        builder.Property(e => e.RequestMethod).HasColumnName("request_method").HasMaxLength(10);
        builder.Property(e => e.OperatorType).HasColumnName("operator_type").HasConversion(EnumConverters.OperatorTypeConverter);
        builder.Property(e => e.OperName).HasColumnName("oper_name").HasMaxLength(50);
        builder.Property(e => e.DeptName).HasColumnName("dept_name").HasMaxLength(50);
        builder.Property(e => e.OperUrl).HasColumnName("oper_url").HasMaxLength(255);
        builder.Property(e => e.OperIpValue).HasColumnName("oper_ip").HasMaxLength(128);
        builder.Property(e => e.OperLocation).HasColumnName("oper_location").HasMaxLength(255);
        builder.Property(e => e.OperParam).HasColumnName("oper_param").HasMaxLength(2000);
        builder.Property(e => e.JsonResult).HasColumnName("json_result").HasMaxLength(2000);
        builder.Property(e => e.Status).HasColumnName("status").HasConversion(EnumConverters.CommonStatusConverter);
        builder.Property(e => e.ErrorMsg).HasColumnName("error_msg").HasMaxLength(2000);
        builder.Property(e => e.OperTime).HasColumnName("oper_time");
        builder.Property(e => e.CostTime).HasColumnName("cost_time");

        // 忽略导航属性（值对象）
        builder.Ignore(e => e.OperIp);

        // 索引
        builder.HasIndex(e => e.BusinessType);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.OperTime);
    }
}
