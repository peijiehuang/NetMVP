using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NetMVP.Application.DTOs.Auth;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers;

public class SysRegisterControllerTests : TestBase
{
    private readonly Mock<IRegisterService> _registerServiceMock;
    private readonly Mock<ILogger<SysRegisterController>> _loggerMock;
    private readonly SysRegisterController _controller;

    public SysRegisterControllerTests()
    {
        _registerServiceMock = new Mock<IRegisterService>();
        _loggerMock = new Mock<ILogger<SysRegisterController>>();
        _controller = new SysRegisterController(_registerServiceMock.Object, _loggerMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task Register_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new RegisterDto
        {
            UserName = "testuser",
            Password = "test123",
            ConfirmPassword = "test123",
            Code = "1234",
            Uuid = "test-uuid"
        };
        _registerServiceMock.Setup(x => x.RegisterAsync(It.IsAny<RegisterDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Register(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }
}
