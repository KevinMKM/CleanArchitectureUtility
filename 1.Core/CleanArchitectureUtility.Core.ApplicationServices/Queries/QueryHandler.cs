using CleanArchitectureUtility.Core.Contract.ApplicationServices.Common;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Queries;

namespace CleanArchitectureUtility.Core.ApplicationServices.Queries;

public abstract class QueryHandler<TQuery, TData> : IQueryHandler<TQuery, TData> where TQuery : class, IQuery<TData>
{
    protected readonly QueryResult<TData> QueryResult = new();

    public abstract Task<QueryResult<TData>> Handle(TQuery request);

    protected virtual Task<QueryResult<TData>> ResultAsync(TData data, ApplicationServiceStatus status)
    {
        QueryResult._data = data;
        QueryResult.Status = status;
        return Task.FromResult(QueryResult);
    }

    protected virtual QueryResult<TData> Result(TData data, ApplicationServiceStatus status)
    {
        QueryResult._data = data;
        QueryResult.Status = status;
        return QueryResult;
    }

    protected virtual Task<QueryResult<TData>> ResultAsync(TData data)
    {
        var status = data != null ? ApplicationServiceStatus.Ok : ApplicationServiceStatus.NotFound;
        return ResultAsync(data, status);
    }

    protected virtual QueryResult<TData> Result(TData data)
    {
        var status = data != null ? ApplicationServiceStatus.Ok : ApplicationServiceStatus.NotFound;
        return Result(data, status);
    }
}