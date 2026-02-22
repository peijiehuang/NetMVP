using AutoMapper;
using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.Menu;
using NetMVP.Application.Mappings;
using NetMVP.Application.Services.Impl;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Domain.Interfaces;
using Xunit;

namespace NetMVP.Application.Tests.Services;

/// <summary>
/// 菜单服务测试
/// </summary>
public class SysMenuServiceTests
{
    private readonly Mock<IRepository<SysMenu>> _menuRepositoryMock;
    private readonly Mock<ISysUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPermissionService> _permissionServiceMock;
    private readonly IMapper _mapper;
    private readonly SysMenuService _menuService;

    public SysMenuServiceTests()
    {
        _menuRepositoryMock = new Mock<IRepository<SysMenu>>();
        _userRepositoryMock = new Mock<ISysUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _permissionServiceMock = new Mock<IPermissionService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _menuService = new SysMenuService(
            _menuRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mapper,
            _unitOfWorkMock.Object,
            _permissionServiceMock.Object
        );
    }

    [Fact]
    public async Task GetMenuByIdAsync_WhenMenuNotExists_ShouldReturnNull()
    {
        // Arrange
        var menuId = 999L;
        _menuRepositoryMock.Setup(x => x.GetByIdAsync(menuId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysMenu?)null);

        // Act
        var result = await _menuService.GetMenuByIdAsync(menuId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetMenuByIdAsync_WhenMenuExists_ShouldReturnMenu()
    {
        // Arrange
        var menuId = 1L;
        var menu = new SysMenu 
        { 
            MenuId = menuId, 
            MenuName = "测试菜单",
            MenuType = MenuType.Menu,
            Path = "/test"
        };
        
        _menuRepositoryMock.Setup(x => x.GetByIdAsync(menuId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(menu);

        // Act
        var result = await _menuService.GetMenuByIdAsync(menuId);

        // Assert
        result.Should().NotBeNull();
        result!.MenuId.Should().Be(menuId);
        result.MenuName.Should().Be("测试菜单");
    }

    [Fact]
    public async Task DeleteMenuAsync_WhenMenuNotExists_ShouldThrowException()
    {
        // Arrange
        var menuId = 1L;
        _menuRepositoryMock.Setup(x => x.GetByIdAsync(menuId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysMenu?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _menuService.DeleteMenuAsync(menuId));
    }
}
