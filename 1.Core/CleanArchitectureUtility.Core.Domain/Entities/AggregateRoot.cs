using System.Reflection;
using CleanArchitectureUtility.Core.Domain.Events;
using CleanArchitectureUtility.Core.Domain.Exceptions;

namespace CleanArchitectureUtility.Core.Domain.Entities;

public abstract class AggregateRoot : BaseEntity, IAggregateRoot
{
    private readonly List<IDomainEvent> _events;
    private readonly Dictionary<Type, MethodInfo> _eventHandlersCache = new();
    protected AggregateRoot() => _events = new List<IDomainEvent>();

    protected AggregateRoot(IEnumerable<IDomainEvent> events)
    {
        if (events?.Any() == true)
            foreach (var @event in events)
                Apply(@event);
    }

    protected void Apply(IDomainEvent @event)
    {
        ApplyEvent(@event);
        AddEvent(@event);
    }

    private void ApplyEvent(IDomainEvent @event)
    {
        if (!_eventHandlersCache.TryGetValue(@event.GetType(), out var handlerMethod))
        {
            handlerMethod = GetType()
                .GetMethod("On", BindingFlags.Instance | BindingFlags.NonPublic, new[] { @event.GetType() });

            if (handlerMethod == null)
                throw new InvalidEntityStateException("No handler method 'On' found for event of type " +
                                                      $"'{@event.GetType().Name}' in aggregate '{@event.GetType().DeclaringType?.Name}'.");

            _eventHandlersCache[@event.GetType()] = handlerMethod;
        }

        handlerMethod.Invoke(this, new[] { @event });
    }

    protected void AddEvent(IDomainEvent @event) => _events.Add(@event);
    public IEnumerable<IDomainEvent> GetEvents() => _events.AsEnumerable();
    public void ClearEvents() => _events.Clear();
    public int EventCount => _events.Count;
}