using CleanArchitectureUtility.Core.Abstractions.Logger;
using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Commands;
using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace CleanArchitectureUtility.Core.ApplicationService.Commands;

public class CommandDispatcherValidationDecorator : CommandDispatcherDecorator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CommandDispatcherValidationDecorator> _logger;

    public CommandDispatcherValidationDecorator(CommandDispatcherDomainExceptionHandlerDecorator commandDispatcher,
        IServiceProvider serviceProvider,
        ILogger<CommandDispatcherValidationDecorator> logger)
        : base(commandDispatcher)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public override async Task<CommandResult> Send<TCommand>(TCommand command)
    {
        _logger.LogDebug(LogEventId.CommandValidation,
            $"Validating command of type {command.GetType()} With value {command}  start at :{DateTime.Now}");
        var validationResult = Validate<TCommand, CommandResult>(command);
        if (validationResult != null)
        {
            _logger.LogInformation(LogEventId.CommandValidation,
                $"Validating command of type {command.GetType()} With value {command}  failed. Validation errors are: {validationResult.Messages}");
            return validationResult;
        }

        _logger.LogDebug(LogEventId.CommandValidation,
            $"Validating command of type {command.GetType()} With value {command}  finished at :{DateTime.Now}");
        return await CommandDispatcher.Send(command);
    }

    public override async Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command)
    {
        _logger.LogDebug(LogEventId.CommandValidation,
            $"Validating command of type {command.GetType()} With value {command}  start at :{DateTime.Now}");
        var validationResult = Validate<TCommand, CommandResult<TData>>(command);
        if (validationResult != null)
        {
            _logger.LogInformation(LogEventId.CommandValidation,
                $"Validating command of type {command.GetType()} With value {command}  failed. Validation errors are: {validationResult.Messages}");
            return validationResult;
        }

        _logger.LogDebug(LogEventId.CommandValidation,
            $"Validating command of type {command.GetType()} With value {command}  finished at :{DateTime.Now}");
        return await CommandDispatcher.Send<TCommand, TData>(command);
    }

    private TValidationResult? Validate<TCommand, TValidationResult>(TCommand command) where TValidationResult : ApplicationServiceResult, new()
    {
        var validator = _serviceProvider.GetService<IValidator<TCommand>>();
        TValidationResult? res = null;
        if (validator != null)
        {
            var validationResult = validator.Validate(command);
            if (validationResult.IsValid)
                return res;
            res = new TValidationResult
            {
                Status = ApplicationServiceStatus.ValidationError
            };
            foreach (var item in validationResult.Errors)
            {
                res.AddMessage(item.ErrorMessage);
            }
        }
        else
        {
            _logger.LogInformation(LogEventId.CommandValidation, $"There is not any validator for {command?.GetType()}");
        }

        return res;
    }
}