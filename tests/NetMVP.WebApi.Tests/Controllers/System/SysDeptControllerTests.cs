using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.Dept;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.System;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.System;

public class SysDeptControllerTests : TestBase
{
    private readonly Mock<ISysDeptService> _deptServiceMock;
    private readonly SysDeptController _controller;

    public SysDeptControllerTests()
    {
        _deptServiceMock = new Mock<ISysDeptService>();
        _controller = new SysDeptController(_deptServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetList_ShouldReturnDeptList()
    {
        var query = new DeptQueryDto();
        _deptServiceMock.Setup(x => x.GetDeptListAsync(
            It.IsAny<DeptQueryDto>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptDto>());

        var result = await _controller.GetList(query);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task GetInfo_WithValidId_ShouldReturnDept()
    {
        var deptId = 1L;
        _deptServiceMock.Setup(x => x.GetDeptByIdAsync(
            It.IsAny<long>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeptDto { DeptId = deptId });

        var result = await _controller.GetInfo(deptId);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Add_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new CreateDeptDto { DeptName = "测试部门" };
        _deptServiceMock.Setup(x => x.CreateDeptAsync(
            It.IsAny<CreateDeptDto>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(1L);

        var result = await _controller.Add(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Update_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new UpdateDeptDto { DeptId = 1, DeptName = "更新部门" };
        _deptServiceMock.Setup(x => x.UpdateDeptAsync(
            It.IsAny<UpdateDeptDto>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Edit(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnSuccess()
    {
        var deptId = 1L;
        _deptServiceMock.Setup(x => x.DeleteDeptAsync(
            It.IsAny<long>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Remove(deptId);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }
}
