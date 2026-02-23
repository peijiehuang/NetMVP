using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetMVP.Application.DTOs.Post;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.System;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.System;

public class SysPostControllerTests : TestBase
{
    private readonly Mock<ISysPostService> _postServiceMock;
    private readonly SysPostController _controller;

    public SysPostControllerTests()
    {
        _postServiceMock = new Mock<ISysPostService>();
        _controller = new SysPostController(_postServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetList_ShouldReturnPostList()
    {
        var query = new PostQueryDto();
        _postServiceMock.Setup(x => x.GetPostListAsync(It.IsAny<PostQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<PostDto>(), 0));

        var result = await _controller.GetList(query);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetInfo_WithValidId_ShouldReturnPost()
    {
        var postId = 1L;
        _postServiceMock.Setup(x => x.GetPostByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PostDto { PostId = postId });

        var result = await _controller.GetInfo(postId);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Add_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new CreatePostDto { PostName = "测试岗位" };
        _postServiceMock.Setup(x => x.CreatePostAsync(It.IsAny<CreatePostDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1L);

        var result = await _controller.Add(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Edit_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new UpdatePostDto { PostId = 1, PostName = "更新岗位" };
        _postServiceMock.Setup(x => x.UpdatePostAsync(It.IsAny<UpdatePostDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Edit(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Remove_WithValidIds_ShouldReturnSuccess()
    {
        var postIds = "1,2,3";
        _postServiceMock.Setup(x => x.DeletePostsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Remove(postIds);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task OptionSelect_ShouldReturnPosts()
    {
        _postServiceMock.Setup(x => x.GetAllPostsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PostDto>());

        var result = await _controller.OptionSelect();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Export_ShouldReturnFile()
    {
        var query = new PostQueryDto();
        var fileData = new byte[] { 1, 2, 3 };
        _postServiceMock.Setup(x => x.ExportPostsAsync(It.IsAny<PostQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileData);

        var result = await _controller.Export(query);

        result.Should().BeOfType<FileContentResult>();
    }
}
