using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetMVP.Application.DTOs.Profile;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Domain.Exceptions;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 个人中心服务实现
/// </summary>
public class ProfileService : IProfileService
{
    private readonly ISysUserRepository _userRepository;
    private readonly ISysDeptRepository _deptRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(
        ISysUserRepository userRepository,
        ISysDeptRepository deptRepository,
        ICurrentUserService currentUserService,
        IFileService fileService,
        IMapper mapper,
        ILogger<ProfileService> logger)
    {
        _userRepository = userRepository;
        _deptRepository = deptRepository;
        _currentUserService = currentUserService;
        _fileService = fileService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProfileDto> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetUserId();
        if (userId == 0)
        {
            throw new UnauthorizedException("用户未登录");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("用户不存在");
        }

        var profile = _mapper.Map<ProfileDto>(user);

        // 获取部门名称
        if (user.DeptId.HasValue)
        {
            var dept = await _deptRepository.GetByIdAsync(user.DeptId.Value, cancellationToken);
            profile.DeptName = dept?.DeptName;
        }

        // 获取岗位和角色（从用户关联表）
        var posts = await _userRepository.GetUserPostsAsync(userId, cancellationToken);
        profile.PostIds = posts.Select(p => p.PostId.ToString()).ToList();

        var roles = await _userRepository.GetUserRolesAsync(userId, cancellationToken);
        profile.RoleIds = roles.Select(r => r.RoleId.ToString()).ToList();

        return profile;
    }

    public async Task UpdateProfileAsync(UpdateProfileDto dto, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetUserId();
        if (userId == 0)
        {
            throw new UnauthorizedException("用户未登录");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("用户不存在");
        }

        // 更新个人信息
        user.NickName = dto.NickName;
        user.EmailValue = dto.Email;
        user.PhoneNumberValue = dto.Phonenumber;
        
        // 解析性别
        if (!string.IsNullOrEmpty(dto.Sex) && Enum.TryParse<Gender>(dto.Sex, out var gender))
        {
            user.Gender = gender;
        }
        
        user.UpdateBy = _currentUserService.GetUserName();
        user.UpdateTime = DateTime.Now;

        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("用户 {UserName} 更新个人信息", user.UserName);
    }

    public async Task UpdatePasswordAsync(UpdatePasswordDto dto, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetUserId();
        if (userId == 0)
        {
            throw new UnauthorizedException("用户未登录");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("用户不存在");
        }

        // 验证旧密码
        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.Password))
        {
            throw new BusinessException("旧密码不正确");
        }

        // 更新密码
        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdateBy = _currentUserService.GetUserName();
        user.UpdateTime = DateTime.Now;

        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("用户 {UserName} 修改密码", user.UserName);
    }

    public async Task<string> UpdateAvatarAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetUserId();
        if (userId == 0)
        {
            throw new UnauthorizedException("用户未登录");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("用户不存在");
        }

        // 上传头像
        var avatarUrl = await _fileService.UploadAsync(file, cancellationToken);

        // 更新用户头像
        user.Avatar = avatarUrl;
        user.UpdateBy = _currentUserService.GetUserName();
        user.UpdateTime = DateTime.Now;

        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("用户 {UserName} 更新头像", user.UserName);

        return avatarUrl;
    }
}
