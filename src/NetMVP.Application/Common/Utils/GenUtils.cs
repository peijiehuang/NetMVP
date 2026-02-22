using NetMVP.Application.Common.Constants;
using NetMVP.Domain.Entities;
using System.Text;
using System.Text.RegularExpressions;

namespace NetMVP.Application.Common.Utils;

/// <summary>
/// 代码生成工具类
/// </summary>
public static class GenUtils
{
    /// <summary>
    /// 初始化表信息
    /// </summary>
    public static void InitTable(GenTable genTable, string operName)
    {
        genTable.ClassName = ConvertClassName(genTable.TableName);
        genTable.PackageName = "NetMVP";
        genTable.ModuleName = GetModuleName(genTable.TableName);
        genTable.BusinessName = GetBusinessName(genTable.TableName);
        genTable.FunctionName = genTable.TableComment ?? genTable.TableName;
        genTable.FunctionAuthor = operName;
        genTable.CreateBy = operName;
        genTable.CreateTime = DateTime.Now;
    }

    /// <summary>
    /// 初始化列属性字段
    /// </summary>
    public static void InitColumnField(GenTableColumn column, GenTable table)
    {
        var dataType = GetDbType(column.ColumnType);
        var columnName = column.ColumnName;
        
        column.TableId = table.TableId;
        column.CreateBy = table.CreateBy;
        column.CreateTime = DateTime.Now;
        
        // 设置C#字段名（PascalCase，首字母大写）
        column.CSharpField = ToPascalCase(columnName);
        
        // 设置C#类型
        column.CSharpType = GetCSharpType(dataType);
        
        // 设置查询类型
        if (IsStringColumn(dataType))
        {
            column.QueryType = "LIKE";
        }
        else
        {
            column.QueryType = "EQ";
        }
        
        // 设置HTML类型
        column.HtmlType = GetHtmlType(dataType, column.ColumnComment);
        
        // 设置是否插入/编辑/列表/查询
        if (!IsSuperColumn(columnName))
        {
            column.IsInsert = "1";
            column.IsEdit = "1";
            column.IsList = "1";
            column.IsQuery = "1";
        }
    }

    /// <summary>
    /// 表名转换成类名
    /// </summary>
    public static string ConvertClassName(string tableName)
    {
        var autoRemovePre = true;
        var tablePrefix = "sys_";
        
        if (autoRemovePre && !string.IsNullOrEmpty(tablePrefix))
        {
            var prefixes = tablePrefix.Split(',');
            foreach (var prefix in prefixes)
            {
                if (tableName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    tableName = tableName.Substring(prefix.Length);
                    break;
                }
            }
        }
        
        return ToPascalCase(tableName);
    }

    /// <summary>
    /// 获取模块名
    /// </summary>
    public static string GetModuleName(string tableName)
    {
        var index = tableName.IndexOf('_');
        return index > 0 ? tableName.Substring(0, index) : tableName;
    }

    /// <summary>
    /// 获取业务名
    /// </summary>
    public static string GetBusinessName(string tableName)
    {
        var index = tableName.LastIndexOf('_');
        return index > 0 ? tableName.Substring(index + 1) : tableName;
    }

    /// <summary>
    /// 获取数据库类型（去除长度）
    /// </summary>
    public static string GetDbType(string columnType)
    {
        if (string.IsNullOrEmpty(columnType))
        {
            return "";
        }
        
        var index = columnType.IndexOf('(');
        return index > 0 ? columnType.Substring(0, index) : columnType;
    }

    /// <summary>
    /// MySQL类型转C#类型
    /// </summary>
    public static string GetCSharpType(string dbType)
    {
        return dbType.ToLower() switch
        {
            "bigint" => "long",
            "int" or "integer" or "mediumint" => "int",
            "smallint" => "short",
            "tinyint" => "byte",
            "bit" => "bool",
            "decimal" or "numeric" => "decimal",
            "float" => "float",
            "double" => "double",
            "date" or "datetime" or "timestamp" => "DateTime?",
            "time" => "TimeSpan?",
            "char" or "varchar" or "text" or "longtext" or "mediumtext" or "tinytext" => "string",
            _ => "string"
        };
    }

    /// <summary>
    /// 获取C#类型的默认值
    /// </summary>
    public static string GetCSharpDefaultValue(string csharpType)
    {
        return csharpType switch
        {
            "string" => "string.Empty",
            "long" => "0L",
            "int" => "0",
            "short" => "0",
            "byte" => "0",
            "bool" => "false",
            "decimal" => "0M",
            "float" => "0F",
            "double" => "0D",
            "DateTime?" => "null",
            "TimeSpan?" => "null",
            _ => "null"
        };
    }

    /// <summary>
    /// 是否字符串类型
    /// </summary>
    public static bool IsStringColumn(string dbType)
    {
        return dbType.ToLower() switch
        {
            "char" or "varchar" or "text" or "longtext" or "mediumtext" or "tinytext" => true,
            _ => false
        };
    }

    /// <summary>
    /// 获取HTML类型
    /// </summary>
    public static string GetHtmlType(string dbType, string? comment)
    {
        if (IsStringColumn(dbType) && !string.IsNullOrEmpty(comment) && comment.Contains("（") && comment.Contains("）"))
        {
            return "select";
        }
        
        return dbType.ToLower() switch
        {
            "text" or "longtext" or "mediumtext" => "textarea",
            "date" or "datetime" or "timestamp" => "datetime",
            _ => "input"
        };
    }

    /// <summary>
    /// 是否基类字段
    /// </summary>
    public static bool IsSuperColumn(string columnName)
    {
        return GenConstants.BASE_ENTITY.Contains(ToPascalCase(columnName));
    }

    /// <summary>
    /// 是否树表字段
    /// </summary>
    public static bool IsTreeColumn(string columnName)
    {
        return GenConstants.TREE_ENTITY.Contains(ToPascalCase(columnName));
    }

    /// <summary>
    /// 获取模板类型描述
    /// </summary>
    public static string GetTplCategoryDesc(string tplCategory)
    {
        return tplCategory switch
        {
            GenConstants.TPL_CRUD => "单表（增删改查）",
            GenConstants.TPL_TREE => "树表（增删改查）",
            GenConstants.TPL_SUB => "主子表（增删改查）",
            _ => "未知"
        };
    }

    /// <summary>
    /// 获取查询类型描述
    /// </summary>
    public static string GetQueryTypeDesc(string queryType)
    {
        return queryType switch
        {
            "EQ" => "等于",
            "NE" => "不等于",
            "GT" => "大于",
            "GTE" => "大于等于",
            "LT" => "小于",
            "LTE" => "小于等于",
            "LIKE" => "模糊",
            "BETWEEN" => "范围",
            _ => "等于"
        };
    }

    /// <summary>
    /// 下划线转驼峰（首字母小写，用于变量名）
    /// </summary>
    public static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }
        
        // 先转换为PascalCase
        var pascal = ToPascalCase(str);
        // 首字母小写
        return char.ToLower(pascal[0]) + pascal.Substring(1);
    }

    /// <summary>
    /// 下划线转帕斯卡（首字母大写，用于类名、属性名）
    /// </summary>
    public static string ToPascalCase(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }
        
        // 按下划线分割
        var parts = str.Split('_', StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder();
        
        foreach (var part in parts)
        {
            if (part.Length > 0)
            {
                // 每个部分首字母大写
                result.Append(char.ToUpper(part[0]));
                if (part.Length > 1)
                {
                    result.Append(part.Substring(1).ToLower());
                }
            }
        }
        
        return result.ToString();
    }

    /// <summary>
    /// 获取C#属性注释
    /// </summary>
    public static string GetCSharpComment(string? comment)
    {
        if (string.IsNullOrEmpty(comment))
        {
            return string.Empty;
        }
        
        // 移除特殊字符
        return comment.Replace("（", "(").Replace("）", ")");
    }

    /// <summary>
    /// 判断是否需要可空类型
    /// </summary>
    public static bool IsNullableType(string csharpType, string? isRequired)
    {
        // 如果已经是可空类型，直接返回false
        if (csharpType.EndsWith("?"))
        {
            return false;
        }
        
        // string类型不需要加?
        if (csharpType == "string")
        {
            return false;
        }
        
        // 如果不是必填，值类型需要加?
        if (isRequired != "1")
        {
            return true;
        }
        
        return false;
    }
}
