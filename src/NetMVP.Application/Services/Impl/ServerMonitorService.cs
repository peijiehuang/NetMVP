using NetMVP.Application.DTOs.Server;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 服务器监控服务实现
/// </summary>
public class ServerMonitorService : IServerMonitorService
{
    private static readonly DateTime _startTime = DateTime.Now;

    /// <summary>
    /// 获取服务器信息
    /// </summary>
    public async Task<ServerInfoDto> GetServerInfoAsync(CancellationToken cancellationToken = default)
    {
        var serverInfo = new ServerInfoDto
        {
            Cpu = await GetCpuInfoAsync(cancellationToken),
            Mem = GetMemoryInfo(),
            Sys = GetSystemInfo(),
            Runtime = GetRuntimeInfo(),
            SysFiles = GetDiskInfo()
        };

        return serverInfo;
    }

    /// <summary>
    /// 获取CPU信息
    /// </summary>
    private async Task<CpuInfo> GetCpuInfoAsync(CancellationToken cancellationToken)
    {
        var cpuInfo = new CpuInfo
        {
            CpuNum = Environment.ProcessorCount
        };

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows系统使用PerformanceCounter
                cpuInfo = await GetWindowsCpuInfoAsync(cpuInfo, cancellationToken);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux系统读取/proc/stat
                cpuInfo = await GetLinuxCpuInfoAsync(cpuInfo, cancellationToken);
            }
            else
            {
                // Mac或其他系统，使用进程CPU时间估算
                cpuInfo = GetProcessCpuInfo(cpuInfo);
            }
        }
        catch
        {
            // 如果获取失败，使用进程CPU时间估算
            cpuInfo = GetProcessCpuInfo(cpuInfo);
        }

        return cpuInfo;
    }

    /// <summary>
    /// 获取Windows系统CPU信息
    /// </summary>
    private async Task<CpuInfo> GetWindowsCpuInfoAsync(CpuInfo cpuInfo, CancellationToken cancellationToken)
    {
        // 只在Windows平台编译此代码
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return GetProcessCpuInfo(cpuInfo);
        }

        try
        {
            using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue(); // 第一次调用返回0
            await Task.Delay(100, cancellationToken); // 等待100ms
            var cpuUsage = cpuCounter.NextValue();
            
            cpuInfo.Used = Math.Round(cpuUsage, 2);
            cpuInfo.Free = Math.Round(100 - cpuUsage, 2);
        }
        catch
        {
            cpuInfo = GetProcessCpuInfo(cpuInfo);
        }

        return cpuInfo;
    }

    /// <summary>
    /// 获取Linux系统CPU信息
    /// </summary>
    private async Task<CpuInfo> GetLinuxCpuInfoAsync(CpuInfo cpuInfo, CancellationToken cancellationToken)
    {
        try
        {
            // 读取/proc/stat获取CPU使用率
            var stat1 = await ReadProcStatAsync(cancellationToken);
            await Task.Delay(100, cancellationToken);
            var stat2 = await ReadProcStatAsync(cancellationToken);

            if (stat1 != null && stat2 != null)
            {
                var totalDiff = stat2.Total - stat1.Total;
                var idleDiff = stat2.Idle - stat1.Idle;
                var cpuUsage = totalDiff > 0 ? ((totalDiff - idleDiff) / (double)totalDiff) * 100 : 0;

                cpuInfo.Used = Math.Round(cpuUsage, 2);
                cpuInfo.Free = Math.Round(100 - cpuUsage, 2);
            }
            else
            {
                cpuInfo = GetProcessCpuInfo(cpuInfo);
            }
        }
        catch
        {
            cpuInfo = GetProcessCpuInfo(cpuInfo);
        }

        return cpuInfo;
    }

    /// <summary>
    /// 读取Linux /proc/stat文件
    /// </summary>
    private async Task<CpuStat?> ReadProcStatAsync(CancellationToken cancellationToken)
    {
        try
        {
            var lines = await File.ReadAllLinesAsync("/proc/stat", cancellationToken);
            var cpuLine = lines.FirstOrDefault(l => l.StartsWith("cpu "));
            if (cpuLine != null)
            {
                var values = cpuLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (values.Length >= 5)
                {
                    var user = long.Parse(values[1]);
                    var nice = long.Parse(values[2]);
                    var system = long.Parse(values[3]);
                    var idle = long.Parse(values[4]);
                    var total = user + nice + system + idle;

                    return new CpuStat { Total = total, Idle = idle };
                }
            }
        }
        catch
        {
            // 读取失败
        }

        return null;
    }

    /// <summary>
    /// 使用进程CPU时间估算
    /// </summary>
    private CpuInfo GetProcessCpuInfo(CpuInfo cpuInfo)
    {
        var process = Process.GetCurrentProcess();
        var totalTime = (DateTime.Now - process.StartTime).TotalMilliseconds;
        var cpuTime = process.TotalProcessorTime.TotalMilliseconds;
        var cpuUsage = totalTime > 0 ? (cpuTime / (Environment.ProcessorCount * totalTime)) * 100 : 0;
        
        cpuInfo.Used = Math.Round(cpuUsage, 2);
        cpuInfo.Free = Math.Round(100 - cpuUsage, 2);

        return cpuInfo;
    }

    /// <summary>
    /// CPU统计信息
    /// </summary>
    private class CpuStat
    {
        public long Total { get; set; }
        public long Idle { get; set; }
    }

    /// <summary>
    /// 获取内存信息
    /// </summary>
    private MemoryInfo GetMemoryInfo()
    {
        var memInfo = new MemoryInfo();

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows系统
                memInfo = GetWindowsMemoryInfo();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux系统
                memInfo = GetLinuxMemoryInfo();
            }
            else
            {
                // Mac或其他系统
                memInfo = GetDefaultMemoryInfo();
            }
        }
        catch
        {
            // 如果获取失败，使用默认值
            memInfo = GetDefaultMemoryInfo();
        }

        return memInfo;
    }

    /// <summary>
    /// 获取Windows系统内存信息
    /// </summary>
    private MemoryInfo GetWindowsMemoryInfo()
    {
        var memInfo = new MemoryInfo();

        // 只在Windows平台使用PerformanceCounter
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return GetDefaultMemoryInfo();
        }

        try
        {
            using var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            var availableMemory = ramCounter.NextValue();
            
            // 获取总内存
            var gcMemoryInfo = GC.GetGCMemoryInfo();
            var totalMemory = gcMemoryInfo.TotalAvailableMemoryBytes / (1024.0 * 1024.0 * 1024.0);
            var usedMemory = totalMemory - (availableMemory / 1024.0);

            memInfo.Total = Math.Round(totalMemory, 2);
            memInfo.Used = Math.Round(usedMemory, 2);
            memInfo.Free = Math.Round(availableMemory / 1024.0, 2);
            memInfo.Usage = Math.Round((usedMemory / totalMemory) * 100, 2);
        }
        catch
        {
            memInfo = GetDefaultMemoryInfo();
        }

        return memInfo;
    }

    /// <summary>
    /// 获取Linux系统内存信息
    /// </summary>
    private MemoryInfo GetLinuxMemoryInfo()
    {
        var memInfo = new MemoryInfo();

        try
        {
            // 读取/proc/meminfo
            var lines = File.ReadAllLines("/proc/meminfo");
            var memTotal = 0L;
            var memAvailable = 0L;

            foreach (var line in lines)
            {
                if (line.StartsWith("MemTotal:"))
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && long.TryParse(parts[1], out var value))
                    {
                        memTotal = value; // KB
                    }
                }
                else if (line.StartsWith("MemAvailable:"))
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && long.TryParse(parts[1], out var value))
                    {
                        memAvailable = value; // KB
                    }
                }
            }

            if (memTotal > 0)
            {
                var totalMemory = memTotal / (1024.0 * 1024.0); // GB
                var availableMemory = memAvailable / (1024.0 * 1024.0); // GB
                var usedMemory = totalMemory - availableMemory;

                memInfo.Total = Math.Round(totalMemory, 2);
                memInfo.Used = Math.Round(usedMemory, 2);
                memInfo.Free = Math.Round(availableMemory, 2);
                memInfo.Usage = Math.Round((usedMemory / totalMemory) * 100, 2);
            }
            else
            {
                memInfo = GetDefaultMemoryInfo();
            }
        }
        catch
        {
            memInfo = GetDefaultMemoryInfo();
        }

        return memInfo;
    }

    /// <summary>
    /// 获取默认内存信息（使用GC信息）
    /// </summary>
    private MemoryInfo GetDefaultMemoryInfo()
    {
        var memInfo = new MemoryInfo();

        try
        {
            var gcMemoryInfo = GC.GetGCMemoryInfo();
            var totalMemory = gcMemoryInfo.TotalAvailableMemoryBytes / (1024.0 * 1024.0 * 1024.0);
            var process = Process.GetCurrentProcess();
            var usedMemory = process.WorkingSet64 / (1024.0 * 1024.0 * 1024.0);

            memInfo.Total = Math.Round(totalMemory, 2);
            memInfo.Used = Math.Round(usedMemory, 2);
            memInfo.Free = Math.Round(totalMemory - usedMemory, 2);
            memInfo.Usage = totalMemory > 0 ? Math.Round((usedMemory / totalMemory) * 100, 2) : 0;
        }
        catch
        {
            memInfo.Total = 0;
            memInfo.Used = 0;
            memInfo.Free = 0;
            memInfo.Usage = 0;
        }

        return memInfo;
    }

    /// <summary>
    /// 获取系统信息
    /// </summary>
    private SystemInfo GetSystemInfo()
    {
        return new SystemInfo
        {
            OsName = RuntimeInformation.OSDescription,
            OsArch = RuntimeInformation.OSArchitecture.ToString(),
            ComputerName = Environment.MachineName,
            UserName = Environment.UserName
        };
    }

    /// <summary>
    /// 获取运行时信息
    /// </summary>
    private RuntimeInfo GetRuntimeInfo()
    {
        var runTime = DateTime.Now - _startTime;
        var runTimeStr = $"{(int)runTime.TotalDays}天{runTime.Hours}小时{runTime.Minutes}分钟";

        return new RuntimeInfo
        {
            DotNetVersion = RuntimeInformation.FrameworkDescription,
            ProjectPath = AppContext.BaseDirectory,
            StartTime = _startTime,
            RunTime = runTimeStr
        };
    }

    /// <summary>
    /// 获取磁盘信息
    /// </summary>
    private List<DiskInfo> GetDiskInfo()
    {
        var diskList = new List<DiskInfo>();

        try
        {
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.IsReady)
                {
                    var total = drive.TotalSize / (1024.0 * 1024.0 * 1024.0);
                    var free = drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                    var used = total - free;

                    // 获取驱动器类型描述
                    var typeName = drive.DriveType switch
                    {
                        DriveType.Fixed => "本地磁盘",
                        DriveType.Network => "网络驱动器",
                        DriveType.CDRom => "光盘驱动器",
                        DriveType.Removable => "可移动磁盘",
                        DriveType.Ram => "RAM磁盘",
                        _ => "未知"
                    };

                    diskList.Add(new DiskInfo
                    {
                        DirName = drive.Name,
                        SysTypeName = drive.DriveFormat,
                        TypeName = typeName,
                        Total = Math.Round(total, 2),
                        Used = Math.Round(used, 2),
                        Free = Math.Round(free, 2),
                        Usage = Math.Round((used / total) * 100, 2)
                    });
                }
            }
        }
        catch
        {
            // 如果获取失败，返回空列表
        }

        return diskList;
    }
}
