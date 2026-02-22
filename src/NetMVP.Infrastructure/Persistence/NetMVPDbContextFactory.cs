using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NetMVP.Infrastructure.Persistence;

/// <summary>
/// DbContext 工厂，用于设计时迁移
/// </summary>
public class NetMVPDbContextFactory : IDesignTimeDbContextFactory<NetMVPDbContext>
{
    public NetMVPDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NetMVPDbContext>();
        
        // 使用默认连接字符串
        var connectionString = "Server=127.0.0.1;Port=3306;Database=ry-vue;User=root;Password=123456;CharSet=utf8mb4;";
        
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new NetMVPDbContext(optionsBuilder.Options);
    }
}
