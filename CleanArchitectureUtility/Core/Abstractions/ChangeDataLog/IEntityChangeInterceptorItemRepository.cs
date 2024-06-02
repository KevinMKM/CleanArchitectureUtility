namespace CleanArchitectureUtility.Core.Abstractions.ChangeDataLog;

public interface IEntityChangeInterceptorItemRepository
{
    public void Save(List<EntityChangeInterceptorItem> entityChangeInterceptorItems);
    public Task SaveAsync(List<EntityChangeInterceptorItem> entityChangeInterceptorItems);
}