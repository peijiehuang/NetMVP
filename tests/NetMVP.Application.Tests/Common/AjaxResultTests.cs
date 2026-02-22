using NetMVP.Application.Common.Constants;
using NetMVP.Application.Common.Models;

namespace NetMVP.Application.Tests.Common;

public class AjaxResultTests
{
    [Fact]
    public void Success_ShouldReturnSuccessResult()
    {
        // Act
        var result = AjaxResult.Success();

        // Assert
        Assert.Equal(HttpStatus.SUCCESS, result.Code);
        Assert.Equal("操作成功", result.Msg);
        Assert.True(result.IsSuccess());
    }

    [Fact]
    public void Success_WithData_ShouldReturnSuccessResultWithData()
    {
        // Arrange
        var data = new { Id = 1, Name = "Test" };

        // Act
        var result = AjaxResult.Success(data);

        // Assert
        Assert.Equal(HttpStatus.SUCCESS, result.Code);
        Assert.True(result.ContainsKey("data"));
        Assert.Equal(data, result["data"]);
    }

    [Fact]
    public void Error_ShouldReturnErrorResult()
    {
        // Act
        var result = AjaxResult.Error("操作失败");

        // Assert
        Assert.Equal(HttpStatus.ERROR, result.Code);
        Assert.Equal("操作失败", result.Msg);
        Assert.True(result.IsError());
    }

    [Fact]
    public void Warn_ShouldReturnWarnResult()
    {
        // Act
        var result = AjaxResult.Warn("警告信息");

        // Assert
        Assert.Equal(HttpStatus.WARN, result.Code);
        Assert.Equal("警告信息", result.Msg);
        Assert.True(result.IsWarn());
    }

    [Fact]
    public void Put_ShouldAddCustomField()
    {
        // Act
        var result = AjaxResult.Success()
            .Put("token", "abc123")
            .Put("userId", 1);

        // Assert
        Assert.True(result.ContainsKey("token"));
        Assert.True(result.ContainsKey("userId"));
        Assert.Equal("abc123", result["token"]);
        Assert.Equal(1, result["userId"]);
    }

    [Fact]
    public void Put_ShouldSupportChaining()
    {
        // Act
        var result = AjaxResult.Success()
            .Put("field1", "value1")
            .Put("field2", "value2")
            .Put("field3", "value3");

        // Assert
        Assert.Equal(5, result.Count); // code, msg, field1, field2, field3
        Assert.True(result.ContainsKey("field1"));
        Assert.True(result.ContainsKey("field2"));
        Assert.True(result.ContainsKey("field3"));
    }

    [Fact]
    public void TableDataInfo_ShouldBuildCorrectly()
    {
        // Arrange
        var rows = new List<object> { new { Id = 1 }, new { Id = 2 } };
        var total = 100L;

        // Act
        var result = TableDataInfo.Build(rows, total);

        // Assert
        Assert.Equal(HttpStatus.SUCCESS, result.Code);
        Assert.Equal("查询成功", result.Msg);
        Assert.Equal(total, result.Total);
        Assert.Equal(rows, result.Rows);
        Assert.True(result.ContainsKey("total"));
        Assert.True(result.ContainsKey("rows"));
    }
}
