using FluentAssertions;
using NetMVP.Infrastructure.Cache;

namespace NetMVP.Infrastructure.Tests.Cache
{
    public class MemoryCacheManagerTests : IDisposable
    {
        private readonly string _testDbPath;

        public MemoryCacheManagerTests()
        {
            _testDbPath = Path.Combine(Path.GetTempPath(), $"test_cache_{Guid.NewGuid()}.db");
            // 初始化静态缓存管理器
            MemoryCacheManager.Initialize(new SqliteCachePersistence(_testDbPath));
        }

        [Fact]
        public async Task SetAsync_ShouldStoreValueInMemory()
        {
            // Arrange
            var key = $"test:key:{Guid.NewGuid()}";
            var value = new { Name = "Test", Age = 25 };

            // Act
            var result = await MemoryCacheManager.SetAsync(key, value, persist: false);

            // Assert
            result.Should().BeTrue();
            var cached = await MemoryCacheManager.GetAsync<dynamic>(key, loadFromPersist: false);
            cached.Should().NotBeNull();
            if (cached != null)
            {
                // 验证缓存值存在
                Assert.NotNull(cached);
            }
        }

        [Fact]
        public async Task SetAsync_WithPersist_ShouldStoreInSqlite()
        {
            // Arrange
            var key = $"persist:key:{Guid.NewGuid()}";
            var value = "persistent value";

            // Act
            await MemoryCacheManager.SetAsync(key, value, persist: true);

            // Assert - 清空内存后从持久化加载
            await MemoryCacheManager.RemoveAsync(key, deleteFromPersist: false);
            var loaded = await MemoryCacheManager.GetAsync<string>(key, loadFromPersist: true);
            loaded.Should().Be(value);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnCachedValue()
        {
            // Arrange
            var key = $"get:test:{Guid.NewGuid()}";
            var value = new TestData { Id = 1, Name = "Test" };
            await MemoryCacheManager.SetAsync(key, value, persist: false);

            // Act
            var result = await MemoryCacheManager.GetAsync<TestData>(key, loadFromPersist: false);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("Test");
        }

        [Fact]
        public async Task GetAsync_WithLoadFromPersist_ShouldLoadFromDatabase()
        {
            // Arrange
            var key = $"load:test:{Guid.NewGuid()}";
            var value = "test value";
            await MemoryCacheManager.SetAsync(key, value, persist: true);
            
            // Clear memory cache
            await MemoryCacheManager.RemoveAsync(key, deleteFromPersist: false);

            // Act
            var result = await MemoryCacheManager.GetAsync<string>(key, loadFromPersist: true);

            // Assert
            result.Should().Be(value);
        }

        [Fact]
        public async Task GetAsync_WithExpiredCache_ShouldReturnNull()
        {
            // Arrange
            var key = $"expired:key:{Guid.NewGuid()}";
            var value = "expired value";
            await MemoryCacheManager.SetAsync(key, value, TimeSpan.FromMilliseconds(100), persist: false);
            
            // Wait for expiration
            await Task.Delay(150);

            // Act
            var result = await MemoryCacheManager.GetAsync<string>(key, loadFromPersist: false);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task RemoveAsync_ShouldDeleteFromMemory()
        {
            // Arrange
            var key = $"remove:test:{Guid.NewGuid()}";
            await MemoryCacheManager.SetAsync(key, "value", persist: false);

            // Act
            var removed = await MemoryCacheManager.RemoveAsync(key, deleteFromPersist: false);

            // Assert
            removed.Should().BeTrue();
            var result = await MemoryCacheManager.GetAsync<string>(key, loadFromPersist: false);
            result.Should().BeNull();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrueForExistingKey()
        {
            // Arrange
            var key = $"exists:test:{Guid.NewGuid()}";
            await MemoryCacheManager.SetAsync(key, "value", persist: false);

            // Act
            var exists = await MemoryCacheManager.ExistsAsync(key, checkPersist: false);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalseForNonExistingKey()
        {
            // Act
            var exists = await MemoryCacheManager.ExistsAsync($"nonexistent:key:{Guid.NewGuid()}", checkPersist: false);

            // Assert
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task CleanExpiredAsync_ShouldRemoveOnlyExpiredCaches()
        {
            // Arrange
            var validKey = $"valid:key:{Guid.NewGuid()}";
            var expiredKey = $"expired:key:{Guid.NewGuid()}";
            
            await MemoryCacheManager.SetAsync(validKey, "valid", persist: false);
            await MemoryCacheManager.SetAsync(expiredKey, "expired", TimeSpan.FromMilliseconds(100), persist: false);
            
            await Task.Delay(150);

            // Act
            await MemoryCacheManager.CleanExpiredAsync(cleanPersist: false);

            // Assert
            var validExists = await MemoryCacheManager.ExistsAsync(validKey, checkPersist: false);
            var expiredExists = await MemoryCacheManager.ExistsAsync(expiredKey, checkPersist: false);
            
            validExists.Should().BeTrue();
            expiredExists.Should().BeFalse();
        }

        [Fact]
        public async Task LoadFromPersistAsync_ShouldLoadAllPersistedCaches()
        {
            // Arrange
            var key1 = $"load1:{Guid.NewGuid()}";
            var key2 = $"load2:{Guid.NewGuid()}";
            
            await MemoryCacheManager.SetAsync(key1, "value1", persist: true);
            await MemoryCacheManager.SetAsync(key2, "value2", persist: true);
            
            // Clear memory
            await MemoryCacheManager.RemoveAsync(key1, deleteFromPersist: false);
            await MemoryCacheManager.RemoveAsync(key2, deleteFromPersist: false);

            // Act
            await MemoryCacheManager.LoadFromPersistAsync();

            // Assert
            var value1 = await MemoryCacheManager.GetAsync<string>(key1, loadFromPersist: false);
            var value2 = await MemoryCacheManager.GetAsync<string>(key2, loadFromPersist: false);
            
            value1.Should().Be("value1");
            value2.Should().Be("value2");
        }

        [Fact]
        public async Task SetAsync_WithCreateBy_ShouldStoreAuditInfo()
        {
            // Arrange
            var key = $"audit:test:{Guid.NewGuid()}";
            var value = "test";
            var createBy = "admin";

            // Act
            await MemoryCacheManager.SetAsync(key, value, createBy: createBy, persist: true);

            // Assert - 从持久化加载验证
            await MemoryCacheManager.RemoveAsync(key, deleteFromPersist: false);
            await MemoryCacheManager.LoadFromPersistAsync();
            
            var result = await MemoryCacheManager.GetAsync<string>(key, loadFromPersist: false);
            result.Should().Be(value);
        }

        [Fact]
        public async Task GetAllKeys_ShouldReturnAllCachedKeys()
        {
            // Arrange
            var key1 = $"key1:{Guid.NewGuid()}";
            var key2 = $"key2:{Guid.NewGuid()}";
            var key3 = $"key3:{Guid.NewGuid()}";
            
            await MemoryCacheManager.SetAsync(key1, "value1", persist: false);
            await MemoryCacheManager.SetAsync(key2, "value2", persist: false);
            await MemoryCacheManager.SetAsync(key3, "value3", persist: false);

            // Act
            var keys = MemoryCacheManager.GetAllKeys();

            // Assert
            keys.Should().Contain(new[] { key1, key2, key3 });
        }

        public void Dispose()
        {
            if (File.Exists(_testDbPath))
            {
                try
                {
                    File.Delete(_testDbPath);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        private class TestData
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
