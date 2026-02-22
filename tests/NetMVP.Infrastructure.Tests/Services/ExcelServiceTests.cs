using NetMVP.Infrastructure.Services.Excel;

namespace NetMVP.Infrastructure.Tests.Services;

public class ExcelServiceTests
{
    private readonly ExcelService _excelService;
    private readonly string _testFilePath;

    public ExcelServiceTests()
    {
        _excelService = new ExcelService();
        _testFilePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.xlsx");
    }

    private class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
    }

    [Fact]
    public async Task ExportAsync_ToFile_ShouldCreateFile()
    {
        // Arrange
        var data = new List<TestModel>
        {
            new() { Id = 1, Name = "Test1", CreateTime = DateTime.Now },
            new() { Id = 2, Name = "Test2", CreateTime = DateTime.Now }
        };

        try
        {
            // Act
            await _excelService.ExportAsync(data, _testFilePath);

            // Assert
            Assert.True(File.Exists(_testFilePath));
        }
        finally
        {
            // Cleanup
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);
        }
    }

    [Fact]
    public async Task ExportAsync_ToStream_ShouldWriteData()
    {
        // Arrange
        var data = new List<TestModel>
        {
            new() { Id = 1, Name = "Test1", CreateTime = DateTime.Now }
        };

        using var stream = new MemoryStream();

        // Act
        await _excelService.ExportAsync(data, stream);

        // Assert
        Assert.True(stream.Length > 0);
    }

    [Fact]
    public async Task ImportAsync_FromFile_ShouldReadData()
    {
        // Arrange
        var data = new List<TestModel>
        {
            new() { Id = 1, Name = "Test1", CreateTime = DateTime.Now },
            new() { Id = 2, Name = "Test2", CreateTime = DateTime.Now }
        };

        try
        {
            await _excelService.ExportAsync(data, _testFilePath);

            // Act
            var result = await _excelService.ImportAsync<TestModel>(_testFilePath);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Test1", result[0].Name);
            Assert.Equal("Test2", result[1].Name);
        }
        finally
        {
            // Cleanup
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);
        }
    }

    [Fact]
    public async Task ImportAsync_FromStream_ShouldReadData()
    {
        // Arrange
        var data = new List<TestModel>
        {
            new() { Id = 1, Name = "Test1", CreateTime = DateTime.Now }
        };

        using var exportStream = new MemoryStream();
        await _excelService.ExportAsync(data, exportStream);
        exportStream.Position = 0;

        // Act
        var result = await _excelService.ImportAsync<TestModel>(exportStream);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test1", result[0].Name);
    }
}
