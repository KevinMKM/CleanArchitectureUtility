using System.Diagnostics;
using System.Text;
using CleanArchitectureUtility.Extensions.Abstractions.MessageBus;
using CleanArchitectureUtility.Extensions.Abstractions.Serializers;
using CleanArchitectureUtility.Extensions.MessageBus.RabbitMQ.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace CleanArchitectureUtility.Extensions.MessageBus.RabbitMQ;

public class RabbitMqSendMessageBus : IDisposable, ISendMessageBus
{
    private readonly IChannel _channel;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ILogger<RabbitMqSendMessageBus> _logger;
    private readonly RabbitMqOptions _rabbitMqOptions;

    public RabbitMqSendMessageBus(IConnection connection, IJsonSerializer jsonSerializer, IOptions<RabbitMqOptions> rabbitMqOptions, ILogger<RabbitMqSendMessageBus> logger)
    {
        _jsonSerializer = jsonSerializer;
        _logger = logger;
        _rabbitMqOptions = rabbitMqOptions.Value;
        _channel = connection.CreateChannelAsync().Result;
        _channel.ExchangeDeclareAsync(_rabbitMqOptions.ExchangeName, ExchangeType.Topic, true, false, null).Wait();
    }

    public void Publish<TInput>(TInput input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        var messageName = input.GetType().Name;
        Parcel parcel = new()
        {
            MessageId = Guid.NewGuid().ToString(),
            MessageBody = _jsonSerializer.Serialize(input),
            MessageName = messageName,
            Route = $"{_rabbitMqOptions.ServiceName}.{RabbitMqSendMessageBusConstants.@event}.{messageName}",
            Headers = new Dictionary<string, object>
            {
                ["OccurredOn"] = DateTime.Now.ToString()
            }
        };
        Send(parcel);
    }

    public void SendCommandTo<TCommandData>(string destinationService, string commandName, TCommandData commandData)
    {
        if (commandData == null)
            throw new ArgumentNullException(nameof(commandData));

        Parcel parcel = new()
        {
            MessageId = Guid.NewGuid().ToString(),
            MessageBody = _jsonSerializer.Serialize(commandData),
            MessageName = commandName,
            Route = $"{destinationService}.{RabbitMqSendMessageBusConstants.command}.{commandName}"
        };
        Send(parcel);
    }

    public void SendCommandTo<TCommandData>(string destinationService, string commandName, string correlationId, TCommandData commandData)
    {
        if (commandData == null)
            throw new ArgumentNullException(nameof(commandData));

        Parcel parcel = new()
        {
            MessageId = Guid.NewGuid().ToString(),
            CorrelationId = correlationId,
            MessageBody = _jsonSerializer.Serialize(commandData),
            MessageName = commandName,
            Route = $"{destinationService}.{RabbitMqSendMessageBusConstants.command}.{commandName}"
        };
        Send(parcel);
    }

    public void Send(Parcel parcel)
    {
        ArgumentNullException.ThrowIfNull(parcel);
        var childActivity = StartChildActivity(parcel);
        AddActivityHeaders(parcel, childActivity);
        var basicProperties = new BasicProperties
        {
            Persistent = _rabbitMqOptions.PersistMessage,
            AppId = _rabbitMqOptions.ServiceName,
            CorrelationId = parcel?.CorrelationId,
            MessageId = parcel?.MessageId,
            Headers = parcel?.Headers,
            Type = parcel?.MessageName ?? "NullMessageName"
        };
        _channel.BasicPublishAsync(_rabbitMqOptions.ExchangeName, parcel.Route, _rabbitMqOptions.IsMessageMandatory, basicProperties, Encoding.ASCII.GetBytes(parcel.MessageBody));
        _logger.LogDebug($"Message Sent {parcel.MessageName}");
    }

    private static void AddActivityHeaders(Parcel parcel, Activity childActivity)
    {
        if (parcel.Headers is null)
            parcel.Headers = new Dictionary<string, object>();

        parcel.Headers["TraceId"] = childActivity.TraceId.ToHexString();
        parcel.Headers["SpanId"] = childActivity.SpanId.ToHexString();
    }

    private Activity StartChildActivity(Parcel parcel)
    {
        var child = new Activity("SendParcel");
        child.AddTag("ParcelName", parcel.MessageName);
        child.AddTag("ApplicationName", _rabbitMqOptions.ServiceName);
        child.Start();
        return child;
    }

    public void Dispose()
    {
        if (_channel != null)
        {
            if (_channel.IsOpen)
                _channel.CloseAsync().Wait();
            _channel.Dispose();
        }
    }
}