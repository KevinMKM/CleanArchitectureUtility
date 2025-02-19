using System.Data;

namespace CleanArchitectureUtility.Extensions.Abstractions.ChangeDataLog;

public interface IEntityChangeInterceptorItemRepository
{
    public void Save(List<EntityChangeInterceptorItem> entityChangeInterceptorItems, IDbTransaction transaction);
    public Task SaveAsync(List<EntityChangeInterceptorItem> entityChangeInterceptorItems, IDbTransaction transaction);
}