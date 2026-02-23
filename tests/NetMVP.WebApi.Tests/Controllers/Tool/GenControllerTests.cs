using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Gen;
using NetMVP.Application.Services.Gen;
using NetMVP.WebApi.Controllers.Tool;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers.Tool;

public class GenControllerTests : TestBase
{
    private readonly Mock<IGenTableService> _genTableServiceMock;
    private readonly Mock<ICodeGeneratorService> _codeGeneratorServiceMock;
    private readonly GenController _controller;

    public GenControllerTests()
    {
        _genTableServiceMock = new Mock<IGenTableService>();
        _codeGeneratorServiceMock = new Mock<ICodeGeneratorService>();
        _controller = new GenController(_genTableServiceMock.Object, _codeGeneratorServiceMock.Object);
        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task GetGenTableList_ShouldReturnTableList()
    {
        var query = new GenTableQueryDto();
        var pagedResult = new PagedResult<GenTableDto>
        {
            Rows = new List<GenTableDto>(),
            Total = 0
        };
        _genTableServiceMock.Setup(x => x.GetGenTableListAsync(It.IsAny<GenTableQueryDto>()))
            .ReturnsAsync(pagedResult);

        var result = await _controller.GetGenTableList(query);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task GetDbTableList_ShouldReturnDbTableList()
    {
        var query = new GenTableQueryDto();
        var pagedResult = new PagedResult<GenTableDto>
        {
            Rows = new List<GenTableDto>(),
            Total = 0
        };
        _genTableServiceMock.Setup(x => x.GetDbTableListAsync(It.IsAny<GenTableQueryDto>()))
            .ReturnsAsync(pagedResult);

        var result = await _controller.GetDbTableList(query);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task GetGenTableById_WithValidId_ShouldReturnTable()
    {
        var tableId = 1L;
        _genTableServiceMock.Setup(x => x.GetGenTableByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new GenTableDto { TableId = tableId });

        var result = await _controller.GetGenTableById(tableId);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task ImportTable_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new ImportTableDto { Tables = new[] { "sys_user", "sys_role" } };
        _genTableServiceMock.Setup(x => x.ImportGenTableAsync(It.IsAny<ImportTableDto>()))
            .ReturnsAsync(true);

        var result = await _controller.ImportTable(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task UpdateGenTable_WithValidDto_ShouldReturnSuccess()
    {
        var dto = new UpdateGenTableDto { TableId = 1 };
        _genTableServiceMock.Setup(x => x.UpdateGenTableAsync(It.IsAny<UpdateGenTableDto>()))
            .ReturnsAsync(true);

        var result = await _controller.UpdateGenTable(dto);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task DeleteGenTable_WithValidIds_ShouldReturnSuccess()
    {
        var tableIds = "1,2,3";
        _genTableServiceMock.Setup(x => x.DeleteGenTableAsync(It.IsAny<long[]>()))
            .ReturnsAsync(true);

        var result = await _controller.DeleteGenTable(tableIds);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task SyncDb_WithValidTableName_ShouldReturnSuccess()
    {
        var tableName = "sys_user";
        _genTableServiceMock.Setup(x => x.SyncDbAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await _controller.SyncDb(tableName);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task PreviewCode_WithValidTableId_ShouldReturnCode()
    {
        var tableId = 1L;
        _codeGeneratorServiceMock.Setup(x => x.PreviewCodeAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, string>());

        var result = await _controller.PreviewCode(tableId);

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public async Task DownloadCode_WithValidTableName_ShouldReturnFile()
    {
        var tableName = "sys_user";
        var fileData = new byte[] { 1, 2, 3 };
        _codeGeneratorServiceMock.Setup(x => x.DownloadCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileData);

        var result = await _controller.DownloadCode(tableName);

        result.Should().BeOfType<FileContentResult>();
    }

    [Fact]
    public async Task GenCode_WithValidTableName_ShouldReturnFile()
    {
        var tableName = "sys_user";
        var fileData = new byte[] { 1, 2, 3 };
        _codeGeneratorServiceMock.Setup(x => x.GenerateCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileData);

        var result = await _controller.GenCode(tableName);

        result.Should().BeOfType<FileContentResult>();
    }

    [Fact]
    public async Task BatchGenCode_WithValidTables_ShouldReturnFile()
    {
        var tables = "sys_user,sys_role";
        var fileData = new byte[] { 1, 2, 3 };
        _codeGeneratorServiceMock.Setup(x => x.BatchGenerateCodeAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileData);

        var result = await _controller.BatchGenCode(tables);

        result.Should().BeOfType<FileContentResult>();
    }
}
