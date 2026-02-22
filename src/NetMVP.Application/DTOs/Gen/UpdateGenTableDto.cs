using System.ComponentModel.DataAnnotations;

namespace NetMVP.Application.DTOs.Gen;

/// <summary>
/// 更新代码生成表DTO
/// </summary>
public class UpdateGenTableDto
{
    /// <summary>
    /// 编号
    /// </summary>
    [Required(ErrorMessage = "表ID不能为空")]
    public long TableId { get; set; }

    /// <summary>
    /// 表名称
    /// </summary>
    [Required(ErrorMessage = "表名称不能为空")]
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// 表描述
    /// </summary>
    [Required(ErrorMessage = "表描述不能为空")]
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
    [Required(ErrorMessage = "实体类名称不能为空")]
    public string ClassName { get; set; } = string.Empty;

    /// <summary>
    /// 使用的模板
    /// </summary>
    public string TplCategory { get; set; } = "crud";

    /// <summary>
    /// 前端模板类型
    /// </summary>
    public string? TplWebType { get; set; }

    /// <summary>
    /// 生成包路径
    /// </summary>
    [Required(ErrorMessage = "生成包路径不能为空")]
    public string PackageName { get; set; } = string.Empty;

    /// <summary>
    /// 生成模块名
    /// </summary>
    [Required(ErrorMessage = "生成模块名不能为空")]
    public string ModuleName { get; set; } = string.Empty;

    /// <summary>
    /// 生成业务名
    /// </summary>
    [Required(ErrorMessage = "生成业务名不能为空")]
    public string BusinessName { get; set; } = string.Empty;

    /// <summary>
    /// 生成功能名
    /// </summary>
    [Required(ErrorMessage = "生成功能名不能为空")]
    public string FunctionName { get; set; } = string.Empty;

    /// <summary>
    /// 生成功能作者
    /// </summary>
    [Required(ErrorMessage = "作者不能为空")]
    public string FunctionAuthor { get; set; } = string.Empty;

    /// <summary>
    /// 生成代码方式
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
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 表列信息
    /// </summary>
    public List<GenTableColumnDto> Columns { get; set; } = new();
}
