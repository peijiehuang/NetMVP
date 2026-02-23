# 缓存工具使用说明

## 功能特性

- 静态工具类，无需依赖注入，直接调用
- 内存缓存，高性能读写
- 方法级控制持久化（默认持久化到 SQLite）
- 自动初始化 SQLite 持久化实现
- 支持过期时间设置（默认不过期）
- 线程安全的并发操作
- 完整的审计字段（创建人、更新人、时间等）

## 基本使用

### 1. 持久化缓存（默认行为）

```csharp
// 直接使用，无需注入
// 设置缓存（默认持久化到 SQLite）
await CacheUtil.SetAsync("user:1001", new { Name = "张三", Age = 25 });

// 设置缓存（5分钟后过期，持久化）
await CacheUtil.SetAsync("session:abc", "token_value", TimeSpan.FromMinutes(5));

// 设置缓存（带创建人，持久化）
await CacheUtil.SetAsync("config:app", configData, createBy: "admin");

// 获取缓存（优先从内存读取，内存没有则从持久化加载）
var user = await CacheUtil.GetAsync<dynamic>("user:1001");

// 检查是否存在（优先检查内存，内存没有则检查持久化）
bool exists = await CacheUtil.ExistsAsync("user:1001");

// 删除缓存（同时删除内存和持久化）
await CacheUtil.RemoveAsync("user:1001");

// 清空所有缓存（同时清空内存和持久化）
await CacheUtil.ClearAsync();

// 清理过期缓存（同时清理内存和持久化）
await CacheUtil.CleanExpiredAsync();
```

### 2. 仅使用内存缓存（不持久化）

```csharp
// 设置缓存（仅内存，不持久化）
await CacheUtil.SetAsync("temp:data", tempData, persist: false);

// 设置缓存（5分钟过期，不持久化）
await CacheUtil.SetAsync("session:abc", "token", TimeSpan.FromMinutes(5), persist: false);

// 获取缓存（仅从内存读取，不加载持久化）
var user = await CacheUtil.GetAsync<UserData>("user:1001", loadFromPersist: false);

// 检查是否存在（仅检查内存）
bool exists = await CacheUtil.ExistsAsync("user:1001", checkPersist: false);

// 删除缓存（仅删除内存）
await CacheUtil.RemoveAsync("user:1001", deleteFromPersist: false);

// 清空所有缓存（仅清空内存）
await CacheUtil.ClearAsync(clearPersist: false);

// 清理过期缓存（仅清理内存）
await CacheUtil.CleanExpiredAsync(cleanPersist: false);
```

### 3. 自定义初始化（可选）

```csharp
// 在 Program.cs 中自定义持久化路径（可选）
CacheUtil.Initialize(new SqliteCachePersistence("Data/cache.db"));

// 或者使用自定义持久化实现
CacheUtil.Initialize(new RedisCachePersistence());

// 如果不调用 Initialize，会自动使用默认的 SQLite（cache.db）
```

### 4. 程序启动时加载持久化数据（推荐）

```csharp
// 在应用启动时加载所有持久化缓存到内存
await CacheUtil.LoadFromPersistAsync();
```

## 数据结构

每个缓存条目包含以下字段：

- `Id`: 唯一标识（自动生成）
- `Key`: 缓存键
- `Value`: 缓存值（JSON序列化）
- `ValueType`: 值类型
- `CreateTime`: 创建时间
- `UpdateTime`: 更新时间
- `CreateBy`: 创建人（可选）
- `UpdateBy`: 更新人（可选）
- `OutTime`: 过期时间（null表示不过期）

## 使用场景建议

### 持久化缓存（默认）
适用于：
- 需要程序重启后保留的配置
- 用户偏好设置
- 需要持久化的业务数据
- 大部分业务缓存场景

### 仅内存缓存（显式指定 persist: false）
适用于：
- 临时数据、会话令牌
- 高频读写的热数据
- 不需要持久化的临时计算结果

## 自定义持久化

实现 `ICachePersistence` 接口可以支持其他存储方式：

```csharp
public class RedisCachePersistence : ICachePersistence
{
    public async Task SaveAsync(CacheEntry entry)
    {
        // 实现 Redis 存储逻辑
    }
    
    // 实现其他接口方法...
}

// 初始化时使用自定义实现
CacheUtil.Initialize(new RedisCachePersistence());
```

## 注意事项

1. 静态类，全局共享缓存实例
2. 内存缓存使用 `ConcurrentDictionary` 保证线程安全
3. 默认所有操作都会持久化到 SQLite，如不需要持久化请显式指定 `persist: false`
4. 持久化操作是异步的，不会阻塞内存操作
5. 过期缓存需要手动调用 `CleanExpiredAsync()` 清理
6. SQLite 持久化实例会在应用生命周期内保持连接
7. 建议在应用启动时调用 `LoadFromPersistAsync()` 加载持久化数据到内存
8. 如果不调用 `Initialize()`，会自动使用默认 SQLite 持久化（cache.db）
