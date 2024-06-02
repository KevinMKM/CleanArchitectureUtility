using CleanArchitectureUtility.Core.Domain.Events;

namespace CleanArchitectureUtility.Core.Contracts.ApplicationServices.Events;

public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task Handle(TDomainEvent @event);
}