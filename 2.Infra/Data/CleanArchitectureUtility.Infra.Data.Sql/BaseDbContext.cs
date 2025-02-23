using CleanArchitectureUtility.Core.Domain.Entities;
using CleanArchitectureUtility.Extensions.Abstractions.UsersManagements;
using CleanArchitectureUtility.Infra.Data.Sql.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Concurrent;

namespace CleanArchitectureUtility.Infra.Data.Sql;

public abstract class BaseDbContext : DbContext
{
    protected IDbContextTransaction? Transaction;
    private readonly Guid _currentUserId;

    protected BaseDbContext(DbContextOptions options, IUserInfoService userInfoService) : base(options)
    {
        _currentUserId = userInfoService?.UserId() ?? throw new ArgumentNullException(nameof(userInfoService));
    }

    protected BaseDbContext()
    {
    }

    public void BeginTransaction()
    {
        if (Transaction != null)
            throw new InvalidOperationException("A transaction is already in progress.");

        Transaction = Database.BeginTransaction();
    }

    public void RollbackTransaction()
    {
        if (Transaction == null)
            throw new InvalidOperationException("No transaction is in progress.");

        Transaction.Rollback();
        Transaction.Dispose();
        Transaction = null;
    }

    public void CommitTransaction()
    {
        if (Transaction == null)
            throw new InvalidOperationException("No transaction is in progress.");

        Transaction.Commit();
        Transaction.Dispose();
        Transaction = null;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType)) 
                continue;
            var configurationType = typeof(BaseEntityConfiguration<>).MakeGenericType(entityType.ClrType);
            var configuration = Activator.CreateInstance(configurationType);
            builder.ApplyConfiguration((dynamic)configuration);
        }
        base.OnModelCreating(builder);
    }

    public override int SaveChanges()
    {
        ChangeTracker.DetectChanges();
        UpdateBaseEntityFields();
        ChangeTracker.AutoDetectChangesEnabled = false;
        try
        {
            return base.SaveChanges();
        }
        finally
        {
            ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();
        UpdateBaseEntityFields();
        ChangeTracker.AutoDetectChangesEnabled = false;
        try
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        finally
        {
            ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }

    protected virtual void UpdateBaseEntityFields()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.SetCreatorDetails(_currentUserId, now);
            else if (entry.State == EntityState.Modified) 
                entry.Entity.SetUpdaterDetails(_currentUserId, now);
        }
    }

    private static readonly ConcurrentDictionary<Type, IEnumerable<string>> IncludePathsCache = new();

    public IEnumerable<string> GetIncludePaths(Type clrEntityType)
    {
        return IncludePathsCache.GetOrAdd(clrEntityType, type =>
        {
            var entityType = Model.FindEntityType(type);
            var includedNavigations = new HashSet<INavigation>();
            var stack = new Stack<IEnumerator<INavigation>>();
            var paths = new List<string>();

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
                        paths.Add(string.Join(".", stack.Reverse().Select(e => e.Current.Name)));
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
                if (stack.Count == 0) 
                    break;
                entityType = stack.Peek().Current.TargetEntityType;
            }

            return paths;
        });
    }
}