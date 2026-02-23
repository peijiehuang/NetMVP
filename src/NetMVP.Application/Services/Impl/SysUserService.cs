using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetMVP.Application.DTOs.User;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 用户服务实现
/// </summary>
public class SysUserService : ISysUserService
{
    private readonly ISysUserRepository _userRepository;
    private readonly ISysDeptRepository _deptRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExcelService _excelService;
    private readonly ICacheService _cacheService;
    private readonly IDataScopeFilter _dataScopeFilter;
    private readonly ICurrentUserService _currentUserService;

    public SysUserService(
        ISysUserRepository userRepository,
        ISysDeptRepository deptRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IExcelService excelService,
        ICacheService cacheService,
        IDataScopeFilter dataScopeFilter,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _deptRepository = deptRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _excelService = excelService;
        _cacheService = cacheService;
        _dataScopeFilter = dataScopeFilter;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    public async Task<(List<UserDto> users, int total)> GetUserListAsync(UserQueryDto query, CancellationToken cancellationToken = default)
    {
        var queryable = _userRepository.GetQueryable();

        // 应用数据权限过滤
        var currentUserId = _currentUserService.GetUserId();
        queryable = await _dataScopeFilter.ApplyDataScopeAsync(queryable, currentUserId);

        // 用户名
        if (!string.IsNullOrWhiteSpace(query.UserName))
        {
            queryable = queryable.Where(u => u.UserName.Contains(query.UserName));
        }

        // 手机号
        if (!string.IsNullOrWhiteSpace(query.Phonenumber))
        {
            queryable = queryable.Where(u => u.PhoneNumberValue != null && u.PhoneNumberValue.Contains(query.Phonenumber));
        }

        // 状态
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(u => u.Status == query.Status);
        }

        // 部门ID
        if (query.DeptId.HasValue)
        {
            queryable = queryable.Where(u => u.DeptId == query.DeptId.Value);
        }

        // 总数
        var total = await queryable.CountAsync(cancellationToken);

        // 分页
        var users = await queryable
            .OrderBy(u => u.UserId)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var userDtos = _mapper.Map<List<UserDto>>(users);

        // 填充部门信息
        var deptIds = userDtos.Where(u => u.DeptId.HasValue).Select(u => u.DeptId!.Value).Distinct().ToList();
        if (deptIds.Any())
        {
            var depts = await _deptRepository.GetQueryable()
                .Where(d => deptIds.Contains(d.DeptId))
                .ToDictionaryAsync(d => d.DeptId, d => d.DeptName, cancellationToken);

            foreach (var user in userDtos.Where(u => u.DeptId.HasValue))
            {
                if (depts.TryGetValue(user.DeptId!.Value, out var deptName))
                {
                    user.Dept = new DeptInfo { DeptName = deptName };
                }
            }
        }

        return (userDtos, total);
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    public async Task<UserDto?> GetUserByIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUserIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return null;
        }

        var userDto = _mapper.Map<UserDto>(user);

        // 填充部门信息
        if (user.DeptId.HasValue)
        {
            var dept = await _deptRepository.GetByDeptIdAsync(user.DeptId.Value, cancellationToken);
            if (dept != null)
            {
                userDto.Dept = new DeptInfo { DeptName = dept.DeptName };
            }
        }

        return userDto;
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    public async Task<long> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        // 检查用户名唯一性
        if (!await CheckUserNameUniqueAsync(dto.UserName, null, cancellationToken))
        {
            throw new InvalidOperationException($"用户名'{dto.UserName}'已存在");
        }

        // 检查手机号唯一性
        if (!string.IsNullOrWhiteSpace(dto.Phonenumber))
        {
            if (!await CheckPhoneUniqueAsync(dto.Phonenumber, null, cancellationToken))
            {
                throw new InvalidOperationException($"手机号'{dto.Phonenumber}'已存在");
            }
        }

        // 检查邮箱唯一性
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            if (!await CheckEmailUniqueAsync(dto.Email, null, cancellationToken))
            {
                throw new InvalidOperationException($"邮箱'{dto.Email}'已存在");
            }
        }

        // 创建用户
        var user = new SysUser
        {
            UserName = dto.UserName,
            NickName = dto.NickName,
            DeptId = dto.DeptId,
            Sex = dto.Sex ?? UserConstants.SEX_UNKNOWN,
            Status = dto.Status,
            Remark = dto.Remark
        };

        // 设置密码
        user.SetPassword(dto.Password);

        // 设置手机号
        if (!string.IsNullOrWhiteSpace(dto.Phonenumber))
        {
            user.PhoneNumberValue = dto.Phonenumber;
        }

        // 设置邮箱
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            user.EmailValue = dto.Email;
        }

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 保存用户角色关联
        if (dto.RoleIds != null && dto.RoleIds.Length > 0)
        {
            var dbContext = _userRepository.GetDbContext();
            var userRoles = dto.RoleIds.Select(roleId => new SysUserRole
            {
                UserId = user.UserId,
                RoleId = roleId
            }).ToList();

            await dbContext.Set<SysUserRole>().AddRangeAsync(userRoles, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // 保存用户岗位关联
        if (dto.PostIds != null && dto.PostIds.Length > 0)
        {
            var dbContext = _userRepository.GetDbContext();
            var userPosts = dto.PostIds.Select(postId => new SysUserPost
            {
                UserId = user.UserId,
                PostId = postId
            }).ToList();

            await dbContext.Set<SysUserPost>().AddRangeAsync(userPosts, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return user.UserId;
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    public async Task UpdateUserAsync(UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("用户不存在");
        }

        // 检查是否为超级管理员
        if (user.IsAdmin())
        {
            throw new InvalidOperationException("不允许修改超级管理员");
        }

        // 检查手机号唯一性
        if (!string.IsNullOrWhiteSpace(dto.Phonenumber))
        {
            if (!await CheckPhoneUniqueAsync(dto.Phonenumber, dto.UserId, cancellationToken))
            {
                throw new InvalidOperationException($"手机号'{dto.Phonenumber}'已存在");
            }
        }

        // 检查邮箱唯一性
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            if (!await CheckEmailUniqueAsync(dto.Email, dto.UserId, cancellationToken))
            {
                throw new InvalidOperationException($"邮箱'{dto.Email}'已存在");
            }
        }

        // 更新用户信息
        user.NickName = dto.NickName;
        user.DeptId = dto.DeptId;
        
        // 处理性别
        if (!string.IsNullOrWhiteSpace(dto.Sex))
        {
            user.Sex = dto.Sex;
        }
        
        user.Status = dto.Status;
        user.Remark = dto.Remark;

        // 更新手机号
        if (!string.IsNullOrWhiteSpace(dto.Phonenumber))
        {
            user.PhoneNumberValue = dto.Phonenumber;
        }

        // 更新邮箱
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            user.EmailValue = dto.Email;
        }

        await _userRepository.UpdateAsync(user, cancellationToken);

        // 更新用户角色关联
        var dbContext = _userRepository.GetDbContext();
        var existingUserRoles = await dbContext.Set<SysUserRole>()
            .Where(ur => ur.UserId == dto.UserId)
            .ToListAsync(cancellationToken);

        dbContext.Set<SysUserRole>().RemoveRange(existingUserRoles);

        if (dto.RoleIds != null && dto.RoleIds.Length > 0)
        {
            var userRoles = dto.RoleIds.Select(roleId => new SysUserRole
            {
                UserId = user.UserId,
                RoleId = roleId
            }).ToList();

            await dbContext.Set<SysUserRole>().AddRangeAsync(userRoles, cancellationToken);
        }

        // 更新用户岗位关联
        var existingUserPosts = await dbContext.Set<SysUserPost>()
            .Where(up => up.UserId == dto.UserId)
            .ToListAsync(cancellationToken);

        dbContext.Set<SysUserPost>().RemoveRange(existingUserPosts);

        if (dto.PostIds != null && dto.PostIds.Length > 0)
        {
            var userPosts = dto.PostIds.Select(postId => new SysUserPost
            {
                UserId = user.UserId,
                PostId = postId
            }).ToList();

            await dbContext.Set<SysUserPost>().AddRangeAsync(userPosts, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除用户权限缓存
        await ClearUserCacheAsync(user.UserId);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    public async Task DeleteUserAsync(long userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("用户不存在");
        }

        // 检查是否为超级管理员
        if (user.IsAdmin())
        {
            throw new InvalidOperationException("不允许删除超级管理员");
        }

        // 删除用户角色关联
        var dbContext = _userRepository.GetDbContext();
        var userRoles = await dbContext.Set<SysUserRole>()
            .Where(ur => ur.UserId == userId)
            .ToListAsync(cancellationToken);
        dbContext.Set<SysUserRole>().RemoveRange(userRoles);

        // 删除用户岗位关联
        var userPosts = await dbContext.Set<SysUserPost>()
            .Where(up => up.UserId == userId)
            .ToListAsync(cancellationToken);
        dbContext.Set<SysUserPost>().RemoveRange(userPosts);

        // 删除用户
        await _userRepository.DeleteAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除用户缓存
        await ClearUserCacheAsync(userId);
    }

    /// <summary>
    /// 批量删除用户
    /// </summary>
    public async Task DeleteUsersAsync(long[] userIds, CancellationToken cancellationToken = default)
    {
        foreach (var userId in userIds)
        {
            await DeleteUserAsync(userId, cancellationToken);
        }
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    public async Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("用户不存在");
        }

        // 检查是否为超级管理员
        if (user.IsAdmin())
        {
            throw new InvalidOperationException("不允许重置超级管理员密码");
        }

        // 重置密码
        user.SetPassword(dto.Password);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除用户缓存
        await ClearUserCacheAsync(user.UserId);
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    public async Task ChangePasswordAsync(long userId, ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("用户不存在");
        }

        // 验证旧密码
        if (!user.VerifyPassword(dto.OldPassword))
        {
            throw new InvalidOperationException("旧密码不正确");
        }

        // 设置新密码
        user.SetPassword(dto.NewPassword);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除用户缓存
        await ClearUserCacheAsync(userId);
    }

    /// <summary>
    /// 修改用户状态
    /// </summary>
    public async Task UpdateUserStatusAsync(long userId, string status, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("用户不存在");
        }

        // 检查是否为超级管理员
        if (user.IsAdmin())
        {
            throw new InvalidOperationException("不允许修改超级管理员状态");
        }

        user.Status = status;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除用户缓存
        await ClearUserCacheAsync(userId);
    }

    /// <summary>
    /// 导出用户
    /// </summary>
    public async Task<byte[]> ExportUsersAsync(UserQueryDto query, CancellationToken cancellationToken = default)
    {
        // 获取所有数据（不分页）
        var queryable = _userRepository.GetQueryable();

        // 应用数据权限过滤
        var currentUserId = _currentUserService.GetUserId();
        queryable = await _dataScopeFilter.ApplyDataScopeAsync(queryable, currentUserId);

        // 用户名
        if (!string.IsNullOrWhiteSpace(query.UserName))
        {
            queryable = queryable.Where(u => u.UserName.Contains(query.UserName));
        }

        // 手机号
        if (!string.IsNullOrWhiteSpace(query.Phonenumber))
        {
            queryable = queryable.Where(u => u.PhoneNumberValue != null && u.PhoneNumberValue.Contains(query.Phonenumber));
        }

        // 状态
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(u => u.Status == query.Status);
        }

        // 部门ID
        if (query.DeptId.HasValue)
        {
            queryable = queryable.Where(u => u.DeptId == query.DeptId.Value);
        }

        var users = await queryable
            .OrderBy(u => u.UserId)
            .ToListAsync(cancellationToken);

        var userDtos = _mapper.Map<List<UserDto>>(users);

        // 填充部门名称
        var deptIds = userDtos.Where(u => u.DeptId.HasValue).Select(u => u.DeptId!.Value).Distinct().ToList();
        if (deptIds.Any())
        {
            var depts = await _deptRepository.GetQueryable()
                .Where(d => deptIds.Contains(d.DeptId))
                .ToDictionaryAsync(d => d.DeptId, d => d.DeptName, cancellationToken);

            foreach (var user in userDtos.Where(u => u.DeptId.HasValue))
            {
                if (depts.TryGetValue(user.DeptId!.Value, out var deptName))
                {
                    user.Dept = new DeptInfo { DeptName = deptName };
                }
            }
        }

        // 转换为导出DTO（带中文列头）
        var exportDtos = userDtos.Select(u => new ExportUserDto
        {
            UserId = u.UserId,
            UserName = u.UserName,
            NickName = u.NickName,
            Email = u.Email,
            Phonenumber = u.Phonenumber,
            Sex = u.Sex switch
            {
                "0" => "男",
                "1" => "女",
                "2" => "未知",
                _ => u.Sex
            },
            Status = u.Status switch
            {
                "0" => "正常",
                "1" => "停用",
                _ => u.Status
            },
            DeptName = u.Dept?.DeptName,
            CreateTime = u.CreateTime
        }).ToList();

        // 导出到内存流
        using var stream = new MemoryStream();
        await _excelService.ExportAsync(exportDtos, stream, cancellationToken);
        return stream.ToArray();
    }

    /// <summary>
    /// 检查用户名唯一性
    /// </summary>
    public async Task<bool> CheckUserNameUniqueAsync(string userName, long? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _userRepository.GetQueryable().Where(u => u.UserName == userName);

        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.UserId != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 检查手机号唯一性
    /// </summary>
    public async Task<bool> CheckPhoneUniqueAsync(string phone, long? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _userRepository.GetQueryable().Where(u => u.PhoneNumberValue == phone);

        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.UserId != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 检查邮箱唯一性
    /// </summary>
    public async Task<bool> CheckEmailUniqueAsync(string email, long? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _userRepository.GetQueryable().Where(u => u.EmailValue == email);

        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.UserId != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 获取用户个人信息
    /// </summary>
    public async Task<UserDto?> GetUserProfileAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await GetUserByIdAsync(userId, cancellationToken);
    }

    /// <summary>
    /// 更新用户个人信息
    /// </summary>
    public async Task UpdateUserProfileAsync(long userId, UpdateProfileDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("用户不存在");
        }

        // 检查手机号唯一性
        if (!string.IsNullOrWhiteSpace(dto.Phonenumber))
        {
            if (!await CheckPhoneUniqueAsync(dto.Phonenumber, userId, cancellationToken))
            {
                throw new InvalidOperationException($"手机号'{dto.Phonenumber}'已存在");
            }
        }

        // 检查邮箱唯一性
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            if (!await CheckEmailUniqueAsync(dto.Email, userId, cancellationToken))
            {
                throw new InvalidOperationException($"邮箱'{dto.Email}'已存在");
            }
        }

        // 更新个人信息
        user.NickName = dto.NickName;
        user.Sex = dto.Sex ?? UserConstants.SEX_UNKNOWN;

        // 更新手机号
        if (!string.IsNullOrWhiteSpace(dto.Phonenumber))
        {
            user.PhoneNumberValue = dto.Phonenumber;
        }

        // 更新邮箱
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            user.EmailValue = dto.Email;
        }

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除用户缓存
        await ClearUserCacheAsync(userId);
    }

    /// <summary>
    /// 更新用户头像
    /// </summary>
    public async Task UpdateUserAvatarAsync(long userId, string avatar, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("用户不存在");
        }

        user.Avatar = avatar;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除用户缓存
        await ClearUserCacheAsync(userId);
    }

    /// <summary>
    /// 清除用户缓存
    /// </summary>
    private async Task ClearUserCacheAsync(long userId)
    {
        // 清除用户权限缓存
        await _cacheService.RemoveAsync($"user:permissions:{userId}");
        await _cacheService.RemoveAsync($"user:roles:{userId}");
    }

    /// <summary>
    /// 获取用户的岗位ID列表
    /// </summary>
    public async Task<List<long>> GetUserPostIdsAsync(long userId, CancellationToken cancellationToken = default)
    {
        var queryable = _userRepository.GetQueryable();
        var user = await queryable
            .Include(u => u.UserPosts)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        return user?.UserPosts?.Select(up => up.PostId).ToList() ?? new List<long>();
    }

    /// <summary>
    /// 获取用户的角色ID列表
    /// </summary>
    public async Task<List<long>> GetUserRoleIdsAsync(long userId, CancellationToken cancellationToken = default)
    {
        var queryable = _userRepository.GetQueryable();
        var user = await queryable
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        return user?.UserRoles?.Select(ur => ur.RoleId).ToList() ?? new List<long>();
    }

    /// <summary>
    /// 更新用户角色
    /// </summary>
    public async Task UpdateUserRolesAsync(long userId, long[] roleIds, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("用户不存在");
        }

        // 检查是否为超级管理员
        if (user.IsAdmin())
        {
            throw new InvalidOperationException("不允许修改超级管理员角色");
        }

        var dbContext = _userRepository.GetDbContext();

        // 删除现有的用户角色关联
        var existingUserRoles = await dbContext.Set<SysUserRole>()
            .Where(ur => ur.UserId == userId)
            .ToListAsync(cancellationToken);

        dbContext.Set<SysUserRole>().RemoveRange(existingUserRoles);

        // 添加新的用户角色关联
        if (roleIds != null && roleIds.Length > 0)
        {
            var userRoles = roleIds.Select(roleId => new SysUserRole
            {
                UserId = userId,
                RoleId = roleId
            }).ToList();

            await dbContext.Set<SysUserRole>().AddRangeAsync(userRoles, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除用户权限缓存
        await ClearUserCacheAsync(userId);
    }

    /// <summary>
    /// 获取已分配该角色的用户列表
    /// </summary>
    public async Task<(List<UserDto> users, int total)> GetAllocatedUserListAsync(
        long roleId, 
        string? userName, 
        string? phonenumber, 
        int pageNum, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var dbContext = _userRepository.GetDbContext();
        
        // 查询已分配该角色的用户
        var query = from u in dbContext.Set<SysUser>()
                    join ur in dbContext.Set<SysUserRole>() on u.UserId equals ur.UserId
                    where ur.RoleId == roleId && u.DelFlag == UserConstants.DEL_FLAG_EXIST
                    select u;

        // 用户名筛选
        if (!string.IsNullOrWhiteSpace(userName))
        {
            query = query.Where(u => u.UserName.Contains(userName));
        }

        // 手机号筛选
        if (!string.IsNullOrWhiteSpace(phonenumber))
        {
            query = query.Where(u => u.PhoneNumberValue != null && u.PhoneNumberValue.Contains(phonenumber));
        }

        // 获取总数
        var total = await query.CountAsync(cancellationToken);

        // 分页查询
        var users = await query
            .OrderBy(u => u.UserId)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var userDtos = _mapper.Map<List<UserDto>>(users);

        // 填充部门和角色信息
        foreach (var userDto in userDtos)
        {
            // 获取部门信息
            if (userDto.DeptId.HasValue)
            {
                var dept = await dbContext.Set<SysDept>()
                    .FirstOrDefaultAsync(d => d.DeptId == userDto.DeptId.Value, cancellationToken);
                if (dept != null)
                {
                    userDto.DeptName = dept.DeptName;
                }
            }
        }

        return (userDtos, total);
    }

    /// <summary>
    /// 获取未分配该角色的用户列表
    /// </summary>
    public async Task<(List<UserDto> users, int total)> GetUnallocatedUserListAsync(
        long roleId, 
        string? userName, 
        string? phonenumber, 
        int pageNum, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var dbContext = _userRepository.GetDbContext();
        
        // 获取已分配该角色的用户ID列表
        var allocatedUserIds = await dbContext.Set<SysUserRole>()
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.UserId)
            .ToListAsync(cancellationToken);

        // 查询未分配该角色的用户
        var query = dbContext.Set<SysUser>()
            .Where(u => u.DelFlag == UserConstants.DEL_FLAG_EXIST && !allocatedUserIds.Contains(u.UserId));

        // 用户名筛选
        if (!string.IsNullOrWhiteSpace(userName))
        {
            query = query.Where(u => u.UserName.Contains(userName));
        }

        // 手机号筛选
        if (!string.IsNullOrWhiteSpace(phonenumber))
        {
            query = query.Where(u => u.PhoneNumberValue != null && u.PhoneNumberValue.Contains(phonenumber));
        }

        // 获取总数
        var total = await query.CountAsync(cancellationToken);

        // 分页查询
        var users = await query
            .OrderBy(u => u.UserId)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var userDtos = _mapper.Map<List<UserDto>>(users);

        // 填充部门信息
        foreach (var userDto in userDtos)
        {
            // 获取部门信息
            if (userDto.DeptId.HasValue)
            {
                var dept = await dbContext.Set<SysDept>()
                    .FirstOrDefaultAsync(d => d.DeptId == userDto.DeptId.Value, cancellationToken);
                if (dept != null)
                {
                    userDto.DeptName = dept.DeptName;
                }
            }
        }

        return (userDtos, total);
    }

    /// <summary>
    /// 导入用户数据
    /// </summary>
    public async Task<string> ImportUsersAsync(Stream fileStream, bool updateSupport, CancellationToken cancellationToken = default)
    {
        // 从Excel读取用户数据
        var importUsers = await _excelService.ImportAsync<ImportUserDto>(fileStream, cancellationToken);
        
        if (importUsers == null || importUsers.Count == 0)
        {
            throw new InvalidOperationException("导入数据不能为空");
        }

        int successNum = 0;
        int failureNum = 0;
        var failureMessages = new List<string>();
        var currentUserName = _currentUserService.GetUserName();

        foreach (var importUser in importUsers)
        {
            try
            {
                // 验证必填字段
                if (string.IsNullOrWhiteSpace(importUser.UserName))
                {
                    failureNum++;
                    failureMessages.Add($"第{importUsers.IndexOf(importUser) + 1}行：用户名不能为空");
                    continue;
                }

                // 转换性别和状态
                var gender = ParseGender(importUser.Gender);
                var status = ParseUserStatus(importUser.Status);

                // 检查用户是否存在
                var existingUser = await _userRepository.GetByUserNameAsync(importUser.UserName, cancellationToken);

                if (existingUser != null)
                {
                    if (!updateSupport)
                    {
                        failureNum++;
                        failureMessages.Add($"用户'{importUser.UserName}'已存在");
                        continue;
                    }

                    // 更新用户
                    existingUser.NickName = importUser.NickName ?? existingUser.NickName;
                    existingUser.EmailValue = !string.IsNullOrWhiteSpace(importUser.Email) 
                        ? importUser.Email 
                        : existingUser.EmailValue;
                    existingUser.PhoneNumberValue = !string.IsNullOrWhiteSpace(importUser.PhoneNumber) 
                        ? importUser.PhoneNumber 
                        : existingUser.PhoneNumberValue;
                    existingUser.Sex = gender ?? existingUser.Sex;
                    existingUser.Status = status ?? existingUser.Status;
                    existingUser.UpdateBy = currentUserName;
                    existingUser.UpdateTime = DateTime.Now;

                    await _userRepository.UpdateAsync(existingUser, cancellationToken);
                    successNum++;
                }
                else
                {
                    // 新增用户
                    var newUser = new SysUser
                    {
                        UserName = importUser.UserName,
                        NickName = importUser.NickName ?? importUser.UserName,
                        EmailValue = importUser.Email,
                        PhoneNumberValue = importUser.PhoneNumber,
                        Sex = gender ?? UserConstants.SEX_UNKNOWN,
                        Status = status ?? UserConstants.NORMAL,
                        Password = BCrypt.Net.BCrypt.HashPassword("123456"), // 默认密码
                        DeptId = importUser.DeptId ?? 100, // 默认部门
                        CreateBy = currentUserName,
                        CreateTime = DateTime.Now
                    };

                    await _userRepository.AddAsync(newUser, cancellationToken);
                    successNum++;
                }
            }
            catch (Exception ex)
            {
                failureNum++;
                failureMessages.Add($"用户'{importUser.UserName}'导入失败：{ex.Message}");
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var message = $"导入完成！成功 {successNum} 条，失败 {failureNum} 条";
        if (failureMessages.Any())
        {
            message += "。失败信息：" + string.Join("；", failureMessages.Take(10));
            if (failureMessages.Count > 10)
            {
                message += $"...等{failureMessages.Count}条";
            }
        }

        return message;
    }

    /// <summary>
    /// 下载用户导入模板
    /// </summary>
    public async Task<byte[]> GetImportTemplateAsync(CancellationToken cancellationToken = default)
    {
        // 创建模板数据（示例数据）
        var templateData = new List<ImportUserDto>
        {
            new ImportUserDto
            {
                UserName = "示例用户",
                NickName = "示例昵称",
                Email = "example@example.com",
                PhoneNumber = "13800138000",
                Gender = "男",
                Status = "正常",
                DeptId = 100
            }
        };

        using var stream = new MemoryStream();
        await _excelService.ExportAsync(templateData, stream, cancellationToken);
        return stream.ToArray();
    }

    #region 辅助方法

    /// <summary>
    /// 解析性别字符串
    /// </summary>
    private string? ParseGender(string? genderStr)
    {
        if (string.IsNullOrWhiteSpace(genderStr))
            return null;

        return genderStr switch
        {
            "男" => UserConstants.SEX_MALE,
            "女" => UserConstants.SEX_FEMALE,
            "未知" => UserConstants.SEX_UNKNOWN,
            "0" => UserConstants.SEX_MALE,
            "1" => UserConstants.SEX_FEMALE,
            "2" => UserConstants.SEX_UNKNOWN,
            _ => null
        };
    }

    /// <summary>
    /// 解析用户状态字符串
    /// </summary>
    private string? ParseUserStatus(string? statusStr)
    {
        if (string.IsNullOrWhiteSpace(statusStr))
            return null;

        return statusStr switch
        {
            "正常" => UserConstants.NORMAL,
            "停用" => UserConstants.USER_DISABLE,
            "0" => UserConstants.NORMAL,
            "1" => UserConstants.USER_DISABLE,
            _ => null
        };
    }

    #endregion
}
