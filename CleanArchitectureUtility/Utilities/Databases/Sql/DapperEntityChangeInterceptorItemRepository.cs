using System.Data;
using CleanArchitectureUtility.Core.Abstractions.ChangeDataLog;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace CleanArchitectureUtility.Utilities.Databases.Sql;

public class DapperEntityChangeInterceptorItemRepository : IEntityChangeInterceptorItemRepository
{
    private readonly ChangeDataLogSqlOptions _options;
    private readonly IDbConnection _dbConnection;

    private readonly string _insertEntityChangeInterceptorItemCommand =
        "INSERT INTO [{0}].[{1}]([Id],[ContextName],[EntityType],[EntityId],[UserId],[IP],[TransactionId],[DateOfOccurrence],[ChangeType]) VALUES (@Id,@ContextName,@EntityType,@EntityId,@UserId,@IP,@TransactionId,@DateOfOccurrence,@ChangeType) ";

    private readonly string _insertPropertyChangeLogItemCommand =
        "INSERT INTO [{0}].[{1}]([Id],[ChangeInterceptorItemId],[PropertyName],[Value]) VALUES (@Id,@ChageInterceptorItemId,@PropertyName,@Value)";

    public DapperEntityChangeInterceptorItemRepository(IOptions<ChangeDataLogSqlOptions> options)
    {
        _options = options.Value;
        _dbConnection = new SqlConnection(_options.ConnectionString);
        if (_options.AutoCreateSqlTable)
        {
            CreateEntityChangeInterceptorItemTableIfNeeded();
            CreatePropertyChangeLogItemTableIfNeeded();
        }

        _insertEntityChangeInterceptorItemCommand = string.Format(_insertEntityChangeInterceptorItemCommand, _options.SchemaName, _options.EntityTableName);
        _insertPropertyChangeLogItemCommand = string.Format(_insertPropertyChangeLogItemCommand, _options.SchemaName, _options.PropertyTableName);
    }

    public void Save(List<EntityChangeInterceptorItem> entityChangeInterceptorItems)
    {
        foreach (var item in entityChangeInterceptorItems)
        {
            if (_dbConnection.State == ConnectionState.Closed)
                _dbConnection.Open();
            using var tran = _dbConnection.BeginTransaction();
            try
            {
                _dbConnection.Execute(_insertEntityChangeInterceptorItemCommand,
                    new
                    {
                        item.Id, item.ContextName, item.EntityType, item.EntityId, item.UserId, item.Ip, item.TransactionId, item.DateOfOccurrence,
                        item.ChangeType
                    });
                _dbConnection.Execute(_insertPropertyChangeLogItemCommand, item.PropertyChangeLogItems.ToArray());
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
            }
        }
    }

    public Task SaveAsync(List<EntityChangeInterceptorItem> entityChangeInterceptorItems) => throw new NotImplementedException();

    private void CreateEntityChangeInterceptorItemTableIfNeeded()
    {
        var table = _options.EntityTableName;
        var schema = _options.SchemaName;
        var createTable = $"IF (NOT EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES WHERE " +
                          $"TABLE_SCHEMA = '{schema}' AND  TABLE_NAME = '{table}' )) Begin " +
                          $"CREATE TABLE [{schema}].[{table}]( " +
                          "Id uniqueidentifier primary key, " +
                          "ContextName nvarchar(200)  not null, " +
                          "EntityType nvarchar(200)  not null, " +
                          "EntityId nvarchar(200)  not null, " +
                          "UserId nvarchar(200) , " +
                          "[IP] nvarchar(50)," +
                          "TransactionId nvarchar(50) ," +
                          "DateOfOccurrence Datetime  not null ," +
                          "ChangeType nvarchar(50)" +
                          $")" +
                          $" End";
        _dbConnection.Execute(createTable);
    }

    private void CreatePropertyChangeLogItemTableIfNeeded()
    {
        var parentTable = _options.EntityTableName;
        var table = _options.PropertyTableName;
        var schema = _options.SchemaName;
        var createTable = $"IF (NOT EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES WHERE " +
                          $"TABLE_SCHEMA = '{schema}' AND  TABLE_NAME = '{table}' )) Begin " +
                          $"CREATE TABLE [{schema}].[{table}]( " +
                          "Id uniqueidentifier primary key, " +
                          $"ChangeInterceptorItemId uniqueidentifier references [{schema}].[{parentTable}](Id)," +
                          "PropertyName nvarchar(200)  not null," +
                          "Value nvarchar(max) ," +
                          $")" +
                          $" End";
        _dbConnection.Execute(createTable);
    }
}