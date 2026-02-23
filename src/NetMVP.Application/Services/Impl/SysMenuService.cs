using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetMVP.Application.DTOs.Menu;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 菜单服务实现
/// </summary>
public class SysMenuService : ISysMenuService
{
    private readonly IRepository<SysMenu> _menuRepository;
    private readonly ISysUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;

    public SysMenuService(
        IRepository<SysMenu> menuRepository,
        ISysUserRepository userRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IPermissionService permissionService)
    {
        _menuRepository = menuRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
    }

    /// <summary>
    /// 获取菜单树
    /// </summary>
    public async Task<List<MenuDto>> GetMenuTreeAsync(MenuQueryDto query, CancellationToken cancellationToken = default)
    {
        var menus = await GetMenuListAsync(query, cancellationToken);
        return BuildMenuTree(menus, 0);
    }

    /// <summary>
    /// 获取菜单列表
    /// </summary>
    public async Task<List<MenuDto>> GetMenuListAsync(MenuQueryDto query, CancellationToken cancellationToken = default)
    {
        var queryable = _menuRepository.GetQueryable();

        // 菜单名称
        if (!string.IsNullOrWhiteSpace(query.MenuName))
        {
            queryable = queryable.Where(m => m.MenuName.Contains(query.MenuName));
        }

        // 状态
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (Enum.TryParse<UserStatus>(query.Status, out var status))
            queryable = queryable.Where(m => m.Status == status);
        }

        var menus = await queryable
            .OrderBy(m => m.ParentId)
            .ThenBy(m => m.OrderNum)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<MenuDto>>(menus);
    }

    /// <summary>
    /// 根据ID获取菜单
    /// </summary>
    public async Task<MenuDto?> GetMenuByIdAsync(long menuId, CancellationToken cancellationToken = default)
    {
        var menu = await _menuRepository.GetByIdAsync(menuId, cancellationToken);
        return menu == null ? null : _mapper.Map<MenuDto>(menu);
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    public async Task<long> CreateMenuAsync(CreateMenuDto dto, CancellationToken cancellationToken = default)
    {
        // 检查菜单名称唯一性
        if (!await CheckMenuNameUniqueAsync(dto.MenuName, dto.ParentId, null, cancellationToken))
        {
            throw new InvalidOperationException($"菜单名称'{dto.MenuName}'已存在");
        }

        var menu = new SysMenu
        {
            MenuName = dto.MenuName,
            ParentId = dto.ParentId,
            OrderNum = dto.OrderNum,
            Path = dto.Path,
            Component = dto.Component,
            Query = dto.Query,
            IsFrame = dto.IsFrame,
            IsCache = dto.IsCache,
            MenuType = dto.MenuType,
            Visible = dto.Visible,
            Status = dto.Status,
            Perms = dto.Perms,
            Icon = dto.Icon,
            Remark = dto.Remark
        };

        await _menuRepository.AddAsync(menu, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return menu.MenuId;
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    public async Task UpdateMenuAsync(UpdateMenuDto dto, CancellationToken cancellationToken = default)
    {
        var menu = await _menuRepository.GetByIdAsync(dto.MenuId, cancellationToken);
        if (menu == null)
        {
            throw new InvalidOperationException("菜单不存在");
        }

        // 不能将父菜单设置为自己
        if (dto.ParentId == dto.MenuId)
        {
            throw new InvalidOperationException("不能将父菜单设置为自己");
        }

        // 检查菜单名称唯一性
        if (!await CheckMenuNameUniqueAsync(dto.MenuName, dto.ParentId, dto.MenuId, cancellationToken))
        {
            throw new InvalidOperationException($"菜单名称'{dto.MenuName}'已存在");
        }

        menu.MenuName = dto.MenuName;
        menu.ParentId = dto.ParentId;
        menu.OrderNum = dto.OrderNum;
        menu.Path = dto.Path;
        menu.Component = dto.Component;
        menu.Query = dto.Query;
        menu.IsFrame = dto.IsFrame;
        menu.IsCache = dto.IsCache;
        menu.MenuType = dto.MenuType;
        menu.Visible = dto.Visible;
        menu.Status = dto.Status;
        menu.Perms = dto.Perms;
        menu.Icon = dto.Icon;
        menu.Remark = dto.Remark;

        await _menuRepository.UpdateAsync(menu, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除所有用户的权限缓存
        await ClearAllUserPermissionCacheAsync(cancellationToken);
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    public async Task DeleteMenuAsync(long menuId, CancellationToken cancellationToken = default)
    {
        var menu = await _menuRepository.GetByIdAsync(menuId, cancellationToken);
        if (menu == null)
        {
            throw new InvalidOperationException("菜单不存在");
        }

        // 检查是否有子菜单
        var hasChildren = await _menuRepository.GetQueryable()
            .AnyAsync(m => m.ParentId == menuId, cancellationToken);

        if (hasChildren)
        {
            throw new InvalidOperationException("存在子菜单，不允许删除");
        }

        // 检查是否有角色使用该菜单
        var dbContext = _userRepository.GetDbContext();
        var hasRoles = await dbContext.Set<SysRoleMenu>()
            .AnyAsync(rm => rm.MenuId == menuId, cancellationToken);

        if (hasRoles)
        {
            throw new InvalidOperationException("该菜单已分配给角色，不能删除");
        }

        await _menuRepository.DeleteAsync(menu, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 清除所有用户的权限缓存
        await ClearAllUserPermissionCacheAsync(cancellationToken);
    }

    /// <summary>
    /// 获取用户菜单树
    /// </summary>
    public async Task<List<MenuDto>> GetMenuTreeByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        var menus = await GetUserMenusAsync(userId, cancellationToken);
        return BuildMenuTree(menus, 0);
    }

    /// <summary>
    /// 获取角色菜单树
    /// </summary>
    public async Task<List<MenuTreeDto>> GetMenuTreeByRoleIdAsync(long roleId, CancellationToken cancellationToken = default)
    {
        var allMenus = await _menuRepository.GetQueryable()
            .Where(m => m.Status == UserStatus.Normal)
            .OrderBy(m => m.ParentId)
            .ThenBy(m => m.OrderNum)
            .ToListAsync(cancellationToken);

        var dbContext = _userRepository.GetDbContext();
        var roleMenuIds = await dbContext.Set<SysRoleMenu>()
            .Where(rm => rm.RoleId == roleId)
            .Select(rm => rm.MenuId)
            .ToListAsync(cancellationToken);

        var menuDtos = _mapper.Map<List<MenuDto>>(allMenus);
        var menuTrees = BuildMenuTreeSelect(menuDtos, 0);

        return menuTrees;
    }

    /// <summary>
    /// 获取角色已选菜单ID列表
    /// </summary>
    public async Task<List<long>> GetMenuIdsByRoleIdAsync(long roleId, CancellationToken cancellationToken = default)
    {
        var dbContext = _userRepository.GetDbContext();
        
        // 获取角色信息，判断是否需要父子联动
        var role = await dbContext.Set<SysRole>()
            .Where(r => r.RoleId == roleId)
            .FirstOrDefaultAsync(cancellationToken);

        if (role == null)
        {
            return new List<long>();
        }

        // 获取角色已选菜单ID
        var roleMenuIds = await dbContext.Set<SysRoleMenu>()
            .Where(rm => rm.RoleId == roleId)
            .Select(rm => rm.MenuId)
            .ToListAsync(cancellationToken);

        // 如果菜单树选择项关联显示（父子联动），则排除父节点，只返回叶子节点
        if (role.MenuCheckStrictly)
        {
            // 查找在角色已选菜单中作为父节点的菜单ID
            // 即：在角色已选菜单中，如果某个菜单的parent_id也在已选菜单中，则该parent_id应该被排除
            var parentIdsInRoleMenus = await (
                from m in dbContext.Set<SysMenu>()
                join rm in dbContext.Set<SysRoleMenu>() on m.MenuId equals rm.MenuId
                where rm.RoleId == roleId
                select m.ParentId
            ).Distinct().ToListAsync(cancellationToken);

            // 排除那些既在roleMenuIds中又在parentIdsInRoleMenus中的ID（即父节点）
            roleMenuIds = roleMenuIds.Where(id => !parentIdsInRoleMenus.Contains(id)).ToList();
        }

        return roleMenuIds;
    }

    /// <summary>
    /// 获取用户路由
    /// </summary>
    public async Task<List<RouterDto>> GetRoutersByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        var menus = await GetUserMenusAsync(userId, cancellationToken);
        
        // 只获取目录和菜单类型（M=目录, C=菜单）
        var routerMenus = menus.Where(m => 
            m.MenuType == "M" || 
            m.MenuType == "C").ToList();

        return BuildRouters(routerMenus, 0);
    }

    /// <summary>
    /// 检查菜单名称唯一性
    /// </summary>
    public async Task<bool> CheckMenuNameUniqueAsync(string menuName, long parentId, long? excludeMenuId = null, CancellationToken cancellationToken = default)
    {
        var query = _menuRepository.GetQueryable()
            .Where(m => m.MenuName == menuName && m.ParentId == parentId);

        if (excludeMenuId.HasValue)
        {
            query = query.Where(m => m.MenuId != excludeMenuId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 获取菜单树选择列表
    /// </summary>
    public async Task<List<MenuTreeDto>> GetMenuTreeSelectAsync(CancellationToken cancellationToken = default)
    {
        var menus = await _menuRepository.GetQueryable()
            .Where(m => m.Status == UserStatus.Normal)
            .OrderBy(m => m.ParentId)
            .ThenBy(m => m.OrderNum)
            .ToListAsync(cancellationToken);

        var menuDtos = _mapper.Map<List<MenuDto>>(menus);
        return BuildMenuTreeSelect(menuDtos, 0);
    }

    #region 私有方法

    /// <summary>
    /// 获取用户菜单列表
    /// </summary>
    private async Task<List<MenuDto>> GetUserMenusAsync(long userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUserIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return new List<MenuDto>();
        }

        // 超级管理员拥有所有菜单
        if (user.IsAdmin())
        {
            var allMenus = await _menuRepository.GetQueryable()
                .Where(m => m.Status == UserStatus.Normal)
                .OrderBy(m => m.ParentId)
                .ThenBy(m => m.OrderNum)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<MenuDto>>(allMenus);
        }

        // 获取用户角色的菜单
        var dbContext = _userRepository.GetDbContext();
        var roleIds = await dbContext.Set<SysUserRole>()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);

        if (!roleIds.Any())
        {
            return new List<MenuDto>();
        }

        var menuIds = await dbContext.Set<SysRoleMenu>()
            .Where(rm => roleIds.Contains(rm.RoleId))
            .Select(rm => rm.MenuId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (!menuIds.Any())
        {
            return new List<MenuDto>();
        }

        var menus = await _menuRepository.GetQueryable()
            .Where(m => menuIds.Contains(m.MenuId) && m.Status == UserStatus.Normal)
            .OrderBy(m => m.ParentId)
            .ThenBy(m => m.OrderNum)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<MenuDto>>(menus);
    }

    /// <summary>
    /// 构建菜单树
    /// </summary>
    private List<MenuDto> BuildMenuTree(List<MenuDto> menus, long parentId)
    {
        var tree = new List<MenuDto>();

        foreach (var menu in menus.Where(m => m.ParentId == parentId))
        {
            menu.Children = BuildMenuTree(menus, menu.MenuId);
            tree.Add(menu);
        }

        return tree;
    }

    /// <summary>
    /// 构建菜单树选择列表
    /// </summary>
    private List<MenuTreeDto> BuildMenuTreeSelect(List<MenuDto> menus, long parentId)
    {
        var tree = new List<MenuTreeDto>();

        foreach (var menu in menus.Where(m => m.ParentId == parentId))
        {
            var node = new MenuTreeDto
            {
                Id = menu.MenuId,
                Label = menu.MenuName,
                Children = BuildMenuTreeSelect(menus, menu.MenuId)
            };
            tree.Add(node);
        }

        return tree;
    }

    /// <summary>
    /// 构建路由
    /// </summary>
    private List<RouterDto> BuildRouters(List<MenuDto> menus, long parentId)
    {
        var routers = new List<RouterDto>();

        foreach (var menu in menus.Where(m => m.ParentId == parentId))
        {
            var router = new RouterDto
            {
                Name = GetRouteName(menu),
                Path = GetRouterPath(menu),
                Hidden = menu.Visible == "1", // 1=隐藏
                Component = GetComponent(menu),
                Query = menu.Query,
                Meta = new RouterMetaDto
                {
                    Title = menu.MenuName,
                    Icon = menu.Icon,
                    NoCache = !menu.IsCache,
                    Link = IsHttpLink(menu.Path) ? menu.Path : null // 只有http链接才设置link
                }
            };

            var cMenus = BuildRouters(menus, menu.MenuId);
            
            // 如果是目录且有子菜单
            if (cMenus.Any() && menu.MenuType == "M")
            {
                router.AlwaysShow = true;
                router.Redirect = "noRedirect";
                router.Children = cMenus;
            }
            // 如果是一级菜单（parent_id=0, type=C, is_frame=1）
            else if (IsMenuFrame(menu))
            {
                router.Meta = null; // 父路由不需要meta
                var children = new RouterDto
                {
                    Path = menu.Path ?? string.Empty,
                    Component = menu.Component ?? string.Empty,
                    Name = GetRouteName(menu),
                    Query = menu.Query,
                    Meta = new RouterMetaDto
                    {
                        Title = menu.MenuName,
                        Icon = menu.Icon,
                        NoCache = !menu.IsCache,
                        Link = IsHttpLink(menu.Path) ? menu.Path : null
                    }
                };
                router.Children = new List<RouterDto> { children };
            }
            // 如果是一级内链（parent_id=0, is_frame=1, path是http开头）
            else if (menu.ParentId == 0 && IsInnerLink(menu))
            {
                router.Meta = new RouterMetaDto
                {
                    Title = menu.MenuName,
                    Icon = menu.Icon
                };
                router.Path = "/";
                var children = new RouterDto
                {
                    Path = menu.Path ?? string.Empty, // 保持原始http路径
                    Component = "InnerLink",
                    Name = GetRouteName(menu),
                    Meta = new RouterMetaDto
                    {
                        Title = menu.MenuName,
                        Icon = menu.Icon,
                        Link = menu.Path // 内链的link就是path
                    }
                };
                router.Children = new List<RouterDto> { children };
            }

            routers.Add(router);
        }

        return routers;
    }

    /// <summary>
    /// 判断是否为http链接
    /// </summary>
    private bool IsHttpLink(string? path)
    {
        return !string.IsNullOrEmpty(path) && 
               (path.StartsWith("http://") || path.StartsWith("https://"));
    }

    /// <summary>
    /// 获取路由名称
    /// </summary>
    private string GetRouteName(MenuDto menu)
    {
        var routeName = menu.Path?.Replace("/", "");
        return string.IsNullOrEmpty(routeName) ? "" : char.ToUpper(routeName[0]) + routeName.Substring(1);
    }

    /// <summary>
    /// 获取路由地址
    /// </summary>
    private string GetRouterPath(MenuDto menu)
    {
        var routerPath = menu.Path ?? "";

        // 内链打开外网方式（is_frame=1且路径是http开头）
        if (menu.ParentId != 0 && IsInnerLink(menu))
        {
            routerPath = "/";
        }
        // 非外链并且是一级目录（类型为目录，M=目录，is_frame=1）
        else if (menu.ParentId == 0 && menu.MenuType == "M" && menu.IsFrame)
        {
            routerPath = "/" + menu.Path;
        }
        // 非外链并且是一级目录（类型为菜单，C=菜单，is_frame=1）
        else if (IsMenuFrame(menu))
        {
            routerPath = "/";
        }

        return routerPath;
    }

    /// <summary>
    /// 是否为内链组件（is_frame=1且路径是http开头）
    /// </summary>
    private bool IsInnerLink(MenuDto menu)
    {
        return menu.IsFrame && !string.IsNullOrEmpty(menu.Path) && 
               (menu.Path.StartsWith("http://") || menu.Path.StartsWith("https://"));
    }

    /// <summary>
    /// 是否为一级菜单且不是外链
    /// </summary>
    private bool IsMenuFrame(MenuDto menu)
    {
        return menu.ParentId == 0 && menu.MenuType == "C" && menu.IsFrame;
    }

    /// <summary>
    /// 获取组件路径
    /// </summary>
    private string? GetComponent(MenuDto menu)
    {
        var component = "Layout";

        if (!string.IsNullOrEmpty(menu.Component) && !IsMenuFrame(menu))
        {
            component = menu.Component;
        }
        else if (string.IsNullOrEmpty(menu.Component) && menu.ParentId != 0 && IsInnerLink(menu))
        {
            component = "InnerLink";
        }
        else if (string.IsNullOrEmpty(menu.Component) && IsParentView(menu))
        {
            component = "ParentView";
        }

        return component;
    }

    /// <summary>
    /// 是否为parent_view组件
    /// </summary>
    private bool IsParentView(MenuDto menu)
    {
        return menu.ParentId != 0 && menu.MenuType == "M";
    }

    /// <summary>
    /// 清除所有用户的权限缓存
    /// </summary>
    private async Task ClearAllUserPermissionCacheAsync(CancellationToken cancellationToken = default)
    {
        // 这里简化处理，实际应该清除所有用户的缓存
        // 可以通过 Redis 的 key 模式匹配来删除
        await Task.CompletedTask;
    }

    #endregion
}
