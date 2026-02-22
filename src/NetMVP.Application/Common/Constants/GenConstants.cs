namespace NetMVP.Application.Common.Constants;

/// <summary>
/// 代码生成常量
/// </summary>
public static class GenConstants
{
    /// <summary>
    /// 单表（增删改查）
    /// </summary>
    public const string TPL_CRUD = "crud";

    /// <summary>
    /// 树表（增删改查）
    /// </summary>
    public const string TPL_TREE = "tree";

    /// <summary>
    /// 主子表（增删改查）
    /// </summary>
    public const string TPL_SUB = "sub";

    /// <summary>
    /// 树编码字段
    /// </summary>
    public const string TREE_CODE = "treeCode";

    /// <summary>
    /// 树父编码字段
    /// </summary>
    public const string TREE_PARENT_CODE = "treeParentCode";

    /// <summary>
    /// 树名称字段
    /// </summary>
    public const string TREE_NAME = "treeName";

    /// <summary>
    /// 上级菜单ID字段
    /// </summary>
    public const string PARENT_MENU_ID = "parentMenuId";

    /// <summary>
    /// 上级菜单名称字段
    /// </summary>
    public const string PARENT_MENU_NAME = "parentMenuName";

    /// <summary>
    /// 数据库字符串类型
    /// </summary>
    public static readonly string[] COLUMNTYPE_STR = { "char", "varchar", "text", "tinytext", "mediumtext", "longtext" };

    /// <summary>
    /// 数据库文本类型
    /// </summary>
    public static readonly string[] COLUMNTYPE_TEXT = { "text", "tinytext", "mediumtext", "longtext" };

    /// <summary>
    /// 数据库时间类型
    /// </summary>
    public static readonly string[] COLUMNTYPE_TIME = { "datetime", "time", "date", "timestamp" };

    /// <summary>
    /// 数据库数字类型
    /// </summary>
    public static readonly string[] COLUMNTYPE_NUMBER = { "tinyint", "smallint", "mediumint", "int", "bigint", "float", "double", "decimal" };

    /// <summary>
    /// 页面不需要编辑字段
    /// </summary>
    public static readonly string[] COLUMNNAME_NOT_EDIT = { "id", "create_by", "create_time", "update_by", "update_time" };

    /// <summary>
    /// 页面不需要显示的列表字段
    /// </summary>
    public static readonly string[] COLUMNNAME_NOT_LIST = { "id", "create_by", "create_time", "update_by", "update_time", "remark" };

    /// <summary>
    /// 页面不需要查询字段
    /// </summary>
    public static readonly string[] COLUMNNAME_NOT_QUERY = { "id", "create_by", "create_time", "update_by", "update_time", "remark" };

    /// <summary>
    /// Entity基类字段
    /// </summary>
    public static readonly string[] BASE_ENTITY = { "createBy", "createTime", "updateBy", "updateTime", "remark" };

    /// <summary>
    /// Tree基类字段
    /// </summary>
    public static readonly string[] TREE_ENTITY = { "parentName", "parentId", "orderNum", "ancestors", "children" };

    /// <summary>
    /// 文本框
    /// </summary>
    public const string HTML_INPUT = "input";

    /// <summary>
    /// 文本域
    /// </summary>
    public const string HTML_TEXTAREA = "textarea";

    /// <summary>
    /// 下拉框
    /// </summary>
    public const string HTML_SELECT = "select";

    /// <summary>
    /// 单选框
    /// </summary>
    public const string HTML_RADIO = "radio";

    /// <summary>
    /// 复选框
    /// </summary>
    public const string HTML_CHECKBOX = "checkbox";

    /// <summary>
    /// 日期控件
    /// </summary>
    public const string HTML_DATETIME = "datetime";

    /// <summary>
    /// 图片上传控件
    /// </summary>
    public const string HTML_IMAGE_UPLOAD = "imageUpload";

    /// <summary>
    /// 文件上传控件
    /// </summary>
    public const string HTML_FILE_UPLOAD = "fileUpload";

    /// <summary>
    /// 富文本控件
    /// </summary>
    public const string HTML_EDITOR = "editor";

    /// <summary>
    /// 字符串类型
    /// </summary>
    public const string TYPE_STRING = "string";

    /// <summary>
    /// 整型
    /// </summary>
    public const string TYPE_INTEGER = "int";

    /// <summary>
    /// 长整型
    /// </summary>
    public const string TYPE_LONG = "long";

    /// <summary>
    /// 浮点型
    /// </summary>
    public const string TYPE_DOUBLE = "double";

    /// <summary>
    /// 高精度计算类型
    /// </summary>
    public const string TYPE_DECIMAL = "decimal";

    /// <summary>
    /// 时间类型
    /// </summary>
    public const string TYPE_DATETIME = "DateTime?";

    /// <summary>
    /// 模糊查询
    /// </summary>
    public const string QUERY_LIKE = "LIKE";

    /// <summary>
    /// 相等查询
    /// </summary>
    public const string QUERY_EQ = "EQ";

    /// <summary>
    /// 需要
    /// </summary>
    public const string REQUIRE = "1";
}
