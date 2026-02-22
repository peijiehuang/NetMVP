using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Auth;
using NetMVP.Application.Services;

namespace NetMVP.WebApi.Controllers;

/// <summary>
/// 用户注册控制器
/// </summary>
[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class SysRegisterController : ControllerBase
{
    private readonly IRegisterService _registerService;
    private readonly ILogger<SysRegisterController> _logger;

    public SysRegisterController(
        IRegisterService registerService,
        ILogger<SysRegisterController> logger)
    {
        _registerService = registerService;
        _logger = logger;
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    [HttpPost]
    public async Task<AjaxResult> Register([FromBody] RegisterDto dto)
    {
        await _registerService.RegisterAsync(dto);
        return AjaxResult.Success("注册成功");
    }
}
