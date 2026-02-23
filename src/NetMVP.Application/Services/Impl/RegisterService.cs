using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetMVP.Application.DTOs.Auth;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Exceptions;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 用户注册服务实现
/// </summary>
public class RegisterService : IRegisterService
{
    private readonly ISysUserRepository _userRepository;
    private readonly ISysRoleRepository _roleRepository;
    private readonly ICacheService _cacheService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RegisterService> _logger;

    public RegisterService(
        ISysUserRepository userRepository,
        ISysRoleRepository roleRepository,
        ICacheService cacheService,
        IConfiguration configuration,
        ILogger<RegisterService> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _cacheService = cacheService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        // 1. 检查注册开关
        var registerEnabled = bool.Parse(_configuration["System:RegisterEnabled"] ?? "false");
        if (!registerEnabled)
        {
            throw new BusinessException("系统未开放注册功能");
        }

        // 2. 验证验证码
        var cacheKey = $"{CacheConstants.CAPTCHA_CODE_KEY}{dto.Uuid}";
        var cachedCode = await _cacheService.GetAsync<string>(cacheKey);
        
        if (string.IsNullOrEmpty(cachedCode))
        {
            throw new BusinessException("验证码已过期");
        }

        if (!string.Equals(cachedCode, dto.Code, StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessException("验证码错误");
        }

        // 删除验证码
        await _cacheService.RemoveAsync(cacheKey);

        // 3. 验证用户名唯一性
        var isUnique = await _userRepository.CheckUserNameUniqueAsync(dto.UserName, null, cancellationToken);
        if (!isUnique)
        {
            throw new BusinessException($"用户名 '{dto.UserName}' 已存在");
        }

        // 4. 创建用户
        var user = new SysUser
        {
            UserName = dto.UserName,
            NickName = dto.UserName, // 默认昵称与用户名相同
            UserType = "00",
            Status = UserConstants.NORMAL,
            DelFlag = UserConstants.DEL_FLAG_EXIST,
            CreateBy = dto.UserName,
            CreateTime = DateTime.Now
        };

        // 设置密码（使用BCrypt加密）
        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        user.PwdUpdateDate = DateTime.Now;

        // 保存用户
        await _userRepository.AddAsync(user, cancellationToken);

        // 5. 分配默认角色
        var defaultRoleIdStr = _configuration["System:DefaultRoleId"] ?? "2";
        var defaultRoleId = long.Parse(defaultRoleIdStr); // 默认角色ID为2（普通用户）
        var role = await _roleRepository.GetByIdAsync(defaultRoleId, cancellationToken);
        
        if (role != null)
        {
            var userRole = new SysUserRole
            {
                UserId = user.UserId,
                RoleId = defaultRoleId
            };
            
            // 这里需要通过DbContext直接添加，因为没有UserRole仓储
            // 实际应该在UserRepository中添加AssignRoleAsync方法
            user.UserRoles.Add(userRole);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        _logger.LogInformation("用户 {UserName} 注册成功", dto.UserName);
    }
}
