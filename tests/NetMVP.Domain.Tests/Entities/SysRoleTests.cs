using FluentAssertions;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;

namespace NetMVP.Domain.Tests.Entities;

public class SysRoleTests
{
    [Fact]
    public void IsAdmin_AdminRole_ShouldReturnTrue()
    {
        // Arrange
        var role = new SysRole { RoleId = 1 };

        // Act
        var result = role.IsAdmin();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAdmin_NonAdminRole_ShouldReturnFalse()
    {
        // Arrange
        var role = new SysRole { RoleId = 2 };

        // Act
        var result = role.IsAdmin();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void DefaultValues_ShouldBeSetCorrectly()
    {
        // Arrange & Act
        var role = new SysRole();

        // Assert
        role.DataScope.Should().Be(DataScopeConstants.DATA_SCOPE_ALL);
        role.MenuCheckStrictly.Should().BeTrue();
        role.DeptCheckStrictly.Should().BeTrue();
        role.Status.Should().Be(UserConstants.NORMAL);
        role.DelFlag.Should().Be(UserConstants.DEL_FLAG_EXIST);
        role.RoleMenus.Should().NotBeNull().And.BeEmpty();
        role.RoleDepts.Should().NotBeNull().And.BeEmpty();
    }
}
