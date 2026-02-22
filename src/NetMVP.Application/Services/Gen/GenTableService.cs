using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetMVP.Application.Common.Models;
using NetMVP.Application.Common.Utils;
using NetMVP.Application.DTOs.Gen;
using NetMVP.Application.Interfaces;
using NetMVP.Domain.Entities;
using NetMVP.Domain.Interfaces;

namespace NetMVP.Application.Services.Gen;

/// <summary>
/// 代码生成表服务实现
/// </summary>
public class GenTableService : IGenTableService
{
    private readonly IGenTableRepository _genTableRepository;
    private readonly IGenTableColumnRepository _genTableColumnRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GenTableService(
        IGenTableRepository genTableRepository,
        IGenTableColumnRepository genTableColumnRepository,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _genTableRepository = genTableRepository;
        _genTableColumnRepository = genTableColumnRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// 获取生成表列表
    /// </summary>
    public async Task<PagedResult<GenTableDto>> GetGenTableListAsync(GenTableQueryDto query)
    {
        var queryable = _genTableRepository.GetQueryable();

        // 表名筛选
        if (!string.IsNullOrEmpty(query.TableName))
        {
            queryable = queryable.Where(t => EF.Functions.Like(t.TableName.ToLower(), $"%{query.TableName.ToLower()}%"));
        }

        // 表描述筛选
        if (!string.IsNullOrEmpty(query.TableComment))
        {
            queryable = queryable.Where(t => EF.Functions.Like(t.TableComment.ToLower(), $"%{query.TableComment.ToLower()}%"));
        }

        // 时间范围筛选
        if (query.BeginTime.HasValue)
        {
            queryable = queryable.Where(t => t.CreateTime >= query.BeginTime.Value);
        }

        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where(t => t.CreateTime <= query.EndTime.Value);
        }

        var total = await queryable.CountAsync();
        var items = await queryable
            .OrderByDescending(t => t.CreateTime)
            .Skip((query.PageNum - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<GenTableDto>>(items);

        return new PagedResult<GenTableDto>
        {
            Rows = dtos,
            Total = total
        };
    }

    /// <summary>
    /// 获取数据库表列表
    /// </summary>
    public async Task<PagedResult<GenTableDto>> GetDbTableListAsync(GenTableQueryDto query)
    {
        var tables = await _genTableRepository.GetDbTablesAsync(query.TableName, query.TableComment);
        
        // 时间范围筛选
        if (query.BeginTime.HasValue)
        {
            tables = tables.Where(t => t.CreateTime >= query.BeginTime.Value).ToList();
        }

        if (query.EndTime.HasValue)
        {
            tables = tables.Where(t => t.CreateTime <= query.EndTime.Value).ToList();
        }

        var total = tables.Count;
        var items = tables
            .Skip((query.PageNum - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        var dtos = _mapper.Map<List<GenTableDto>>(items);

        return new PagedResult<GenTableDto>
        {
            Rows = dtos,
            Total = total
        };
    }

    /// <summary>
    /// 根据ID获取生成表详情
    /// </summary>
    public async Task<GenTableDto?> GetGenTableByIdAsync(long tableId)
    {
        var table = await _genTableRepository.GetGenTableByIdWithColumnsAsync(tableId);
        return _mapper.Map<GenTableDto>(table);
    }

    /// <summary>
    /// 导入表
    /// </summary>
    public async Task<bool> ImportGenTableAsync(ImportTableDto dto)
    {
        var operName = _currentUserService.GetUserName() ?? "admin";
        var dbTables = await _genTableRepository.GetDbTablesByNamesAsync(dto.Tables);

        foreach (var table in dbTables)
        {
            // 初始化表信息
            GenUtils.InitTable(table, operName);

            // 保存表信息
            await _genTableRepository.AddAsync(table);

            // 获取表字段信息
            var columns = await _genTableColumnRepository.GetDbTableColumnsByNameAsync(table.TableName);

            // 初始化字段信息
            foreach (var column in columns)
            {
                GenUtils.InitColumnField(column, table);
                await _genTableColumnRepository.AddAsync(column);
            }
        }

        return true;
    }

    /// <summary>
    /// 更新生成表
    /// </summary>
    public async Task<bool> UpdateGenTableAsync(UpdateGenTableDto dto)
    {
        var operName = _currentUserService.GetUserName() ?? "admin";
        
        // 更新表信息
        var table = await _genTableRepository.GetByIdAsync(dto.TableId);
        if (table == null)
        {
            return false;
        }

        table.TableName = dto.TableName;
        table.TableComment = dto.TableComment;
        table.SubTableName = dto.SubTableName;
        table.SubTableFkName = dto.SubTableFkName;
        table.ClassName = dto.ClassName;
        table.TplCategory = dto.TplCategory;
        table.TplWebType = dto.TplWebType;
        table.PackageName = dto.PackageName;
        table.ModuleName = dto.ModuleName;
        table.BusinessName = dto.BusinessName;
        table.FunctionName = dto.FunctionName;
        table.FunctionAuthor = dto.FunctionAuthor;
        table.GenType = dto.GenType;
        table.GenPath = dto.GenPath;
        table.Options = dto.Options;
        table.Remark = dto.Remark;
        table.UpdateBy = operName;
        table.UpdateTime = DateTime.Now;

        await _genTableRepository.UpdateAsync(table);

        // 更新字段信息
        foreach (var columnDto in dto.Columns)
        {
            var column = await _genTableColumnRepository.GetByIdAsync(columnDto.ColumnId);
            if (column != null)
            {
                column.ColumnComment = columnDto.ColumnComment;
                column.CSharpType = columnDto.CSharpType;
                column.CSharpField = columnDto.CSharpField;
                column.IsInsert = columnDto.IsInsert;
                column.IsEdit = columnDto.IsEdit;
                column.IsList = columnDto.IsList;
                column.IsQuery = columnDto.IsQuery;
                column.IsRequired = columnDto.IsRequired;
                column.QueryType = columnDto.QueryType;
                column.HtmlType = columnDto.HtmlType;
                column.DictType = columnDto.DictType;
                column.Sort = columnDto.Sort;
                column.UpdateBy = operName;
                column.UpdateTime = DateTime.Now;

                await _genTableColumnRepository.UpdateAsync(column);
            }
        }

        return true;
    }

    /// <summary>
    /// 删除生成表
    /// </summary>
    public async Task<bool> DeleteGenTableAsync(long[] tableIds)
    {
        foreach (var tableId in tableIds)
        {
            // 删除字段
            await _genTableColumnRepository.DeleteByTableIdAsync(tableId);
            
            // 删除表
            var table = await _genTableRepository.GetByIdAsync(tableId);
            if (table != null)
            {
                await _genTableRepository.DeleteAsync(table);
            }
        }

        return true;
    }

    /// <summary>
    /// 同步数据库
    /// </summary>
    public async Task<bool> SyncDbAsync(string tableName)
    {
        var operName = _currentUserService.GetUserName() ?? "admin";
        
        // 获取生成表信息
        var table = await _genTableRepository.GetGenTableByNameAsync(tableName);
        if (table == null)
        {
            return false;
        }

        // 获取数据库表字段信息
        var dbColumns = await _genTableColumnRepository.GetDbTableColumnsByNameAsync(tableName);
        var existingColumns = table.Columns;

        // 删除不存在的字段
        var dbColumnNames = dbColumns.Select(c => c.ColumnName).ToList();
        var columnsToDelete = existingColumns.Where(c => !dbColumnNames.Contains(c.ColumnName)).ToList();
        foreach (var column in columnsToDelete)
        {
            await _genTableColumnRepository.DeleteAsync(column);
        }

        // 更新或新增字段
        foreach (var dbColumn in dbColumns)
        {
            var existingColumn = existingColumns.FirstOrDefault(c => c.ColumnName == dbColumn.ColumnName);
            if (existingColumn != null)
            {
                // 更新字段
                existingColumn.ColumnType = dbColumn.ColumnType;
                existingColumn.ColumnComment = dbColumn.ColumnComment;
                existingColumn.IsPk = dbColumn.IsPk;
                existingColumn.IsIncrement = dbColumn.IsIncrement;
                existingColumn.IsRequired = dbColumn.IsRequired;
                existingColumn.UpdateBy = operName;
                existingColumn.UpdateTime = DateTime.Now;

                await _genTableColumnRepository.UpdateAsync(existingColumn);
            }
            else
            {
                // 新增字段
                GenUtils.InitColumnField(dbColumn, table);
                await _genTableColumnRepository.AddAsync(dbColumn);
            }
        }

        return true;
    }

    /// <summary>
    /// 根据ID获取表信息（用于代码生成）
    /// </summary>
    public async Task<dynamic?> GetTableByIdAsync(long tableId, CancellationToken cancellationToken = default)
    {
        var table = await _genTableRepository.GetGenTableByIdWithColumnsAsync(tableId);
        if (table == null)
        {
            return null;
        }

        return new
        {
            TableName = table.TableName,
            TableComment = table.TableComment,
            ClassName = table.ClassName,
            ModuleName = table.ModuleName,
            BusinessName = table.BusinessName,
            FunctionName = table.FunctionName,
            FunctionAuthor = table.FunctionAuthor,
            Columns = table.Columns.Select(c => new
            {
                ColumnName = c.ColumnName,
                ColumnComment = c.ColumnComment,
                ColumnType = c.ColumnType,
                CsharpType = c.CSharpType,
                CsharpField = c.CSharpField,
                IsPk = c.IsPk,
                IsIncrement = c.IsIncrement,
                IsRequired = c.IsRequired == "1",
                IsInsert = c.IsInsert == "1",
                IsEdit = c.IsEdit == "1",
                IsList = c.IsList == "1",
                IsQuery = c.IsQuery == "1",
                QueryType = c.QueryType,
                HtmlType = c.HtmlType,
                DictType = c.DictType,
                Sort = c.Sort,
                ColumnLength = GetColumnLength(c.ColumnType),
                IsNullable = c.IsRequired != "1"
            }).OrderBy(c => c.Sort).ToList()
        };
    }

    /// <summary>
    /// 根据表名获取表信息（用于代码生成）
    /// </summary>
    public async Task<dynamic?> GetTableByNameAsync(string tableName, CancellationToken cancellationToken = default)
    {
        var table = await _genTableRepository.GetGenTableByNameAsync(tableName);
        if (table == null)
        {
            return null;
        }

        return await GetTableByIdAsync(table.TableId, cancellationToken);
    }

    /// <summary>
    /// 获取字段长度
    /// </summary>
    private int GetColumnLength(string columnType)
    {
        if (string.IsNullOrEmpty(columnType))
        {
            return 0;
        }

        var startIndex = columnType.IndexOf('(');
        var endIndex = columnType.IndexOf(')');
        if (startIndex > 0 && endIndex > startIndex)
        {
            var lengthStr = columnType.Substring(startIndex + 1, endIndex - startIndex - 1);
            if (int.TryParse(lengthStr, out var length))
            {
                return length;
            }
        }

        return 0;
    }
}
