using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetMVP.Domain.Common;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 代码生成业务表
/// </summary>
[Table("gen_table")]
public class GenTable : BaseEntity
{
    /// <summary>
    /// 编号
    /// </summary>
    [Key]
    [Column("table_id")]
    public long TableId { get; set; }

    /// <summary>
    /// 表名称
    /// </summary>
    [Column("table_name")]
    [MaxLength(200)]
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// 表描述
    /// </summary>
    [Column("table_comment")]
    [MaxLength(500)]
    public string TableComment { get; set; } = string.Empty;

    /// <summary>
    /// 关联子表的表名
    /// </summary>
    [Column("sub_table_name")]
    [MaxLength(64)]
    public string? SubTableName { get; set; }

    /// <summary>
    /// 子表关联的外键名
    /// </summary>
    [Column("sub_table_fk_name")]
    [MaxLength(64)]
    public string? SubTableFkName { get; set; }

    /// <summary>
    /// 实体类名称
    /// </summary>
    [Column("class_name")]
    [MaxLength(100)]
    public string ClassName { get; set; } = string.Empty;

    /// <summary>
    /// 使用的模板（crud单表操作 tree树表操作 sub主子表操作）
    /// </summary>
    [Column("tpl_category")]
    [MaxLength(200)]
    public string TplCategory { get; set; } = "crud";

    /// <summary>
    /// 前端模板类型（element-ui模版 element-plus模版）
    /// </summary>
    [Column("tpl_web_type")]
    [MaxLength(30)]
    public string? TplWebType { get; set; }

    /// <summary>
    /// 生成包路径
    /// </summary>
    [Column("package_name")]
    [MaxLength(100)]
    public string PackageName { get; set; } = string.Empty;

    /// <summary>
    /// 生成模块名
    /// </summary>
    [Column("module_name")]
    [MaxLength(30)]
    public string ModuleName { get; set; } = string.Empty;

    /// <summary>
    /// 生成业务名
    /// </summary>
    [Column("business_name")]
    [MaxLength(30)]
    public string BusinessName { get; set; } = string.Empty;

    /// <summary>
    /// 生成功能名
    /// </summary>
    [Column("function_name")]
    [MaxLength(50)]
    public string FunctionName { get; set; } = string.Empty;

    /// <summary>
    /// 生成功能作者
    /// </summary>
    [Column("function_author")]
    [MaxLength(50)]
    public string FunctionAuthor { get; set; } = string.Empty;

    /// <summary>
    /// 生成代码方式（0zip压缩包 1自定义路径）
    /// </summary>
    [Column("gen_type")]
    [MaxLength(1)]
    public string GenType { get; set; } = "0";

    /// <summary>
    /// 生成路径（不填默认项目路径）
    /// </summary>
    [Column("gen_path")]
    [MaxLength(200)]
    public string GenPath { get; set; } = "/";

    /// <summary>
    /// 其它生成选项
    /// </summary>
    [Column("options")]
    [MaxLength(1000)]
    public string? Options { get; set; }

    /// <summary>
    /// 表列信息
    /// </summary>
    [NotMapped]
    public List<GenTableColumn> Columns { get; set; } = new();

    /// <summary>
    /// 主键信息
    /// </summary>
    [NotMapped]
    public GenTableColumn? PkColumn { get; set; }

    /// <summary>
    /// 子表信息
    /// </summary>
    [NotMapped]
    public GenTable? SubTable { get; set; }

    /// <summary>
    /// 树编码字段
    /// </summary>
    [NotMapped]
    public string? TreeCode { get; set; }

    /// <summary>
    /// 树父编码字段
    /// </summary>
    [NotMapped]
    public string? TreeParentCode { get; set; }

    /// <summary>
    /// 树名称字段
    /// </summary>
    [NotMapped]
    public string? TreeName { get; set; }

    /// <summary>
    /// 上级菜单ID字段
    /// </summary>
    [NotMapped]
    public long? ParentMenuId { get; set; }

    /// <summary>
    /// 上级菜单名称字段
    /// </summary>
    [NotMapped]
    public string? ParentMenuName { get; set; }
}
