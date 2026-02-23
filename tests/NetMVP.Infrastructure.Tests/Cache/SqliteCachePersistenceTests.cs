using FluentAssertions;
using NetMVP.Infrastructure.Cache;

namespace NetMVP.Infrastructure.Tests.Cache
{
    public class SqliteCachePersistenceTests : IDisposable
    {
        private readonly string _testDbPath;
        private readonly SqliteCachePersistence _persistence;

        public SqliteCachePersistenceTests()
        {
            _testDbPath = Path.Combine(Path.GetTempPath(), $"test_persistence_{Guid.NewGuid()}.db");
            _persistence = new SqliteCachePersistence(_testDbPath);
        }

        [Fact]
        public async Task SaveAsync_ShouldPersistEntry()
        {
            // Arrange
            var entry = new CacheEntry
            {
                Key = "test:key",
                Value = "test value",
                ValueType = "System.String",
                CreateBy = "admin"
            };

            // Act
            await _persistence.SaveAsync(entry);

            // Assert
            var retrieved = await _persistence.GetAsync("test:key");
            retrieved.Should().NotBeNull();
            retrieved!.Key.Should().Be(entry.Key);
            retrieved.Value.Should().Be(entry.Value);
            retrieved.CreateBy.Should().Be(entry.CreateBy);
        }

        [Fact]
        public async Task SaveAsync_UpdateExisting_ShouldReplaceEntry()
        {
            // Arrange
            var entry1 = new CacheEntry
            {
                Key = "update:key",
                Value = "value1",
                ValueType = "System.String"
            };
            await _persistence.SaveAsync(entry1);

            var entry2 = new CacheEntry
            {
                Key = "update:key",
                Value = "value2",
                ValueType = "System.String"
            };

            // Act
            await _persistence.SaveAsync(entry2);

            // Assert
            var retrieved = await _persistence.GetAsync("update:key");
            retrieved.Should().NotBeNull();
            retrieved!.Value.Should().Be("value2");
        }

        [Fact]
        public async Task GetAsync_NonExistentKey_ShouldReturnNull()
        {
            // Act
            var result = await _persistence.GetAsync("nonexistent:key");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntry()
        {
            // Arrange
            var entry = new CacheEntry
            {
                Key = "delete:key",
                Value = "value",
                ValueType = "System.String"
            };
            await _persistence.SaveAsync(entry);

            // Act
            var deleted = await _persistence.DeleteAsync("delete:key");

            // Assert
            deleted.Should().BeTrue();
            var retrieved = await _persistence.GetAsync("delete:key");
            retrieved.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_NonExistentKey_ShouldReturnFalse()
        {
            // Act
            var deleted = await _persistence.DeleteAsync("nonexistent:key");

            // Assert
            deleted.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntries()
        {
            // Arrange
            await _persistence.SaveAsync(new CacheEntry { Key = "key1", Value = "value1", ValueType = "System.String" });
            await _persistence.SaveAsync(new CacheEntry { Key = "key2", Value = "value2", ValueType = "System.String" });
            await _persistence.SaveAsync(new CacheEntry { Key = "key3", Value = "value3", ValueType = "System.String" });

            // Act
            var entries = await _persistence.GetAllAsync();

            // Assert
            entries.Should().HaveCount(3);
            entries.Select(e => e.Key).Should().Contain(new[] { "key1", "key2", "key3" });
        }

        [Fact]
        public async Task ClearAsync_ShouldRemoveAllEntries()
        {
            // Arrange
            await _persistence.SaveAsync(new CacheEntry { Key = "key1", Value = "value1", ValueType = "System.String" });
            await _persistence.SaveAsync(new CacheEntry { Key = "key2", Value = "value2", ValueType = "System.String" });

            // Act
            await _persistence.ClearAsync();

            // Assert
            var entries = await _persistence.GetAllAsync();
            entries.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteExpiredAsync_ShouldRemoveOnlyExpiredEntries()
        {
            // Arrange
            var validEntry = new CacheEntry
            {
                Key = "valid:key",
                Value = "valid",
                ValueType = "System.String",
                OutTime = DateTime.Now.AddHours(1)
            };

            var expiredEntry = new CacheEntry
            {
                Key = "expired:key",
                Value = "expired",
                ValueType = "System.String",
                OutTime = DateTime.Now.AddHours(-1)
            };

            await _persistence.SaveAsync(validEntry);
            await _persistence.SaveAsync(expiredEntry);

            // Act
            await _persistence.DeleteExpiredAsync();

            // Assert
            var validExists = await _persistence.GetAsync("valid:key");
            var expiredExists = await _persistence.GetAsync("expired:key");

            validExists.Should().NotBeNull();
            expiredExists.Should().BeNull();
        }

        [Fact]
        public async Task SaveAsync_WithNullOutTime_ShouldPersistCorrectly()
        {
            // Arrange
            var entry = new CacheEntry
            {
                Key = "no-expiry:key",
                Value = "value",
                ValueType = "System.String",
                OutTime = null
            };

            // Act
            await _persistence.SaveAsync(entry);

            // Assert
            var retrieved = await _persistence.GetAsync("no-expiry:key");
            retrieved.Should().NotBeNull();
            retrieved!.OutTime.Should().BeNull();
        }

        [Fact]
        public async Task SaveAsync_WithAuditFields_ShouldPersistCorrectly()
        {
            // Arrange
            var entry = new CacheEntry
            {
                Key = "audit:key",
                Value = "value",
                ValueType = "System.String",
                CreateBy = "user1",
                UpdateBy = "user2",
                CreateTime = DateTime.Now.AddDays(-1),
                UpdateTime = DateTime.Now
            };

            // Act
            await _persistence.SaveAsync(entry);

            // Assert
            var retrieved = await _persistence.GetAsync("audit:key");
            retrieved.Should().NotBeNull();
            retrieved!.CreateBy.Should().Be("user1");
            retrieved.UpdateBy.Should().Be("user2");
            retrieved.CreateTime.Should().BeCloseTo(entry.CreateTime, TimeSpan.FromSeconds(1));
            retrieved.UpdateTime.Should().BeCloseTo(entry.UpdateTime, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Constructor_WithNestedPath_ShouldCreateDirectory()
        {
            // Arrange
            var nestedPath = Path.Combine(Path.GetTempPath(), $"test_nested_{Guid.NewGuid()}", "cache.db");

            // Act
            using var persistence = new SqliteCachePersistence(nestedPath);

            // Assert
            File.Exists(nestedPath).Should().BeTrue();

            // Cleanup
            var directory = Path.GetDirectoryName(nestedPath);
            if (directory != null && Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }

        public void Dispose()
        {
            _persistence?.Dispose();
            if (File.Exists(_testDbPath))
            {
                File.Delete(_testDbPath);
            }
        }
    }
}
