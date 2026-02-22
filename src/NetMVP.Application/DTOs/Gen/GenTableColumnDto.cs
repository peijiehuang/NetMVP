namespace NetMVP.Application.DTOs.Gen;

/// <summary>
/// 代码生成表字段DTO
/// </summary>
public class GenTableColumnDto
{
    /// <summary>
    /// 编号
    /// </summary>
    public long ColumnId { get; set; }

    /// <summary>
    /// 归属表编号
    /// </summary>
    public long TableId { get; set; }

    /// <summary>
    /// 列名称
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// 列描述
    /// </summary>
    public string? ColumnComment { get; set; }

    /// <summary>
    /// 列类型
    /// </summary>
    public string ColumnType { get; set; } = string.Empty;

    /// <summary>
    /// C#类型
    /// </summary>
    public string? CSharpType { get; set; }

    /// <summary>
    /// C#字段名
    /// </summary>
    public string CSharpField { get; set; } = string.Empty;

    /// <summary>
    /// 是否主键（1是）
    /// </summary>
    public string? IsPk { get; set; }

    /// <summary>
    /// 是否自增（1是）
    /// </summary>
    public string? IsIncrement { get; set; }

    /// <summary>
    /// 是否必填（1是）
    /// </summary>
    public string? IsRequired { get; set; }

    /// <summary>
    /// 是否为插入字段（1是）
    /// </summary>
    public string? IsInsert { get; set; }

    /// <summary>
    /// 是否编辑字段（1是）
    /// </summary>
    public string? IsEdit { get; set; }

    /// <summary>
    /// 是否列表字段（1是）
    /// </summary>
    public string? IsList { get; set; }

    /// <summary>
    /// 是否查询字段（1是）
    /// </summary>
    public string? IsQuery { get; set; }

    /// <summary>
    /// 查询方式
    /// </summary>
    public string QueryType { get; set; } = "EQ";

    /// <summary>
    /// 显示类型
    /// </summary>
    public string? HtmlType { get; set; }

    /// <summary>
    /// 字典类型
    /// </summary>
    public string? DictType { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; set; }

    /// <summary>
    /// 创建者
    /// </summary>
    public string? CreateBy { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdateBy { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}
