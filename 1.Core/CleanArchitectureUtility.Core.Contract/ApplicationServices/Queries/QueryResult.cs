using CleanArchitectureUtility.Core.Contract.ApplicationServices.Common;

namespace CleanArchitectureUtility.Core.Contract.ApplicationServices.Queries;

public sealed class QueryResult<TData> : ApplicationServiceResult
{
    public TData? _data;
    public TData? Data => _data;
}