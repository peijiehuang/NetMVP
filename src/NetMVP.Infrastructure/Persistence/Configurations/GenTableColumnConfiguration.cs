using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Entities;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 代码生成表字段实体配置
/// </summary>
public class GenTableColumnConfiguration : IEntityTypeConfiguration<GenTableColumn>
{
    public void Configure(EntityTypeBuilder<GenTableColumn> builder)
    {
        builder.ToTable("gen_table_column");

        builder.HasKey(e => e.ColumnId);
        builder.Property(e => e.ColumnId).HasColumnName("column_id");

        builder.Property(e => e.TableId).HasColumnName("table_id");
        builder.Property(e => e.ColumnName).HasColumnName("column_name").HasMaxLength(200);
        builder.Property(e => e.ColumnComment).HasColumnName("column_comment").HasMaxLength(500);
        builder.Property(e => e.ColumnType).HasColumnName("column_type").HasMaxLength(100);
        builder.Property(e => e.CSharpType).HasColumnName("csharp_type").HasMaxLength(500);
        builder.Property(e => e.CSharpField).HasColumnName("csharp_field").HasMaxLength(200);
        builder.Property(e => e.IsPk).HasColumnName("is_pk").HasMaxLength(1);
        builder.Property(e => e.IsIncrement).HasColumnName("is_increment").HasMaxLength(1);
        builder.Property(e => e.IsRequired).HasColumnName("is_required").HasMaxLength(1);
        builder.Property(e => e.IsInsert).HasColumnName("is_insert").HasMaxLength(1);
        builder.Property(e => e.IsEdit).HasColumnName("is_edit").HasMaxLength(1);
        builder.Property(e => e.IsList).HasColumnName("is_list").HasMaxLength(1);
        builder.Property(e => e.IsQuery).HasColumnName("is_query").HasMaxLength(1);
        builder.Property(e => e.QueryType).HasColumnName("query_type").HasMaxLength(200);
        builder.Property(e => e.HtmlType).HasColumnName("html_type").HasMaxLength(200);
        builder.Property(e => e.DictType).HasColumnName("dict_type").HasMaxLength(200);
        builder.Property(e => e.Sort).HasColumnName("sort");

        // BaseEntity 字段
        builder.Property(e => e.CreateBy).HasColumnName("create_by").HasMaxLength(64);
        builder.Property(e => e.CreateTime).HasColumnName("create_time");
        builder.Property(e => e.UpdateBy).HasColumnName("update_by").HasMaxLength(64);
        builder.Property(e => e.UpdateTime).HasColumnName("update_time");
    }
}
