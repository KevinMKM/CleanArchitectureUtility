using CleanArchitectureUtility.Core.Contract.Data.Commands;

namespace CleanArchitectureUtility.Infra.Data.Sql;

public abstract class BaseEntityFrameworkUnitOfWork<TDbContext> : IUnitOfWork where TDbContext : BaseDbContext
{
    protected readonly TDbContext DbContext;

    protected BaseEntityFrameworkUnitOfWork(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public void BeginTransaction() => DbContext.BeginTransaction();

    public int Commit() => DbContext.SaveChanges();

    public async Task<int> CommitAsync() => await DbContext.SaveChangesAsync();

    public void CommitTransaction() => DbContext.CommitTransaction();

    public void RollbackTransaction() => DbContext.RollbackTransaction();
}