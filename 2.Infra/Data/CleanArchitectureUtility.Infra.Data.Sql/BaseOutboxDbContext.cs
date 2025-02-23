using CleanArchitectureUtility.Core.Domain.Entities;
using CleanArchitectureUtility.Core.Domain.Events;
using CleanArchitectureUtility.Extensions.Abstractions.Events;
using CleanArchitectureUtility.Extensions.Abstractions.Serializers;
using CleanArchitectureUtility.Extensions.Abstractions.UsersManagements;
using CleanArchitectureUtility.Infra.Data.Sql.Extensions;
using CleanArchitectureUtility.Infra.Data.Sql.OutBoxEventItems;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureUtility.Infra.Data.Sql;

public abstract class BaseOutboxDbContext : BaseDbContext
{
    public DbSet<OutBoxEventItem> OutBoxEventItems { get; set; }

    private readonly IUserInfoService _userInfoService;
    private readonly IJsonSerializer _jsonSerializer;

    protected BaseOutboxDbContext(DbContextOptions options, IUserInfoService userInfoService, IJsonSerializer jsonSerializer)
        : base(options, userInfoService)
    {
        _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
    }

    protected BaseOutboxDbContext()
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new OutBoxEventItemConfig());
    }

    protected override void UpdateBaseEntityFields()
    {
        base.UpdateBaseEntityFields();
        AddOutboxEventItems();
    }

    private void AddOutboxEventItems()
    {
        foreach (var aggregate in ChangeTracker.Aggregates())
        {
            var events = aggregate.Entity.GetEvents();
            foreach (var @event in events)
                OutBoxEventItems.Add(CreateOutboxEventItem(aggregate.Entity, @event));
        }
    }

    private OutBoxEventItem CreateOutboxEventItem(IAggregateRoot aggregate, IDomainEvent @event)
        => new()
        {
            EventId = Guid.NewGuid(),
            OccurredByUserId = _userInfoService.UserId(),
            OccurredOn = DateTime.UtcNow,
            AggregateId = aggregate.Id.ToString(),
            AggregateName = aggregate.GetType().Name,
            AggregateTypeName = aggregate.GetType().FullName,
            EventName = @event.GetType().Name,
            EventTypeName = @event.GetType().FullName,
            EventPayload = _jsonSerializer.Serialize(@event),
            IsProcessed = false
        };
}