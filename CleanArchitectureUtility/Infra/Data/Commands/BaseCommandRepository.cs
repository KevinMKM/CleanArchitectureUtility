using System.Linq.Expressions;
using CleanArchitectureUtility.Core.Contracts.Data.Commands;
using CleanArchitectureUtility.Core.Domain.Entities;
using CleanArchitectureUtility.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureUtility.Infra.Data.Commands;

public class BaseCommandRepository<TEntity, TDbContext> : ICommandRepository<TEntity>, IUnitOfWork where TEntity : AggregateRoot where TDbContext : BaseCommandDbContext
{
    protected readonly TDbContext DbContext;

    public BaseCommandRepository(TDbContext dbContext) => DbContext = dbContext;

    void ICommandRepository<TEntity>.Delete(long id)
    {
        var entity = DbContext.Set<TEntity>().Find(id);
        if (entity is not null)
            DbContext.Set<TEntity>().Remove(entity);
    }

    void ICommandRepository<TEntity>.Delete(TEntity entity) => DbContext.Set<TEntity>().Remove(entity);

    void ICommandRepository<TEntity>.DeleteGraph(long id)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        var query = DbContext.Set<TEntity>().AsQueryable();
        query = graphPath.Aggregate(query, (current, item) => current.Include(item));
        var entity = query.FirstOrDefault(c => c.Id == id);
        if (entity?.Id > 0)
            DbContext.Set<TEntity>().Remove(entity);
    }

    TEntity? ICommandRepository<TEntity>.Get(long id) => DbContext.Set<TEntity>().Find(id);

    public TEntity? Get(BusinessId businessId) => DbContext.Set<TEntity>().FirstOrDefault(c => c.BusinessId == businessId);

    TEntity? ICommandRepository<TEntity>.GetGraph(long id)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        var query = DbContext.Set<TEntity>().AsQueryable();
        query = graphPath.Aggregate(query, (current, item) => current.Include(item));
        return query.FirstOrDefault(c => c.Id == id);
    }

    void ICommandRepository<TEntity>.Insert(TEntity entity) => DbContext.Set<TEntity>().Add(entity);

    bool ICommandRepository<TEntity>.Exists(Expression<Func<TEntity, bool>> expression) => DbContext.Set<TEntity>().Any(expression);

    public TEntity? GetGraph(BusinessId businessId)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        var query = DbContext.Set<TEntity>().AsQueryable();
        query = graphPath.Aggregate(query, (current, item) => current.Include(item));
        return query.FirstOrDefault(c => c.BusinessId == businessId);
    }

    async Task ICommandRepository<TEntity>.InsertAsync(TEntity entity) => await DbContext.Set<TEntity>().AddAsync(entity);

    async Task<TEntity?> ICommandRepository<TEntity>.GetAsync(long id) => await DbContext.Set<TEntity>().FindAsync(id);

    public async Task<TEntity?> GetAsync(BusinessId businessId) => await DbContext.Set<TEntity>().FirstOrDefaultAsync(c => c.BusinessId == businessId);

    async Task<TEntity?> ICommandRepository<TEntity>.GetGraphAsync(long id)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        var query = DbContext.Set<TEntity>().AsQueryable();
        query = graphPath.Aggregate(query, (current, item) => current.Include(item));
        return await query.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<TEntity?> GetGraphAsync(BusinessId businessId)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        var query = DbContext.Set<TEntity>().AsQueryable();
        query = graphPath.Aggregate(query, (current, item) => current.Include(item));
        return await query.FirstOrDefaultAsync(c => c.BusinessId == businessId);
    }

    async Task<bool> ICommandRepository<TEntity>.ExistsAsync(Expression<Func<TEntity, bool>> expression) => await DbContext.Set<TEntity>().AnyAsync(expression);

    public int Commit() => DbContext.SaveChanges();

    public Task<int> CommitAsync() => DbContext.SaveChangesAsync();

    public void BeginTransaction() => DbContext.BeginTransaction();

    public void CommitTransaction() => DbContext.CommitTransaction();

    public void RollbackTransaction() => DbContext.RollbackTransaction();
}