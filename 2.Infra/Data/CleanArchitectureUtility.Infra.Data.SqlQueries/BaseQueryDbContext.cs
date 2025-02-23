using CleanArchitectureUtility.Extensions.Abstractions.UsersManagements;
using CleanArchitectureUtility.Infra.Data.Sql;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureUtility.Infra.Data.SqlQueries;

public abstract class BaseQueryDbContext : BaseDbContext
{
    protected BaseQueryDbContext(DbContextOptions options, IUserInfoService userInfoService) : base(options, userInfoService)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    public override int SaveChanges()
    {
        throw new NotSupportedException();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        throw new NotSupportedException();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }
}