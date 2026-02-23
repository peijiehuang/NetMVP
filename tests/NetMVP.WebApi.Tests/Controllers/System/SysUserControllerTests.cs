using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.User;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.System;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.System;

public class SysUserControllerTests : TestBase
{
    private readonly Mock<ISysUserService> _userServiceMock;
    private readonly Mock<ISysDeptService> _deptServiceMock;
    private readonly Mock<ISysRoleService> _roleServiceMock;
    private readonly Mock<ISysPostService> _postServiceMock;
    private readonly SysUserController _controller;

    public SysUserControllerTests()
    {
        _userServiceMock = new Mock<ISysUserService>();
        _deptServiceMock = new Mock<ISysDeptService>();
        _roleServiceMock = new Mock<ISysRoleService>();
        _postServiceMock = new Mock<ISysPostService>();

        _controller = new SysUserController(
            _userServiceMock.Object,
            _deptServiceMock.Object,
            _roleServiceMock.Object,
            _postServiceMock.Object
        );

        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetList_ShouldReturnUserList()
    {
        var query = new UserQueryDto();
        _userServiceMock.Setup(x => x.GetUserListAsync(
            It.IsAny<UserQueryDto>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<UserDto>(), 0));

        var result = await _controller.GetList(query);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetInfo_WithValidId_ShouldReturnUser()
    {
        var userId = 1L;
        _userServiceMock.Setup(x => x.GetUserByIdAsync(
            It.IsAny<long>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDto { UserId = userId, UserName = "admin" });

        var result = await _controller.GetInfo(userId);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Add_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new CreateUserDto { UserName = "test", Password = "test123" };
        _userServiceMock.Setup(x => x.CreateUserAsync(
            It.IsAny<CreateUserDto>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(1L);

        var result = await _controller.Add(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Update_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new UpdateUserDto { UserId = 1, UserName = "test" };
        _userServiceMock.Setup(x => x.UpdateUserAsync(
            It.IsAny<UpdateUserDto>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Edit(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Delete_WithValidIds_ShouldReturnSuccess()
    {
        var userIds = "1,2,3";
        _userServiceMock.Setup(x => x.DeleteUsersAsync(
            It.IsAny<long[]>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Remove(userIds);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task ResetPassword_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new ResetPasswordDto { UserId = 1, Password = "newpass123" };
        _userServiceMock.Setup(x => x.ResetPasswordAsync(
            It.IsAny<ResetPasswordDto>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.ResetPassword(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task ChangeStatus_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new UpdateUserStatusDto { UserId = 1, Status = "0" };
        _userServiceMock.Setup(x => x.UpdateUserStatusAsync(
            It.IsAny<long>(), 
            It.IsAny<string>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.ChangeStatus(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }
}
