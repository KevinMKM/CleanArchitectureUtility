using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Common;

namespace CleanArchitectureUtility.Core.Contracts.ApplicationServices.Queries;

public sealed class QueryResult<TData> : ApplicationServiceResult
{
    public TData? Data { get; set; }
}