using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Events;
using CleanArchitectureUtility.Core.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Core.ApplicationService.Events;

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventDispatcher> _logger;

    public EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task PublishDomainEventAsync<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent
    {
        var handlers = _serviceProvider.GetServices<IDomainEventHandler<TDomainEvent>>();

        _logger.LogDebug($"Routing event of type {@event.GetType()} With value {@event}  Start at {DateTime.Now}");
        var counter = 0;
        foreach (var handler in handlers)
        {
            counter++;
            handler.Handle(@event);
        }

        _logger.LogDebug($"Total number of handler for {@event.GetType()} is {counter}");
        return Task.CompletedTask;
    }
}