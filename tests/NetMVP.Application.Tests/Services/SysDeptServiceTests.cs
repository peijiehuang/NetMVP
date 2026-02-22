using AutoMapper;
using FluentAssertions;
using Moq;
using NetMVP.Application.DTOs.Dept;
using NetMVP.Application.Mappings;
using NetMVP.Application.Services.Impl;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Domain.Interfaces;
using Xunit;

namespace NetMVP.Application.Tests.Services;

/// <summary>
/// 部门服务测试
/// </summary>
public class SysDeptServiceTests
{
    private readonly Mock<IRepository<SysDept>> _deptRepositoryMock;
    private readonly Mock<IRepository<SysUser>> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly SysDeptService _deptService;

    public SysDeptServiceTests()
    {
        _deptRepositoryMock = new Mock<IRepository<SysDept>>();
        _userRepositoryMock = new Mock<IRepository<SysUser>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _deptService = new SysDeptService(
            _deptRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mapper,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task GetDeptByIdAsync_WhenDeptNotExists_ShouldReturnNull()
    {
        // Arrange
        var deptId = 999L;
        _deptRepositoryMock.Setup(x => x.GetByIdAsync(deptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysDept?)null);

        // Act
        var result = await _deptService.GetDeptByIdAsync(deptId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDeptByIdAsync_WhenDeptExists_ShouldReturnDept()
    {
        // Arrange
        var deptId = 1L;
        var dept = new SysDept 
        { 
            DeptId = deptId, 
            DeptName = "测试部门",
            ParentId = 0,
            Ancestors = "0"
        };
        
        _deptRepositoryMock.Setup(x => x.GetByIdAsync(deptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dept);

        // Act
        var result = await _deptService.GetDeptByIdAsync(deptId);

        // Assert
        result.Should().NotBeNull();
        result!.DeptId.Should().Be(deptId);
        result.DeptName.Should().Be("测试部门");
    }

    [Fact]
    public async Task DeleteDeptAsync_WhenDeptNotExists_ShouldThrowException()
    {
        // Arrange
        var deptId = 1L;
        _deptRepositoryMock.Setup(x => x.GetByIdAsync(deptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SysDept?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _deptService.DeleteDeptAsync(deptId));
    }
}
