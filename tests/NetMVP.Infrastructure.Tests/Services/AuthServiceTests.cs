using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Configuration;
using NetMVP.Infrastructure.Services.Auth;
using Xunit;

namespace NetMVP.Infrastructure.Tests.Services;

/// <summary>
/// 认证服务测试
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<ISysUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<ICaptchaService> _captchaServiceMock;
    private readonly Mock<IOptions<SystemSettings>> _systemSettingsMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<ISysUserRepository>();
        _jwtServiceMock = new Mock<IJwtService>();
        _captchaServiceMock = new Mock<ICaptchaService>();
        _systemSettingsMock = new Mock<IOptions<SystemSettings>>();

        var systemSettings = new SystemSettings
        {
            CaptchaEnabled = true
        };
        _systemSettingsMock.Setup(x => x.Value).Returns(systemSettings);

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _jwtServiceMock.Object,
            _captchaServiceMock.Object,
            _systemSettingsMock.Object
        );
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCaptcha_ShouldThrowException()
    {
        // Arrange
        var userName = "admin";
        var password = "admin123";
        var code = "1234";
        var uuid = "test-uuid";

        _captchaServiceMock.Setup(x => x.ValidateCaptchaAsync(uuid, code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _authService.LoginAsync(userName, password, code, uuid));
    }

    [Fact]
    public async Task GetCaptchaAsync_ShouldReturnCaptchaInfo()
    {
        // Arrange
        var uuid = "test-uuid";
        var captchaImage = "test";
        
        _captchaServiceMock.Setup(x => x.GenerateCaptchaAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((uuid, captchaImage));

        // Act
        var result = await _authService.GetCaptchaAsync();

        // Assert
        result.uuid.Should().Be(uuid);
        result.image.Should().Be(captchaImage);
        result.captchaEnabled.Should().BeTrue();
    }
}
