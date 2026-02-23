using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.Role;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.System;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.System;

public class SysRoleControllerTests : TestBase
{
    private readonly Mock<ISysRoleService> _roleServiceMock;
    private readonly Mock<ISysDeptService> _deptServiceMock;
    private readonly Mock<ISysUserService> _userServiceMock;
    private readonly SysRoleController _controller;

    public SysRoleControllerTests()
    {
        _roleServiceMock = new Mock<ISysRoleService>();
        _deptServiceMock = new Mock<ISysDeptService>();
        _userServiceMock = new Mock<ISysUserService>();
        _controller = new SysRoleController(_roleServiceMock.Object, _deptServiceMock.Object, _userServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetList_ShouldReturnRoleList()
    {
        var query = new RoleQueryDto();
        _roleServiceMock.Setup(x => x.GetRoleListAsync(
            It.IsAny<RoleQueryDto>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<RoleDto>(), 0));

        var result = await _controller.GetList(query);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetInfo_WithValidId_ShouldReturnRole()
    {
        var roleId = 1L;
        _roleServiceMock.Setup(x => x.GetRoleByIdAsync(
            It.IsAny<long>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RoleDto { RoleId = roleId });

        var result = await _controller.GetInfo(roleId);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Add_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new CreateRoleDto { RoleName = "测试角色" };
        _roleServiceMock.Setup(x => x.CreateRoleAsync(
            It.IsAny<CreateRoleDto>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(1L);

        var result = await _controller.Add(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Update_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new UpdateRoleDto { RoleId = 1, RoleName = "更新角色" };
        _roleServiceMock.Setup(x => x.UpdateRoleAsync(
            It.IsAny<UpdateRoleDto>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Edit(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Delete_WithValidIds_ShouldReturnSuccess()
    {
        var roleIds = "1,2,3";
        _roleServiceMock.Setup(x => x.DeleteRolesAsync(
            It.IsAny<long[]>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Remove(roleIds);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }
}
