using System.Data;
using CleanArchitectureUtility.Extensions.Abstractions.Events;
using CleanArchitectureUtility.Extensions.Events.PollingPublisher.Dapper.Options;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanArchitectureUtility.Extensions.Events.PollingPublisher.Dapper.DataAccess;

public class SqlOutBoxEventItemRepository : IOutBoxEventItemRepository
{
    private readonly PollingPublisherDalRedisOptions _options;
    private readonly IDbConnection _dbConnection;
    private readonly ILogger<SqlOutBoxEventItemRepository> _logger;

    public SqlOutBoxEventItemRepository(IOptions<PollingPublisherDalRedisOptions> options,
        ILogger<SqlOutBoxEventItemRepository> logger)
    {
        _options = options.Value;
        _dbConnection = new SqlConnection(_options.ConnectionString);
        _logger = logger;
        _logger.LogInformation("New Instance of SqlOutBoxEventItemRepository Created");
    }

    public List<OutBoxEventItem> GetOutBoxEventItemsForPublish(int maxCount = 100)
    {
        try
        {
            var result = _dbConnection.Query<OutBoxEventItem>(_options.SelectCommand, new { Count = maxCount })
                .ToList();
            _logger.LogDebug($"{result.Count} of event fetched for sending from service {_options.ApplicationName} at {DateTime.UtcNow}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"fetching events failed in application {_options.ApplicationName}");
            throw;
        }
    }

    public void MarkAsRead(List<OutBoxEventItem> outBoxEventItems)
    {
        try
        {
            var idForMark = outBoxEventItems.Where(c => c.IsProcessed).Select(c => c.OutBoxEventItemId).ToList();
            if (idForMark != null && idForMark.Any())
            {
                _dbConnection.Execute(_options.UpdateCommand, new
                {
                    Ids = idForMark
                });
                _logger.LogInformation($"{outBoxEventItems.Count} of event marked as processed in service {_options.ApplicationName} at {DateTime.UtcNow}. marked ids are {idForMark}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Marking events as processed failed in application {_options.ApplicationName}");
            throw;
        }
    }
}