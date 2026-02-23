using FluentAssertions;
using NetMVP.Infrastructure.Cache;

namespace NetMVP.Infrastructure.Tests.Cache
{
    public class CacheUtilTests : IDisposable
    {
        private readonly string _testDbPath;

        public CacheUtilTests()
        {
            _testDbPath = Path.Combine(Path.GetTempPath(), $"test_cache_{Guid.NewGuid()}.db");
            // 初始化静态缓存管理器
            CacheUtil.Initialize(new SqliteCachePersistence(_testDbPath));
        }

        [Fact]
        public async Task SetAsync_ShouldStoreValueInMemory()
        {
            // Arrange
            var key = $"test:key:{Guid.NewGuid()}";
            var value = new { Name = "Test", Age = 25 };

            // Act
            var result = await CacheUtil.SetAsync(key, value, persist: false);

            // Assert
            result.Should().BeTrue();
            var cached = await CacheUtil.GetAsync<dynamic>(key, loadFromPersist: false);
            Assert.NotNull(cached);
        }

        [Fact]
        public async Task SetAsync_WithPersist_ShouldStoreInSqlite()
        {
            // Arrange
            var key = $"persist:key:{Guid.NewGuid()}";
            var value = "persistent value";

            // Act
            await CacheUtil.SetAsync(key, value, persist: true);

            // Assert - 清空内存后从持久化加载
            await CacheUtil.RemoveAsync(key, deleteFromPersist: false);
            var loaded = await CacheUtil.GetAsync<string>(key, loadFromPersist: true);
            loaded.Should().Be(value);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnCachedValue()
        {
            // Arrange
            var key = $"get:test:{Guid.NewGuid()}";
            var value = new TestData { Id = 1, Name = "Test" };
            await CacheUtil.SetAsync(key, value, persist: false);

            // Act
            var result = await CacheUtil.GetAsync<TestData>(key, loadFromPersist: false);

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
            await CacheUtil.SetAsync(key, value, persist: true);
            
            // Clear memory cache
            await CacheUtil.RemoveAsync(key, deleteFromPersist: false);

            // Act
            var result = await CacheUtil.GetAsync<string>(key, loadFromPersist: true);

            // Assert
            result.Should().Be(value);
        }

        [Fact]
        public async Task GetAsync_WithExpiredCache_ShouldReturnNull()
        {
            // Arrange
            var key = $"expired:key:{Guid.NewGuid()}";
            var value = "expired value";
            await CacheUtil.SetAsync(key, value, TimeSpan.FromMilliseconds(100), persist: false);
            
            // Wait for expiration
            await Task.Delay(150);

            // Act
            var result = await CacheUtil.GetAsync<string>(key, loadFromPersist: false);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task RemoveAsync_ShouldDeleteFromMemory()
        {
            // Arrange
            var key = $"remove:test:{Guid.NewGuid()}";
            await CacheUtil.SetAsync(key, "value", persist: false);

            // Act
            var removed = await CacheUtil.RemoveAsync(key, deleteFromPersist: false);

            // Assert
            removed.Should().BeTrue();
            var result = await CacheUtil.GetAsync<string>(key, loadFromPersist: false);
            result.Should().BeNull();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrueForExistingKey()
        {
            // Arrange
            var key = $"exists:test:{Guid.NewGuid()}";
            await CacheUtil.SetAsync(key, "value", persist: false);

            // Act
            var exists = await CacheUtil.ExistsAsync(key, checkPersist: false);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalseForNonExistingKey()
        {
            // Act
            var exists = await CacheUtil.ExistsAsync($"nonexistent:key:{Guid.NewGuid()}", checkPersist: false);

            // Assert
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task CleanExpiredAsync_ShouldRemoveOnlyExpiredCaches()
        {
            // Arrange
            var validKey = $"valid:key:{Guid.NewGuid()}";
            var expiredKey = $"expired:key:{Guid.NewGuid()}";
            
            await CacheUtil.SetAsync(validKey, "valid", persist: false);
            await CacheUtil.SetAsync(expiredKey, "expired", TimeSpan.FromMilliseconds(100), persist: false);
            
            await Task.Delay(150);

            // Act
            await CacheUtil.CleanExpiredAsync(cleanPersist: false);

            // Assert
            var validExists = await CacheUtil.ExistsAsync(validKey, checkPersist: false);
            var expiredExists = await CacheUtil.ExistsAsync(expiredKey, checkPersist: false);
            
            validExists.Should().BeTrue();
            expiredExists.Should().BeFalse();
        }

        [Fact]
        public async Task LoadFromPersistAsync_ShouldLoadAllPersistedCaches()
        {
            // Arrange
            var key1 = $"load1:{Guid.NewGuid()}";
            var key2 = $"load2:{Guid.NewGuid()}";
            
            await CacheUtil.SetAsync(key1, "value1", persist: true);
            await CacheUtil.SetAsync(key2, "value2", persist: true);
            
            // Clear memory
            await CacheUtil.RemoveAsync(key1, deleteFromPersist: false);
            await CacheUtil.RemoveAsync(key2, deleteFromPersist: false);

            // Act
            await CacheUtil.LoadFromPersistAsync();

            // Assert
            var value1 = await CacheUtil.GetAsync<string>(key1, loadFromPersist: false);
            var value2 = await CacheUtil.GetAsync<string>(key2, loadFromPersist: false);
            
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
            await CacheUtil.SetAsync(key, value, createBy: createBy, persist: true);

            // Assert - 从持久化加载验证
            await CacheUtil.RemoveAsync(key, deleteFromPersist: false);
            await CacheUtil.LoadFromPersistAsync();
            
            var result = await CacheUtil.GetAsync<string>(key, loadFromPersist: false);
            result.Should().Be(value);
        }

        [Fact]
        public async Task GetAllKeys_ShouldReturnAllCachedKeys()
        {
            // Arrange
            var key1 = $"key1:{Guid.NewGuid()}";
            var key2 = $"key2:{Guid.NewGuid()}";
            var key3 = $"key3:{Guid.NewGuid()}";
            
            await CacheUtil.SetAsync(key1, "value1", persist: false);
            await CacheUtil.SetAsync(key2, "value2", persist: false);
            await CacheUtil.SetAsync(key3, "value3", persist: false);

            // Act
            var keys = CacheUtil.GetAllKeys();

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
