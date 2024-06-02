namespace CleanArchitectureUtility.Core.Abstractions.MessageBus;

public class Parcel
{
    public string MessageId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string MessageName { get; set; } = string.Empty;
    public Dictionary<string, object> Headers { get; set; } = new();
    public string MessageBody { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public override string ToString() 
        => $"MessageId : {MessageId} , CorrelationId : {CorrelationId} , Route : {Route} , MessageBody : {MessageBody}";
}