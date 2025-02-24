﻿namespace CleanArchitectureUtility.Core.Contract.ApplicationServices.Commands;

public interface ICommandHandler<TCommand, TData> where TCommand : ICommand<TData>
{
    Task<CommandResult<TData>> Handle(TCommand request, CancellationToken cancellationToken);
}

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task<CommandResult> Handle(TCommand request, CancellationToken cancellationToken);
}