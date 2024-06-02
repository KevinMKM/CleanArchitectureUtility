using System.Diagnostics;
using CleanArchitectureUtility.Core.Abstractions.Logger;
using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Core.ApplicationService.Commands;

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
            _logger.LogDebug($"Routing command of type {command.GetType()} With value {command}  Start at {DateTime.Now}.");
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
            return await handler.Handle(command);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"There is not suitable handler for {command.GetType()} Routing failed at {DateTime.Now}.");
            throw;
        }
        finally
        {
            _stopwatch.Stop();
            _logger.LogInformation(LogEventId.PerformanceMeasurement,
                $"Processing the {command.GetType()} command took {_stopwatch.ElapsedMilliseconds} Milliseconds");
        }
    }

    public async Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command) where TCommand : class, ICommand<TData>
    {
        _stopwatch.Start();
        try
        {
            _logger.LogDebug($"Routing command of type {command.GetType()} With value {command}  Start at {DateTime.Now}.");
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TData>>();
            return await handler.Handle(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"There is not suitable handler for {command.GetType()} Routing failed at {DateTime.Now}.");
            throw;
        }
        finally
        {
            _stopwatch.Stop();
            _logger.LogInformation(LogEventId.PerformanceMeasurement,
                $"Processing the {command.GetType()} command took {_stopwatch.ElapsedMilliseconds} Milliseconds");
        }
    }
}