using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetMVP.Application.DTOs.Dept;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 部门服务实现
/// </summary>
public class SysDeptService : ISysDeptService
{
    private readonly IRepository<SysDept> _deptRepository;
    private readonly IRepository<SysUser> _userRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public SysDeptService(
        IRepository<SysDept> deptRepository,
        IRepository<SysUser> userRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _deptRepository = deptRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// 获取部门树
    /// </summary>
    public async Task<List<DeptDto>> GetDeptTreeAsync(DeptQueryDto query, CancellationToken cancellationToken = default)
    {
        var depts = await GetDeptListAsync(query, cancellationToken);
        return BuildDeptTree(depts, 0);
    }

    /// <summary>
    /// 获取部门列表
    /// </summary>
    public async Task<List<DeptDto>> GetDeptListAsync(DeptQueryDto query, CancellationToken cancellationToken = default)
    {
        var queryable = _deptRepository.GetQueryable();

        // 部门名称
        if (!string.IsNullOrWhiteSpace(query.DeptName))
        {
            queryable = queryable.Where(d => d.DeptName.Contains(query.DeptName));
        }

        // 状态
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(d => d.Status == query.Status);
        }

        var depts = await queryable
            .OrderBy(d => d.ParentId)
            .ThenBy(d => d.OrderNum)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<DeptDto>>(depts);
    }

    /// <summary>
    /// 根据ID获取部门
    /// </summary>
    public async Task<DeptDto?> GetDeptByIdAsync(long deptId, CancellationToken cancellationToken = default)
    {
        var dept = await _deptRepository.GetByIdAsync(deptId, cancellationToken);
        return dept == null ? null : _mapper.Map<DeptDto>(dept);
    }

    /// <summary>
    /// 创建部门
    /// </summary>
    public async Task<long> CreateDeptAsync(CreateDeptDto dto, CancellationToken cancellationToken = default)
    {
        // 检查部门名称唯一性
        if (!await CheckDeptNameUniqueAsync(dto.DeptName, dto.ParentId, null, cancellationToken))
        {
            throw new InvalidOperationException($"部门名称'{dto.DeptName}'已存在");
        }

        // 获取父部门信息
        var ancestors = "0";
        if (dto.ParentId != 0)
        {
            var parentDept = await _deptRepository.GetByIdAsync(dto.ParentId, cancellationToken);
            if (parentDept == null)
            {
                throw new InvalidOperationException("父部门不存在");
            }

            if (parentDept.Status == UserConstants.USER_DISABLE)
            {
                throw new InvalidOperationException("父部门已停用，不允许新增");
            }

            ancestors = parentDept.Ancestors + "," + dto.ParentId;
        }

        var dept = new SysDept
        {
            ParentId = dto.ParentId,
            Ancestors = ancestors,
            DeptName = dto.DeptName,
            OrderNum = dto.OrderNum,
            Leader = dto.Leader,
            PhoneValue = dto.Phone,
            EmailValue = dto.Email,
            Status = dto.Status
        };

        await _deptRepository.AddAsync(dept, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return dept.DeptId;
    }

    /// <summary>
    /// 更新部门
    /// </summary>
    public async Task UpdateDeptAsync(UpdateDeptDto dto, CancellationToken cancellationToken = default)
    {
        var dept = await _deptRepository.GetByIdAsync(dto.DeptId, cancellationToken);
        if (dept == null)
        {
            throw new InvalidOperationException("部门不存在");
        }

        // 不能将父部门设置为自己或自己的子部门
        if (dto.ParentId == dto.DeptId)
        {
            throw new InvalidOperationException("不能将父部门设置为自己");
        }

        // 检查是否将父部门设置为自己的子部门
        if (dto.ParentId != dept.ParentId)
        {
            var newParentDept = await _deptRepository.GetByIdAsync(dto.ParentId, cancellationToken);
            if (newParentDept != null && newParentDept.Ancestors != null)
            {
                if (newParentDept.Ancestors.Contains($",{dto.DeptId},") || 
                    newParentDept.Ancestors.EndsWith($",{dto.DeptId}"))
                {
                    throw new InvalidOperationException("不能将父部门设置为自己的子部门");
                }
            }
        }

        // 检查部门名称唯一性
        if (!await CheckDeptNameUniqueAsync(dto.DeptName, dto.ParentId, dto.DeptId, cancellationToken))
        {
            throw new InvalidOperationException($"部门名称'{dto.DeptName}'已存在");
        }

        var oldAncestors = dept.Ancestors;
        var newAncestors = "0";

        // 更新祖级列表
        if (dto.ParentId != 0)
        {
            var parentDept = await _deptRepository.GetByIdAsync(dto.ParentId, cancellationToken);
            if (parentDept == null)
            {
                throw new InvalidOperationException("父部门不存在");
            }

            if (parentDept.Status == UserConstants.USER_DISABLE)
            {
                throw new InvalidOperationException("父部门已停用，不允许修改");
            }

            newAncestors = parentDept.Ancestors + "," + dto.ParentId;
        }

        dept.ParentId = dto.ParentId;
        dept.Ancestors = newAncestors;
        dept.DeptName = dto.DeptName;
        dept.OrderNum = dto.OrderNum;
        dept.Leader = dto.Leader;
        dept.PhoneValue = dto.Phone;
        dept.EmailValue = dto.Email;
        dept.Status = dto.Status;

        await _deptRepository.UpdateAsync(dept, cancellationToken);

        // 更新子部门的祖级列表
        if (!string.IsNullOrEmpty(oldAncestors) && oldAncestors != newAncestors)
        {
            await UpdateChildrenAncestorsAsync(dept.DeptId, oldAncestors, newAncestors, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    public async Task DeleteDeptAsync(long deptId, CancellationToken cancellationToken = default)
    {
        var dept = await _deptRepository.GetByIdAsync(deptId, cancellationToken);
        if (dept == null)
        {
            throw new InvalidOperationException("部门不存在");
        }

        // 检查是否有子部门
        var hasChildren = await _deptRepository.GetQueryable()
            .AnyAsync(d => d.ParentId == deptId, cancellationToken);

        if (hasChildren)
        {
            throw new InvalidOperationException("存在子部门，不允许删除");
        }

        // 检查是否有用户
        var hasUsers = await _userRepository.GetQueryable()
            .AnyAsync(u => u.DeptId == deptId, cancellationToken);

        if (hasUsers)
        {
            throw new InvalidOperationException("该部门下存在用户，不能删除");
        }

        await _deptRepository.DeleteAsync(dept, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 获取部门树选择列表
    /// </summary>
    public async Task<List<DeptTreeDto>> GetDeptTreeSelectAsync(CancellationToken cancellationToken = default)
    {
        var depts = await _deptRepository.GetQueryable()
            .Where(d => d.Status == UserConstants.NORMAL)
            .OrderBy(d => d.ParentId)
            .ThenBy(d => d.OrderNum)
            .ToListAsync(cancellationToken);

        var deptDtos = _mapper.Map<List<DeptDto>>(depts);
        return BuildDeptTreeSelect(deptDtos, 0);
    }

    /// <summary>
    /// 获取角色部门树
    /// </summary>
    public async Task<List<DeptTreeDto>> GetRoleDeptTreeSelectAsync(long roleId, CancellationToken cancellationToken = default)
    {
        var depts = await _deptRepository.GetQueryable()
            .Where(d => d.Status == UserConstants.NORMAL)
            .OrderBy(d => d.ParentId)
            .ThenBy(d => d.OrderNum)
            .ToListAsync(cancellationToken);

        var dbContext = _userRepository.GetDbContext();
        var roleDeptIds = await dbContext.Set<SysRoleDept>()
            .Where(rd => rd.RoleId == roleId)
            .Select(rd => rd.DeptId)
            .ToListAsync(cancellationToken);

        var deptDtos = _mapper.Map<List<DeptDto>>(depts);
        var deptTrees = BuildDeptTreeSelect(deptDtos, 0);

        return deptTrees;
    }

    /// <summary>
    /// 检查部门名称唯一性
    /// </summary>
    public async Task<bool> CheckDeptNameUniqueAsync(string deptName, long parentId, long? excludeDeptId = null, CancellationToken cancellationToken = default)
    {
        var query = _deptRepository.GetQueryable()
            .Where(d => d.DeptName == deptName && d.ParentId == parentId);

        if (excludeDeptId.HasValue)
        {
            query = query.Where(d => d.DeptId != excludeDeptId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    #region 私有方法

    /// <summary>
    /// 构建部门树
    /// </summary>
    private List<DeptDto> BuildDeptTree(List<DeptDto> depts, long parentId)
    {
        var tree = new List<DeptDto>();

        foreach (var dept in depts.Where(d => d.ParentId == parentId))
        {
            dept.Children = BuildDeptTree(depts, dept.DeptId);
            tree.Add(dept);
        }

        return tree;
    }

    /// <summary>
    /// 构建部门树选择列表
    /// </summary>
    private List<DeptTreeDto> BuildDeptTreeSelect(List<DeptDto> depts, long parentId)
    {
        var tree = new List<DeptTreeDto>();

        foreach (var dept in depts.Where(d => d.ParentId == parentId))
        {
            var node = new DeptTreeDto
            {
                Id = dept.DeptId,
                Label = dept.DeptName,
                Disabled = dept.Status == "1", // 1=停用
                Children = BuildDeptTreeSelect(depts, dept.DeptId)
            };
            tree.Add(node);
        }

        return tree;
    }

    /// <summary>
    /// 更新子部门的祖级列表
    /// </summary>
    private async Task UpdateChildrenAncestorsAsync(long deptId, string oldAncestors, string newAncestors, CancellationToken cancellationToken = default)
    {
        var children = await _deptRepository.GetQueryable()
            .Where(d => d.Ancestors != null && 
                   (d.Ancestors.Contains($",{deptId},") || d.Ancestors.EndsWith($",{deptId}")))
            .ToListAsync(cancellationToken);

        foreach (var child in children)
        {
            if (child.Ancestors != null)
            {
                child.Ancestors = child.Ancestors.Replace(oldAncestors, newAncestors);
                await _deptRepository.UpdateAsync(child, cancellationToken);
            }
        }
    }

    #endregion

    /// <summary>
    /// 获取角色已选部门ID列表
    /// </summary>
    public async Task<List<long>> GetDeptIdsByRoleIdAsync(long roleId, CancellationToken cancellationToken = default)
    {
        var dbContext = _userRepository.GetDbContext();
        return await dbContext.Set<SysRoleDept>()
            .Where(rd => rd.RoleId == roleId)
            .Select(rd => rd.DeptId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 获取部门列表（排除指定节点及其子节点）
    /// </summary>
    public async Task<List<DeptDto>> GetDeptListExcludeChildAsync(long deptId, CancellationToken cancellationToken = default)
    {
        var allDepts = await _deptRepository.GetQueryable()
            .OrderBy(d => d.ParentId)
            .ThenBy(d => d.OrderNum)
            .ToListAsync(cancellationToken);

        var deptDtos = _mapper.Map<List<DeptDto>>(allDepts);

        // 获取要排除的部门
        var excludeDept = deptDtos.FirstOrDefault(d => d.DeptId == deptId);
        if (excludeDept == null)
        {
            return deptDtos;
        }

        // 排除指定部门及其所有子部门
        var excludeIds = new HashSet<long> { deptId };
        var ancestors = excludeDept.Ancestors ?? "";
        
        // 找出所有子部门（ancestors包含当前deptId的）
        foreach (var dept in deptDtos)
        {
            if (dept.Ancestors != null && dept.Ancestors.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Any(a => long.TryParse(a, out var ancestorId) && ancestorId == deptId))
            {
                excludeIds.Add(dept.DeptId);
            }
        }

        return deptDtos.Where(d => !excludeIds.Contains(d.DeptId)).ToList();
    }
}
