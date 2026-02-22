using AutoMapper;
using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.Dict;
using NetMVP.Application.Mappings;
using NetMVP.Application.Services.Impl;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Domain.Interfaces;
using Xunit;

namespace NetMVP.Application.Tests.Services;

/// <summary>
/// 字典类型服务测试
/// </summary>
public class SysDictTypeServiceTests
{
    private readonly Mock<IRepository<SysDictType>> _dictTypeRepositoryMock;
    private readonly Mock<IRepository<SysDictData>> _dictDataRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IExcelService> _excelServiceMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly IMapper _mapper;
    private readonly SysDictTypeService _dictTypeService;

    public SysDictTypeServiceTests()
    {
        _dictTypeRepositoryMock = new Mock<IRepository<SysDictType>>();
        _dictDataRepositoryMock = new Mock<IRepository<SysDictData>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _excelServiceMock = new Mock<IExcelService>();
        _cacheServiceMock = new Mock<ICacheService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _dictTypeService = new SysDictTypeService(
            _dictTypeRepositoryMock.Object,
            _dictDataRepositoryMock.Object,
            _mapper,
            _unitOfWorkMock.Object,
            _excelServiceMock.Object,
            _cacheServiceMock.Object
        );
    }

    [Fact]
    public async Task GetDictTypeByIdAsync_WhenDictTypeNotExists_ShouldReturnNull()
    {
        // Arrange
        var dictId = 999L;
        _dictTypeRepositoryMock.Setup(x => x.GetByIdAsync(dictId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysDictType?)null);

        // Act
        var result = await _dictTypeService.GetDictTypeByIdAsync(dictId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDictTypeByIdAsync_WhenDictTypeExists_ShouldReturnDictType()
    {
        // Arrange
        var dictId = 1L;
        var dictType = new SysDictType 
        { 
            DictId = dictId, 
            DictType = "test_type",
            DictName = "测试类型"
        };
        
        _dictTypeRepositoryMock.Setup(x => x.GetByIdAsync(dictId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dictType);

        // Act
        var result = await _dictTypeService.GetDictTypeByIdAsync(dictId);

        // Assert
        result.Should().NotBeNull();
        result!.DictId.Should().Be(dictId);
        result.DictName.Should().Be("测试类型");
    }

    [Fact]
    public async Task DeleteDictTypeAsync_WhenDictTypeNotExists_ShouldThrowException()
    {
        // Arrange
        var dictId = 1L;
        _dictTypeRepositoryMock.Setup(x => x.GetByIdAsync(dictId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysDictType?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _dictTypeService.DeleteDictTypeAsync(dictId));
    }
}
