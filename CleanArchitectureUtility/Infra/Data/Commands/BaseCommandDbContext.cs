using System.Globalization;
using CleanArchitectureUtility.Core.Abstractions.UsersManagement;
using CleanArchitectureUtility.Core.Domain.ValueObjects;
using CleanArchitectureUtility.Infra.Data.Commands.Extensions;
using CleanArchitectureUtility.Infra.Data.Commands.ValueConversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace CleanArchitectureUtility.Infra.Data.Commands;

public abstract class BaseCommandDbContext : DbContext
{
    protected IDbContextTransaction? Transaction;

    protected BaseCommandDbContext(DbContextOptions options) : base(options)
    {
    }

    protected BaseCommandDbContext()
    {
    }

    public void BeginTransaction() => Transaction = Database.BeginTransaction();

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

    public T? GetShadowPropertyValue<T>(object entity, string propertyName) where T : IConvertible
    {
        var value = Entry(entity).Property(propertyName).CurrentValue;
        return value != null
            ? (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)
            : default;
    }

    public object? GetShadowPropertyValue(object entity, string propertyName) => Entry(entity).Property(propertyName).CurrentValue;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.AddAuditableShadowProperties();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<BusinessId>().HaveConversion<BusinessIdConversion>();
    }

    public override int SaveChanges()
    {
        ChangeTracker.DetectChanges();
        BeforeSaveTriggers();
        ChangeTracker.AutoDetectChangesEnabled = false;
        var result = base.SaveChanges();
        ChangeTracker.AutoDetectChangesEnabled = true;
        return result;
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();
        BeforeSaveTriggers();
        ChangeTracker.AutoDetectChangesEnabled = false;
        var result = base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        ChangeTracker.AutoDetectChangesEnabled = true;
        return result;
    }

    protected virtual void BeforeSaveTriggers() => SetShadowProperties();

    private void SetShadowProperties()
    {
        var userInfoService = this.GetService<IUserInfoService>();
        ChangeTracker.SetAuditableEntityPropertyValues(userInfoService);
    }

    public IEnumerable<string> GetIncludePaths(Type clrEntityType)
    {
        var entityType = Model.FindEntityType(clrEntityType);
        var includedNavigations = new HashSet<INavigation>();
        var stack = new Stack<IEnumerator<INavigation>>();
        while (true)
        {
            var entityNavigations = entityType?.GetNavigations().Where(includedNavigations.Add).ToList();

            if (entityNavigations == null || entityNavigations.Count == 0)
            {
                if (stack.Count > 0)
                    yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
            }
            else
            {
                foreach (var inverseNavigation in entityNavigations.Select(n => n.Inverse).Where(inverseNavigation => inverseNavigation != null))
                {
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
    }
}