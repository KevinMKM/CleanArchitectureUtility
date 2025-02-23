using CleanArchitectureUtility.Core.Contract.Data.Queries;

namespace CleanArchitectureUtility.Infra.Data.SqlQueries;

public class BaseQueryRepository<TDbContext> : IQueryRepository where TDbContext : BaseQueryDbContext
{
    protected readonly TDbContext DbContext;

    public BaseQueryRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }
}