﻿using CleanArchitectureUtility.Core.Domain.Events;

namespace CleanArchitectureUtility.Core.Contract.Data.Commands;

public interface IDomainEventStore
{
    void Save<TEvent>(string aggregateName, string aggregateId, IEnumerable<TEvent> events) where TEvent : IDomainEvent;

    Task SaveAsync<TEvent>(string aggregateName, string aggregateId, IEnumerable<TEvent> events) where TEvent : IDomainEvent;
}