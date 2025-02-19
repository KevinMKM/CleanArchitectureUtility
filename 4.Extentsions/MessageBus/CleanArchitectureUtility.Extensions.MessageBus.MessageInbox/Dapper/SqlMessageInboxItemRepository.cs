using System.Data;
using CleanArchitectureUtility.Extensions.Abstractions.MessageBus;
using CleanArchitectureUtility.Extensions.MessageBus.MessageInbox.Dapper.Options;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanArchitectureUtility.Extensions.MessageBus.MessageInbox.Dapper;
public class SqlMessageInboxItemRepository : IMessageInboxItemRepository
{
    private readonly IDbConnection _dbConnection;

    private readonly MessageInboxDalDapperOptions _options;
    private readonly string _selectQuery;
    private readonly string _insertQuery;
    private readonly ILogger<SqlMessageInboxItemRepository> _logger;

    public SqlMessageInboxItemRepository(IOptions<MessageInboxDalDapperOptions> options, ILogger<SqlMessageInboxItemRepository> logger)
    {
        _options = options.Value;
        _selectQuery = $"Select Id from {_options.SchemaName}.{_options.TableName} Where [OwnerService] = @OwnerService and [MessageId] = @MessageId";
        _insertQuery = $"Insert Into {_options.SchemaName}.{_options.TableName} ([OwnerService] ,[MessageId],[Payload] ) values(@OwnerService,@MessageId,@Payload)";
        _dbConnection = new SqlConnection(_options.ConnectionString);
        _logger = logger;
        if (_options.AutoCreateSqlTable)
            CreateTableIfNeeded();
    }

    public bool AllowReceive(string messageId, string fromService)
    {
        var result = _dbConnection.Query<long>(_selectQuery, new
        {
            OwnerService = fromService,
            MessageId = messageId
        }).FirstOrDefault();
        return result < 1;
    }

    public void Receive(string messageId, string fromService, string payload)
    {
        _dbConnection.Execute(_insertQuery, new
        {
            OwnerService = fromService,
            MessageId = messageId,
            Payload = payload
        });
    }
    
    private void CreateTableIfNeeded()
    {
        try
        {
            var table = _options.TableName;
            var schema = _options.SchemaName;
            _logger.LogInformation($"Sql Message Inbox try to create table in database with connection {_options.ConnectionString}. Schema name is {schema}. Table name is {table}");
            var createTable = $"IF (NOT EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES WHERE " +
                              $"TABLE_SCHEMA = '{schema}' AND  TABLE_NAME = '{table}' )) Begin " +
                              $"CREATE TABLE [{schema}].[{table}]( " +
                              $"Id bigint  Primary Key Identity(1,1)," +
                              $"[OwnerService] [nvarchar](100) NOT NULL," +
                              $"[MessageId] [nvarchar](50) NOT NULL," +
                              $"[Payload] [nvarchar](max) NULL," +
                              $"[ReceivedDate] [DateTime] default(GetDate()))" +
                              $" End";
            _dbConnection.Execute(createTable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create table for Sql Message Inbox Failed");
            throw;
        }
    }
}