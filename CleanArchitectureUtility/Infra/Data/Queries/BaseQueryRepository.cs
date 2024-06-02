using CleanArchitectureUtility.Core.Contracts.Data.Queries;

namespace CleanArchitectureUtility.Infra.Data.Queries;

public class BaseQueryRepository<TDbContext> : IQueryRepository where TDbContext : BaseQueryDbContext
{
    protected readonly TDbContext DbContext;

    public BaseQueryRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }
}