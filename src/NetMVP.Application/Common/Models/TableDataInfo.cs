using NetMVP.Application.Common.Constants;

namespace NetMVP.Application.Common.Models;

/// <summary>
/// 表格数据响应
/// </summary>
public class TableDataInfo : AjaxResult
{
    /// <summary>
    /// 总记录数
    /// </summary>
    public long Total
    {
        get => (long)(this.ContainsKey("total") ? this["total"] : 0L);
        set => this["total"] = value;
    }

    /// <summary>
    /// 数据列表
    /// </summary>
    public object Rows
    {
        get => this.ContainsKey("rows") ? this["rows"] : new List<object>();
        set => this["rows"] = value;
    }

    /// <summary>
    /// 默认构造函数
    /// </summary>
    public TableDataInfo()
    {
        Code = HttpStatus.SUCCESS;
        Msg = "查询成功";
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public TableDataInfo(object rows, long total)
    {
        Code = HttpStatus.SUCCESS;
        Msg = "查询成功";
        Rows = rows;
        Total = total;
    }

    /// <summary>
    /// 构建表格数据响应
    /// </summary>
    public static TableDataInfo Build<T>(List<T> rows, long total)
    {
        return new TableDataInfo(rows, total);
    }
}
