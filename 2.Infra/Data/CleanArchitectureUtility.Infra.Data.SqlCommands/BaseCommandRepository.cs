using System.Linq.Expressions;
using CleanArchitectureUtility.Core.Contract.Data.Commands;
using CleanArchitectureUtility.Core.Domain.Entities;
using CleanArchitectureUtility.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureUtility.Infra.Data.SqlCommands;

public class BaseCommandRepository<TEntity, TDbContext> : ICommandRepository<TEntity>, IUnitOfWork 
    where TEntity : AggregateRoot
    where TDbContext : BaseCommandDbContext
{
    protected readonly TDbContext DbContext;

    public BaseCommandRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    void ICommandRepository<TEntity>.Delete(Guid id)
    {
        var entity = DbContext.Set<TEntity>().Find(id);
        DbContext.Set<TEntity>().Remove(entity);
    }

    void ICommandRepository<TEntity>.Delete(TEntity entity) => DbContext.Set<TEntity>().Remove(entity);

    void ICommandRepository<TEntity>.DeleteGraph(Guid id)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        var query = DbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath) 
            query = query.Include(item);

        var entity = query.FirstOrDefault(c => c.Id.Value == id);
        if (entity?.Id?.Value != null && entity?.Id?.Value != Guid.Empty)
            DbContext.Set<TEntity>().Remove(entity);
    }

    TEntity ICommandRepository<TEntity>.Get(Guid id) => DbContext.Set<TEntity>().Find(id);

    public TEntity Get(IdVO id) => DbContext.Set<TEntity>().FirstOrDefault(c => c.Id == id);

    TEntity ICommandRepository<TEntity>.GetGraph(Guid id)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        var query = DbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath) 
            query = query.Include(item);

        return query.FirstOrDefault(c => c.Id.Value == id);
    }

    void ICommandRepository<TEntity>.Insert(TEntity entity) => DbContext.Set<TEntity>().Add(entity);

    bool ICommandRepository<TEntity>.Exists(Expression<Func<TEntity, bool>> expression) => DbContext.Set<TEntity>().Any(expression);

    public TEntity GetGraph(IdVO id)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        var query = DbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath) 
            query = query.Include(item);

        return query.FirstOrDefault(c => c.Id == id);
    }

    async Task ICommandRepository<TEntity>.InsertAsync(TEntity entity) => await DbContext.Set<TEntity>().AddAsync(entity);

    async Task<TEntity> ICommandRepository<TEntity>.GetAsync(Guid id) => await DbContext.Set<TEntity>().FindAsync(id);

    public async Task<TEntity> GetAsync(IdVO id) => await DbContext.Set<TEntity>().FirstOrDefaultAsync(c => c.Id == id);

    async Task<TEntity> ICommandRepository<TEntity>.GetGraphAsync(Guid id)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        var query = DbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath) 
            query = query.Include(item);

        return await query.FirstOrDefaultAsync(c => c.Id.Value == id);
    }

    public async Task<TEntity> GetGraphAsync(IdVO id)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        var query = DbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath) 
            query = query.Include(item);

        return await query.FirstOrDefaultAsync(c => c.Id == id);
    }

    async Task<bool> ICommandRepository<TEntity>.ExistsAsync(Expression<Func<TEntity, bool>> expression) 
        => await DbContext.Set<TEntity>().AnyAsync(expression);

    public int Commit() => DbContext.SaveChanges();

    public Task<int> CommitAsync() => DbContext.SaveChangesAsync();

    public void BeginTransaction() => DbContext.BeginTransaction();

    public void CommitTransaction() => DbContext.CommitTransaction();

    public void RollbackTransaction() => DbContext.RollbackTransaction();
}