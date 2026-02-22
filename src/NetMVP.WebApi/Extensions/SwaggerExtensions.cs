namespace NetMVP.WebApi.Extensions;

/// <summary>
/// Swagger扩展配置
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// 添加Swagger服务
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // API分组
            options.SwaggerDoc("system", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "系统管理",
                Version = "v1",
                Description = "用户、角色、菜单、部门、岗位、字典、参数、通知公告"
            });

            options.SwaggerDoc("monitor", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "系统监控",
                Version = "v1",
                Description = "在线用户、操作日志、登录日志、服务监控、缓存监控"
            });

            options.SwaggerDoc("tool", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "系统工具",
                Version = "v1",
                Description = "代码生成、定时任务"
            });

            // JWT认证配置
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）",
                Name = "Authorization",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // XML注释
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, true);
            }

            // Application层XML注释
            var applicationXmlFile = "NetMVP.Application.xml";
            var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXmlFile);
            if (File.Exists(applicationXmlPath))
            {
                options.IncludeXmlComments(applicationXmlPath, true);
            }

            // 按控制器分组
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                var controllerName = apiDesc.ActionDescriptor.RouteValues["controller"];
                if (string.IsNullOrEmpty(controllerName))
                {
                    return false;
                }

                return docName.ToLower() switch
                {
                    "system" => controllerName.StartsWith("Sys") || controllerName == "Auth" || controllerName == "Common" || controllerName == "Test",
                    "monitor" => controllerName.Contains("Monitor") || controllerName.Contains("Online") || 
                                 controllerName.Contains("OperLog") || controllerName.Contains("LoginInfo") ||
                                 controllerName == "Server" || controllerName == "Cache",
                    "tool" => controllerName == "Gen" || controllerName.Contains("Job"),
                    _ => false
                };
            });

            // 自定义操作ID
            options.CustomOperationIds(apiDesc =>
            {
                var controllerName = apiDesc.ActionDescriptor.RouteValues["controller"];
                var actionName = apiDesc.ActionDescriptor.RouteValues["action"];
                return $"{controllerName}_{actionName}";
            });
        });

        return services;
    }

    /// <summary>
    /// 使用Swagger中间件
    /// </summary>
    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/system/swagger.json", "系统管理");
            options.SwaggerEndpoint("/swagger/monitor/swagger.json", "系统监控");
            options.SwaggerEndpoint("/swagger/tool/swagger.json", "系统工具");

            options.RoutePrefix = "swagger";
            options.DocumentTitle = "NetMVP API 文档";
            options.DefaultModelsExpandDepth(-1); // 隐藏Models
            options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            options.DisplayRequestDuration();
        });

        return app;
    }
}
