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
    protected readonly TDbContext _dbContext;

    public BaseCommandRepository(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    void ICommandRepository<TEntity>.Delete(Guid id)
    {
        var entity = _dbContext.Set<TEntity>().Find(id);
        _dbContext.Set<TEntity>().Remove(entity);
    }

    void ICommandRepository<TEntity>.Delete(TEntity entity)
    {
        _dbContext.Set<TEntity>().Remove(entity);
    }

    void ICommandRepository<TEntity>.DeleteGraph(Guid id)
    {
        var graphPath = _dbContext.GetIncludePaths(typeof(TEntity));
        var query = _dbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath) 
            query = query.Include(item);

        var entity = query.FirstOrDefault(c => c.Id.Value == id);
        if (entity?.Id?.Value != null && entity?.Id?.Value != Guid.Empty)
            _dbContext.Set<TEntity>().Remove(entity);
    }

    TEntity ICommandRepository<TEntity>.Get(Guid id) => _dbContext.Set<TEntity>().Find(id);

    public TEntity Get(IdVO id) => _dbContext.Set<TEntity>().FirstOrDefault(c => c.Id == id);

    TEntity ICommandRepository<TEntity>.GetGraph(Guid id)
    {
        var graphPath = _dbContext.GetIncludePaths(typeof(TEntity));
        var query = _dbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath) 
            query = query.Include(item);

        return query.FirstOrDefault(c => c.Id.Value == id);
    }

    void ICommandRepository<TEntity>.Insert(TEntity entity)
    {
        _dbContext.Set<TEntity>().Add(entity);
    }

    bool ICommandRepository<TEntity>.Exists(Expression<Func<TEntity, bool>> expression)
    {
        return _dbContext.Set<TEntity>().Any(expression);
    }

    public TEntity GetGraph(IdVO id)
    {
        var graphPath = _dbContext.GetIncludePaths(typeof(TEntity));
        var query = _dbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath) 
            query = query.Include(item);

        return query.FirstOrDefault(c => c.Id == id);
    }

    async Task ICommandRepository<TEntity>.InsertAsync(TEntity entity)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity);
    }

    async Task<TEntity> ICommandRepository<TEntity>.GetAsync(Guid id) => 
        await _dbContext.Set<TEntity>().FindAsync(id);

    public async Task<TEntity> GetAsync(IdVO id) 
        => await _dbContext.Set<TEntity>().FirstOrDefaultAsync(c => c.Id == id);

    async Task<TEntity> ICommandRepository<TEntity>.GetGraphAsync(Guid id)
    {
        var graphPath = _dbContext.GetIncludePaths(typeof(TEntity));
        var query = _dbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath) 
            query = query.Include(item);

        return await query.FirstOrDefaultAsync(c => c.Id.Value == id);
    }

    public async Task<TEntity> GetGraphAsync(IdVO id)
    {
        var graphPath = _dbContext.GetIncludePaths(typeof(TEntity));
        var query = _dbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath) 
            query = query.Include(item);

        return await query.FirstOrDefaultAsync(c => c.Id == id);
    }

    async Task<bool> ICommandRepository<TEntity>.ExistsAsync(Expression<Func<TEntity, bool>> expression) => 
        await _dbContext.Set<TEntity>().AnyAsync(expression);

    public int Commit() => _dbContext.SaveChanges();

    public Task<int> CommitAsync() => _dbContext.SaveChangesAsync();

    public void BeginTransaction()
    {
        _dbContext.BeginTransaction();
    }

    public void CommitTransaction()
    {
        _dbContext.CommitTransaction();
    }

    public void RollbackTransaction()
    {
        _dbContext.RollbackTransaction();
    }
}