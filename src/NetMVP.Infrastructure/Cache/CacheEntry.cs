using System;

namespace NetMVP.Infrastructure.Cache
{
    /// <summary>
    /// 缓存条目
    /// </summary>
    public class CacheEntry
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 缓存键
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// 缓存值（JSON序列化）
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 值类型
        /// </summary>
        public string ValueType { get; set; } = string.Empty;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人
        /// </summary>
        public string? CreateBy { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string? UpdateBy { get; set; }

        /// <summary>
        /// 过期时间（null表示不过期）
        /// </summary>
        public DateTime? OutTime { get; set; }

        /// <summary>
        /// 是否已过期
        /// </summary>
        public bool IsExpired => OutTime.HasValue && OutTime.Value < DateTime.Now;
    }
}
