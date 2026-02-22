using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetMVP.Application.DTOs.Post;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Enums;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 岗位服务实现
/// </summary>
public class SysPostService : ISysPostService
{
    private readonly IRepository<SysPost> _postRepository;
    private readonly IRepository<SysUser> _userRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExcelService _excelService;

    public SysPostService(
        IRepository<SysPost> postRepository,
        IRepository<SysUser> userRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IExcelService excelService)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _excelService = excelService;
    }

    /// <summary>
    /// 获取岗位列表
    /// </summary>
    public async Task<(List<PostDto> posts, int total)> GetPostListAsync(PostQueryDto query, CancellationToken cancellationToken = default)
    {
        var queryable = _postRepository.GetQueryable();

        // 岗位编码
        if (!string.IsNullOrWhiteSpace(query.PostCode))
        {
            queryable = queryable.Where(p => p.PostCode.Contains(query.PostCode));
        }

        // 岗位名称
        if (!string.IsNullOrWhiteSpace(query.PostName))
        {
            queryable = queryable.Where(p => p.PostName.Contains(query.PostName));
        }

        // 状态
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (Enum.TryParse<UserStatus>(query.Status, out var status))
            queryable = queryable.Where(p => p.Status == status);
        }

        // 总数
        var total = await queryable.CountAsync(cancellationToken);

        // 分页
        var posts = await queryable
            .OrderBy(p => p.PostSort)
            .Skip((query.PageNum - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var postDtos = _mapper.Map<List<PostDto>>(posts);

        return (postDtos, total);
    }

    /// <summary>
    /// 根据ID获取岗位
    /// </summary>
    public async Task<PostDto?> GetPostByIdAsync(long postId, CancellationToken cancellationToken = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, cancellationToken);
        return post == null ? null : _mapper.Map<PostDto>(post);
    }

    /// <summary>
    /// 创建岗位
    /// </summary>
    public async Task<long> CreatePostAsync(CreatePostDto dto, CancellationToken cancellationToken = default)
    {
        // 检查岗位名称唯一性
        if (!await CheckPostNameUniqueAsync(dto.PostName, null, cancellationToken))
        {
            throw new InvalidOperationException($"岗位名称'{dto.PostName}'已存在");
        }

        // 检查岗位编码唯一性
        if (!await CheckPostCodeUniqueAsync(dto.PostCode, null, cancellationToken))
        {
            throw new InvalidOperationException($"岗位编码'{dto.PostCode}'已存在");
        }

        var post = new SysPost
        {
            PostCode = dto.PostCode,
            PostName = dto.PostName,
            PostSort = dto.PostSort,
            Status = dto.Status,
            Remark = dto.Remark
        };

        await _postRepository.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return post.PostId;
    }

    /// <summary>
    /// 更新岗位
    /// </summary>
    public async Task UpdatePostAsync(UpdatePostDto dto, CancellationToken cancellationToken = default)
    {
        var post = await _postRepository.GetByIdAsync(dto.PostId, cancellationToken);
        if (post == null)
        {
            throw new InvalidOperationException("岗位不存在");
        }

        // 检查岗位名称唯一性
        if (!await CheckPostNameUniqueAsync(dto.PostName, dto.PostId, cancellationToken))
        {
            throw new InvalidOperationException($"岗位名称'{dto.PostName}'已存在");
        }

        // 检查岗位编码唯一性
        if (!await CheckPostCodeUniqueAsync(dto.PostCode, dto.PostId, cancellationToken))
        {
            throw new InvalidOperationException($"岗位编码'{dto.PostCode}'已存在");
        }

        post.PostCode = dto.PostCode;
        post.PostName = dto.PostName;
        post.PostSort = dto.PostSort;
        post.Status = dto.Status;
        post.Remark = dto.Remark;

        await _postRepository.UpdateAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 删除岗位
    /// </summary>
    public async Task DeletePostAsync(long postId, CancellationToken cancellationToken = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, cancellationToken);
        if (post == null)
        {
            throw new InvalidOperationException("岗位不存在");
        }

        // 检查是否有用户使用该岗位
        var dbContext = _userRepository.GetDbContext();
        var hasUsers = await dbContext.Set<SysUserPost>()
            .AnyAsync(up => up.PostId == postId, cancellationToken);

        if (hasUsers)
        {
            throw new InvalidOperationException("该岗位已分配给用户，不能删除");
        }

        await _postRepository.DeleteAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 批量删除岗位
    /// </summary>
    public async Task DeletePostsAsync(long[] postIds, CancellationToken cancellationToken = default)
    {
        foreach (var postId in postIds)
        {
            await DeletePostAsync(postId, cancellationToken);
        }
    }

    /// <summary>
    /// 导出岗位
    /// </summary>
    public async Task<byte[]> ExportPostsAsync(PostQueryDto query, CancellationToken cancellationToken = default)
    {
        // 获取所有数据（不分页）
        var queryable = _postRepository.GetQueryable();

        // 岗位编码
        if (!string.IsNullOrWhiteSpace(query.PostCode))
        {
            queryable = queryable.Where(p => p.PostCode.Contains(query.PostCode));
        }

        // 岗位名称
        if (!string.IsNullOrWhiteSpace(query.PostName))
        {
            queryable = queryable.Where(p => p.PostName.Contains(query.PostName));
        }

        // 状态
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (Enum.TryParse<UserStatus>(query.Status, out var status))
            queryable = queryable.Where(p => p.Status == status);
        }

        var posts = await queryable
            .OrderBy(p => p.PostSort)
            .ToListAsync(cancellationToken);

        var postDtos = _mapper.Map<List<PostDto>>(posts);

        // 导出到内存流
        using var stream = new MemoryStream();
        await _excelService.ExportAsync(postDtos, stream, cancellationToken);
        return stream.ToArray();
    }

    /// <summary>
    /// 检查岗位名称唯一性
    /// </summary>
    public async Task<bool> CheckPostNameUniqueAsync(string postName, long? excludePostId = null, CancellationToken cancellationToken = default)
    {
        var query = _postRepository.GetQueryable().Where(p => p.PostName == postName);

        if (excludePostId.HasValue)
        {
            query = query.Where(p => p.PostId != excludePostId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 检查岗位编码唯一性
    /// </summary>
    public async Task<bool> CheckPostCodeUniqueAsync(string postCode, long? excludePostId = null, CancellationToken cancellationToken = default)
    {
        var query = _postRepository.GetQueryable().Where(p => p.PostCode == postCode);

        if (excludePostId.HasValue)
        {
            query = query.Where(p => p.PostId != excludePostId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }
}
