using System.Reflection;
using CleanArchitectureUtility.Core.Domain.Events;

namespace CleanArchitectureUtility.Core.Domain.Entities;

public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _events = new();
    protected AggregateRoot() => _events = new List<IDomainEvent>();

    protected AggregateRoot(List<IDomainEvent>? events)
    {
        if (events == null || !events.Any())
            events = new List<IDomainEvent>();
        foreach (var @event in events)
            Mutate(@event);
    }

    protected void Apply(IDomainEvent @event)
    {
        Mutate(@event);
        AddEvent(@event);
    }

    private void Mutate(IDomainEvent @event)
    {
        var onMethod = this.GetType().GetMethod("On", BindingFlags.Instance | BindingFlags.NonPublic, new Type[] { @event.GetType() });
        onMethod.Invoke(this, new object?[] { @event });
    }

    protected void AddEvent(IDomainEvent @event) => _events.Add(@event);
    public IEnumerable<IDomainEvent> GetEvents() => _events.AsEnumerable();
    public void ClearEvents() => _events.Clear();
}