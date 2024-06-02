namespace CleanArchitectureUtility.Core.Contracts.ApplicationServices.Queries;

public interface IQueryHandler<in TQuery, TData> where TQuery : class, IQuery<TData>
{
    Task<QueryResult<TData>> Handle(TQuery request);
}