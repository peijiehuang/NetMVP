using NetMVP.Application.Extensions;
using NetMVP.Infrastructure.Extensions;
using NetMVP.Infrastructure.Services;
using NetMVP.WebApi.Extensions;
using NetMVP.WebApi.Filters;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    // 添加操作日志过滤器
    options.Filters.Add<OperationLogFilter>();
    // 添加登录日志过滤器
    options.Filters.Add<LoginLogFilter>();
})
    .AddJsonOptions(options =>
    {
        // 属性名使用 camelCase
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        // 忽略 null 值
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        // 允许读取数字字符串
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        // 中文不编码
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        // 枚举转换为字符串
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Add Application services
builder.Services.AddApplication();

// Add Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Add Database Initialization Service
builder.Services.AddSingleton<DatabaseInitializationService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Initialize database if needed
using (var scope = app.Services.CreateScope())
{
    var dbInitService = scope.ServiceProvider.GetRequiredService<DatabaseInitializationService>();
    await dbInitService.InitializeAsync();
}

// Configure the HTTP request pipeline.
// 全局异常处理（必须在最前面）
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();

// 启用 CORS
app.UseCors("AllowAll");

// 配置静态文件服务
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
