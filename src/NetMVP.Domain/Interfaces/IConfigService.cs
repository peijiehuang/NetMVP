using Microsoft.Extensions.Configuration;

namespace NetMVP.Domain.Interfaces;

/// <summary>
/// 配置服务接口
/// </summary>
public interface IConfigService
{
    /// <summary>
    /// 获取配置值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">配置键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>配置值</returns>
    T? GetValue<T>(string key, T? defaultValue = default);

    /// <summary>
    /// 获取配置节
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>配置节</returns>
    IConfigurationSection GetSection(string key);

    /// <summary>
    /// 重新加载配置
    /// </summary>
    void Reload();
}
