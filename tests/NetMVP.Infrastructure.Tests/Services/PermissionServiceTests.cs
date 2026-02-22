using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Persistence;
using NetMVP.Infrastructure.Services.Auth;
using Xunit;

namespace NetMVP.Infrastructure.Tests.Services;

/// <summary>
/// 权限服务测试
/// </summary>
public class PermissionServiceTests
{
    private readonly Mock<IRepository<SysUser>> _userRepositoryMock;
    private readonly Mock<IRepository<SysRole>> _roleRepositoryMock;
    private readonly Mock<IRepository<SysMenu>> _menuRepositoryMock;
    private readonly Mock<NetMVPDbContext> _dbContextMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly PermissionService _permissionService;

    public PermissionServiceTests()
    {
        _userRepositoryMock = new Mock<IRepository<SysUser>>();
        _roleRepositoryMock = new Mock<IRepository<SysRole>>();
        _menuRepositoryMock = new Mock<IRepository<SysMenu>>();
        
        var options = new DbContextOptionsBuilder<NetMVPDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _dbContextMock = new Mock<NetMVPDbContext>(options);
        
        _cacheServiceMock = new Mock<ICacheService>();

        _permissionService = new PermissionService(
            _userRepositoryMock.Object,
            _roleRepositoryMock.Object,
            _menuRepositoryMock.Object,
            _dbContextMock.Object,
            _cacheServiceMock.Object
        );
    }

    [Fact]
    public async Task HasPermissionAsync_WithEmptyPermission_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1L;
        var permission = "";

        // Act
        var result = await _permissionService.HasPermissionAsync(userId, permission);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasRoleAsync_WithEmptyRole_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1L;
        var roleKey = "";

        // Act
        var result = await _permissionService.HasRoleAsync(userId, roleKey);

        // Assert
        result.Should().BeFalse();
    }
}
