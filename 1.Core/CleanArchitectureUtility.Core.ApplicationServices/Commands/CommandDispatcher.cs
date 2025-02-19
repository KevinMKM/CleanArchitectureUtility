using System.Diagnostics;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Commands;
using CleanArchitectureUtility.Extensions.Abstractions.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Core.ApplicationServices.Commands;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CommandDispatcher> _logger;
    private readonly Stopwatch _stopwatch;

    public CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _stopwatch = new Stopwatch();
        _logger = logger;
    }

    public async Task<CommandResult> Send<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        _stopwatch.Start();
        try
        {
            _logger.LogDebug($"Routing command of type {command.GetType()} With value {command}  Start at {DateTime.UtcNow}");
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
            return await handler.Handle(command);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"There is not suitable handler for {command.GetType()} Routing failed at {DateTime.UtcNow}");
            throw;
        }
        finally
        {
            _stopwatch.Stop();
            _logger.LogInformation(LoggerEventId.PerformanceMeasurement, $"Processing the {command.GetType()} command took {_stopwatch.ElapsedMilliseconds} Milliseconds");
        }
    }

    public async Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command)
        where TCommand : class, ICommand<TData>
    {
        _stopwatch.Start();
        try
        {
            _logger.LogDebug($"Routing command of type {command.GetType()} With value {command}  Start at {DateTime.UtcNow}");
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TData>>();
            return await handler.Handle(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"There is not suitable handler for {command.GetType()} Routing failed at {DateTime.UtcNow}");
            throw;
        }
        finally
        {
            _stopwatch.Stop();
            _logger.LogInformation(LoggerEventId.PerformanceMeasurement, $"Processing the {command.GetType()} command took {_stopwatch.ElapsedMilliseconds} Milliseconds");
        }
    }
}