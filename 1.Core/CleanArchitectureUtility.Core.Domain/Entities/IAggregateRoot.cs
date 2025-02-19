using CleanArchitectureUtility.Core.Domain.Events;

namespace CleanArchitectureUtility.Core.Domain.Entities
{
    public interface IAggregateRoot
    {
        void ClearEvents();
        IEnumerable<IDomainEvent> GetEvents();
    }
}