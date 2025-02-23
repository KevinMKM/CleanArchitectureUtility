using CleanArchitectureUtility.Core.Domain.Events;
using CleanArchitectureUtility.Core.Domain.ValueObjects;

namespace CleanArchitectureUtility.Core.Domain.Entities
{
    public interface IAggregateRoot
    {
        public IdVO Id { get;}
        void ClearEvents();
        IEnumerable<IDomainEvent> GetEvents();
    }
}