using System.Globalization;
using CleanArchitectureUtility.Core.Domain.Entities;
using CleanArchitectureUtility.Core.Domain.ValueObjects;
using CleanArchitectureUtility.Extensions.Abstractions.UsersManagements;
using CleanArchitectureUtility.Infra.Data.SqlCommands.Extensions;
using CleanArchitectureUtility.Infra.Data.SqlCommands.ValueConversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace CleanArchitectureUtility.Infra.Data.SqlCommands;

public abstract class BaseCommandDbContext : DbContext
{
    protected IDbContextTransaction Transaction;
    private readonly Guid _currentUserId;

    protected BaseCommandDbContext(DbContextOptions options, IUserInfoService userInfoService) : base(options)
    {
        _currentUserId = userInfoService.UserId();
    }

    protected BaseCommandDbContext()
    {
    }

    public void BeginTransaction()
    {
        Transaction = Database.BeginTransaction();
    }

    public void RollbackTransaction()
    {
        if (Transaction == null)
            throw new NullReferenceException("Please call `BeginTransaction()` method first.");

        Transaction.Rollback();
    }

    public void CommitTransaction()
    {
        if (Transaction == null)
            throw new NullReferenceException("Please call `BeginTransaction()` method first.");

        Transaction.Commit();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.AddAuditableShadowProperties();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<IdVO>().HaveConversion<IdConversion>();
    }

    public override int SaveChanges()
    {
        ChangeTracker.DetectChanges();
        UpdateBaseEntityFields();
        ChangeTracker.AutoDetectChangesEnabled = false;
        var result = base.SaveChanges();
        ChangeTracker.AutoDetectChangesEnabled = true;
        return result;
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();
        UpdateBaseEntityFields();
        ChangeTracker.AutoDetectChangesEnabled = false;
        var result = base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        ChangeTracker.AutoDetectChangesEnabled = true;
        return result;
    }

    protected virtual void UpdateBaseEntityFields()
    {
        DateTime now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.SetUpdaterDetails(_currentUserId, now);
            }
        }
    }

    public IEnumerable<string> GetIncludePaths(Type clrEntityType)
    {
        var entityType = Model.FindEntityType(clrEntityType);
        var includedNavigations = new HashSet<INavigation>();
        var stack = new Stack<IEnumerator<INavigation>>();
        while (true)
        {
            var entityNavigations = new List<INavigation>();
            foreach (var navigation in entityType.GetNavigations())
            {
                if (includedNavigations.Add(navigation))
                    entityNavigations.Add(navigation);
            }

            if (entityNavigations.Count == 0)
            {
                if (stack.Count > 0)
                    yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
            }
            else
            {
                foreach (var navigation in entityNavigations)
                {
                    var inverseNavigation = navigation.Inverse;
                    if (inverseNavigation != null)
                        includedNavigations.Add(inverseNavigation);
                }

                stack.Push(entityNavigations.GetEnumerator());
            }

            while (stack.Count > 0 && !stack.Peek().MoveNext())
                stack.Pop();
            if (stack.Count == 0) break;
            entityType = stack.Peek().Current.TargetEntityType;
        }
    }
}