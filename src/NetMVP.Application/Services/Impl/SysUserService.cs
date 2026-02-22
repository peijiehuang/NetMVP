using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetMVP.Application.DTOs.User;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Domain.Interfaces;
using NetMVP.Domain.ValueObjects;

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
            if (Enum.TryParse<UserStatus>(query.Status, out var status))
            queryable = queryable.Where(u => u.Status == status);
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
            Gender = dto.Gender,
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
        
        // 处理性别：如果前端发送了Sex字符串，转换为Gender枚举
        if (!string.IsNullOrWhiteSpace(dto.Sex))
        {
            user.Gender = dto.Sex switch
            {
                "0" => Gender.Male,
                "1" => Gender.Female,
                "2" => Gender.Unknown,
                _ => dto.Gender
            };
        }
        else
        {
            user.Gender = dto.Gender;
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
    public async Task UpdateUserStatusAsync(long userId, UserStatus status, CancellationToken cancellationToken = default)
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
            if (Enum.TryParse<UserStatus>(query.Status, out var status))
            queryable = queryable.Where(u => u.Status == status);
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

        // 导出到内存流
        using var stream = new MemoryStream();
        await _excelService.ExportAsync(userDtos, stream, cancellationToken);
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
        user.Gender = dto.Gender;

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
                    where ur.RoleId == roleId && u.DelFlag == DelFlag.Exist
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
            .Where(u => u.DelFlag == DelFlag.Exist && !allocatedUserIds.Contains(u.UserId));

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
}
