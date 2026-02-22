using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.Hosting;
using NetMVP.Domain.Interfaces;
using Scriban;

namespace NetMVP.Application.Services.Gen;

/// <summary>
/// 代码生成服务实现
/// </summary>
public class CodeGeneratorService : ICodeGeneratorService
{
    private readonly IGenTableService _genTableService;
    private readonly IHostEnvironment _environment;
    private readonly string _templatePath;

    public CodeGeneratorService(
        IGenTableService genTableService,
        IHostEnvironment environment)
    {
        _genTableService = genTableService;
        _environment = environment;
        _templatePath = Path.Combine(AppContext.BaseDirectory, "Templates");
    }

    public async Task<Dictionary<string, string>> PreviewCodeAsync(long tableId, CancellationToken cancellationToken = default)
    {
        var table = await _genTableService.GetTableByIdAsync(tableId, cancellationToken);
        if (table == null)
        {
            throw new Exception("表不存在");
        }

        var result = new Dictionary<string, string>();
        var context = BuildTemplateContext(table);

        // 生成各个文件
        result[$"Domain/Entities/{table.ClassName}.cs"] = await RenderTemplateAsync("Entity.sbn", context);
        result[$"Application/DTOs/{table.ModuleName}/{table.ClassName}Dto.cs"] = await RenderTemplateAsync("Dto.sbn", context);
        result[$"Domain/Interfaces/I{table.ClassName}Repository.cs"] = await RenderTemplateAsync("Repository.sbn", context);
        result[$"Infrastructure/Repositories/{table.ClassName}Repository.cs"] = await RenderTemplateAsync("RepositoryImpl.sbn", context);
        result[$"Application/Services/I{table.ClassName}Service.cs"] = await RenderTemplateAsync("Service.sbn", context);
        result[$"Application/Services/Impl/{table.ClassName}Service.cs"] = await RenderTemplateAsync("ServiceImpl.sbn", context);
        result[$"WebApi/Controllers/{table.ModuleName}/{table.ClassName}Controller.cs"] = await RenderTemplateAsync("Controller.sbn", context);
        result[$"Infrastructure/Data/Configurations/{table.ClassName}Configuration.cs"] = await RenderTemplateAsync("EntityConfiguration.sbn", context);

        return result;
    }

    public async Task<byte[]> GenerateCodeAsync(string tableName, CancellationToken cancellationToken = default)
    {
        var table = await _genTableService.GetTableByNameAsync(tableName, cancellationToken);
        if (table == null)
        {
            throw new Exception("表不存在");
        }

        return await GenerateZipAsync(new[] { table }, cancellationToken);
    }

    public async Task<byte[]> BatchGenerateCodeAsync(string[] tableNames, CancellationToken cancellationToken = default)
    {
        var tables = new List<dynamic>();
        foreach (var tableName in tableNames)
        {
            var table = await _genTableService.GetTableByNameAsync(tableName, cancellationToken);
            if (table != null)
            {
                tables.Add(table);
            }
        }

        if (tables.Count == 0)
        {
            throw new Exception("没有找到要生成的表");
        }

        return await GenerateZipAsync(tables.ToArray(), cancellationToken);
    }

    public async Task<byte[]> DownloadCodeAsync(string tableName, CancellationToken cancellationToken = default)
    {
        return await GenerateCodeAsync(tableName, cancellationToken);
    }

    private async Task<byte[]> GenerateZipAsync(dynamic[] tables, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var table in tables)
            {
                var context = BuildTemplateContext(table);

                // 生成各个文件并添加到ZIP
                await AddFileToZipAsync(archive, $"Domain/Entities/{table.ClassName}.cs", "Entity.sbn", context);
                await AddFileToZipAsync(archive, $"Application/DTOs/{table.ModuleName}/{table.ClassName}Dto.cs", "Dto.sbn", context);
                await AddFileToZipAsync(archive, $"Domain/Interfaces/I{table.ClassName}Repository.cs", "Repository.sbn", context);
                await AddFileToZipAsync(archive, $"Infrastructure/Repositories/{table.ClassName}Repository.cs", "RepositoryImpl.sbn", context);
                await AddFileToZipAsync(archive, $"Application/Services/I{table.ClassName}Service.cs", "Service.sbn", context);
                await AddFileToZipAsync(archive, $"Application/Services/Impl/{table.ClassName}Service.cs", "ServiceImpl.sbn", context);
                await AddFileToZipAsync(archive, $"WebApi/Controllers/{table.ModuleName}/{table.ClassName}Controller.cs", "Controller.sbn", context);
                await AddFileToZipAsync(archive, $"Infrastructure/Data/Configurations/{table.ClassName}Configuration.cs", "EntityConfiguration.sbn", context);
            }
        }

        return memoryStream.ToArray();
    }

    private async Task AddFileToZipAsync(ZipArchive archive, string entryName, string templateName, object context)
    {
        var content = await RenderTemplateAsync(templateName, context);
        var entry = archive.CreateEntry(entryName);
        using var entryStream = entry.Open();
        using var writer = new StreamWriter(entryStream, Encoding.UTF8);
        await writer.WriteAsync(content);
    }

    private async Task<string> RenderTemplateAsync(string templateName, object context)
    {
        var templatePath = Path.Combine(_templatePath, templateName);
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"模板文件不存在: {templatePath}");
        }

        var templateContent = await File.ReadAllTextAsync(templatePath);
        var template = Template.Parse(templateContent);
        return await template.RenderAsync(context);
    }

    private object BuildTemplateContext(dynamic table)
    {
        // 获取主键列
        string pkColumnName = "Id";
        if (table.Columns != null)
        {
            foreach (var c in table.Columns)
            {
                if (c.IsPk)
                {
                    pkColumnName = c.CsharpField;
                    break;
                }
            }
        }

        return new
        {
            table_name = table.TableName,
            class_name = table.ClassName,
            class_comment = table.TableComment,
            module_name = table.ModuleName,
            business_name = table.BusinessName,
            function_name = table.FunctionName,
            author = table.FunctionAuthor ?? "NetMVP",
            datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            columns = table.Columns,
            pk_column = pkColumnName
        };
    }
}
