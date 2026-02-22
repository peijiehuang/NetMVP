using FluentAssertions;
using NetMVP.Domain.ValueObjects;

namespace NetMVP.Domain.Tests.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("13800138000")]
    [InlineData("15912345678")]
    [InlineData("18888888888")]
    public void Create_ValidPhoneNumber_ShouldCreatePhoneNumber(string phoneNumber)
    {
        // Act
        var phone = PhoneNumber.Create(phoneNumber);

        // Assert
        phone.Should().NotBeNull();
        phone!.Value.Should().Be(phoneNumber);
    }

    [Theory]
    [InlineData("12345678901")] // 不是1开头
    [InlineData("10012345678")] // 第二位不是3-9
    [InlineData("1381234567")]  // 少于11位
    [InlineData("138123456789")] // 多于11位
    [InlineData("abcdefghijk")] // 非数字
    public void Create_InvalidPhoneNumber_ShouldThrowException(string phoneNumber)
    {
        // Act
        Action act = () => PhoneNumber.Create(phoneNumber);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("手机号格式不正确*");
    }

    [Fact]
    public void Create_NullPhoneNumber_ShouldReturnNull()
    {
        // Act
        var phone = PhoneNumber.Create(null);

        // Assert
        phone.Should().BeNull();
    }

    [Fact]
    public void Create_EmptyPhoneNumber_ShouldReturnNull()
    {
        // Act
        var phone = PhoneNumber.Create("");

        // Assert
        phone.Should().BeNull();
    }

    [Fact]
    public void IsValid_ValidPhoneNumber_ShouldReturnTrue()
    {
        // Arrange
        var phoneNumber = "13800138000";

        // Act
        var result = PhoneNumber.IsValid(phoneNumber);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_InvalidPhoneNumber_ShouldReturnFalse()
    {
        // Arrange
        var phoneNumber = "12345678901";

        // Act
        var result = PhoneNumber.IsValid(phoneNumber);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_SamePhoneNumber_ShouldReturnTrue()
    {
        // Arrange
        var phone1 = PhoneNumber.Create("13800138000");
        var phone2 = PhoneNumber.Create("13800138000");

        // Act
        var result = phone1!.Equals(phone2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnValue()
    {
        // Arrange
        var phone = PhoneNumber.Create("13800138000");

        // Act
        string value = phone!;

        // Assert
        value.Should().Be("13800138000");
    }
}
