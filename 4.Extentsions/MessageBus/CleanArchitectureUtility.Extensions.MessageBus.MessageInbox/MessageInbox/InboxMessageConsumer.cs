using CleanArchitectureUtility.Extensions.MessageBus.MessageInbox.MessageInbox.Options;
using Microsoft.Extensions.Options;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Commands;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Events;
using CleanArchitectureUtility.Core.Domain.Events;
using CleanArchitectureUtility.Extensions.Abstractions.MessageBus;
using CleanArchitectureUtility.Extensions.Abstractions.Serializers;

namespace CleanArchitectureUtility.Extensions.MessageBus.MessageInbox.MessageInbox;
public class InboxMessageConsumer : IMessageConsumer
{
    private readonly MessageInboxOptions _messageInboxOptions;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IMessageInboxItemRepository _messageInboxItemRepository;
    private readonly List<Type> _domainEventTypes = new List<Type>();
    private readonly List<Type> _commandTypes = new List<Type>();
    public InboxMessageConsumer(IOptions<MessageInboxOptions> messageInboxOptions,
                                IJsonSerializer jsonSerializer,
                                IMessageInboxItemRepository messageInboxItemRepository,
                                ICommandDispatcher commandDispatcher,
                                IEventDispatcher eventDispatcher)
    {
        _messageInboxOptions = messageInboxOptions.Value;
        _eventDispatcher = eventDispatcher;
        _jsonSerializer = jsonSerializer;
        _commandDispatcher = commandDispatcher;
        _messageInboxItemRepository = messageInboxItemRepository;
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        _domainEventTypes.AddRange(assemblies.SelectMany(assembly => assembly.GetTypes().Where(c => c.IsAssignableTo(typeof(IDomainEvent)) && c.IsClass).ToList()).ToList());
        _commandTypes.AddRange(assemblies.SelectMany(assembly => assembly.GetTypes().Where(c => c.IsAssignableTo(typeof(ICommand)) && c.IsClass).ToList()).ToList());
    }
    public async Task<bool> ConsumeEvent(string sender, Parcel parcel)
    {
        if (_messageInboxItemRepository.AllowReceive(parcel.MessageId, sender))
        {
            var eventType = _domainEventTypes.FirstOrDefault(c => c.Name == parcel.MessageName);
            if (eventType != null)
            {
                dynamic @event = _jsonSerializer.Deserialize(parcel.MessageBody, eventType);
                await _eventDispatcher.PublishDomainEventAsync(@event);
                _messageInboxItemRepository.Receive(parcel.MessageId, sender, parcel.MessageBody);
            }
        }
        return true;
    }

    public Task<bool> ConsumeCommand(string sender, Parcel parcel)
    {
        throw new NotImplementedException();
    }
}