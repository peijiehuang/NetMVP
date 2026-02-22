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
/// 字典数据服务测试
/// </summary>
public class SysDictDataServiceTests
{
    private readonly Mock<IRepository<SysDictData>> _dictDataRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IExcelService> _excelServiceMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly IMapper _mapper;
    private readonly SysDictDataService _dictDataService;

    public SysDictDataServiceTests()
    {
        _dictDataRepositoryMock = new Mock<IRepository<SysDictData>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _excelServiceMock = new Mock<IExcelService>();
        _cacheServiceMock = new Mock<ICacheService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _dictDataService = new SysDictDataService(
            _dictDataRepositoryMock.Object,
            _mapper,
            _unitOfWorkMock.Object,
            _excelServiceMock.Object,
            _cacheServiceMock.Object
        );
    }

    [Fact]
    public async Task GetDictDataByIdAsync_WhenDictDataNotExists_ShouldReturnNull()
    {
        // Arrange
        var dictCode = 999L;
        _dictDataRepositoryMock.Setup(x => x.GetByIdAsync(dictCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysDictData?)null);

        // Act
        var result = await _dictDataService.GetDictDataByIdAsync(dictCode);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDictDataByIdAsync_WhenDictDataExists_ShouldReturnDictData()
    {
        // Arrange
        var dictCode = 1L;
        var dictData = new SysDictData 
        { 
            DictCode = dictCode, 
            DictType = "test_type",
            DictLabel = "测试标签",
            DictValue = "1"
        };
        
        _dictDataRepositoryMock.Setup(x => x.GetByIdAsync(dictCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dictData);

        // Act
        var result = await _dictDataService.GetDictDataByIdAsync(dictCode);

        // Assert
        result.Should().NotBeNull();
        result!.DictCode.Should().Be(dictCode);
        result.DictLabel.Should().Be("测试标签");
    }

    [Fact]
    public async Task DeleteDictDataAsync_WhenDictDataNotExists_ShouldThrowException()
    {
        // Arrange
        var dictCode = 1L;
        _dictDataRepositoryMock.Setup(x => x.GetByIdAsync(dictCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysDictData?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _dictDataService.DeleteDictDataAsync(dictCode));
    }

    [Fact]
    public async Task GetDictDataByTypeAsync_WhenCacheExists_ShouldReturnCachedData()
    {
        // Arrange
        var dictType = "test_type";
        var cachedData = new List<DictDataDto>
        {
            new DictDataDto { DictCode = 1, DictLabel = "测试1", DictValue = "1" }
        };

        _cacheServiceMock.Setup(x => x.GetAsync<List<DictDataDto>>(
            It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedData);

        // Act
        var result = await _dictDataService.GetDictDataByTypeAsync(dictType);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        _dictDataRepositoryMock.Verify(x => x.GetQueryable(), Times.Never);
    }
}
