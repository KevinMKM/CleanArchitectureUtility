using System.Diagnostics;
using CleanArchitectureUtility.Extensions.Abstractions.Events;
using CleanArchitectureUtility.Extensions.Abstractions.MessageBus;
using CleanArchitectureUtility.Extensions.Events.PollingPublisher.EF.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanArchitectureUtility.Extensions.Events.PollingPublisher.EF.Services
{
    public class PoolingPublisherBackgroundService : BackgroundService
    {
        private readonly ISendMessageBus _sendMessageBus;
        private readonly IOutBoxEventItemRepository _outBoxEventItemRepository;
        private readonly ILogger<PoolingPublisherBackgroundService> _logger;
        private readonly PollingPublisherOptions _options;

        public PoolingPublisherBackgroundService(IOutBoxEventItemRepository outBoxEventItemRepository,
            IOptions<PollingPublisherOptions> options, 
            ILogger<PoolingPublisherBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();

            _options = options.Value;
            _sendMessageBus = scope.ServiceProvider.GetRequiredService<ISendMessageBus>();
            _outBoxEventItemRepository = outBoxEventItemRepository;
            _logger = logger;
            _logger.LogInformation($"PoolingPublisherBackgroundService start working for {_options.ApplicationName} at {DateTime.UtcNow}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var outboxItems = _outBoxEventItemRepository.GetOutBoxEventItemsForPublish(_options.SendCount);
                    foreach (var item in outboxItems)
                    {
                        using var trace = StartChildActivity(item);
                        _sendMessageBus.Send(new Parcel
                        {
                            CorrelationId = item.AggregateId,
                            MessageBody = item.EventPayload,
                            MessageId = item.EventId.ToString(),
                            MessageName = item.EventName,
                            Route = $"{_options.ApplicationName}.event.{item.EventName}",
                            Headers = new Dictionary<string, object>
                            {
                                ["OccurredByUserId"] = item.OccurredByUserId,
                                ["OccurredOn"] = item.OccurredOn.ToString(),
                                ["AggregateName"] = item.AggregateName,
                                ["AggregateTypeName"] = item.AggregateTypeName,
                                ["EventTypeName"] = item.EventTypeName,
                            }
                        });
                        item.IsProcessed = true;
                        _logger.LogDebug($"event {item.EventName} with {item.EventId} sent from {_options.ApplicationName} at {DateTime.UtcNow}");
                    }

                    _outBoxEventItemRepository.MarkAsRead(outboxItems);
                    if (outboxItems.Any())
                        _logger.LogInformation($"{outboxItems.Count} events {_options.ApplicationName} at {DateTime.UtcNow} with id {string.Join(',', outboxItems.Select(c => c.EventId))}");
                    await Task.Delay(_options.SendInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Exception acquired in PoolingPublisherBackgroundService for application {_options.ApplicationName}");
                    await Task.Delay(_options.ExceptionInterval, stoppingToken);
                }
            }

            _logger.LogInformation($"PoolingPublisherBackgroundService stop working for {_options.ApplicationName} at {DateTime.UtcNow}");
        }

        private static Activity StartChildActivity(OutBoxEventItem item)
        {
            var trace = new Activity("PublishUsingPoolingPublisher");
            if (item.TraceId != null && item.SpanId != null)
                trace.SetParentId($"00-{item.TraceId}-{item.SpanId}-00");
            trace.Start();
            return trace;
        }
    }
}