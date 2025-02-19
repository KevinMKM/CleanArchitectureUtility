using CleanArchitectureUtility.Core.Contract.Data.Queries;

namespace CleanArchitectureUtility.Infra.Data.SqlQueries;

public class BaseQueryRepository<TDbContext> : IQueryRepository where TDbContext : BaseQueryDbContext
{
    protected readonly TDbContext _dbContext;

    public BaseQueryRepository(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}