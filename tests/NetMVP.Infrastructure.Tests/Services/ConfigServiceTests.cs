using Microsoft.Extensions.Configuration;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Services.Config;

namespace NetMVP.Infrastructure.Tests.Services;

public class ConfigServiceTests
{
    private readonly IConfigService _configService;
    private readonly IConfiguration _configuration;

    public ConfigServiceTests()
    {
        // 创建内存配置
        var configData = new Dictionary<string, string?>
        {
            { "TestKey", "TestValue" },
            { "TestInt", "123" },
            { "TestBool", "true" },
            { "Section:Key1", "Value1" },
            { "Section:Key2", "Value2" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        _configService = new ConfigService(_configuration);
    }

    [Fact]
    public void GetValue_ShouldReturnCorrectValue()
    {
        // Act
        var value = _configService.GetValue<string>("TestKey");

        // Assert
        Assert.Equal("TestValue", value);
    }

    [Fact]
    public void GetValue_ShouldReturnDefaultValue_WhenKeyNotExists()
    {
        // Act
        var value = _configService.GetValue<string>("NonExistentKey", "DefaultValue");

        // Assert
        Assert.Equal("DefaultValue", value);
    }

    [Fact]
    public void GetValue_ShouldConvertToCorrectType()
    {
        // Act
        var intValue = _configService.GetValue<int>("TestInt");
        var boolValue = _configService.GetValue<bool>("TestBool");

        // Assert
        Assert.Equal(123, intValue);
        Assert.True(boolValue);
    }

    [Fact]
    public void GetSection_ShouldReturnCorrectSection()
    {
        // Act
        var section = _configService.GetSection("Section");

        // Assert
        Assert.NotNull(section);
        Assert.Equal("Value1", section["Key1"]);
        Assert.Equal("Value2", section["Key2"]);
    }

    [Fact]
    public void Reload_ShouldNotThrowException()
    {
        // Act & Assert
        var exception = Record.Exception(() => _configService.Reload());
        Assert.Null(exception);
    }
}
