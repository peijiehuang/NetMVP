using Microsoft.Extensions.Configuration;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Infrastructure.Services.Config;

/// <summary>
/// 配置服务实现
/// </summary>
public class ConfigService : IConfigService
{
    private readonly IConfiguration _configuration;

    public ConfigService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public T? GetValue<T>(string key, T? defaultValue = default)
    {
        return _configuration.GetValue<T>(key, defaultValue!);
    }

    /// <inheritdoc/>
    public IConfigurationSection GetSection(string key)
    {
        return _configuration.GetSection(key);
    }

    /// <inheritdoc/>
    public void Reload()
    {
        if (_configuration is IConfigurationRoot configurationRoot)
        {
            configurationRoot.Reload();
        }
    }
}
