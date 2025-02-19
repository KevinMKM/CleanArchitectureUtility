using CleanArchitectureUtility.Core.Contract.ApplicationServices.Common;

namespace CleanArchitectureUtility.Core.Contract.ApplicationServices.Commands;

public class CommandResult : ApplicationServiceResult
{
}

public class CommandResult<TData> : CommandResult
{
    public TData? _data;
    public TData? Data => _data;
}