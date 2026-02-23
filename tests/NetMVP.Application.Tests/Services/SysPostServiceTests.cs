using AutoMapper;
using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.Post;
using NetMVP.Application.Mappings;
using NetMVP.Application.Services.Impl;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;
using Xunit;

namespace NetMVP.Application.Tests.Services;

/// <summary>
/// 岗位服务测试
/// </summary>
public class SysPostServiceTests
{
    private readonly Mock<IRepository<SysPost>> _postRepositoryMock;
    private readonly Mock<IRepository<SysUser>> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IExcelService> _excelServiceMock;
    private readonly IMapper _mapper;
    private readonly SysPostService _postService;

    public SysPostServiceTests()
    {
        _postRepositoryMock = new Mock<IRepository<SysPost>>();
        _userRepositoryMock = new Mock<IRepository<SysUser>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _excelServiceMock = new Mock<IExcelService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _postService = new SysPostService(
            _postRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mapper,
            _unitOfWorkMock.Object,
            _excelServiceMock.Object
        );
    }

    [Fact]
    public async Task GetPostByIdAsync_WhenPostNotExists_ShouldReturnNull()
    {
        // Arrange
        var postId = 999L;
        _postRepositoryMock.Setup(x => x.GetByIdAsync(postId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysPost?)null);

        // Act
        var result = await _postService.GetPostByIdAsync(postId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPostByIdAsync_WhenPostExists_ShouldReturnPost()
    {
        // Arrange
        var postId = 1L;
        var post = new SysPost 
        { 
            PostId = postId, 
            PostName = "测试岗位",
            PostCode = "test",
            PostSort = 1
        };
        
        _postRepositoryMock.Setup(x => x.GetByIdAsync(postId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);

        // Act
        var result = await _postService.GetPostByIdAsync(postId);

        // Assert
        result.Should().NotBeNull();
        result!.PostId.Should().Be(postId);
        result.PostName.Should().Be("测试岗位");
    }

    [Fact]
    public async Task DeletePostAsync_WhenPostNotExists_ShouldThrowException()
    {
        // Arrange
        var postId = 1L;
        _postRepositoryMock.Setup(x => x.GetByIdAsync(postId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysPost?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _postService.DeletePostAsync(postId));
    }
}
