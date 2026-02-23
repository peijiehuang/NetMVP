using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.Auth;
using NetMVP.Application.Services;
using NetMVP.Domain.Interfaces;
using NetMVP.WebApi.Controllers;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers;

public class AuthControllerTests : TestBase
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<ISysUserService> _userServiceMock;
    private readonly Mock<ISysMenuService> _menuServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IPermissionService> _permissionServiceMock;
    private readonly Mock<ISysUserRepository> _userRepositoryMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _jwtServiceMock = new Mock<IJwtService>();
        _userServiceMock = new Mock<ISysUserService>();
        _menuServiceMock = new Mock<ISysMenuService>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _permissionServiceMock = new Mock<IPermissionService>();
        _userRepositoryMock = new Mock<ISysUserRepository>();

        _controller = new AuthController(
            _authServiceMock.Object,
            _jwtServiceMock.Object,
            _userServiceMock.Object,
            _menuServiceMock.Object,
            _currentUserServiceMock.Object,
            _permissionServiceMock.Object,
            _userRepositoryMock.Object
        );

        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetCaptcha_ShouldReturnSuccess()
    {
        _authServiceMock.Setup(x => x.GetCaptchaAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(("test-uuid", "base64-image", true));

        var result = await _controller.GetCaptcha();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        var loginDto = new LoginDto
        {
            UserName = "admin",
            Password = "admin123",
            Code = "1234",
            Uuid = "test-uuid"
        };

        _authServiceMock.Setup(x => x.LoginAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("access-token", "refresh-token"));

        var result = await _controller.Login(loginDto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Logout_ShouldReturnSuccess()
    {
        // Setup Headers
        var headers = new Microsoft.AspNetCore.Http.HeaderDictionary
        {
            { "Authorization", "Bearer test-token" }
        };
        MockHttpRequest.Setup(x => x.Headers).Returns(headers);

        var result = await _controller.Logout();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task GetInfo_ShouldReturnUserInfo()
    {
        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(1L);
        _userServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Application.DTOs.User.UserDto { UserId = 1, UserName = "admin" });
        _permissionServiceMock.Setup(x => x.GetUserRolesAsync(It.IsAny<long>()))
            .ReturnsAsync(new List<string> { "admin" });
        _permissionServiceMock.Setup(x => x.GetUserPermissionsAsync(It.IsAny<long>()))
            .ReturnsAsync(new List<string> { "*:*:*" });

        var result = await _controller.GetInfo();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }
}
