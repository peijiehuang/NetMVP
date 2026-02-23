using AutoMapper;
using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.Role;
using NetMVP.Application.Mappings;
using NetMVP.Application.Services;
using NetMVP.Application.Services.Impl;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;
using Xunit;

namespace NetMVP.Application.Tests.Services;

/// <summary>
/// 角色服务测试
/// </summary>
public class SysRoleServiceTests
{
    private readonly Mock<IRepository<SysRole>> _roleRepositoryMock;
    private readonly Mock<IRepository<SysUser>> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPermissionService> _permissionServiceMock;
    private readonly Mock<ISysMenuService> _menuServiceMock;
    private readonly Mock<IExcelService> _excelServiceMock;
    private readonly IMapper _mapper;
    private readonly SysRoleService _roleService;

    public SysRoleServiceTests()
    {
        _roleRepositoryMock = new Mock<IRepository<SysRole>>();
        _userRepositoryMock = new Mock<IRepository<SysUser>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _permissionServiceMock = new Mock<IPermissionService>();
        _menuServiceMock = new Mock<ISysMenuService>();
        _excelServiceMock = new Mock<IExcelService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _roleService = new SysRoleService(
            _roleRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mapper,
            _unitOfWorkMock.Object,
            _permissionServiceMock.Object,
            _menuServiceMock.Object,
            _excelServiceMock.Object
        );
    }

    [Fact]
    public async Task GetRoleByIdAsync_WhenRoleNotExists_ShouldReturnNull()
    {
        // Arrange
        var roleId = 999L;
        _roleRepositoryMock.Setup(x => x.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysRole?)null);

        // Act
        var result = await _roleService.GetRoleByIdAsync(roleId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateRoleStatusAsync_WhenRoleNotExists_ShouldThrowException()
    {
        // Arrange
        var roleId = 1L;
        _roleRepositoryMock.Setup(x => x.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysRole?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _roleService.UpdateRoleStatusAsync(roleId, UserConstants.NORMAL));
    }

    [Fact]
    public async Task DeleteRoleAsync_WhenRoleNotExists_ShouldThrowException()
    {
        // Arrange
        var roleId = 1L;
        _roleRepositoryMock.Setup(x => x.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysRole?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _roleService.DeleteRoleAsync(roleId));
    }
}
