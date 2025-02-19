using System.Data;
using System.Data.Common;
using CleanArchitectureUtility.Extensions.Abstractions.ChangeDataLog;
using CleanArchitectureUtility.Extensions.Databases.Dapper.Options;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanArchitectureUtility.Extensions.Databases.Dapper.Repository;

public sealed class DapperEntityChangeInterceptorItemRepository : IEntityChangeInterceptorItemRepository
{
    private readonly ChangeDataLogSqlOptions _options;
    private readonly string _insertEntityChangeInterceptorItemCommand;
    private readonly string _insertPropertyChangeLogItemCommand;
    private readonly ILogger<DapperEntityChangeInterceptorItemRepository> _logger;

    public DapperEntityChangeInterceptorItemRepository(IOptions<ChangeDataLogSqlOptions> options, ILogger<DapperEntityChangeInterceptorItemRepository> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            throw new ArgumentException("Connection string cannot be null or empty.",
                nameof(_options.ConnectionString));

        if (string.IsNullOrWhiteSpace(_options.SchemaName))
            throw new ArgumentException("Schema name cannot be null or empty.", nameof(_options.SchemaName));

        if (string.IsNullOrWhiteSpace(_options.EntityTableName))
            throw new ArgumentException("Entity table name cannot be null or empty.", nameof(_options.EntityTableName));

        if (string.IsNullOrWhiteSpace(_options.PropertyTableName))
            throw new ArgumentException("Property table name cannot be null or empty.",
                nameof(_options.PropertyTableName));

        _insertEntityChangeInterceptorItemCommand = $@"
            INSERT INTO [{_options.SchemaName}].[{_options.EntityTableName}] 
                ([Id], [ContextName], [EntityType], [EntityId], [UserId], [IP], [TransactionId], [DateOfOccurrence], [ChangeType]) 
            VALUES 
                (@Id, @ContextName, @EntityType, @EntityId, @UserId, @IP, @TransactionId, @DateOfOccurrence, @ChangeType)";

        _insertPropertyChangeLogItemCommand = $@"
            INSERT INTO [{_options.SchemaName}].[{_options.PropertyTableName}] 
                ([Id], [ChangeInterceptorItemId], [PropertyName], [Value]) 
            VALUES 
                (@Id, @ChangeInterceptorItemId, @PropertyName, @Value)";

        if (_options.AutoCreateSqlTable)
        {
            using var connection = CreateDbConnection();
            connection.Open();
            CreateEntityChangeInterceptorItemTableIfNeeded(connection);
            CreatePropertyChangeLogItemTableIfNeeded(connection);
        }
    }

    public void Save(List<EntityChangeInterceptorItem> entityChangeInterceptorItems, IDbTransaction transaction = null)
    {
        if (entityChangeInterceptorItems == null)
            throw new ArgumentNullException(nameof(entityChangeInterceptorItems));

        if (entityChangeInterceptorItems.Count == 0)
            return;

        var externalTransactionProvided = transaction != null;
        var connection = externalTransactionProvided ? (DbConnection)transaction.Connection : CreateDbConnection();
        DbTransaction localTransaction = null;
        try
        {
            if (!externalTransactionProvided)
            {
                connection.Open();
                localTransaction = connection.BeginTransaction();
            }

            foreach (var item in entityChangeInterceptorItems)
            {
                connection.Execute(_insertEntityChangeInterceptorItemCommand, new
                {
                    item.Id,
                    item.ContextName,
                    item.EntityType,
                    item.EntityId,
                    item.UserId,
                    item.Ip,
                    item.TransactionId,
                    item.DateOfOccurrence,
                    item.ChangeType
                }, externalTransactionProvided ? transaction : localTransaction);

                if (item.PropertyChangeLogItems != null && item.PropertyChangeLogItems.Count > 0)
                    foreach (var prop in item.PropertyChangeLogItems)
                        connection.Execute(_insertPropertyChangeLogItemCommand, new
                        {
                            prop.Id,
                            ChangeInterceptorItemId = item.Id,
                            prop.PropertyName,
                            prop.Value
                        }, externalTransactionProvided ? transaction : localTransaction);
            }

            if (!externalTransactionProvided)
                localTransaction.Commit();
        }
        catch (Exception ex)
        {
            if (!externalTransactionProvided && localTransaction != null)
                localTransaction.Rollback();

            _logger.LogError(ex, "An error occurred while saving entity change interceptor items.");
            throw;
        }
        finally
        {
            if (!externalTransactionProvided)
            {
                connection.Close();
                localTransaction?.Dispose();
            }
        }
    }

    public async Task SaveAsync(List<EntityChangeInterceptorItem> entityChangeInterceptorItems, IDbTransaction transaction = null)
    {
        if (entityChangeInterceptorItems == null)
            throw new ArgumentNullException(nameof(entityChangeInterceptorItems));

        if (entityChangeInterceptorItems.Count == 0)
            return;

        var externalTransactionProvided = transaction != null;
        var connection = externalTransactionProvided ? (DbConnection)transaction.Connection : CreateDbConnection();
        DbTransaction localTransaction = null;
        try
        {
            if (!externalTransactionProvided)
            {
                await connection.OpenAsync();
                localTransaction = await connection.BeginTransactionAsync();
            }

            foreach (var item in entityChangeInterceptorItems)
            {
                await connection.ExecuteAsync(_insertEntityChangeInterceptorItemCommand, new
                {
                    item.Id,
                    item.ContextName,
                    item.EntityType,
                    item.EntityId,
                    item.UserId,
                    item.Ip,
                    item.TransactionId,
                    item.DateOfOccurrence,
                    item.ChangeType
                }, externalTransactionProvided ? transaction : localTransaction);

                if (item.PropertyChangeLogItems != null && item.PropertyChangeLogItems.Count > 0)
                    foreach (var prop in item.PropertyChangeLogItems)
                        await connection.ExecuteAsync(_insertPropertyChangeLogItemCommand, new
                        {
                            prop.Id,
                            ChangeInterceptorItemId = item.Id,
                            prop.PropertyName,
                            prop.Value
                        }, externalTransactionProvided ? transaction : localTransaction);
            }

            if (!externalTransactionProvided)
                await localTransaction.CommitAsync();
        }
        catch (Exception ex)
        {
            if (!externalTransactionProvided && localTransaction != null)
                await localTransaction.RollbackAsync();

            _logger.LogError(ex, "An error occurred while saving entity change interceptor items.");
            throw;
        }
        finally
        {
            if (!externalTransactionProvided)
            {
                await connection.CloseAsync();
                localTransaction?.DisposeAsync();
            }
        }
    }

    private DbConnection CreateDbConnection() => new SqlConnection(_options.ConnectionString);

    private void CreateEntityChangeInterceptorItemTableIfNeeded(DbConnection connection)
    {
        var createTable = $@"
            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @Table)
            BEGIN
                CREATE TABLE [{_options.SchemaName}].[{_options.EntityTableName}](
                    Id UNIQUEIDENTIFIER PRIMARY KEY, 
                    ContextName NVARCHAR(200) NOT NULL, 
                    EntityType NVARCHAR(200) NOT NULL, 
                    EntityId NVARCHAR(200) NOT NULL, 
                    UserId NVARCHAR(200), 
                    IP NVARCHAR(50),
                    TransactionId NVARCHAR(50),
                    DateOfOccurrence DATETIME NOT NULL,
                    ChangeType NVARCHAR(50)
                );
            END";
        connection.Execute(createTable, new { Schema = _options.SchemaName, Table = _options.EntityTableName });
    }

    private void CreatePropertyChangeLogItemTableIfNeeded(DbConnection connection)
    {
        var createTable = $@"
            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @Table)
            BEGIN
                CREATE TABLE [{_options.SchemaName}].[{_options.PropertyTableName}](
                    Id UNIQUEIDENTIFIER PRIMARY KEY, 
                    ChangeInterceptorItemId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [{_options.SchemaName}].[{_options.EntityTableName}](Id),
                    PropertyName NVARCHAR(200) NOT NULL,
                    Value NVARCHAR(MAX)
                );
            END";
        connection.Execute(createTable, new { Schema = _options.SchemaName, Table = _options.PropertyTableName });
    }
}