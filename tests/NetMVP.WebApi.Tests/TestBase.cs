using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace NetMVP.WebApi.Tests;

/// <summary>
/// 测试基类
/// </summary>
public abstract class TestBase
{
    protected Mock<HttpContext> MockHttpContext { get; }
    protected Mock<HttpRequest> MockHttpRequest { get; }
    protected Mock<HttpResponse> MockHttpResponse { get; }

    protected TestBase()
    {
        MockHttpContext = new Mock<HttpContext>();
        MockHttpRequest = new Mock<HttpRequest>();
        MockHttpResponse = new Mock<HttpResponse>();

        MockHttpContext.Setup(x => x.Request).Returns(MockHttpRequest.Object);
        MockHttpContext.Setup(x => x.Response).Returns(MockHttpResponse.Object);
    }

    /// <summary>
    /// 设置控制器上下文（包含用户信息）
    /// </summary>
    protected void SetupControllerContext(ControllerBase controller, long userId = 1, string userName = "admin")
    {
        var claims = new List<Claim>
        {
            new Claim("userId", userId.ToString()),
            new Claim("userName", userName),
            new Claim("sub", userId.ToString()),
            new Claim("name", userName)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        MockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = MockHttpContext.Object
        };
    }
}
