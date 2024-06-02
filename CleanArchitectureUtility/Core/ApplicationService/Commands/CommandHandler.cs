using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Commands;
using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Common;
using CleanArchitectureUtility.Utilities.Utilities;

namespace CleanArchitectureUtility.Core.ApplicationService.Commands;

public abstract class CommandHandler<TCommand, TData> : ICommandHandler<TCommand, TData> where TCommand : ICommand<TData>
{
    private readonly CommonService _cs;
    private readonly CommandResult<TData> _result = new();

    protected CommandHandler(CommonService cs)
    {
        _cs = cs;
    }

    public abstract Task<CommandResult<TData>> Handle(TCommand request);

    protected virtual Task<CommandResult<TData>> OkAsync(TData data)
    {
        _result.Data = data;
        _result.Status = ApplicationServiceStatus.Ok;
        return Task.FromResult(_result);
    }

    protected virtual CommandResult<TData> Ok(TData data)
    {
        _result.Data = data;
        _result.Status = ApplicationServiceStatus.Ok;
        return _result;
    }

    protected virtual Task<CommandResult<TData>> ResultAsync(TData data, ApplicationServiceStatus status)
    {
        _result.Data = data;
        _result.Status = status;
        return Task.FromResult(_result);
    }

    protected virtual CommandResult<TData> Result(TData data, ApplicationServiceStatus status)
    {
        _result.Data = data;
        _result.Status = status;
        return _result;
    }

    protected void AddMessage(string message) => _result.AddMessage(_cs.Translator.GetString(message));

    protected void AddMessage(string message, params string[] arguments) => _result.AddMessage(_cs.Translator.GetString(message, arguments));
}

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
{
    private readonly CommonService _cs;
    private readonly CommandResult _result = new();

    protected CommandHandler(CommonService cs)
    {
        _cs = cs;
    }

    public abstract Task<CommandResult> Handle(TCommand request);

    protected virtual Task<CommandResult> OkAsync()
    {
        _result.Status = ApplicationServiceStatus.Ok;
        return Task.FromResult(_result);
    }

    protected virtual CommandResult Ok()
    {
        _result.Status = ApplicationServiceStatus.Ok;
        return _result;
    }

    protected virtual Task<CommandResult> ResultAsync(ApplicationServiceStatus status)
    {
        _result.Status = status;
        return Task.FromResult(_result);
    }

    protected virtual CommandResult Result(ApplicationServiceStatus status)
    {
        _result.Status = status;
        return _result;
    }

    protected void AddMessage(string message) => _result.AddMessage(_cs.Translator.GetString(message));

    protected void AddMessage(string message, params string[] arguments) => _result.AddMessage(_cs.Translator.GetString(message, arguments));
}