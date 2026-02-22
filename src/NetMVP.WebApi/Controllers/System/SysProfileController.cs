using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetMVP.Application.Common.Models;
using NetMVP.Application.DTOs.Profile;
using NetMVP.Application.Services;

namespace NetMVP.WebApi.Controllers.System;

/// <summary>
/// 个人中心控制器
/// </summary>
[ApiController]
[Route("system/user/profile")]
[Authorize]
public class SysProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly ILogger<SysProfileController> _logger;

    public SysProfileController(
        IProfileService profileService,
        ILogger<SysProfileController> logger)
    {
        _profileService = profileService;
        _logger = logger;
    }

    /// <summary>
    /// 获取个人信息
    /// </summary>
    [HttpGet]
    public async Task<AjaxResult> GetProfile()
    {
        var profile = await _profileService.GetProfileAsync();
        return AjaxResult.Success("查询成功", profile);
    }

    /// <summary>
    /// 更新个人信息
    /// </summary>
    [HttpPut]
    public async Task<AjaxResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        await _profileService.UpdateProfileAsync(dto);
        return AjaxResult.Success("修改成功");
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    [HttpPut("updatePwd")]
    public async Task<AjaxResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
    {
        await _profileService.UpdatePasswordAsync(dto);
        return AjaxResult.Success("修改成功");
    }

    /// <summary>
    /// 更新头像
    /// </summary>
    [HttpPost("avatar")]
    public async Task<AjaxResult> UpdateAvatar(IFormFile avatarfile)
    {
        var avatarUrl = await _profileService.UpdateAvatarAsync(avatarfile);
        return AjaxResult.Success("上传成功", new { imgUrl = avatarUrl });
    }
}
