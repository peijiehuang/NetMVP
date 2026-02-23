using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.Notice;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.System;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.System;

public class SysNoticeControllerTests : TestBase
{
    private readonly Mock<ISysNoticeService> _noticeServiceMock;
    private readonly SysNoticeController _controller;

    public SysNoticeControllerTests()
    {
        _noticeServiceMock = new Mock<ISysNoticeService>();
        _controller = new SysNoticeController(_noticeServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetList_ShouldReturnNoticeList()
    {
        var query = new NoticeQueryDto();
        _noticeServiceMock.Setup(x => x.GetNoticeListAsync(It.IsAny<NoticeQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<NoticeDto>(), 0));

        var result = await _controller.GetList(query);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task GetInfo_WithValidId_ShouldReturnNotice()
    {
        var noticeId = 1;
        _noticeServiceMock.Setup(x => x.GetNoticeByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NoticeDto { NoticeId = noticeId });

        var result = await _controller.GetInfo(noticeId);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Add_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new CreateNoticeDto { NoticeTitle = "测试公告" };
        _noticeServiceMock.Setup(x => x.CreateNoticeAsync(It.IsAny<CreateNoticeDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _controller.Add(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Edit_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new UpdateNoticeDto { NoticeId = 1, NoticeTitle = "更新公告" };
        _noticeServiceMock.Setup(x => x.UpdateNoticeAsync(It.IsAny<UpdateNoticeDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Edit(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Remove_WithValidIds_ShouldReturnSuccess()
    {
        var noticeIds = "1,2,3";
        _noticeServiceMock.Setup(x => x.DeleteNoticesAsync(It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Remove(noticeIds);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }
}
