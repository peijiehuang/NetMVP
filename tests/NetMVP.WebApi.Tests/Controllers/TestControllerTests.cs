using FluentAssertions;
using NetMVP.WebApi.Controllers;
using Xunit;

namespace NetMVP.WebApi.Tests.Controllers;

public class TestControllerTests : TestBase
{
    private readonly TestController _controller;

    public TestControllerTests()
    {
        _controller = new TestController();
        SetupControllerContext(_controller);
    }

    [Fact]
    public void TestSuccess_ShouldReturnSuccess()
    {
        var result = _controller.TestSuccess();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public void TestSuccessWithData_ShouldReturnSuccessWithData()
    {
        var result = _controller.TestSuccessWithData();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public void TestTableData_ShouldReturnTableData()
    {
        var result = _controller.TestTableData();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public void TestCustomFields_ShouldReturnCustomFields()
    {
        var result = _controller.TestCustomFields();

        result.Should().NotBeNull();
        result.Code.Should().Be(200);
    }

    [Fact]
    public void TestError_ShouldReturnError()
    {
        var result = _controller.TestError();

        result.Should().NotBeNull();
        result.Code.Should().Be(500);
    }

    [Fact]
    public void TestWarn_ShouldReturnWarn()
    {
        var result = _controller.TestWarn();

        result.Should().NotBeNull();
        result.Code.Should().Be(601);
    }
}
