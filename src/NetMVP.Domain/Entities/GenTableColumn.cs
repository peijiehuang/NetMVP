using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetMVP.Domain.Common;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 代码生成业务表字段
/// </summary>
[Table("gen_table_column")]
public class GenTableColumn : BaseEntity
{
    /// <summary>
    /// 编号
    /// </summary>
    [Key]
    [Column("column_id")]
    public long ColumnId { get; set; }

    /// <summary>
    /// 归属表编号
    /// </summary>
    [Column("table_id")]
    public long TableId { get; set; }

    /// <summary>
    /// 列名称
    /// </summary>
    [Column("column_name")]
    [MaxLength(200)]
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// 列描述
    /// </summary>
    [Column("column_comment")]
    [MaxLength(500)]
    public string? ColumnComment { get; set; }

    /// <summary>
    /// 列类型
    /// </summary>
    [Column("column_type")]
    [MaxLength(100)]
    public string ColumnType { get; set; } = string.Empty;

    /// <summary>
    /// C#类型（数据库字段名保持java_type以兼容若依）
    /// </summary>
    [Column("java_type")]
    [MaxLength(500)]
    public string? CSharpType { get; set; }

    /// <summary>
    /// C#字段名（数据库字段名保持java_field以兼容若依）
    /// </summary>
    [Column("java_field")]
    [MaxLength(200)]
    public string CSharpField { get; set; } = string.Empty;

    /// <summary>
    /// 是否主键（1是）
    /// </summary>
    [Column("is_pk")]
    [MaxLength(1)]
    public string? IsPk { get; set; }

    /// <summary>
    /// 是否自增（1是）
    /// </summary>
    [Column("is_increment")]
    [MaxLength(1)]
    public string? IsIncrement { get; set; }

    /// <summary>
    /// 是否必填（1是）
    /// </summary>
    [Column("is_required")]
    [MaxLength(1)]
    public string? IsRequired { get; set; }

    /// <summary>
    /// 是否为插入字段（1是）
    /// </summary>
    [Column("is_insert")]
    [MaxLength(1)]
    public string? IsInsert { get; set; }

    /// <summary>
    /// 是否编辑字段（1是）
    /// </summary>
    [Column("is_edit")]
    [MaxLength(1)]
    public string? IsEdit { get; set; }

    /// <summary>
    /// 是否列表字段（1是）
    /// </summary>
    [Column("is_list")]
    [MaxLength(1)]
    public string? IsList { get; set; }

    /// <summary>
    /// 是否查询字段（1是）
    /// </summary>
    [Column("is_query")]
    [MaxLength(1)]
    public string? IsQuery { get; set; }

    /// <summary>
    /// 查询方式（EQ等于、NE不等于、GT大于、LT小于、LIKE模糊、BETWEEN范围）
    /// </summary>
    [Column("query_type")]
    [MaxLength(200)]
    public string QueryType { get; set; } = "EQ";

    /// <summary>
    /// 显示类型（input文本框、textarea文本域、select下拉框、checkbox复选框、radio单选框、datetime日期控件、image图片上传控件、upload文件上传控件、editor富文本控件）
    /// </summary>
    [Column("html_type")]
    [MaxLength(200)]
    public string? HtmlType { get; set; }

    /// <summary>
    /// 字典类型
    /// </summary>
    [Column("dict_type")]
    [MaxLength(200)]
    public string? DictType { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Column("sort")]
    public int? Sort { get; set; }
}
