using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetMVP.Application.DTOs.Dict;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.System;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.System;

public class SysDictTypeControllerTests : TestBase
{
    private readonly Mock<ISysDictTypeService> _dictTypeServiceMock;
    private readonly SysDictTypeController _controller;

    public SysDictTypeControllerTests()
    {
        _dictTypeServiceMock = new Mock<ISysDictTypeService>();
        _controller = new SysDictTypeController(_dictTypeServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetList_ShouldReturnDictTypeList()
    {
        var query = new DictTypeQueryDto();
        _dictTypeServiceMock.Setup(x => x.GetDictTypeListAsync(It.IsAny<DictTypeQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<DictTypeDto>(), 0));

        var result = await _controller.GetList(query);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetInfo_WithValidId_ShouldReturnDictType()
    {
        var dictId = 1L;
        _dictTypeServiceMock.Setup(x => x.GetDictTypeByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DictTypeDto { DictId = dictId });

        var result = await _controller.GetInfo(dictId);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Add_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new CreateDictTypeDto { DictName = "测试字典" };
        _dictTypeServiceMock.Setup(x => x.CreateDictTypeAsync(It.IsAny<CreateDictTypeDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1L);

        var result = await _controller.Add(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Edit_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new UpdateDictTypeDto { DictId = 1, DictName = "更新字典" };
        _dictTypeServiceMock.Setup(x => x.UpdateDictTypeAsync(It.IsAny<UpdateDictTypeDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Edit(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Remove_WithValidIds_ShouldReturnSuccess()
    {
        var dictIds = "1,2,3";
        _dictTypeServiceMock.Setup(x => x.DeleteDictTypesAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Remove(dictIds);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task RefreshCache_ShouldReturnSuccess()
    {
        _dictTypeServiceMock.Setup(x => x.RefreshDictCacheAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.RefreshCache();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task OptionSelect_ShouldReturnDictTypes()
    {
        _dictTypeServiceMock.Setup(x => x.GetAllDictTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DictTypeDto>());

        var result = await _controller.OptionSelect();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Export_ShouldReturnFile()
    {
        var query = new DictTypeQueryDto();
        var fileData = new byte[] { 1, 2, 3 };
        _dictTypeServiceMock.Setup(x => x.ExportDictTypesAsync(It.IsAny<DictTypeQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileData);

        var result = await _controller.Export(query);

        result.Should().BeOfType<FileContentResult>();
    }
}
