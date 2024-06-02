using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Common;

namespace CleanArchitectureUtility.Core.Contracts.ApplicationServices.Commands;

public class CommandResult : ApplicationServiceResult
{
}

public class CommandResult<TData> : CommandResult
{
    public TData? Data { get; set; }
}