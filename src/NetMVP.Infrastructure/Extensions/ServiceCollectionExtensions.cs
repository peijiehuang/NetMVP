using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NetMVP.Domain.Interfaces;
using NetMVP.Infrastructure.Configuration;
using NetMVP.Infrastructure.Persistence;
using NetMVP.Infrastructure.Persistence.Interceptors;
using NetMVP.Infrastructure.Repositories;
using NetMVP.Infrastructure.Services.Auth;
using NetMVP.Infrastructure.Services.Cache;
using NetMVP.Infrastructure.Services.Config;
using NetMVP.Infrastructure.Services.Excel;
using NetMVP.Infrastructure.Services.Scheduler;
using NetMVP.Infrastructure.Jobs;
using StackExchange.Redis;
using System.Text;
using Quartz;

namespace NetMVP.Infrastructure.Extensions;

/// <summary>
/// 服务集合扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加基础设施层服务
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 添加 HttpContextAccessor
        services.AddHttpContextAccessor();

        // 添加配置服务
        services.AddSingleton<IConfigService, ConfigService>();
        
        // 添加强类型配置
        services.AddOptions<SystemSettings>()
            .Bind(configuration.GetSection("System"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
            
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection("Jwt"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
            
        services.AddOptions<DatabaseSettings>()
            .Bind(configuration.GetSection("Database"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
            
        services.AddOptions<RedisSettings>()
            .Bind(configuration.GetSection("Redis"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
            
        services.AddOptions<FileUploadSettings>()
            .Bind(configuration.GetSection("FileUpload"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        // 读取缓存配置
        var cacheSection = configuration.GetSection("Cache");
        var cacheOptions = new CacheOptions();
        cacheSection.Bind(cacheOptions);
        
        // 添加缓存配置到服务
        services.Configure<CacheOptions>(options => cacheSection.Bind(options));
        cacheSection.Bind(cacheOptions);

        // 添加 Redis 连接
        if (!string.IsNullOrEmpty(cacheOptions.RedisConnection))
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConfig = ConfigurationOptions.Parse(cacheOptions.RedisConnection);
                redisConfig.AbortOnConnectFail = false;
                redisConfig.AllowAdmin = true; // 启用管理命令，用于缓存监控
                return ConnectionMultiplexer.Connect(redisConfig);
            });
            
            services.AddSingleton<ICacheService, RedisCacheService>();
            
            // 添加缓存监控服务
            services.AddScoped<ICacheMonitorService, CacheMonitorService>();
        }
        else
        {
            throw new InvalidOperationException("Redis连接字符串未配置，请在appsettings.json中配置Cache:RedisConnection");
        }

        // 添加审计拦截器
        services.AddScoped<ISaveChangesInterceptor, AuditableEntitySaveChangesInterceptor>();

        // 添加数据库上下文
        services.AddDbContext<NetMVPDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            // 获取审计拦截器
            var auditInterceptor = serviceProvider.GetService<ISaveChangesInterceptor>();
            
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                   .AddInterceptors(auditInterceptor!);
        });

        // 添加工作单元
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 添加仓储
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        // 添加特定仓储
        services.AddScoped<ISysUserRepository, SysUserRepository>();
        services.AddScoped<ISysRoleRepository, SysRoleRepository>();
        services.AddScoped<ISysDeptRepository, SysDeptRepository>();
        services.AddScoped<ISysMenuRepository, SysMenuRepository>();
        services.AddScoped<ISysOperLogRepository, SysOperLogRepository>();
        services.AddScoped<ISysLoginInfoRepository, SysLoginInfoRepository>();
        services.AddScoped<ISysJobRepository, SysJobRepository>();
        services.AddScoped<ISysJobLogRepository, SysJobLogRepository>();
        services.AddScoped<IGenTableRepository, GenTableRepository>();
        services.AddScoped<IGenTableColumnRepository, GenTableColumnRepository>();

        // 添加 Excel 服务
        services.AddScoped<IExcelService, ExcelService>();

        // 添加 JWT 服务
        services.AddScoped<IJwtService, JwtService>();

        // 添加验证码服务
        services.AddScoped<ICaptchaService, CaptchaService>();

        // 添加认证服务
        services.AddScoped<IAuthService, AuthService>();

        // 添加权限服务
        services.AddScoped<IPermissionService, PermissionService>();

        // 添加数据权限过滤服务
        services.AddScoped<IDataScopeFilter, DataScopeFilter>();

        // 添加当前用户服务
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // 添加 Quartz 调度服务
        services.AddQuartz(q =>
        {
            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        // 添加任务监听器
        services.AddSingleton<JobListener>();

        // 添加任务执行器（使用带日志记录的版本）
        services.AddScoped<IJobExecutor, LoggingJobExecutor>();

        // 添加调度服务
        services.AddScoped<ISchedulerService, SchedulerService>();

        // 添加 JWT 认证
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
        if (jwtSettings != null && !string.IsNullOrEmpty(jwtSettings.SecretKey))
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };

                // 配置认证失败时的处理
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        // 阻止默认的401响应
                        context.HandleResponse();

                        // 设置HTTP状态码为200，但返回业务状态码401
                        context.Response.StatusCode = 200;
                        context.Response.ContentType = "application/json;charset=utf-8";

                        var result = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            code = 401,
                            msg = $"请求访问：{context.Request.Path}，认证失败，无法访问系统资源"
                        });

                        return context.Response.WriteAsync(result);
                    }
                };
            });
        }

        return services;
    }
}
