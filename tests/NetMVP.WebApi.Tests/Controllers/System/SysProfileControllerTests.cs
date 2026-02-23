using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NetMVP.Application.DTOs.Profile;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.System;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.System;

public class SysProfileControllerTests : TestBase
{
    private readonly Mock<IProfileService> _profileServiceMock;
    private readonly Mock<ILogger<SysProfileController>> _loggerMock;
    private readonly SysProfileController _controller;

    public SysProfileControllerTests()
    {
        _profileServiceMock = new Mock<IProfileService>();
        _loggerMock = new Mock<ILogger<SysProfileController>>();
        _controller = new SysProfileController(_profileServiceMock.Object, _loggerMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetProfile_ShouldReturnProfile()
    {
        _profileServiceMock.Setup(x => x.GetProfileAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProfileDto { UserName = "admin" });

        var result = await _controller.GetProfile();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task UpdateProfile_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new UpdateProfileDto { NickName = "测试用户" };
        _profileServiceMock.Setup(x => x.UpdateProfileAsync(It.IsAny<UpdateProfileDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.UpdateProfile(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task UpdatePassword_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new UpdatePasswordDto { OldPassword = "old123", NewPassword = "new123" };
        _profileServiceMock.Setup(x => x.UpdatePasswordAsync(It.IsAny<UpdatePasswordDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.UpdatePassword(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task UpdateAvatar_WithValidFile_ShouldReturnSuccess()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("avatar.jpg");
        
        _profileServiceMock.Setup(x => x.UpdateAvatarAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("/uploads/avatar.jpg");

        var result = await _controller.UpdateAvatar(fileMock.Object);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }
}
