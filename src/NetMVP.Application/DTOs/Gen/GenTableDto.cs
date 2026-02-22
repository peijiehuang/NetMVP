namespace NetMVP.Application.DTOs.Gen;

/// <summary>
/// 代码生成表DTO
/// </summary>
public class GenTableDto
{
    /// <summary>
    /// 编号
    /// </summary>
    public long TableId { get; set; }

    /// <summary>
    /// 表名称
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// 表描述
    /// </summary>
    public string TableComment { get; set; } = string.Empty;

    /// <summary>
    /// 关联子表的表名
    /// </summary>
    public string? SubTableName { get; set; }

    /// <summary>
    /// 子表关联的外键名
    /// </summary>
    public string? SubTableFkName { get; set; }

    /// <summary>
    /// 实体类名称
    /// </summary>
    public string ClassName { get; set; } = string.Empty;

    /// <summary>
    /// 使用的模板（crud单表操作 tree树表操作 sub主子表操作）
    /// </summary>
    public string TplCategory { get; set; } = "crud";

    /// <summary>
    /// 前端模板类型
    /// </summary>
    public string? TplWebType { get; set; }

    /// <summary>
    /// 生成包路径
    /// </summary>
    public string PackageName { get; set; } = string.Empty;

    /// <summary>
    /// 生成模块名
    /// </summary>
    public string ModuleName { get; set; } = string.Empty;

    /// <summary>
    /// 生成业务名
    /// </summary>
    public string BusinessName { get; set; } = string.Empty;

    /// <summary>
    /// 生成功能名
    /// </summary>
    public string FunctionName { get; set; } = string.Empty;

    /// <summary>
    /// 生成功能作者
    /// </summary>
    public string FunctionAuthor { get; set; } = string.Empty;

    /// <summary>
    /// 生成代码方式（0zip压缩包 1自定义路径）
    /// </summary>
    public string GenType { get; set; } = "0";

    /// <summary>
    /// 生成路径
    /// </summary>
    public string GenPath { get; set; } = "/";

    /// <summary>
    /// 其它生成选项
    /// </summary>
    public string? Options { get; set; }

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

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 表列信息
    /// </summary>
    public List<GenTableColumnDto> Columns { get; set; } = new();

    /// <summary>
    /// 主键信息
    /// </summary>
    public GenTableColumnDto? PkColumn { get; set; }

    /// <summary>
    /// 树编码字段
    /// </summary>
    public string? TreeCode { get; set; }

    /// <summary>
    /// 树父编码字段
    /// </summary>
    public string? TreeParentCode { get; set; }

    /// <summary>
    /// 树名称字段
    /// </summary>
    public string? TreeName { get; set; }

    /// <summary>
    /// 上级菜单ID
    /// </summary>
    public long? ParentMenuId { get; set; }

    /// <summary>
    /// 上级菜单名称
    /// </summary>
    public string? ParentMenuName { get; set; }
}
