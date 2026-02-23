using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NetMVP.Application.DTOs.UserOnline;
using NetMVP.Application.Services.Impl;
using NetMVP.Domain.Interfaces;
using System.Text.Json;
using Xunit;

namespace NetMVP.Application.Tests.Services;

/// <summary>
/// 在线用户服务测试
/// </summary>
public class SysUserOnlineServiceTests
{
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<ILogger<SysUserOnlineService>> _loggerMock;
    private readonly SysUserOnlineService _service;

    public SysUserOnlineServiceTests()
    {
        _cacheServiceMock = new Mock<ICacheService>();
        _jwtServiceMock = new Mock<IJwtService>();
        _loggerMock = new Mock<ILogger<SysUserOnlineService>>();
        _service = new SysUserOnlineService(_cacheServiceMock.Object, _jwtServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetOnlineUserListAsync_ShouldReturnEmptyList_WhenNoOnlineUsers()
    {
        // Arrange
        var query = new OnlineUserQueryDto { PageNum = 1, PageSize = 10 };
        _cacheServiceMock.Setup(x => x.GetKeysAsync("online_user:*", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());

        // Act
        var (users, total) = await _service.GetOnlineUserListAsync(query);

        // Assert
        users.Should().BeEmpty();
        total.Should().Be(0);
    }

    [Fact]
    public async Task GetOnlineUserListAsync_ShouldReturnFilteredUsers_WhenUserNameProvided()
    {
        // Arrange
        var query = new OnlineUserQueryDto { PageNum = 1, PageSize = 10, UserName = "admin" };
        
        var onlineUser1 = new OnlineUserDto
        {
            TokenId = "token1",
            UserId = 1,
            UserName = "admin",
            Ipaddr = "127.0.0.1",
            LoginTime = DateTimeOffset.Now.ToUnixTimeMilliseconds()
        };
        
        var onlineUser2 = new OnlineUserDto
        {
            TokenId = "token2",
            UserId = 2,
            UserName = "user",
            Ipaddr = "127.0.0.1",
            LoginTime = DateTimeOffset.Now.ToUnixTimeMilliseconds()
        };

        _cacheServiceMock.Setup(x => x.GetKeysAsync("online_user:*", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "online_user:token1", "online_user:token2" });
        
        _cacheServiceMock.Setup(x => x.GetAsync<OnlineUserDto>("online_user:token1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(onlineUser1);
        
        _cacheServiceMock.Setup(x => x.GetAsync<OnlineUserDto>("online_user:token2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(onlineUser2);

        // Act
        var (users, total) = await _service.GetOnlineUserListAsync(query);

        // Assert
        users.Should().HaveCount(1);
        users[0].UserName.Should().Be("admin");
        total.Should().Be(1);
    }

    [Fact]
    public async Task ForceLogoutAsync_ShouldRemoveOnlineUser()
    {
        // Arrange
        var tokenId = "test-token-id";
        var onlineUser = new OnlineUserDto
        {
            TokenId = tokenId,
            UserId = 1,
            UserName = "admin",
            Ipaddr = "127.0.0.1",
            LoginTime = DateTimeOffset.Now.ToUnixTimeMilliseconds()
        };

        _cacheServiceMock.Setup(x => x.GetAsync<OnlineUserDto>(
            $"online_user:{tokenId}",
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(onlineUser);

        // Act
        await _service.ForceLogoutAsync(tokenId);

        // Assert
        _cacheServiceMock.Verify(x => x.RemoveAsync(
            $"online_user:{tokenId}",
            It.IsAny<CancellationToken>()), Times.Once);

        _cacheServiceMock.Verify(x => x.RemoveAsync(
            $"user_session:{onlineUser.UserId}",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BatchForceLogoutAsync_ShouldForceLogoutMultipleUsers()
    {
        // Arrange
        var tokenIds = new[] { "token1", "token2", "token3" };
        
        foreach (var tokenId in tokenIds)
        {
            var onlineUser = new OnlineUserDto
            {
                TokenId = tokenId,
                UserId = long.Parse(tokenId.Replace("token", "")),
                UserName = $"user{tokenId}",
                Ipaddr = "127.0.0.1",
                LoginTime = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            _cacheServiceMock.Setup(x => x.GetAsync<OnlineUserDto>(
                $"online_user:{tokenId}",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(onlineUser);
        }

        // Act
        await _service.BatchForceLogoutAsync(tokenIds);

        // Assert
        foreach (var tokenId in tokenIds)
        {
            _cacheServiceMock.Verify(x => x.RemoveAsync(
                $"online_user:{tokenId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
