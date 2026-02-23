using FluentAssertions;
using NetMVP.Application.Services.Impl;

namespace NetMVP.Application.Tests.Services;

public class ServerMonitorServiceTests
{
    private readonly ServerMonitorService _service;

    public ServerMonitorServiceTests()
    {
        _service = new ServerMonitorService();
    }

    [Fact]
    public async Task GetServerInfoAsync_ShouldReturnServerInfo()
    {
        // Act
        var result = await _service.GetServerInfoAsync();

        // Assert
        result.Should().NotBeNull();
        result.Cpu.Should().NotBeNull();
        result.Mem.Should().NotBeNull();
        result.Sys.Should().NotBeNull();
        result.Runtime.Should().NotBeNull();
        result.SysFiles.Should().NotBeNull();
    }

    [Fact]
    public async Task GetServerInfoAsync_CpuInfo_ShouldHaveValidData()
    {
        // Act
        var result = await _service.GetServerInfoAsync();

        // Assert
        result.Cpu.CpuNum.Should().BeGreaterThan(0);
        result.Cpu.Used.Should().BeGreaterThanOrEqualTo(0);
        result.Cpu.Free.Should().BeGreaterThanOrEqualTo(0);
        (result.Cpu.Used + result.Cpu.Free).Should().BeApproximately(100, 0.1);
    }

    [Fact]
    public async Task GetServerInfoAsync_MemoryInfo_ShouldHaveValidData()
    {
        // Act
        var result = await _service.GetServerInfoAsync();

        // Assert
        result.Mem.Total.Should().BeGreaterThanOrEqualTo(0);
        result.Mem.Used.Should().BeGreaterThanOrEqualTo(0);
        result.Mem.Free.Should().BeGreaterThanOrEqualTo(0);
        result.Mem.Usage.Should().BeGreaterThanOrEqualTo(0).And.BeLessThanOrEqualTo(100);
    }

    [Fact]
    public async Task GetServerInfoAsync_SystemInfo_ShouldHaveValidData()
    {
        // Act
        var result = await _service.GetServerInfoAsync();

        // Assert
        result.Sys.OsName.Should().NotBeNullOrEmpty();
        result.Sys.OsArch.Should().NotBeNullOrEmpty();
        result.Sys.ComputerName.Should().NotBeNullOrEmpty();
        result.Sys.UserName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetServerInfoAsync_RuntimeInfo_ShouldHaveValidData()
    {
        // Act
        var result = await _service.GetServerInfoAsync();

        // Assert
        result.Runtime.DotNetVersion.Should().NotBeNullOrEmpty();
        result.Runtime.ProjectPath.Should().NotBeNullOrEmpty();
        result.Runtime.StartTime.Should().BeBefore(DateTime.Now);
        result.Runtime.RunTime.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetServerInfoAsync_DiskInfo_ShouldHaveValidData()
    {
        // Act
        var result = await _service.GetServerInfoAsync();

        // Assert
        result.SysFiles.Should().NotBeNull();
        
        if (result.SysFiles.Count > 0)
        {
            foreach (var disk in result.SysFiles)
            {
                disk.DirName.Should().NotBeNullOrEmpty();
                disk.Total.Should().BeGreaterThan(0);
                disk.Usage.Should().BeGreaterThanOrEqualTo(0).And.BeLessThanOrEqualTo(100);
            }
        }
    }
}
