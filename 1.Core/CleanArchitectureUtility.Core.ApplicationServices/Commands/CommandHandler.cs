using CleanArchitectureUtility.Core.Contract.ApplicationServices.Commands;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Common;
using CleanArchitectureUtility.Extensions.Abstractions.Translations;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Core.ApplicationServices.Commands;

public abstract class CommandHandler<TCommand, TData> : ICommandHandler<TCommand, TData> where TCommand : ICommand<TData>
{
    private readonly ITranslator _translator;
    protected readonly CommandResult<TData> CommandResult = new();

    protected CommandHandler(IServiceProvider serviceProvider)
    {
        _translator = serviceProvider.GetService<ITranslator>();
    }
    
    public abstract Task<CommandResult<TData>> Handle(TCommand request, CancellationToken cancellationToken);

    protected virtual Task<CommandResult<TData>> OkAsync(TData data)
    {
        CommandResult._data = data;
        CommandResult.Status = ApplicationServiceStatus.Ok;
        return Task.FromResult(CommandResult);
    }

    protected virtual CommandResult<TData> Ok(TData data)
    {
        CommandResult._data = data;
        CommandResult.Status = ApplicationServiceStatus.Ok;
        return CommandResult;
    }

    protected virtual Task<CommandResult<TData>> ResultAsync(TData data, ApplicationServiceStatus status)
    {
        CommandResult._data = data;
        CommandResult.Status = status;
        return Task.FromResult(CommandResult);
    }

    protected virtual CommandResult<TData> Result(TData data, ApplicationServiceStatus status)
    {
        CommandResult._data = data;
        CommandResult.Status = status;
        return CommandResult;
    }

    protected void AddMessage(string message) => CommandResult.AddMessage(_translator[message]);

    protected void AddMessage(string message, params string[] arguments) => CommandResult.AddMessage(_translator[message, arguments]);
}

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
{
    private readonly ITranslator _translator;
    protected readonly CommandResult CommandResult = new();

    protected CommandHandler(IServiceProvider serviceProvider)
    {
        _translator = serviceProvider.GetService<ITranslator>();
    }

    public abstract Task<CommandResult> Handle(TCommand request, CancellationToken cancellationToken);

    protected virtual Task<CommandResult> OkAsync()
    {
        CommandResult.Status = ApplicationServiceStatus.Ok;
        return Task.FromResult(CommandResult);
    }

    protected virtual CommandResult Ok()
    {
        CommandResult.Status = ApplicationServiceStatus.Ok;
        return CommandResult;
    }

    protected virtual Task<CommandResult> ResultAsync(ApplicationServiceStatus status)
    {
        CommandResult.Status = status;
        return Task.FromResult(CommandResult);
    }

    protected virtual CommandResult Result(ApplicationServiceStatus status)
    {
        CommandResult.Status = status;
        return CommandResult;
    }

    protected void AddMessage(string message) => CommandResult.AddMessage(_translator[message]);

    protected void AddMessage(string message, params string[] arguments) => CommandResult.AddMessage(_translator[message, arguments]);
}