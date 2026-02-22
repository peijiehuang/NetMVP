using NetMVP.Infrastructure.Helpers;

namespace NetMVP.Infrastructure.Tests.Helpers;

public class ValidationHelperTests
{
    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("invalid.email", false)]
    [InlineData("", false)]
    public void IsEmail_ShouldValidateCorrectly(string email, bool expected)
    {
        // Act
        var result = ValidationHelper.IsEmail(email);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("13800138000", true)]
    [InlineData("12345678901", false)]
    [InlineData("", false)]
    public void IsPhone_ShouldValidateCorrectly(string phone, bool expected)
    {
        // Act
        var result = ValidationHelper.IsPhone(phone);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://www.example.com", true)]
    [InlineData("http://example.com", true)]
    [InlineData("not a url", false)]
    [InlineData("", false)]
    public void IsUrl_ShouldValidateCorrectly(string url, bool expected)
    {
        // Act
        var result = ValidationHelper.IsUrl(url);

        // Assert
        Assert.Equal(expected, result);
    }
}
