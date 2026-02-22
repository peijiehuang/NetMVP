using NetMVP.Infrastructure.Helpers;

namespace NetMVP.Infrastructure.Tests.Helpers;

public class StringHelperTests
{
    [Fact]
    public void ToSnakeCase_ShouldConvertCorrectly()
    {
        // Arrange
        var input = "HelloWorld";

        // Act
        var result = StringHelper.ToSnakeCase(input);

        // Assert
        Assert.Equal("hello_world", result);
    }

    [Fact]
    public void ToPascalCase_ShouldConvertCorrectly()
    {
        // Arrange
        var input = "hello_world";

        // Act
        var result = StringHelper.ToPascalCase(input);

        // Assert
        Assert.Equal("HelloWorld", result);
    }

    [Fact]
    public void ToCamelCase_ShouldConvertCorrectly()
    {
        // Arrange
        var input = "hello_world";

        // Act
        var result = StringHelper.ToCamelCase(input);

        // Assert
        Assert.Equal("helloWorld", result);
    }

    [Fact]
    public void Truncate_ShouldTruncateCorrectly()
    {
        // Arrange
        var input = "This is a long string";
        var maxLength = 10;

        // Act
        var result = StringHelper.Truncate(input, maxLength);

        // Assert
        Assert.Equal("This is a ...", result);
    }

    [Fact]
    public void GenerateRandomString_ShouldReturnCorrectLength()
    {
        // Arrange
        var length = 10;

        // Act
        var result = StringHelper.GenerateRandomString(length);

        // Assert
        Assert.Equal(length, result.Length);
    }
}
