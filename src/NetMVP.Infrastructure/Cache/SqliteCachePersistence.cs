using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace NetMVP.Infrastructure.Cache
{
    /// <summary>
    /// SQLite 缓存持久化实现
    /// </summary>
    public class SqliteCachePersistence : ICachePersistence, IDisposable
    {
        private readonly string _connectionString;
        private readonly SqliteConnection _connection;

        public SqliteCachePersistence(string dbPath = "cache.db")
        {
            // 确保数据库文件所在目录存在
            var directory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 如果是相对路径，转换为绝对路径
            if (!Path.IsPathRooted(dbPath))
            {
                dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbPath);
            }

            _connectionString = $"Data Source={dbPath}";
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            var createTableSql = @"
                CREATE TABLE IF NOT EXISTS CacheEntries (
                    Id TEXT PRIMARY KEY,
                    Key TEXT NOT NULL UNIQUE,
                    Value TEXT NOT NULL,
                    ValueType TEXT NOT NULL,
                    CreateTime TEXT NOT NULL,
                    UpdateTime TEXT NOT NULL,
                    CreateBy TEXT,
                    UpdateBy TEXT,
                    OutTime TEXT
                );
                CREATE INDEX IF NOT EXISTS idx_key ON CacheEntries(Key);
                CREATE INDEX IF NOT EXISTS idx_outtime ON CacheEntries(OutTime);
            ";

            using var command = new SqliteCommand(createTableSql, _connection);
            command.ExecuteNonQuery();
        }

        public async Task SaveAsync(CacheEntry entry)
        {
            var sql = @"
                INSERT OR REPLACE INTO CacheEntries 
                (Id, Key, Value, ValueType, CreateTime, UpdateTime, CreateBy, UpdateBy, OutTime)
                VALUES 
                (@Id, @Key, @Value, @ValueType, @CreateTime, @UpdateTime, @CreateBy, @UpdateBy, @OutTime)
            ";

            using var command = new SqliteCommand(sql, _connection);
            command.Parameters.AddWithValue("@Id", entry.Id);
            command.Parameters.AddWithValue("@Key", entry.Key);
            command.Parameters.AddWithValue("@Value", entry.Value);
            command.Parameters.AddWithValue("@ValueType", entry.ValueType);
            command.Parameters.AddWithValue("@CreateTime", entry.CreateTime.ToString("O"));
            command.Parameters.AddWithValue("@UpdateTime", entry.UpdateTime.ToString("O"));
            command.Parameters.AddWithValue("@CreateBy", (object?)entry.CreateBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdateBy", (object?)entry.UpdateBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@OutTime", entry.OutTime?.ToString("O") ?? (object)DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<CacheEntry?> GetAsync(string key)
        {
            var sql = "SELECT * FROM CacheEntries WHERE Key = @Key";

            using var command = new SqliteCommand(sql, _connection);
            command.Parameters.AddWithValue("@Key", key);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToEntry(reader);
            }

            return null;
        }

        public async Task<bool> DeleteAsync(string key)
        {
            var sql = "DELETE FROM CacheEntries WHERE Key = @Key";

            using var command = new SqliteCommand(sql, _connection);
            command.Parameters.AddWithValue("@Key", key);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<List<CacheEntry>> GetAllAsync()
        {
            var entries = new List<CacheEntry>();
            var sql = "SELECT * FROM CacheEntries";

            using var command = new SqliteCommand(sql, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                entries.Add(MapToEntry(reader));
            }

            return entries;
        }

        public async Task ClearAsync()
        {
            var sql = "DELETE FROM CacheEntries";

            using var command = new SqliteCommand(sql, _connection);
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteExpiredAsync()
        {
            var sql = "DELETE FROM CacheEntries WHERE OutTime IS NOT NULL AND OutTime < @Now";

            using var command = new SqliteCommand(sql, _connection);
            command.Parameters.AddWithValue("@Now", DateTime.Now.ToString("O"));

            await command.ExecuteNonQueryAsync();
        }

        private CacheEntry MapToEntry(SqliteDataReader reader)
        {
            return new CacheEntry
            {
                Id = reader.GetString(reader.GetOrdinal("Id")),
                Key = reader.GetString(reader.GetOrdinal("Key")),
                Value = reader.GetString(reader.GetOrdinal("Value")),
                ValueType = reader.GetString(reader.GetOrdinal("ValueType")),
                CreateTime = DateTime.Parse(reader.GetString(reader.GetOrdinal("CreateTime"))),
                UpdateTime = DateTime.Parse(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                CreateBy = reader.IsDBNull(reader.GetOrdinal("CreateBy")) ? null : reader.GetString(reader.GetOrdinal("CreateBy")),
                UpdateBy = reader.IsDBNull(reader.GetOrdinal("UpdateBy")) ? null : reader.GetString(reader.GetOrdinal("UpdateBy")),
                OutTime = reader.IsDBNull(reader.GetOrdinal("OutTime")) ? null : DateTime.Parse(reader.GetString(reader.GetOrdinal("OutTime")))
            };
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
