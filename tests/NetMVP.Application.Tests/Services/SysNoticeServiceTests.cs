using AutoMapper;
using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.Notice;
using NetMVP.Application.Mappings;
using NetMVP.Application.Services.Impl;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;
using Xunit;

namespace NetMVP.Application.Tests.Services;

/// <summary>
/// 通知公告服务测试
/// </summary>
public class SysNoticeServiceTests
{
    private readonly Mock<IRepository<SysNotice>> _noticeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly SysNoticeService _noticeService;

    public SysNoticeServiceTests()
    {
        _noticeRepositoryMock = new Mock<IRepository<SysNotice>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _noticeService = new SysNoticeService(
            _noticeRepositoryMock.Object,
            _mapper,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task GetNoticeByIdAsync_WhenNoticeNotExists_ShouldReturnNull()
    {
        // Arrange
        var noticeId = 999;
        _noticeRepositoryMock.Setup(x => x.GetByIdAsync(noticeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysNotice?)null);

        // Act
        var result = await _noticeService.GetNoticeByIdAsync(noticeId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetNoticeByIdAsync_WhenNoticeExists_ShouldReturnNotice()
    {
        // Arrange
        var noticeId = 1;
        var notice = new SysNotice
        {
            NoticeId = noticeId,
            NoticeTitle = "测试公告",
            NoticeType = NoticeConstants.NOTICE_TYPE_NOTICE,
            Status = NoticeConstants.NOTICE_STATUS_NORMAL
        };

        _noticeRepositoryMock.Setup(x => x.GetByIdAsync(noticeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notice);

        // Act
        var result = await _noticeService.GetNoticeByIdAsync(noticeId);

        // Assert
        result.Should().NotBeNull();
        result!.NoticeId.Should().Be(noticeId);
        result.NoticeTitle.Should().Be("测试公告");
    }

    [Fact]
    public async Task DeleteNoticeAsync_WhenNoticeNotExists_ShouldThrowException()
    {
        // Arrange
        var noticeId = 1;
        _noticeRepositoryMock.Setup(x => x.GetByIdAsync(noticeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysNotice?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _noticeService.DeleteNoticeAsync(noticeId));
    }
}
