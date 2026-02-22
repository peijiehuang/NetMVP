namespace NetMVP.Application.DTOs.Server;

/// <summary>
/// 服务器信息DTO
/// </summary>
public class ServerInfoDto
{
    /// <summary>
    /// CPU信息
    /// </summary>
    public CpuInfo Cpu { get; set; } = new();

    /// <summary>
    /// 内存信息
    /// </summary>
    public MemoryInfo Mem { get; set; } = new();

    /// <summary>
    /// 系统信息
    /// </summary>
    public SystemInfo Sys { get; set; } = new();

    /// <summary>
    /// 运行时信息
    /// </summary>
    public RuntimeInfo Runtime { get; set; } = new();

    /// <summary>
    /// 磁盘信息列表
    /// </summary>
    public List<DiskInfo> Disks { get; set; } = new();
}

/// <summary>
/// CPU信息
/// </summary>
public class CpuInfo
{
    /// <summary>
    /// CPU核心数
    /// </summary>
    public int CpuNum { get; set; }

    /// <summary>
    /// CPU使用率（%）
    /// </summary>
    public double Used { get; set; }

    /// <summary>
    /// CPU空闲率（%）
    /// </summary>
    public double Free { get; set; }
}

/// <summary>
/// 内存信息
/// </summary>
public class MemoryInfo
{
    /// <summary>
    /// 总内存（GB）
    /// </summary>
    public double Total { get; set; }

    /// <summary>
    /// 已用内存（GB）
    /// </summary>
    public double Used { get; set; }

    /// <summary>
    /// 空闲内存（GB）
    /// </summary>
    public double Free { get; set; }

    /// <summary>
    /// 使用率（%）
    /// </summary>
    public double Usage { get; set; }
}

/// <summary>
/// 系统信息
/// </summary>
public class SystemInfo
{
    /// <summary>
    /// 操作系统
    /// </summary>
    public string OsName { get; set; } = string.Empty;

    /// <summary>
    /// 系统架构
    /// </summary>
    public string OsArch { get; set; } = string.Empty;

    /// <summary>
    /// 主机名
    /// </summary>
    public string ComputerName { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;
}

/// <summary>
/// 运行时信息
/// </summary>
public class RuntimeInfo
{
    /// <summary>
    /// .NET版本
    /// </summary>
    public string DotNetVersion { get; set; } = string.Empty;

    /// <summary>
    /// 项目路径
    /// </summary>
    public string ProjectPath { get; set; } = string.Empty;

    /// <summary>
    /// 启动时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 运行时长
    /// </summary>
    public string RunTime { get; set; } = string.Empty;
}

/// <summary>
/// 磁盘信息
/// </summary>
public class DiskInfo
{
    /// <summary>
    /// 盘符路径
    /// </summary>
    public string DirName { get; set; } = string.Empty;

    /// <summary>
    /// 文件系统类型
    /// </summary>
    public string SysTypeName { get; set; } = string.Empty;

    /// <summary>
    /// 总大小（GB）
    /// </summary>
    public double Total { get; set; }

    /// <summary>
    /// 已用大小（GB）
    /// </summary>
    public double Used { get; set; }

    /// <summary>
    /// 可用大小（GB）
    /// </summary>
    public double Free { get; set; }

    /// <summary>
    /// 使用率（%）
    /// </summary>
    public double Usage { get; set; }
}
