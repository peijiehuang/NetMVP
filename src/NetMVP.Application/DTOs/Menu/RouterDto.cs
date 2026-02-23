namespace NetMVP.Application.DTOs.Menu;

/// <summary>
/// 路由 DTO
/// </summary>
public class RouterDto
{
    /// <summary>
    /// 路由名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 路由地址
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// 是否隐藏路由
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// 重定向地址
    /// </summary>
    public string? Redirect { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 路由参数
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// 是否总是显示
    /// </summary>
    public bool? AlwaysShow { get; set; }

    /// <summary>
    /// 路由元信息
    /// </summary>
    public RouterMetaDto? Meta { get; set; } = new();

    /// <summary>
    /// 子路由
    /// </summary>
    public List<RouterDto> Children { get; set; } = new();
}

/// <summary>
/// 路由元信息 DTO
/// </summary>
public class RouterMetaDto
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 是否不缓存
    /// </summary>
    public bool NoCache { get; set; }

    /// <summary>
    /// 内链地址
    /// </summary>
    public string? Link { get; set; }
}
