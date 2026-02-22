using FluentAssertions;
using NetMVP.Domain.ValueObjects;

namespace NetMVP.Domain.Tests.ValueObjects;

public class IpAddressTests
{
    [Theory]
    [InlineData("192.168.1.1")]
    [InlineData("127.0.0.1")]
    [InlineData("10.0.0.1")]
    [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334")] // IPv6
    [InlineData("::1")] // IPv6 loopback
    public void Create_ValidIpAddress_ShouldCreateIpAddress(string ipAddress)
    {
        // Act
        var ip = IpAddress.Create(ipAddress);

        // Assert
        ip.Should().NotBeNull();
        ip!.Value.Should().Be(ipAddress);
    }

    [Theory]
    [InlineData("256.1.1.1")]
    [InlineData("192.168.1.1.1")]
    [InlineData("invalid")]
    [InlineData("abc.def.ghi.jkl")]
    public void Create_InvalidIpAddress_ShouldThrowException(string ipAddress)
    {
        // Act
        Action act = () => IpAddress.Create(ipAddress);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("IP地址格式不正确*");
    }

    [Fact]
    public void Create_NullIpAddress_ShouldReturnNull()
    {
        // Act
        var ip = IpAddress.Create(null);

        // Assert
        ip.Should().BeNull();
    }

    [Fact]
    public void Create_EmptyIpAddress_ShouldReturnNull()
    {
        // Act
        var ip = IpAddress.Create("");

        // Assert
        ip.Should().BeNull();
    }

    [Fact]
    public void IsValid_ValidIpAddress_ShouldReturnTrue()
    {
        // Arrange
        var ipAddress = "192.168.1.1";

        // Act
        var result = IpAddress.IsValid(ipAddress);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_InvalidIpAddress_ShouldReturnFalse()
    {
        // Arrange
        var ipAddress = "256.1.1.1";

        // Act
        var result = IpAddress.IsValid(ipAddress);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_SameIpAddress_ShouldReturnTrue()
    {
        // Arrange
        var ip1 = IpAddress.Create("192.168.1.1");
        var ip2 = IpAddress.Create("192.168.1.1");

        // Act
        var result = ip1!.Equals(ip2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnValue()
    {
        // Arrange
        var ip = IpAddress.Create("192.168.1.1");

        // Act
        string value = ip!;

        // Assert
        value.Should().Be("192.168.1.1");
    }
}
