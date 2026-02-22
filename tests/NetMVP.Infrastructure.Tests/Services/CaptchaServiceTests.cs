using FluentAssertions;
using Moq;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Services.Auth;
using Xunit;

namespace NetMVP.Infrastructure.Tests.Services;

/// <summary>
/// 验证码服务测试
/// </summary>
public class CaptchaServiceTests
{
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly CaptchaService _captchaService;

    public CaptchaServiceTests()
    {
        _cacheServiceMock = new Mock<ICacheService>();
        _captchaService = new CaptchaService(_cacheServiceMock.Object);
    }

    [Fact]
    public async Task GenerateCaptchaAsync_ShouldReturnUuidAndImage()
    {
        // Act
        var (uuid, image) = await _captchaService.GenerateCaptchaAsync();

        // Assert
        uuid.Should().NotBeNullOrEmpty();
        image.Should().NotBeNullOrEmpty();
        // 验证是Base64编码的图片数据（不包含data:image/png;base64,前缀）
        image.Length.Should().BeGreaterThan(100);
    }

    [Fact]
    public async Task ValidateCaptchaAsync_WithValidCode_ShouldReturnTrue()
    {
        // Arrange
        var uuid = "test-uuid";
        var code = "1234";
        
        _cacheServiceMock.Setup(x => x.GetAsync<string>($"captcha:{uuid}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(code.ToLower());

        // Act
        var result = await _captchaService.ValidateCaptchaAsync(uuid, code);

        // Assert
        result.Should().BeTrue();
        _cacheServiceMock.Verify(x => x.RemoveAsync($"captcha:{uuid}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidateCaptchaAsync_WithInvalidCode_ShouldReturnFalse()
    {
        // Arrange
        var uuid = "test-uuid";
        var code = "1234";
        var wrongCode = "5678";
        
        _cacheServiceMock.Setup(x => x.GetAsync<string>($"captcha:{uuid}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(code.ToLower());

        // Act
        var result = await _captchaService.ValidateCaptchaAsync(uuid, wrongCode);

        // Assert
        result.Should().BeFalse();
        _cacheServiceMock.Verify(x => x.RemoveAsync($"captcha:{uuid}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidateCaptchaAsync_WithExpiredCaptcha_ShouldReturnFalse()
    {
        // Arrange
        var uuid = "test-uuid";
        var code = "1234";
        
        _cacheServiceMock.Setup(x => x.GetAsync<string>($"captcha:{uuid}", It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        // Act
        var result = await _captchaService.ValidateCaptchaAsync(uuid, code);

        // Assert
        result.Should().BeFalse();
    }
}
