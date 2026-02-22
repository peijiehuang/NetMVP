using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace NetMVP.Infrastructure.Services;

/// <summary>
/// 数据库初始化服务
/// </summary>
public class DatabaseInitializationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseInitializationService> _logger;

    public DatabaseInitializationService(
        IConfiguration configuration,
        ILogger<DatabaseInitializationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 初始化数据库
    /// </summary>
    public async Task InitializeAsync()
    {
        var enabled = _configuration.GetValue<bool>("DatabaseInitialization:Enabled");
        if (!enabled)
        {
            _logger.LogInformation("数据库自动初始化已禁用");
            return;
        }

        _logger.LogInformation("开始检查数据库初始化状态...");

        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogError("未找到数据库连接字符串");
            return;
        }

        try
        {
            // 解析连接字符串获取数据库名
            var builder = new MySqlConnectionStringBuilder(connectionString);
            var databaseName = builder.Database;

            // 检查数据库是否存在
            var dbExists = await CheckDatabaseExistsAsync(builder, databaseName);

            if (!dbExists)
            {
                _logger.LogInformation($"数据库 {databaseName} 不存在，开始创建并初始化...");
                await CreateAndInitializeDatabaseAsync(builder, databaseName);
                _logger.LogInformation("数据库初始化完成！");
            }
            else
            {
                // 检查是否有表
                var hasData = await CheckDatabaseHasDataAsync(connectionString);
                if (!hasData)
                {
                    _logger.LogInformation("数据库存在但无数据，开始初始化...");
                    await InitializeDatabaseDataAsync(connectionString, databaseName);
                    _logger.LogInformation("数据库初始化完成！");
                }
                else
                {
                    _logger.LogInformation("数据库已存在且包含数据，跳过初始化");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据库初始化失败");
            throw;
        }
    }

    /// <summary>
    /// 检查数据库是否存在
    /// </summary>
    private async Task<bool> CheckDatabaseExistsAsync(MySqlConnectionStringBuilder builder, string databaseName)
    {
        // 连接到MySQL服务器（不指定数据库）
        var serverConnectionString = new MySqlConnectionStringBuilder
        {
            Server = builder.Server,
            Port = builder.Port,
            UserID = builder.UserID,
            Password = builder.Password
        }.ConnectionString;

        await using var connection = new MySqlConnection(serverConnectionString);
        await connection.OpenAsync();

        var sql = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{databaseName}'";
        await using var command = new MySqlCommand(sql, connection);
        var result = await command.ExecuteScalarAsync();

        return result != null;
    }

    /// <summary>
    /// 检查数据库是否有数据
    /// </summary>
    private async Task<bool> CheckDatabaseHasDataAsync(string connectionString)
    {
        try
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            // 检查sys_user表是否存在且有数据
            var sql = "SELECT COUNT(*) FROM sys_user LIMIT 1";
            await using var command = new MySqlCommand(sql, connection);
            var count = Convert.ToInt32(await command.ExecuteScalarAsync());

            return count > 0;
        }
        catch
        {
            // 如果查询失败，说明表不存在
            return false;
        }
    }

    /// <summary>
    /// 创建并初始化数据库
    /// </summary>
    private async Task CreateAndInitializeDatabaseAsync(MySqlConnectionStringBuilder builder, string databaseName)
    {
        // 连接到MySQL服务器（不指定数据库）
        var serverConnectionString = new MySqlConnectionStringBuilder
        {
            Server = builder.Server,
            Port = builder.Port,
            UserID = builder.UserID,
            Password = builder.Password
        }.ConnectionString;

        await using var connection = new MySqlConnection(serverConnectionString);
        await connection.OpenAsync();

        // 创建数据库
        var createDbSql = $"CREATE DATABASE `{databaseName}` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci";
        await using var createCommand = new MySqlCommand(createDbSql, connection);
        await createCommand.ExecuteNonQueryAsync();

        _logger.LogInformation($"数据库 {databaseName} 创建成功");

        // 切换到新创建的数据库
        await connection.ChangeDatabaseAsync(databaseName);

        // 执行SQL文件
        await ExecuteSqlFilesAsync(connection);
    }

    /// <summary>
    /// 初始化数据库数据
    /// </summary>
    private async Task InitializeDatabaseDataAsync(string connectionString, string databaseName)
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        // 执行SQL文件
        await ExecuteSqlFilesAsync(connection);
    }

    /// <summary>
    /// 执行SQL文件
    /// </summary>
    private async Task ExecuteSqlFilesAsync(MySqlConnection connection)
    {
        var sqlFiles = _configuration.GetSection("DatabaseInitialization:SqlFilePaths").Get<string[]>();
        if (sqlFiles == null || sqlFiles.Length == 0)
        {
            _logger.LogWarning("未配置SQL文件路径");
            return;
        }

        foreach (var sqlFile in sqlFiles)
        {
            if (!File.Exists(sqlFile))
            {
                _logger.LogWarning($"SQL文件不存在: {sqlFile}");
                continue;
            }

            _logger.LogInformation($"执行SQL文件: {sqlFile}");

            var sqlContent = await File.ReadAllTextAsync(sqlFile);
            
            // 分割SQL语句（按分号分割，但要处理存储过程等特殊情况）
            var statements = SplitSqlStatements(sqlContent);

            foreach (var statement in statements)
            {
                if (string.IsNullOrWhiteSpace(statement))
                    continue;

                try
                {
                    await using var command = new MySqlCommand(statement, connection);
                    command.CommandTimeout = 300; // 5分钟超时
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"执行SQL语句失败: {ex.Message}");
                    // 继续执行其他语句
                }
            }

            _logger.LogInformation($"SQL文件执行完成: {sqlFile}");
        }
    }

    /// <summary>
    /// 分割SQL语句
    /// </summary>
    private List<string> SplitSqlStatements(string sqlContent)
    {
        var statements = new List<string>();
        var currentStatement = new System.Text.StringBuilder();
        var inString = false;
        var stringChar = '\0';
        var inComment = false;
        var inMultiLineComment = false;

        for (int i = 0; i < sqlContent.Length; i++)
        {
            var c = sqlContent[i];
            var nextChar = i < sqlContent.Length - 1 ? sqlContent[i + 1] : '\0';

            // 处理多行注释
            if (!inString && c == '/' && nextChar == '*')
            {
                inMultiLineComment = true;
                i++;
                continue;
            }

            if (inMultiLineComment && c == '*' && nextChar == '/')
            {
                inMultiLineComment = false;
                i++;
                continue;
            }

            if (inMultiLineComment)
                continue;

            // 处理单行注释
            if (!inString && c == '-' && nextChar == '-')
            {
                inComment = true;
                continue;
            }

            if (inComment && c == '\n')
            {
                inComment = false;
                continue;
            }

            if (inComment)
                continue;

            // 处理字符串
            if ((c == '\'' || c == '"') && !inString)
            {
                inString = true;
                stringChar = c;
                currentStatement.Append(c);
                continue;
            }

            if (c == stringChar && inString && (i == 0 || sqlContent[i - 1] != '\\'))
            {
                inString = false;
                currentStatement.Append(c);
                continue;
            }

            // 处理分号
            if (c == ';' && !inString)
            {
                var statement = currentStatement.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(statement))
                {
                    statements.Add(statement);
                }
                currentStatement.Clear();
                continue;
            }

            currentStatement.Append(c);
        }

        // 添加最后一个语句
        var lastStatement = currentStatement.ToString().Trim();
        if (!string.IsNullOrWhiteSpace(lastStatement))
        {
            statements.Add(lastStatement);
        }

        return statements;
    }
}
