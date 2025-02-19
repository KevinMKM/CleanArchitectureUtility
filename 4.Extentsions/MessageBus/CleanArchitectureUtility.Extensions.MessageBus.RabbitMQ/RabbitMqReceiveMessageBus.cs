using System.Diagnostics;
using CleanArchitectureUtility.Extensions.Abstractions.MessageBus;
using CleanArchitectureUtility.Extensions.MessageBus.RabbitMQ.Extensions;
using CleanArchitectureUtility.Extensions.MessageBus.RabbitMQ.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CleanArchitectureUtility.Extensions.MessageBus.RabbitMQ;

public class RabbitMqReceiveMessageBus : IReceiveMessageBus, IDisposable
{
    private readonly ILogger<RabbitMqReceiveMessageBus> _logger;
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _eventQueueName;
    private readonly string _commandQueueName;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RabbitMqReceiveMessageBus(IConnection connection, ILogger<RabbitMqReceiveMessageBus> logger, IOptions<RabbitMqOptions> rabbitMqOptions, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _rabbitMqOptions = rabbitMqOptions.Value;
        _serviceScopeFactory = serviceScopeFactory;
        _connection = connection;
        _channel = _connection.CreateChannelAsync().Result;
        _channel.ExchangeDeclareAsync(_rabbitMqOptions.ExchangeName, ExchangeType.Topic, true, false, null).Wait();
        _commandQueueName = $"{_rabbitMqOptions.ServiceName}.CommandsInputQueue";
        _eventQueueName = $"{_rabbitMqOptions.ServiceName}.EventsInputQueue";
        CreateCommandQueue();
        CreateEventQueue();
    }

    private void CreateEventQueue()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += Consumer_ReceivedAsync;
        var queue = _channel.QueueDeclareAsync(_eventQueueName, true, false, false).Result;
        _channel.BasicConsumeAsync(queue.QueueName, true, consumer).Wait();
        _logger.LogInformation($"Event Queue With Name {queue.QueueName} Created.");
    }

    private async Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs e)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        try
        {
            var span = StartChildActivity(e);
            _logger.LogDebug($"Event Received With RoutingKey: {e.RoutingKey}.");
            var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();
            await consumer.ConsumeEvent(e?.BasicProperties?.AppId ?? "NullAppId", e.ToParcel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    private void CreateCommandQueue()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += Consumer_CommandReceivedAsync;
        var queue = _channel.QueueDeclareAsync(_commandQueueName, true, false, false).Result;
        _channel.BasicConsumeAsync(queue.QueueName, true, consumer).Wait();
        _logger.LogInformation($"Command Queue With Name {queue.QueueName} Created.");
    }

    private async Task Consumer_CommandReceivedAsync(object sender, BasicDeliverEventArgs e)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        try
        {
            var span = StartChildActivity(e);
            _logger.LogDebug($"Command Received With RoutingKey: {e.RoutingKey}.");
            var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();
            await consumer.ConsumeCommand(e?.BasicProperties?.AppId ?? "NullAppId", e.ToParcel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public void Subscribe(string serviceId, string eventName)
    {
        var route = $"{serviceId}.{RabbitMqSendMessageBusConstants.@event}.{eventName}";
        _channel.QueueBindAsync(_eventQueueName, _rabbitMqOptions.ExchangeName, route).Wait();
        _logger.LogInformation($"ServiceId: {serviceId} With EventName: {eventName} Bound.");
    }

    public void Receive(string commandName)
    {
        var route = $"{_rabbitMqOptions.ServiceName}.{RabbitMqSendMessageBusConstants.command}.{commandName}";
        _channel.QueueBindAsync(_commandQueueName, _rabbitMqOptions.ExchangeName, route).Wait();
        _logger.LogInformation($"Command With CommandName: {commandName} Bound.");
    }


    private Activity StartChildActivity(BasicDeliverEventArgs e)
    {
        var span = new Activity("RabbitMqCommandReceived");
        span.AddTag("ApplicationName", _rabbitMqOptions.ServiceName);
        if (e.BasicProperties != null && e.BasicProperties.Headers != null && e.BasicProperties.Headers.ContainsKey("TraceId") && e.BasicProperties.Headers.ContainsKey("SpanId"))
            span.SetParentId($"00-{System.Text.Encoding.UTF8.GetString(e.BasicProperties.Headers["TraceId"] as byte[])}-{System.Text.Encoding.UTF8.GetString(e.BasicProperties.Headers["SpanId"] as byte[])}-00");
        span.Start();
        return span;
    }

    public void Dispose()
    {
        _channel.CloseAsync().Wait();
        _connection.CloseAsync().Wait();
    }
}