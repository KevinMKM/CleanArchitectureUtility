using CleanArchitectureUtility.Core.Contract.ApplicationServices.Commands;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Common;
using CleanArchitectureUtility.Core.Domain.Exceptions;
using CleanArchitectureUtility.Extensions.Abstractions.Loggers;
using Microsoft.Extensions.Logging;
using CleanArchitectureUtility.Extensions.Abstractions.Translations;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Core.ApplicationServices.Commands;

public class CommandDispatcherDomainExceptionHandlerDecorator : CommandDispatcherDecorator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CommandDispatcherDomainExceptionHandlerDecorator> _logger;

    public CommandDispatcherDomainExceptionHandlerDecorator(ICommandDispatcher commandDispatcher,
        ILogger<CommandDispatcherDomainExceptionHandlerDecorator> logger, 
        IServiceProvider serviceProvider)
        : base(commandDispatcher)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public override async Task<CommandResult> Send<TCommand>(TCommand command)
    {
        try
        {
            var result = CommandDispatcher.Send(command);
            return await result;
        }
        catch (DomainStateException ex)
        {
            _logger.LogError(LoggerEventId.DomainValidationException,
                ex,
                $"Processing of {command.GetType()} With value {command} failed at {DateTime.UtcNow} because there are domain exceptions");
            return DomainExceptionHandlingWithoutReturnValue<TCommand>(ex);
        }
        catch (AggregateException ex)
        {
            if (ex.InnerException is not DomainStateException domainStateException)
                throw;
            _logger.LogError(LoggerEventId.DomainValidationException,
                domainStateException,
                $"Processing of {command.GetType()} With value {command} failed at {DateTime.UtcNow} because there are domain exceptions");
            return DomainExceptionHandlingWithoutReturnValue<TCommand>(domainStateException);
        }
    }

    public override async Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command)
    {
        try
        {
            var result = await CommandDispatcher.Send<TCommand, TData>(command);
            return result;
        }
        catch (DomainStateException ex)
        {
            _logger.LogError(LoggerEventId.DomainValidationException,
                ex,
                $"Processing of {command.GetType()} With value {command} failed at {DateTime.UtcNow} because there are domain exceptions");
            return DomainExceptionHandlingWithReturnValue<TCommand, TData>(ex);
        }
        catch (AggregateException ex)
        {
            if (ex.InnerException is not DomainStateException domainStateException)
                throw;
            _logger.LogError(LoggerEventId.DomainValidationException,
                domainStateException,
                $"Processing of {command.GetType()} With value {command} failed at {DateTime.UtcNow} because there are domain exceptions");
            return DomainExceptionHandlingWithReturnValue<TCommand, TData>(domainStateException);
        }
    }

    private CommandResult DomainExceptionHandlingWithoutReturnValue<TCommand>(DomainStateException ex)
    {
        var commandResult = new CommandResult
        {
            Status = ApplicationServiceStatus.InvalidDomainState
        };

        commandResult.AddMessage(GetExceptionText(ex));

        return commandResult;
    }

    private CommandResult<TData> DomainExceptionHandlingWithReturnValue<TCommand, TData>(DomainStateException ex)
    {
        var commandResult = new CommandResult<TData>()
        {
            Status = ApplicationServiceStatus.InvalidDomainState
        };

        commandResult.AddMessage(GetExceptionText(ex));

        return commandResult;
    }

    private string GetExceptionText(DomainStateException domainStateException)
    {
        var translator = _serviceProvider.GetService<ITranslator>();
        if (translator == null)
            return domainStateException.ToString();

        var result = translator[domainStateException.Message];

        _logger.LogInformation(LoggerEventId.DomainValidationException, $"Domain Exception message is {result}");

        return result;
    }
}