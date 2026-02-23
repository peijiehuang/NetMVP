using Microsoft.EntityFrameworkCore;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;
using NetMVP.Infrastructure.Persistence;
using NetMVP.Infrastructure.Repositories;

namespace NetMVP.Infrastructure.Tests.Repositories;

/// <summary>
/// 仓储基础功能测试
/// </summary>
public class RepositoryTests : IDisposable
{
    private readonly NetMVPDbContext _context;
    private readonly Repository<SysUser> _repository;

    public RepositoryTests()
    {
        // 使用内存数据库进行测试
        var options = new DbContextOptionsBuilder<NetMVPDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new NetMVPDbContext(options);
        _repository = new Repository<SysUser>(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        var user = new SysUser
        {
            UserName = "testuser",
            NickName = "测试用户",
            Status = UserConstants.NORMAL
        };

        // Act
        await _repository.AddAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(user.UserId);
        Assert.NotNull(result);
        Assert.Equal("testuser", result.UserName);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var users = new List<SysUser>
        {
            new SysUser { UserName = "user1", NickName = "用户1", Status = UserConstants.NORMAL },
            new SysUser { UserName = "user2", NickName = "用户2", Status = UserConstants.NORMAL },
            new SysUser { UserName = "user3", NickName = "用户3", Status = UserConstants.NORMAL }
        };

        foreach (var user in users)
        {
            await _repository.AddAsync(user);
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnMatchingEntities()
    {
        // Arrange
        var users = new List<SysUser>
        {
            new SysUser { UserName = "admin", NickName = "管理员", Status = UserConstants.NORMAL },
            new SysUser { UserName = "user1", NickName = "用户1", Status = UserConstants.USER_DISABLE },
            new SysUser { UserName = "user2", NickName = "用户2", Status = UserConstants.NORMAL }
        };

        foreach (var user in users)
        {
            await _repository.AddAsync(user);
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(u => u.Status == UserConstants.NORMAL);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        var user = new SysUser
        {
            UserName = "testuser",
            NickName = "测试用户",
            Status = UserConstants.NORMAL
        };

        await _repository.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        user.NickName = "更新后的用户";
        await _repository.UpdateAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(user.UserId);
        Assert.NotNull(result);
        Assert.Equal("更新后的用户", result.NickName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var user = new SysUser
        {
            UserName = "testuser",
            NickName = "测试用户",
            Status = UserConstants.NORMAL
        };

        await _repository.AddAsync(user);
        await _context.SaveChangesAsync();
        var userId = user.UserId;

        // Act
        await _repository.DeleteAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(userId);
        Assert.Null(result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
