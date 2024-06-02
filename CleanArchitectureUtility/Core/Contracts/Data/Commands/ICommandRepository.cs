using System.Linq.Expressions;
using CleanArchitectureUtility.Core.Domain.Entities;
using CleanArchitectureUtility.Core.Domain.ValueObjects;

namespace CleanArchitectureUtility.Core.Contracts.Data.Commands;

public interface ICommandRepository<TEntity> : IUnitOfWork where TEntity : AggregateRoot
{
    void Delete(long id);
    void DeleteGraph(long id);
    void Delete(TEntity entity);
    void Insert(TEntity entity);
    Task InsertAsync(TEntity entity);
    TEntity? Get(long id);
    Task<TEntity?> GetAsync(long id);
    TEntity? Get(BusinessId businessId);
    Task<TEntity?> GetAsync(BusinessId businessId);
    TEntity? GetGraph(long id);
    Task<TEntity?> GetGraphAsync(long id);
    TEntity? GetGraph(BusinessId businessId);
    Task<TEntity?> GetGraphAsync(BusinessId businessId);
    bool Exists(Expression<Func<TEntity, bool>> expression);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression);
}