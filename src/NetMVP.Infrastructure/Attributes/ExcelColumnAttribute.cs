namespace NetMVP.Infrastructure.Attributes;

/// <summary>
/// Excel 列特性
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ExcelColumnAttribute : Attribute
{
    /// <summary>
    /// 列名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 列索引
    /// </summary>
    public int Index { get; set; } = -1;

    /// <summary>
    /// 列宽
    /// </summary>
    public double Width { get; set; } = 15;

    /// <summary>
    /// 格式化
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// 是否忽略
    /// </summary>
    public bool Ignore { get; set; } = false;
}
