using CleanArchitectureUtility.Core.Domain.Events;

namespace CleanArchitectureUtility.Core.Contracts.ApplicationServices.Events;

public interface IEventDispatcher
{
    Task PublishDomainEventAsync<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent;
}