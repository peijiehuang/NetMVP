using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers;

public class CommonControllerTests : TestBase
{
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<ILogger<CommonController>> _loggerMock;
    private readonly CommonController _controller;

    public CommonControllerTests()
    {
        _fileServiceMock = new Mock<IFileService>();
        _loggerMock = new Mock<ILogger<CommonController>>();
        _controller = new CommonController(_fileServiceMock.Object, _loggerMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task Upload_WithValidFile_ShouldReturnSuccess()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("test.txt");
        fileMock.Setup(f => f.Length).Returns(1024);

        _fileServiceMock.Setup(x => x.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("/uploads/test.txt");

        var result = await _controller.Upload(fileMock.Object);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Download_WithValidFileName_ShouldReturnFile()
    {
        var fileName = "test.txt";
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        _fileServiceMock.Setup(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((stream, "text/plain", fileName));

        var result = await _controller.Download(fileName);

        result.Should().BeOfType<FileStreamResult>();
    }

    [Fact]
    public async Task Delete_WithValidFileName_ShouldReturnSuccess()
    {
        var fileName = "test.txt";
        _fileServiceMock.Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Delete(fileName);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }
}
