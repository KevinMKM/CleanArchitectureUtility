namespace CleanArchitectureUtility.Core.Contract.ApplicationServices.Queries;

public interface IQueryDispatcher
{
    Task<QueryResult<TData>> Execute<TQuery, TData>(TQuery query, CancellationToken cancellationToken) where TQuery : class, IQuery<TData>;
}