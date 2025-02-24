using System.Diagnostics;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Queries;
using CleanArchitectureUtility.Extensions.Abstractions.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Core.ApplicationServices.Queries;

public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QueryDispatcher> _logger;
    private readonly Stopwatch _stopwatch;

    public QueryDispatcher(IServiceProvider serviceProvider, ILogger<QueryDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _stopwatch = new Stopwatch();
        _logger = logger;
    }

    public Task<QueryResult<TData>> Execute<TQuery, TData>(TQuery query, CancellationToken cancellationToken) where TQuery : class, IQuery<TData>
    {
        _stopwatch.Start();
        try
        {
            _logger.LogDebug($"Routing query of type {query.GetType()} With value {query}  Start at {DateTime.UtcNow}");
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TData>>();
            return handler.Handle(query, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"There is not suitable handler for {query.GetType()} Routing failed at {DateTime.UtcNow}");
            throw;
        }
        finally
        {
            _stopwatch.Stop();
            _logger.LogInformation(LoggerEventId.PerformanceMeasurement,
                $"Processing the {query.GetType()} command took {_stopwatch.ElapsedMilliseconds} Milliseconds");
        }
    }
}