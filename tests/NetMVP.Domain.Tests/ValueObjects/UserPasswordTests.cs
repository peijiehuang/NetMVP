using FluentAssertions;
using NetMVP.Domain.ValueObjects;

namespace NetMVP.Domain.Tests.ValueObjects;

public class UserPasswordTests
{
    [Fact]
    public void CreateFromPlainText_ValidPassword_ShouldCreatePassword()
    {
        // Arrange
        var plainPassword = "password123";

        // Act
        var password = UserPassword.CreateFromPlainText(plainPassword);

        // Assert
        password.Should().NotBeNull();
        password.HashedPassword.Should().NotBeNullOrEmpty();
        password.HashedPassword.Should().NotBe(plainPassword);
    }

    [Fact]
    public void CreateFromPlainText_EmptyPassword_ShouldThrowException()
    {
        // Arrange
        var plainPassword = "";

        // Act
        Action act = () => UserPassword.CreateFromPlainText(plainPassword);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("密码不能为空*");
    }

    [Fact]
    public void CreateFromPlainText_ShortPassword_ShouldThrowException()
    {
        // Arrange
        var plainPassword = "12345";

        // Act
        Action act = () => UserPassword.CreateFromPlainText(plainPassword);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("密码长度不能少于6位*");
    }

    [Fact]
    public void Verify_CorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var plainPassword = "password123";
        var password = UserPassword.CreateFromPlainText(plainPassword);

        // Act
        var result = password.Verify(plainPassword);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_IncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var plainPassword = "password123";
        var password = UserPassword.CreateFromPlainText(plainPassword);

        // Act
        var result = password.Verify("wrongpassword");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Verify_EmptyPassword_ShouldReturnFalse()
    {
        // Arrange
        var plainPassword = "password123";
        var password = UserPassword.CreateFromPlainText(plainPassword);

        // Act
        var result = password.Verify("");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CreateFromHash_ValidHash_ShouldCreatePassword()
    {
        // Arrange
        var hashedPassword = "$2a$11$abcdefghijklmnopqrstuvwxyz1234567890";

        // Act
        var password = UserPassword.CreateFromHash(hashedPassword);

        // Assert
        password.Should().NotBeNull();
        password.HashedPassword.Should().Be(hashedPassword);
    }

    [Fact]
    public void ToString_ShouldReturnMaskedPassword()
    {
        // Arrange
        var plainPassword = "password123";
        var password = UserPassword.CreateFromPlainText(plainPassword);

        // Act
        var result = password.ToString();

        // Assert
        result.Should().Be("********");
    }

    [Fact]
    public void Equals_SameHashedPassword_ShouldReturnTrue()
    {
        // Arrange
        var hashedPassword = "$2a$11$abcdefghijklmnopqrstuvwxyz1234567890";
        var password1 = UserPassword.CreateFromHash(hashedPassword);
        var password2 = UserPassword.CreateFromHash(hashedPassword);

        // Act
        var result = password1.Equals(password2);

        // Assert
        result.Should().BeTrue();
    }
}
