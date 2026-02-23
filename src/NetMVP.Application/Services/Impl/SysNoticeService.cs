using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetMVP.Application.DTOs.Notice;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;
using System.Text;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 通知公告服务实现
/// </summary>
public class SysNoticeService : ISysNoticeService
{
    private readonly IRepository<SysNotice> _noticeRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public SysNoticeService(
        IRepository<SysNotice> noticeRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _noticeRepository = noticeRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// 获取公告列表
    /// </summary>
    public async Task<(List<NoticeDto> notices, int total)> GetNoticeListAsync(NoticeQueryDto query, CancellationToken cancellationToken = default)
    {
        var queryable = _noticeRepository.GetQueryable();

        // 公告标题
        if (!string.IsNullOrWhiteSpace(query.NoticeTitle))
        {
            queryable = queryable.Where(n => n.NoticeTitle.Contains(query.NoticeTitle));
        }

        // 公告类型
        if (!string.IsNullOrWhiteSpace(query.NoticeType))
        {
            queryable = queryable.Where(n => n.NoticeType == query.NoticeType);
        }

        // 创建者
        if (!string.IsNullOrWhiteSpace(query.CreateBy))
        {
            queryable = queryable.Where(n => n.CreateBy != null && n.CreateBy.Contains(query.CreateBy));
        }

        // 总数
        var total = await queryable.CountAsync(cancellationToken);

        // 分页
        var notices = await queryable
            .OrderByDescending(n => n.CreateTime)
            .Skip((query.PageNum - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var noticeDtos = _mapper.Map<List<NoticeDto>>(notices);

        return (noticeDtos, total);
    }

    /// <summary>
    /// 根据ID获取公告
    /// </summary>
    public async Task<NoticeDto?> GetNoticeByIdAsync(int noticeId, CancellationToken cancellationToken = default)
    {
        var notice = await _noticeRepository.GetByIdAsync(noticeId, cancellationToken);
        return notice == null ? null : _mapper.Map<NoticeDto>(notice);
    }

    /// <summary>
    /// 创建公告
    /// </summary>
    public async Task<int> CreateNoticeAsync(CreateNoticeDto dto, CancellationToken cancellationToken = default)
    {
        var notice = new SysNotice
        {
            NoticeTitle = dto.NoticeTitle,
            NoticeType = dto.NoticeType,
            NoticeContent = string.IsNullOrEmpty(dto.NoticeContent) ? null : Encoding.UTF8.GetBytes(dto.NoticeContent),
            Status = dto.Status,
            Remark = dto.Remark
        };

        await _noticeRepository.AddAsync(notice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return notice.NoticeId;
    }

    /// <summary>
    /// 更新公告
    /// </summary>
    public async Task UpdateNoticeAsync(UpdateNoticeDto dto, CancellationToken cancellationToken = default)
    {
        var notice = await _noticeRepository.GetByIdAsync(dto.NoticeId, cancellationToken);
        if (notice == null)
        {
            throw new InvalidOperationException("公告不存在");
        }

        notice.NoticeTitle = dto.NoticeTitle;
        notice.NoticeType = dto.NoticeType;
        notice.NoticeContent = string.IsNullOrEmpty(dto.NoticeContent) ? null : Encoding.UTF8.GetBytes(dto.NoticeContent);
        notice.Status = dto.Status;
        notice.Remark = dto.Remark;

        await _noticeRepository.UpdateAsync(notice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 删除公告
    /// </summary>
    public async Task DeleteNoticeAsync(int noticeId, CancellationToken cancellationToken = default)
    {
        var notice = await _noticeRepository.GetByIdAsync(noticeId, cancellationToken);
        if (notice == null)
        {
            throw new InvalidOperationException("公告不存在");
        }

        await _noticeRepository.DeleteAsync(notice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 批量删除公告
    /// </summary>
    public async Task DeleteNoticesAsync(int[] noticeIds, CancellationToken cancellationToken = default)
    {
        foreach (var noticeId in noticeIds)
        {
            await DeleteNoticeAsync(noticeId, cancellationToken);
        }
    }
}
