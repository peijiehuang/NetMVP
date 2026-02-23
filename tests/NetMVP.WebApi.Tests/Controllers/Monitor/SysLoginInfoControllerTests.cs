using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.LoginInfo;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.Monitor;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.Monitor;

public class SysLoginInfoControllerTests : TestBase
{
    private readonly Mock<ISysLoginInfoService> _loginInfoServiceMock;
    private readonly SysLoginInfoController _controller;

    public SysLoginInfoControllerTests()
    {
        _loginInfoServiceMock = new Mock<ISysLoginInfoService>();
        _controller = new SysLoginInfoController(_loginInfoServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetList_ShouldReturnLoginInfoList()
    {
        var query = new LoginInfoQueryDto();
        _loginInfoServiceMock.Setup(x => x.GetLoginInfoListAsync(It.IsAny<LoginInfoQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<LoginInfoDto>(), 0));

        var result = await _controller.GetList(query);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Delete_WithValidIds_ShouldReturnSuccess()
    {
        var infoIds = "1,2,3";
        _loginInfoServiceMock.Setup(x => x.DeleteLoginInfosAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var result = await _controller.Delete(infoIds);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Clean_ShouldReturnSuccess()
    {
        _loginInfoServiceMock.Setup(x => x.CleanLoginInfoAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(100);

        var result = await _controller.Clean();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Unlock_WithValidUserName_ShouldReturnSuccess()
    {
        var userName = "testuser";
        _loginInfoServiceMock.Setup(x => x.UnlockUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _controller.Unlock(userName);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }
}
