using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.Server;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.Monitor;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.Monitor;

public class ServerControllerTests : TestBase
{
    private readonly Mock<IServerMonitorService> _serverMonitorServiceMock;
    private readonly ServerController _controller;

    public ServerControllerTests()
    {
        _serverMonitorServiceMock = new Mock<IServerMonitorService>();
        _controller = new ServerController(_serverMonitorServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetServerInfo_ShouldReturnServerInfo()
    {
        _serverMonitorServiceMock.Setup(x => x.GetServerInfoAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServerInfoDto());

        var result = await _controller.GetServerInfo();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }
}
