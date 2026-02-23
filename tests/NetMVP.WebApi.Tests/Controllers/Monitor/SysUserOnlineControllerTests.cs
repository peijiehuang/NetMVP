using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.UserOnline;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.Monitor;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.Monitor;

public class SysUserOnlineControllerTests : TestBase
{
    private readonly Mock<ISysUserOnlineService> _userOnlineServiceMock;
    private readonly SysUserOnlineController _controller;

    public SysUserOnlineControllerTests()
    {
        _userOnlineServiceMock = new Mock<ISysUserOnlineService>();
        _controller = new SysUserOnlineController(_userOnlineServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetList_ShouldReturnOnlineUserList()
    {
        var query = new OnlineUserQueryDto();
        _userOnlineServiceMock.Setup(x => x.GetOnlineUserListAsync(It.IsAny<OnlineUserQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<OnlineUserDto>(), 0));

        var result = await _controller.GetList(query);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ForceLogout_WithValidTokenId_ShouldReturnSuccess()
    {
        var tokenId = "test-token-id";
        _userOnlineServiceMock.Setup(x => x.ForceLogoutAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.ForceLogout(tokenId);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }
}
