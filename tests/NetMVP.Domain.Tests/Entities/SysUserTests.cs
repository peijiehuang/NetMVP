using FluentAssertions;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;

namespace NetMVP.Domain.Tests.Entities;

public class SysUserTests
{
    [Fact]
    public void SetPassword_ValidPassword_ShouldSetHashedPassword()
    {
        // Arrange
        var user = new SysUser();
        var plainPassword = "password123";

        // Act
        user.SetPassword(plainPassword);

        // Assert
        user.Password.Should().NotBeNullOrEmpty();
        user.Password.Should().NotBe(plainPassword);
        user.PwdUpdateDate.Should().NotBeNull();
        user.PwdUpdateDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var user = new SysUser();
        var plainPassword = "password123";
        user.SetPassword(plainPassword);

        // Act
        var result = user.VerifyPassword(plainPassword);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_IncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var user = new SysUser();
        var plainPassword = "password123";
        user.SetPassword(plainPassword);

        // Act
        var result = user.VerifyPassword("wrongpassword");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAdmin_AdminUser_ShouldReturnTrue()
    {
        // Arrange
        var user = new SysUser { UserId = 1 };

        // Act
        var result = user.IsAdmin();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAdmin_NonAdminUser_ShouldReturnFalse()
    {
        // Arrange
        var user = new SysUser { UserId = 2 };

        // Act
        var result = user.IsAdmin();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void UpdateLoginInfo_ShouldUpdateLoginIpAndDate()
    {
        // Arrange
        var user = new SysUser();
        var ipAddress = "192.168.1.1";

        // Act
        user.UpdateLoginInfo(ipAddress);

        // Assert
        user.LoginIpValue.Should().Be(ipAddress);
        user.LoginDate.Should().NotBeNull();
        user.LoginDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Email_ValidEmailValue_ShouldReturnEmailValueObject()
    {
        // Arrange
        var user = new SysUser { EmailValue = "test@example.com" };

        // Act
        var email = user.Email;

        // Assert
        email.Should().NotBeNull();
        email!.Value.Should().Be("test@example.com");
    }

    [Fact]
    public void Email_NullEmailValue_ShouldReturnNull()
    {
        // Arrange
        var user = new SysUser { EmailValue = null };

        // Act
        var email = user.Email;

        // Assert
        email.Should().BeNull();
    }

    [Fact]
    public void PhoneNumber_ValidPhoneValue_ShouldReturnPhoneNumberValueObject()
    {
        // Arrange
        var user = new SysUser { PhoneNumberValue = "13800138000" };

        // Act
        var phone = user.PhoneNumber;

        // Assert
        phone.Should().NotBeNull();
        phone!.Value.Should().Be("13800138000");
    }

    [Fact]
    public void LoginIp_ValidIpValue_ShouldReturnIpAddressValueObject()
    {
        // Arrange
        var user = new SysUser { LoginIpValue = "192.168.1.1" };

        // Act
        var ip = user.LoginIp;

        // Assert
        ip.Should().NotBeNull();
        ip!.Value.Should().Be("192.168.1.1");
    }

    [Fact]
    public void DefaultValues_ShouldBeSetCorrectly()
    {
        // Arrange & Act
        var user = new SysUser();

        // Assert
        user.Gender.Should().Be(Gender.Unknown);
        user.Status.Should().Be(UserStatus.Normal);
        user.DelFlag.Should().Be(DelFlag.Exist);
        user.UserType.Should().Be("00");
        user.UserRoles.Should().NotBeNull().And.BeEmpty();
        user.UserPosts.Should().NotBeNull().And.BeEmpty();
    }
}
