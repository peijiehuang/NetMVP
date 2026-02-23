using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetMVP.Application.DTOs.Role;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 角色服务实现
/// </summary>
public class SysRoleService : ISysRoleService
{
    private readonly IRepository<SysRole> _roleRepository;
    private readonly IRepository<SysUser> _userRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly ISysMenuService _menuService;
    private readonly IExcelService _excelService;

    public SysRoleService(
        IRepository<SysRole> roleRepository,
        IRepository<SysUser> userRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IPermissionService permissionService,
        ISysMenuService menuService,
        IExcelService excelService)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _menuService = menuService;
        _excelService = excelService;
    }

    /// <summary>
    /// 获取角色列表
    /// </summary>
    public async Task<(List<RoleDto> roles, int total)> GetRoleListAsync(RoleQueryDto query, CancellationToken cancellationToken = default)
    {
        var queryable = _roleRepository.GetQueryable();

        // 角色名称
        if (!string.IsNullOrWhiteSpace(query.RoleName))
        {
            queryable = queryable.Where(r => r.RoleName.Contains(query.RoleName));
        }

        // 角色权限字符串
        if (!string.IsNullOrWhiteSpace(query.RoleKey))
        {
            queryable = queryable.Where(r => r.RoleKey.Contains(query.RoleKey));
        }

        // 状态
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (Enum.TryParse<UserStatus>(query.Status, out var status))
            queryable = queryable.Where(r => r.Status == status);
        }

        // 时间范围
        if (query.BeginTime.HasValue)
        {
            queryable = queryable.Where(r => r.CreateTime >= query.BeginTime.Value);
        }

        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where(r => r.CreateTime <= query.EndTime.Value);
        }

        // 总数
        var total = await queryable.CountAsync(cancellationToken);

        // 分页
        var roles = await queryable
            .OrderBy(r => r.RoleSort)
            .Skip((query.PageNum - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var roleDtos = _mapper.Map<List<RoleDto>>(roles);

        // 加载菜单ID和部门ID
        foreach (var roleDto in roleDtos)
        {
            roleDto.MenuIds = await GetRoleMenuIdsAsync(roleDto.RoleId, cancellationToken);
            roleDto.DeptIds = await GetRoleDeptIdsAsync(roleDto.RoleId, cancellationToken);
        }

        return (roleDtos, total);
    }

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    public async Task<RoleDto?> GetRoleByIdAsync(long roleId, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            return null;
        }

        var roleDto = _mapper.Map<RoleDto>(role);
        // 使用 MenuService 的方法获取菜单ID，该方法会根据 MenuCheckStrictly 处理父子联动
        roleDto.MenuIds = await _menuService.GetMenuIdsByRoleIdAsync(roleId, cancellationToken);
        roleDto.DeptIds = await GetRoleDeptIdsAsync(roleId, cancellationToken);

        return roleDto;
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    public async Task<long> CreateRoleAsync(CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        // 检查角色名称唯一性
        if (!await CheckRoleNameUniqueAsync(dto.RoleName, null, cancellationToken))
        {
            throw new InvalidOperationException($"角色名称'{dto.RoleName}'已存在");
        }

        // 检查角色权限字符串唯一性
        if (!await CheckRoleKeyUniqueAsync(dto.RoleKey, null, cancellationToken))
        {
            throw new InvalidOperationException($"角色权限字符'{dto.RoleKey}'已存在");
        }

        var role = new SysRole
        {
            RoleName = dto.RoleName,
            RoleKey = dto.RoleKey,
            RoleSort = dto.RoleSort,
            DataScope = dto.DataScope,
            MenuCheckStrictly = dto.MenuCheckStrictly,
            DeptCheckStrictly = dto.DeptCheckStrictly,
            Status = dto.Status,
            DelFlag = DelFlag.Exist,  // 显式设置删除标志
            Remark = dto.Remark
        };

        await _roleRepository.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 保存角色菜单关联
        await SaveRoleMenusAsync(role.RoleId, dto.MenuIds, cancellationToken);

        // 保存角色部门关联（数据权限）
        await SaveRoleDeptsAsync(role.RoleId, dto.DeptIds, cancellationToken);

        return role.RoleId;
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    public async Task UpdateRoleAsync(UpdateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(dto.RoleId, cancellationToken);
        if (role == null)
        {
            throw new InvalidOperationException("角色不存在");
        }

        // 检查是否为超级管理员角色
        if (role.RoleId == 1)
        {
            throw new InvalidOperationException("不允许修改超级管理员角色");
        }

        // 检查角色名称唯一性
        if (!await CheckRoleNameUniqueAsync(dto.RoleName, dto.RoleId, cancellationToken))
        {
            throw new InvalidOperationException($"角色名称'{dto.RoleName}'已存在");
        }

        // 检查角色权限字符串唯一性
        if (!await CheckRoleKeyUniqueAsync(dto.RoleKey, dto.RoleId, cancellationToken))
        {
            throw new InvalidOperationException($"角色权限字符'{dto.RoleKey}'已存在");
        }

        role.RoleName = dto.RoleName;
        role.RoleKey = dto.RoleKey;
        role.RoleSort = dto.RoleSort;
        role.DataScope = dto.DataScope;
        role.MenuCheckStrictly = dto.MenuCheckStrictly;
        role.DeptCheckStrictly = dto.DeptCheckStrictly;
        role.Status = dto.Status;
        role.Remark = dto.Remark;

        await _roleRepository.UpdateAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 更新角色菜单关联
        await SaveRoleMenusAsync(role.RoleId, dto.MenuIds, cancellationToken);

        // 更新角色部门关联（数据权限）
        await SaveRoleDeptsAsync(role.RoleId, dto.DeptIds, cancellationToken);

        // 清除相关用户的权限缓存
        await ClearRoleUsersCacheAsync(role.RoleId, cancellationToken);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    public async Task DeleteRoleAsync(long roleId, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            throw new InvalidOperationException("角色不存在");
        }

        // 检查是否为超级管理员角色
        if (role.RoleId == 1)
        {
            throw new InvalidOperationException("不允许删除超级管理员角色");
        }

        // 检查是否有用户使用该角色
        var dbContext = _userRepository.GetDbContext();
        var hasUsers = await dbContext.Set<SysUserRole>()
            .AnyAsync(ur => ur.RoleId == roleId, cancellationToken);

        if (hasUsers)
        {
            throw new InvalidOperationException("该角色已分配给用户，不能删除");
        }

        // 删除角色菜单关联
        await DeleteRoleMenusAsync(roleId, cancellationToken);

        // 删除角色部门关联
        await DeleteRoleDeptsAsync(roleId, cancellationToken);

        // 删除角色
        await _roleRepository.DeleteAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 批量删除角色
    /// </summary>
    public async Task DeleteRolesAsync(long[] roleIds, CancellationToken cancellationToken = default)
    {
        foreach (var roleId in roleIds)
        {
            await DeleteRoleAsync(roleId, cancellationToken);
        }
    }

    /// <summary>
    /// 修改角色状态
    /// </summary>
    public async Task UpdateRoleStatusAsync(long roleId, UserStatus status, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            throw new InvalidOperationException("角色不存在");
        }

        // 检查是否为超级管理员角色
        if (role.RoleId == 1)
        {
            throw new InvalidOperationException("不允许修改超级管理员角色状态");
        }

        role.Status = status;
        await _roleRepository.UpdateAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除相关用户的权限缓存
        await ClearRoleUsersCacheAsync(role.RoleId, cancellationToken);
    }

    /// <summary>
    /// 检查角色名称唯一性
    /// </summary>
    public async Task<bool> CheckRoleNameUniqueAsync(string roleName, long? excludeRoleId = null, CancellationToken cancellationToken = default)
    {
        var query = _roleRepository.GetQueryable().Where(r => r.RoleName == roleName);

        if (excludeRoleId.HasValue)
        {
            query = query.Where(r => r.RoleId != excludeRoleId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 检查角色权限字符串唯一性
    /// </summary>
    public async Task<bool> CheckRoleKeyUniqueAsync(string roleKey, long? excludeRoleId = null, CancellationToken cancellationToken = default)
    {
        var query = _roleRepository.GetQueryable().Where(r => r.RoleKey == roleKey);

        if (excludeRoleId.HasValue)
        {
            query = query.Where(r => r.RoleId != excludeRoleId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    #region 私有方法

    /// <summary>
    /// 获取角色的菜单ID列表
    /// </summary>
    private async Task<List<long>> GetRoleMenuIdsAsync(long roleId, CancellationToken cancellationToken = default)
    {
        var dbContext = _userRepository.GetDbContext();
        return await dbContext.Set<SysRoleMenu>()
            .Where(rm => rm.RoleId == roleId)
            .Select(rm => rm.MenuId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 获取角色的部门ID列表
    /// </summary>
    private async Task<List<long>> GetRoleDeptIdsAsync(long roleId, CancellationToken cancellationToken = default)
    {
        var dbContext = _userRepository.GetDbContext();
        return await dbContext.Set<SysRoleDept>()
            .Where(rd => rd.RoleId == roleId)
            .Select(rd => rd.DeptId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 保存角色菜单关联
    /// </summary>
    private async Task SaveRoleMenusAsync(long roleId, List<long> menuIds, CancellationToken cancellationToken = default)
    {
        // 删除旧的关联
        await DeleteRoleMenusAsync(roleId, cancellationToken);

        // 添加新的关联
        if (menuIds.Any())
        {
            var dbContext = _userRepository.GetDbContext();
            var roleMenus = menuIds.Select(menuId => new SysRoleMenu
            {
                RoleId = roleId,
                MenuId = menuId
            }).ToList();

            await dbContext.Set<SysRoleMenu>().AddRangeAsync(roleMenus, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// 删除角色菜单关联
    /// </summary>
    private async Task DeleteRoleMenusAsync(long roleId, CancellationToken cancellationToken = default)
    {
        var dbContext = _userRepository.GetDbContext();
        var roleMenus = await dbContext.Set<SysRoleMenu>()
            .Where(rm => rm.RoleId == roleId)
            .ToListAsync(cancellationToken);

        if (roleMenus.Any())
        {
            dbContext.Set<SysRoleMenu>().RemoveRange(roleMenus);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// 保存角色部门关联
    /// </summary>
    private async Task SaveRoleDeptsAsync(long roleId, List<long> deptIds, CancellationToken cancellationToken = default)
    {
        // 删除旧的关联
        await DeleteRoleDeptsAsync(roleId, cancellationToken);

        // 添加新的关联
        if (deptIds.Any())
        {
            var dbContext = _userRepository.GetDbContext();
            var roleDepts = deptIds.Select(deptId => new SysRoleDept
            {
                RoleId = roleId,
                DeptId = deptId
            }).ToList();

            await dbContext.Set<SysRoleDept>().AddRangeAsync(roleDepts, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// 删除角色部门关联
    /// </summary>
    private async Task DeleteRoleDeptsAsync(long roleId, CancellationToken cancellationToken = default)
    {
        var dbContext = _userRepository.GetDbContext();
        var roleDepts = await dbContext.Set<SysRoleDept>()
            .Where(rd => rd.RoleId == roleId)
            .ToListAsync(cancellationToken);

        if (roleDepts.Any())
        {
            dbContext.Set<SysRoleDept>().RemoveRange(roleDepts);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// 清除角色相关用户的权限缓存
    /// </summary>
    private async Task ClearRoleUsersCacheAsync(long roleId, CancellationToken cancellationToken = default)
    {
        var dbContext = _userRepository.GetDbContext();
        var userIds = await dbContext.Set<SysUserRole>()
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.UserId)
            .ToListAsync(cancellationToken);

        foreach (var userId in userIds)
        {
            await _permissionService.ClearUserPermissionCacheAsync(userId);
        }
    }

    #endregion

    /// <summary>
    /// 更新角色数据权限范围
    /// </summary>
    public async Task UpdateDataScopeAsync(long roleId, string dataScope, List<long> deptIds, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            throw new InvalidOperationException("角色不存在");
        }

        // 将字符串转换为 DataScopeType 枚举
        role.DataScope = dataScope switch
        {
            "1" => DataScopeType.All,
            "2" => DataScopeType.Custom,
            "3" => DataScopeType.Department,
            "4" => DataScopeType.DepartmentAndBelow,
            "5" => DataScopeType.Self,
            _ => DataScopeType.All
        };
        await _roleRepository.UpdateAsync(role, cancellationToken);

        // 删除旧的部门权限
        await DeleteRoleDeptsAsync(roleId, cancellationToken);

        // 保存新的部门权限（仅当数据范围为自定义时）
        if (dataScope == "2" && deptIds.Any())
        {
            await SaveRoleDeptsAsync(roleId, deptIds, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 取消单个用户的角色授权
    /// </summary>
    public async Task CancelAuthUserAsync(long roleId, long userId, CancellationToken cancellationToken = default)
    {
        var dbContext = _userRepository.GetDbContext();
        var userRole = await dbContext.Set<SysUserRole>()
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);

        if (userRole != null)
        {
            dbContext.Set<SysUserRole>().Remove(userRole);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _permissionService.ClearUserPermissionCacheAsync(userId);
        }
    }

    /// <summary>
    /// 批量取消用户的角色授权
    /// </summary>
    public async Task CancelAuthUsersAsync(long roleId, long[] userIds, CancellationToken cancellationToken = default)
    {
        var dbContext = _userRepository.GetDbContext();
        var userRoles = await dbContext.Set<SysUserRole>()
            .Where(ur => ur.RoleId == roleId && userIds.Contains(ur.UserId))
            .ToListAsync(cancellationToken);

        if (userRoles.Any())
        {
            foreach (var userRole in userRoles)
            {
                dbContext.Set<SysUserRole>().Remove(userRole);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 清除用户权限缓存
            foreach (var userId in userIds)
            {
                await _permissionService.ClearUserPermissionCacheAsync(userId);
            }
        }
    }

    /// <summary>
    /// 批量给用户授予角色
    /// </summary>
    public async Task InsertAuthUsersAsync(long roleId, long[] userIds, CancellationToken cancellationToken = default)
    {
        // 检查角色是否存在
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            throw new InvalidOperationException("角色不存在");
        }

        var dbContext = _userRepository.GetDbContext();
        
        // 获取已存在的用户角色关系
        var existingUserRoles = await dbContext.Set<SysUserRole>()
            .Where(ur => ur.RoleId == roleId && userIds.Contains(ur.UserId))
            .Select(ur => ur.UserId)
            .ToListAsync(cancellationToken);

        // 只添加不存在的用户角色关系
        var newUserIds = userIds.Except(existingUserRoles).ToList();
        if (newUserIds.Any())
        {
            foreach (var userId in newUserIds)
            {
                dbContext.Set<SysUserRole>().Add(new SysUserRole
                {
                    UserId = userId,
                    RoleId = roleId
                });
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 清除用户权限缓存
            foreach (var userId in newUserIds)
            {
                await _permissionService.ClearUserPermissionCacheAsync(userId);
            }
        }
    }

    /// <summary>
    /// 获取所有角色列表（用于下拉选择）
    /// </summary>
    public async Task<List<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetQueryable()
            .Where(r => r.Status == UserStatus.Normal)
            .OrderBy(r => r.RoleSort)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<RoleDto>>(roles);
    }

    /// <summary>
    /// 导出角色数据
    /// </summary>
    public async Task<byte[]> ExportRolesAsync(RoleQueryDto query, CancellationToken cancellationToken = default)
    {
        // 获取所有数据（不分页）
        var queryable = _roleRepository.GetQueryable();

        // 角色名称
        if (!string.IsNullOrWhiteSpace(query.RoleName))
        {
            queryable = queryable.Where(r => r.RoleName.Contains(query.RoleName));
        }

        // 角色权限
        if (!string.IsNullOrWhiteSpace(query.RoleKey))
        {
            queryable = queryable.Where(r => r.RoleKey.Contains(query.RoleKey));
        }

        // 状态
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (Enum.TryParse<UserStatus>(query.Status, out var status))
                queryable = queryable.Where(r => r.Status == status);
        }

        var roles = await queryable
            .OrderBy(r => r.RoleSort)
            .ToListAsync(cancellationToken);

        var roleDtos = _mapper.Map<List<RoleDto>>(roles);

        // 转换为导出DTO（带中文列头）
        var exportDtos = roleDtos.Select(r => new ExportRoleDto
        {
            RoleId = r.RoleId,
            RoleName = r.RoleName,
            RoleKey = r.RoleKey,
            RoleSort = r.RoleSort,
            DataScope = r.DataScope switch
            {
                "1" => "全部数据权限",
                "2" => "自定数据权限",
                "3" => "本部门数据权限",
                "4" => "本部门及以下数据权限",
                "5" => "仅本人数据权限",
                _ => r.DataScope
            },
            Status = r.Status switch
            {
                "0" => "正常",
                "1" => "停用",
                _ => r.Status
            },
            CreateTime = r.CreateTime,
            Remark = r.Remark
        }).ToList();

        // 导出到内存流
        using var stream = new MemoryStream();
        await _excelService.ExportAsync(exportDtos, stream, cancellationToken);
        return stream.ToArray();
    }
}
