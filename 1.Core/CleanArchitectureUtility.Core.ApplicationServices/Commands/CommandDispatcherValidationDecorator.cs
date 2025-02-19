using CleanArchitectureUtility.Core.Contract.ApplicationServices.Commands;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Common;
using CleanArchitectureUtility.Extensions.Abstractions.Loggers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Core.ApplicationServices.Commands;

public class CommandDispatcherValidationDecorator : CommandDispatcherDecorator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CommandDispatcherValidationDecorator> _logger;

    public CommandDispatcherValidationDecorator(ICommandDispatcher commandDispatcher,
        IServiceProvider serviceProvider,
        ILogger<CommandDispatcherValidationDecorator> logger)
        : base(commandDispatcher)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public override async Task<CommandResult> Send<TCommand>(TCommand command)
    {
        _logger.LogDebug(LoggerEventId.CommandValidation,
            $"Validating command of type {command.GetType()} With value {command}  start at :{DateTime.UtcNow}");
        var validationResult = Validate<TCommand, CommandResult>(command);

        if (validationResult != null)
        {
            _logger.LogInformation(LoggerEventId.CommandValidation,
                $"Validating command of type {command.GetType()} With value {command}  failed. Validation errors are: {validationResult.Messages}");
            return validationResult;
        }

        _logger.LogDebug(LoggerEventId.CommandValidation,
            $"Validating command of type {command.GetType()} With value {command}  finished at :{DateTime.UtcNow}");
        return await CommandDispatcher.Send(command);
    }

    public override async Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command)
    {
        _logger.LogDebug(LoggerEventId.CommandValidation,
            $"Validating command of type {command.GetType()} With value {command}  start at :{DateTime.UtcNow}");

        var validationResult = Validate<TCommand, CommandResult<TData>>(command);

        if (validationResult != null)
        {
            _logger.LogInformation(LoggerEventId.CommandValidation,
                $"Validating command of type {command.GetType()} With value {command}  failed. Validation errors are: {validationResult.Messages}");
            return validationResult;
        }

        _logger.LogDebug(LoggerEventId.CommandValidation,
            $"Validating command of type {command.GetType()} With value {command}  finished at :{DateTime.UtcNow}");
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
                res.AddMessage(item.ErrorMessage);
        }
        else
            _logger.LogInformation(LoggerEventId.CommandValidation, $"There is not any validator for {command?.GetType()}");

        return res;
    }
}