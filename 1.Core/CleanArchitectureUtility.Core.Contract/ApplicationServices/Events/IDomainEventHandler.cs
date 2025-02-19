using CleanArchitectureUtility.Core.Domain.Events;

namespace CleanArchitectureUtility.Core.Contract.ApplicationServices.Events;

public interface IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task Handle(TDomainEvent @event);
}