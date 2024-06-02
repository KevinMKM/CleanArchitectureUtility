using CleanArchitectureUtility.Core.Abstractions.Logger;
using CleanArchitectureUtility.Core.Abstractions.Translations;
using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Commands;
using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Common;
using CleanArchitectureUtility.Core.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Core.ApplicationService.Commands;

public class CommandDispatcherDomainExceptionHandlerDecorator : CommandDispatcherDecorator
{
    private readonly ITranslator _translator;
    private readonly ILogger<CommandDispatcherDomainExceptionHandlerDecorator> _logger;

    public CommandDispatcherDomainExceptionHandlerDecorator(ICommandDispatcher commandDispatcher,
        ILogger<CommandDispatcherDomainExceptionHandlerDecorator> logger, 
        ITranslator translator)
        : base(commandDispatcher)
    {
        _logger = logger;
        _translator = translator;
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
            _logger.LogError(LogEventId.DomainValidationException, 
                ex, 
                $"Processing of {command.GetType()} With value {command} failed at {DateTime.Now} because there are domain exceptions.");
            return DomainExceptionHandlingWithoutReturnValue(ex);
        }
        catch (AggregateException ex)
        {
            if (ex.InnerException is not DomainStateException domainStateException)
                throw;
            _logger.LogError(LogEventId.DomainValidationException, 
                domainStateException, 
                $"Processing of {command.GetType()} With value {command} failed at {DateTime.Now} because there are domain exceptions.");
            return DomainExceptionHandlingWithoutReturnValue(domainStateException);
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
            _logger.LogError(LogEventId.DomainValidationException, 
                ex, 
                $"Processing of {command.GetType()} With value {command} failed at {DateTime.Now} because there are domain exceptions.");
            return DomainExceptionHandlingWithReturnValue<TData>(ex);
        }
        catch (AggregateException ex)
        {
            if (ex.InnerException is not DomainStateException domainStateException)
                throw;
            _logger.LogError(LogEventId.DomainValidationException, 
                ex, 
                $"Processing of {command.GetType()} With value {command} failed at {DateTime.Now} because there are domain exceptions.");
            return DomainExceptionHandlingWithReturnValue<TData>(domainStateException);
        }
    }

    private CommandResult DomainExceptionHandlingWithoutReturnValue(DomainStateException ex)
    {
        var commandResult = new CommandResult
        {
            Status = ApplicationServiceStatus.InvalidDomainState
        };

        commandResult.AddMessage(GetExceptionText(ex));

        return commandResult;
    }

    private CommandResult<TData> DomainExceptionHandlingWithReturnValue<TData>(DomainStateException ex)
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
        var result = domainStateException?.Parameters.Any() == true
            ? _translator.GetString(domainStateException?.Message, domainStateException?.Parameters)
            : _translator.GetString(domainStateException?.Message);

        _logger.LogInformation(LogEventId.DomainValidationException, $"Domain Exception message is {result}");

        return result;
    }
}