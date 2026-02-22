using FluentAssertions;
using NetMVP.Domain.ValueObjects;

namespace NetMVP.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@example.com")]
    [InlineData("user+tag@example.co.uk")]
    [InlineData("test123@test-domain.com")]
    public void Create_ValidEmail_ShouldCreateEmail(string emailAddress)
    {
        // Act
        var email = Email.Create(emailAddress);

        // Assert
        email.Should().NotBeNull();
        email!.Value.Should().Be(emailAddress);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test@.com")]
    public void Create_InvalidEmail_ShouldThrowException(string emailAddress)
    {
        // Act
        Action act = () => Email.Create(emailAddress);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("邮箱格式不正确*");
    }

    [Fact]
    public void Create_NullEmail_ShouldReturnNull()
    {
        // Act
        var email = Email.Create(null);

        // Assert
        email.Should().BeNull();
    }

    [Fact]
    public void Create_EmptyEmail_ShouldReturnNull()
    {
        // Act
        var email = Email.Create("");

        // Assert
        email.Should().BeNull();
    }

    [Fact]
    public void IsValid_ValidEmail_ShouldReturnTrue()
    {
        // Arrange
        var emailAddress = "test@example.com";

        // Act
        var result = Email.IsValid(emailAddress);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_InvalidEmail_ShouldReturnFalse()
    {
        // Arrange
        var emailAddress = "invalid-email";

        // Act
        var result = Email.IsValid(emailAddress);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_SameEmail_CaseInsensitive_ShouldReturnTrue()
    {
        // Arrange
        var email1 = Email.Create("Test@Example.com");
        var email2 = Email.Create("test@example.com");

        // Act
        var result = email1!.Equals(email2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnValue()
    {
        // Arrange
        var email = Email.Create("test@example.com");

        // Act
        string value = email!;

        // Assert
        value.Should().Be("test@example.com");
    }
}
