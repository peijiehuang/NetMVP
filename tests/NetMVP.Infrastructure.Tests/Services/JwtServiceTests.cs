using FluentAssertions;
using Microsoft.Extensions.Options;
using NetMVP.Infrastructure.Configuration;
using NetMVP.Infrastructure.Services.Auth;
using Xunit;

namespace NetMVP.Infrastructure.Tests.Services;

/// <summary>
/// JWT服务测试
/// </summary>
public class JwtServiceTests
{
    private readonly JwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public JwtServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = "test-secret-key-for-jwt-token-generation-must-be-long-enough",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpireMinutes = 120
        };

        var optionsMock = Options.Create(_jwtSettings);
        var cacheServiceMock = new Moq.Mock<NetMVP.Domain.Interfaces.ICacheService>();
        var userRepositoryMock = new Moq.Mock<NetMVP.Domain.Interfaces.ISysUserRepository>();
        var deptRepositoryMock = new Moq.Mock<NetMVP.Domain.Interfaces.ISysDeptRepository>();
        
        _jwtService = new JwtService(
            optionsMock, 
            cacheServiceMock.Object, 
            userRepositoryMock.Object, 
            deptRepositoryMock.Object);
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldReturnValidTokens()
    {
        // Arrange
        var userId = 1L;
        var userName = "admin";

        // Act
        var (accessToken, refreshToken) = await _jwtService.GenerateTokenAsync(userId, userName);

        // Assert
        accessToken.Should().NotBeNullOrEmpty();
        refreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ValidateTokenAsync_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var userId = 1L;
        var userName = "admin";
        var (token, _) = await _jwtService.GenerateTokenAsync(userId, userName);

        // Act
        var isValid = await _jwtService.ValidateTokenAsync(token);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateTokenAsync_WithInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var isValid = await _jwtService.ValidateTokenAsync(invalidToken);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserIdFromToken_WithValidToken_ShouldReturnUserId()
    {
        // Arrange
        var userId = 1L;
        var userName = "admin";
        var (token, _) = await _jwtService.GenerateTokenAsync(userId, userName);

        // Act
        var extractedUserId = _jwtService.GetUserIdFromToken(token);

        // Assert
        extractedUserId.Should().Be(userId);
    }
}
