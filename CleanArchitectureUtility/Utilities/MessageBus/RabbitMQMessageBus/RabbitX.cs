using System.Text;
using CleanArchitectureUtility.Core.Abstractions.MessageBus;
using RabbitMQ.Client.Events;

namespace CleanArchitectureUtility.Utilities.MessageBus.RabbitMQMessageBus;

static class RabbitX
{
    public static Parcel ToParcel(this BasicDeliverEventArgs basicDeliverEventArgs)
    {
        Parcel parcel = new()
        {
            CorrelationId = basicDeliverEventArgs?.BasicProperties?.CorrelationId,
            MessageId = basicDeliverEventArgs?.BasicProperties?.MessageId,
            Route = basicDeliverEventArgs?.RoutingKey,
            MessageBody = Encoding.UTF8.GetString(basicDeliverEventArgs.Body.ToArray()),
            MessageName = basicDeliverEventArgs.BasicProperties?.Type,
            Headers = (Dictionary<string, object>)basicDeliverEventArgs?.BasicProperties?.Headers
        };
        return parcel;
    }
}