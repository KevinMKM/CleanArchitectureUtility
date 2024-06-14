using System.Diagnostics;
using CleanArchitectureUtility.Core.Abstractions.Events;
using CleanArchitectureUtility.Core.Abstractions.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanArchitectureUtility.Utilities.Events.PollingPublisher
{
    public class PoolingPublisherBackgroundService : BackgroundService
    {
        private readonly ISendMessageBus _sendMessageBus;
        private readonly IOutBoxEventItemRepository _outBoxEventItemRepository;
        private readonly ILogger<PoolingPublisherBackgroundService> _logger;
        private readonly PollingPublisherOptions _options;

        public PoolingPublisherBackgroundService(IOutBoxEventItemRepository outBoxEventItemRepository,
            IOptions<PollingPublisherOptions> options,
            ILogger<PoolingPublisherBackgroundService> logger, IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();

            _options = options.Value;
            _sendMessageBus = scope.ServiceProvider.GetRequiredService<ISendMessageBus>();
            _outBoxEventItemRepository = outBoxEventItemRepository;
            _logger = logger;
            _logger.LogInformation($"PoolingPublisherBackgroundService start working for {_options.ApplicationName} at {DateTime.Now}");
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
                            Headers = new Dictionary<string, object?>
                            {
                                ["OccurredByUserId"] = item.OccurredByUserId,
                                ["OccurredOn"] = item.OccurredOn.ToString(),
                                ["AggregateName"] = item.AggregateName,
                                ["AggregateTypeName"] = item.AggregateTypeName,
                                ["EventTypeName"] = item.EventTypeName,
                            }
                        });
                        item.IsProcessed = true;
                        _logger.LogDebug($"event {item.EventName} with {item.EventId} sent from {_options.ApplicationName} at {DateTime.Now}");
                    }

                    _outBoxEventItemRepository.MarkAsRead(outboxItems);
                    if (outboxItems.Count != 0)
                        _logger.LogInformation($"{outboxItems.Count} events {_options.ApplicationName} at {DateTime.Now} " +
                                               $"with id {string.Join(',', outboxItems.Select(c => c.EventId))}");
                    await Task.Delay(_options.SendInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Exception acquired in PoolingPublisherBackgroundService for application {_options.ApplicationName}");
                    await Task.Delay(_options.ExceptionInterval, stoppingToken);
                }
            }

            _logger.LogInformation($"PoolingPublisherBackgroundService stop working for {_options.ApplicationName} at {DateTime.Now}");
        }

        private static Activity StartChildActivity(OutBoxEventItem item)
        {
            var trace = new Activity("PublishUsingPoolingPublisher");
            if (item.TraceId is not null && item.SpanId is not null)
                trace.SetParentId($"00-{item.TraceId}-{item.SpanId}-00");

            trace.Start();
            return trace;
        }
    }
}