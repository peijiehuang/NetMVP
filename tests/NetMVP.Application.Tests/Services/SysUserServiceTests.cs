using AutoMapper;
using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.User;
using NetMVP.Application.Mappings;
using NetMVP.Application.Services.Impl;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;
using Xunit;

namespace NetMVP.Application.Tests.Services;

/// <summary>
/// 用户服务测试
/// </summary>
public class SysUserServiceTests
{
    private readonly Mock<ISysUserRepository> _userRepositoryMock;
    private readonly Mock<ISysDeptRepository> _deptRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IExcelService> _excelServiceMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<IDataScopeFilter> _dataScopeFilterMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly IMapper _mapper;
    private readonly SysUserService _userService;

    public SysUserServiceTests()
    {
        _userRepositoryMock = new Mock<ISysUserRepository>();
        _deptRepositoryMock = new Mock<ISysDeptRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _excelServiceMock = new Mock<IExcelService>();
        _cacheServiceMock = new Mock<ICacheService>();
        _dataScopeFilterMock = new Mock<IDataScopeFilter>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _userService = new SysUserService(
            _userRepositoryMock.Object,
            _deptRepositoryMock.Object,
            _mapper,
            _unitOfWorkMock.Object,
            _excelServiceMock.Object,
            _cacheServiceMock.Object,
            _dataScopeFilterMock.Object,
            _currentUserServiceMock.Object
        );
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserNotExists_ShouldReturnNull()
    {
        // Arrange
        var userId = 999L;
        _userRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysUser?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var userId = 1L;
        var user = new SysUser 
        { 
            UserId = userId, 
            UserName = "testuser",
            NickName = "测试用户",
            DeptId = 1
        };
        
        _userRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.UserName.Should().Be("testuser");
    }

    [Fact]
    public async Task ResetPasswordAsync_WhenUserNotExists_ShouldThrowException()
    {
        // Arrange
        var dto = new ResetPasswordDto { UserId = 1L, Password = "newpassword" };
        _userRepositoryMock.Setup(x => x.GetByUserIdAsync(dto.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysUser?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _userService.ResetPasswordAsync(dto));
    }
}
