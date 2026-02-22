using Microsoft.Extensions.DependencyInjection;
using NetMVP.Application.Mappings;
using NetMVP.Application.Services;
using NetMVP.Application.Services.Gen;
using NetMVP.Application.Services.Impl;

namespace NetMVP.Application.Extensions;

/// <summary>
/// 应用层服务集合扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加应用层服务
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 添加 AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // 添加应用服务
        services.AddScoped<ISysUserService, SysUserService>();
        services.AddScoped<ISysRoleService, SysRoleService>();
        services.AddScoped<ISysMenuService, SysMenuService>();
        services.AddScoped<ISysDeptService, SysDeptService>();
        services.AddScoped<ISysPostService, SysPostService>();
        services.AddScoped<ISysDictTypeService, SysDictTypeService>();
        services.AddScoped<ISysDictDataService, SysDictDataService>();
        services.AddScoped<ISysConfigService, SysConfigService>();
        services.AddScoped<ISysNoticeService, SysNoticeService>();
        services.AddScoped<ISysUserOnlineService, SysUserOnlineService>();
        services.AddScoped<ISysOperLogService, SysOperLogService>();
        services.AddScoped<ISysLoginInfoService, SysLoginInfoService>();
        services.AddScoped<IServerMonitorService, ServerMonitorService>();
        services.AddScoped<ISysJobService, SysJobService>();
        services.AddScoped<ISysJobLogService, SysJobLogService>();
        services.AddScoped<IGenTableService, GenTableService>();
        services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IRegisterService, RegisterService>();

        return services;
    }
}
