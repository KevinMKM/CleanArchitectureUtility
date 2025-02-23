using CleanArchitectureUtility.Extensions.Abstractions.UsersManagements;
using CleanArchitectureUtility.Extensions.Events.PollingPublisher.Outbox.Configs;
using CleanArchitectureUtility.Extensions.Events.PollingPublisher.Outbox.Interceptors;
using CleanArchitectureUtility.Infra.Data.SqlCommands;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureUtility.Extensions.Events.PollingPublisher.Outbox.DbContext;

public abstract class BaseOutboxCommandDbContext : BaseCommandDbContext
{
    protected BaseOutboxCommandDbContext(DbContextOptions options, IUserInfoService userInfoService) 
        : base(options, userInfoService)
    {
    }

    protected BaseOutboxCommandDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.AddInterceptors(new AddOutBoxEventItemInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new OutBoxEventItemConfig());
    }
}