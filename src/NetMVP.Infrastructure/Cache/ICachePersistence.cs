using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetMVP.Infrastructure.Cache
{
    /// <summary>
    /// 缓存持久化接口
    /// </summary>
    public interface ICachePersistence
    {
        /// <summary>
        /// 保存缓存条目
        /// </summary>
        Task SaveAsync(CacheEntry entry);

        /// <summary>
        /// 获取缓存条目
        /// </summary>
        Task<CacheEntry?> GetAsync(string key);

        /// <summary>
        /// 删除缓存条目
        /// </summary>
        Task<bool> DeleteAsync(string key);

        /// <summary>
        /// 获取所有缓存条目
        /// </summary>
        Task<List<CacheEntry>> GetAllAsync();

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        Task ClearAsync();

        /// <summary>
        /// 删除过期缓存
        /// </summary>
        Task DeleteExpiredAsync();
    }
}
