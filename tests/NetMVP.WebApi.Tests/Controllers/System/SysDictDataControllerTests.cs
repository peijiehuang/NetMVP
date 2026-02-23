using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetMVP.Application.DTOs.Dict;
using NetMVP.Application.Services;
using NetMVP.WebApi.Controllers.System;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.System;

public class SysDictDataControllerTests : TestBase
{
    private readonly Mock<ISysDictDataService> _dictDataServiceMock;
    private readonly SysDictDataController _controller;

    public SysDictDataControllerTests()
    {
        _dictDataServiceMock = new Mock<ISysDictDataService>();
        _controller = new SysDictDataController(_dictDataServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetList_ShouldReturnDictDataList()
    {
        var query = new DictDataQueryDto();
        _dictDataServiceMock.Setup(x => x.GetDictDataListAsync(It.IsAny<DictDataQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<DictDataDto>(), 0));

        var result = await _controller.GetList(query);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDictDataByType_WithValidType_ShouldReturnData()
    {
        var dictType = "sys_user_sex";
        _dictDataServiceMock.Setup(x => x.GetDictDataByTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DictDataDto>());

        var result = await _controller.GetDictDataByType(dictType);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task GetInfo_WithValidId_ShouldReturnDictData()
    {
        var dictCode = 1L;
        _dictDataServiceMock.Setup(x => x.GetDictDataByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DictDataDto { DictCode = dictCode });

        var result = await _controller.GetInfo(dictCode);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Add_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new CreateDictDataDto { DictLabel = "测试" };
        _dictDataServiceMock.Setup(x => x.CreateDictDataAsync(It.IsAny<CreateDictDataDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1L);

        var result = await _controller.Add(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Edit_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new UpdateDictDataDto { DictCode = 1, DictLabel = "更新" };
        _dictDataServiceMock.Setup(x => x.UpdateDictDataAsync(It.IsAny<UpdateDictDataDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Edit(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Remove_WithValidIds_ShouldReturnSuccess()
    {
        var dictCodes = "1,2,3";
        _dictDataServiceMock.Setup(x => x.DeleteDictDataAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Remove(dictCodes);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task Export_ShouldReturnFile()
    {
        var query = new DictDataQueryDto();
        var fileData = new byte[] { 1, 2, 3 };
        _dictDataServiceMock.Setup(x => x.ExportDictDataAsync(It.IsAny<DictDataQueryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileData);

        var result = await _controller.Export(query);

        result.Should().BeOfType<FileContentResult>();
    }
}
