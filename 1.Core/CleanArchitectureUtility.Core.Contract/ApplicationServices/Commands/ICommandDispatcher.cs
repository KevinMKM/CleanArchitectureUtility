namespace CleanArchitectureUtility.Core.Contract.ApplicationServices.Commands;

public interface ICommandDispatcher
{
    Task<CommandResult> Send<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand;
    Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand<TData>;
}