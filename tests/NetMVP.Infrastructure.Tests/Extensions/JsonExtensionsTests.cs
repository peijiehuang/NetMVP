using NetMVP.Infrastructure.Extensions;

namespace NetMVP.Infrastructure.Tests.Extensions;

public class JsonExtensionsTests
{
    private class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void ToJson_ShouldSerializeCorrectly()
    {
        // Arrange
        var obj = new TestModel { Id = 1, Name = "Test" };

        // Act
        var json = obj.ToJson();

        // Assert
        Assert.Contains("\"id\":1", json);
        Assert.Contains("\"name\":\"Test\"", json);
    }

    [Fact]
    public void ToObject_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = "{\"id\":1,\"name\":\"Test\"}";

        // Act
        var obj = json.ToObject<TestModel>();

        // Assert
        Assert.NotNull(obj);
        Assert.Equal(1, obj.Id);
        Assert.Equal("Test", obj.Name);
    }

    [Fact]
    public async Task ToJsonAsync_ShouldSerializeCorrectly()
    {
        // Arrange
        var obj = new TestModel { Id = 1, Name = "Test" };

        // Act
        var json = await obj.ToJsonAsync();

        // Assert
        Assert.Contains("\"id\":1", json);
        Assert.Contains("\"name\":\"Test\"", json);
    }

    [Fact]
    public async Task ToObjectAsync_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = "{\"id\":1,\"name\":\"Test\"}";

        // Act
        var obj = await json.ToObjectAsync<TestModel>();

        // Assert
        Assert.NotNull(obj);
        Assert.Equal(1, obj.Id);
        Assert.Equal("Test", obj.Name);
    }
}
