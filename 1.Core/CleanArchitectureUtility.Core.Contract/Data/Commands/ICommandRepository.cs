using System.Linq.Expressions;
using CleanArchitectureUtility.Core.Domain.Entities;
using CleanArchitectureUtility.Core.Domain.ValueObjects;

namespace CleanArchitectureUtility.Core.Contract.Data.Commands;

public interface ICommandRepository<TEntity> : IUnitOfWork where TEntity : AggregateRoot
{
    void Delete(Guid id);
    void DeleteGraph(Guid id);
    void Delete(TEntity entity);
    void Insert(TEntity entity);
    Task InsertAsync(TEntity entity);
    TEntity Get(Guid id);
    Task<TEntity> GetAsync(Guid id);
    TEntity Get(IdVO id);
    Task<TEntity> GetAsync(IdVO id);
    TEntity GetGraph(Guid id);
    Task<TEntity> GetGraphAsync(Guid id);
    TEntity GetGraph(IdVO id);
    Task<TEntity?> GetGraphAsync(IdVO id);
    bool Exists(Expression<Func<TEntity, bool>> expression);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression);
}