using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetMVP.Application.DTOs.LoginInfo;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Impl;

/// <summary>
/// 登录日志服务实现
/// </summary>
public class SysLoginInfoService : ISysLoginInfoService
{
    private readonly ISysLoginInfoRepository _loginInfoRepository;
    private readonly IMapper _mapper;
    private readonly IExcelService _excelService;
    private readonly ICacheService _cacheService;

    public SysLoginInfoService(
        ISysLoginInfoRepository loginInfoRepository,
        IMapper mapper,
        IExcelService excelService,
        ICacheService cacheService)
    {
        _loginInfoRepository = loginInfoRepository;
        _mapper = mapper;
        _excelService = excelService;
        _cacheService = cacheService;
    }

    /// <summary>
    /// 获取登录日志列表
    /// </summary>
    public async Task<(List<LoginInfoDto> list, int total)> GetLoginInfoListAsync(
        LoginInfoQueryDto query,
        CancellationToken cancellationToken = default)
    {
        var queryable = _loginInfoRepository.GetQueryable();

        // 用户账号
        if (!string.IsNullOrWhiteSpace(query.UserName))
        {
            queryable = queryable.Where(x => x.UserName.Contains(query.UserName));
        }

        // 登录IP
        if (!string.IsNullOrWhiteSpace(query.IpAddr))
        {
            queryable = queryable.Where(x => x.IpAddrValue.Contains(query.IpAddr));
        }

        // 登录状态
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(x => x.Status == query.Status);
        }

        // 时间范围
        if (query.BeginTime.HasValue)
        {
            queryable = queryable.Where(x => x.LoginTime >= query.BeginTime.Value);
        }
        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where(x => x.LoginTime <= query.EndTime.Value);
        }

        // 排序
        queryable = queryable.OrderByDescending(x => x.LoginTime);

        // 分页
        var total = await queryable.CountAsync(cancellationToken);
        var items = await queryable
            .Skip((query.PageNum - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(x => new LoginInfoDto
        {
            InfoId = x.InfoId,
            UserName = x.UserName,
            IpAddr = x.IpAddrValue,
            LoginLocation = x.LoginLocation,
            Browser = x.Browser,
            Os = x.Os,
            Status = x.Status,
            Msg = x.Msg,
            LoginTime = x.LoginTime ?? DateTime.Now
        }).ToList();

        return (dtos, total);
    }

    /// <summary>
    /// 创建登录日志
    /// </summary>
    public async Task<long> CreateLoginInfoAsync(
        CreateLoginInfoDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = new SysLoginInfo
        {
            UserName = dto.UserName,
            IpAddrValue = dto.IpAddr,
            LoginLocation = dto.LoginLocation ?? "内网IP",
            Browser = dto.Browser,
            Os = dto.Os,
            Status = dto.Status,
            Msg = dto.Msg,
            LoginTime = DateTime.Now
        };

        await _loginInfoRepository.AddAsync(entity, cancellationToken);
        return entity.InfoId;
    }

    /// <summary>
    /// 删除登录日志
    /// </summary>
    public async Task<bool> DeleteLoginInfoAsync(
        long infoId,
        CancellationToken cancellationToken = default)
    {
        var entity = await Task.Run(() => _loginInfoRepository.GetQueryable()
            .FirstOrDefault(x => x.InfoId == infoId), cancellationToken);
        if (entity == null)
            return false;

        await _loginInfoRepository.DeleteAsync(entity, cancellationToken);
        return true;
    }

    /// <summary>
    /// 批量删除登录日志
    /// </summary>
    public async Task<int> DeleteLoginInfosAsync(
        long[] infoIds,
        CancellationToken cancellationToken = default)
    {
        var count = await _loginInfoRepository
            .GetQueryable()
            .Where(x => infoIds.Contains(x.InfoId))
            .ExecuteDeleteAsync(cancellationToken);

        return count;
    }

    /// <summary>
    /// 清空登录日志
    /// </summary>
    public async Task<int> CleanLoginInfoAsync(CancellationToken cancellationToken = default)
    {
        return await _loginInfoRepository.CleanAsync(cancellationToken);
    }

    /// <summary>
    /// 解锁用户
    /// </summary>
    public async Task<bool> UnlockUserAsync(
        string userName,
        CancellationToken cancellationToken = default)
    {
        // 清除密码错误次数缓存
        var cacheKey = $"pwd_err_cnt:{userName}";
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);
        return true;
    }

    /// <summary>
    /// 导出登录日志
    /// </summary>
    public async Task<byte[]> ExportLoginInfosAsync(
        LoginInfoQueryDto query,
        CancellationToken cancellationToken = default)
    {
        var queryable = _loginInfoRepository.GetQueryable();

        // 应用查询条件
        if (!string.IsNullOrWhiteSpace(query.UserName))
        {
            queryable = queryable.Where(x => x.UserName.Contains(query.UserName));
        }
        if (!string.IsNullOrWhiteSpace(query.IpAddr))
        {
            queryable = queryable.Where(x => x.IpAddrValue.Contains(query.IpAddr));
        }
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(x => x.Status == query.Status);
        }
        if (query.BeginTime.HasValue)
        {
            queryable = queryable.Where(x => x.LoginTime >= query.BeginTime.Value);
        }
        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where(x => x.LoginTime <= query.EndTime.Value);
        }

        var items = await queryable
            .OrderByDescending(x => x.LoginTime)
            .ToListAsync(cancellationToken);

        var exportData = items.Select(x => new
        {
            访问编号 = x.InfoId,
            用户名称 = x.UserName,
            登录地址 = x.IpAddrValue,
            登录地点 = x.LoginLocation,
            浏览器 = x.Browser,
            操作系统 = x.Os,
            登录状态 = x.Status == CommonConstants.SUCCESS ? "成功" : "失败",
            操作信息 = x.Msg,
            登录日期 = x.LoginTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""
        }).ToList();

        using var stream = new MemoryStream();
        await _excelService.ExportAsync(exportData, stream, cancellationToken);
        return stream.ToArray();
    }
}
