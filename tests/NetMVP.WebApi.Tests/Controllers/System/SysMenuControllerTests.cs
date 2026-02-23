using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.Menu;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.System;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.System;

public class SysMenuControllerTests : TestBase
{
    private readonly Mock<ISysMenuService> _menuServiceMock;
    private readonly SysMenuController _controller;

    public SysMenuControllerTests()
    {
        _menuServiceMock = new Mock<ISysMenuService>();
        _controller = new SysMenuController(_menuServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetList_ShouldReturnMenuList()
    {
        var query = new MenuQueryDto();
        _menuServiceMock.Setup(x => x.GetMenuListAsync(It.IsAny<MenuQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MenuDto>());

        var result = await _controller.GetList(query);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task GetInfo_WithValidId_ShouldReturnMenu()
    {
        var menuId = 1L;
        _menuServiceMock.Setup(x => x.GetMenuByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MenuDto { MenuId = menuId });

        var result = await _controller.GetInfo(menuId);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Add_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new CreateMenuDto { MenuName = "测试菜单" };
        _menuServiceMock.Setup(x => x.CreateMenuAsync(It.IsAny<CreateMenuDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1L);

        var result = await _controller.Add(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Update_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new UpdateMenuDto { MenuId = 1, MenuName = "更新菜单" };
        _menuServiceMock.Setup(x => x.UpdateMenuAsync(It.IsAny<UpdateMenuDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Edit(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnSuccess()
    {
        var menuId = 1L;
        _menuServiceMock.Setup(x => x.DeleteMenuAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Remove(menuId);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }
}
