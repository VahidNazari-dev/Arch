using Arch.Domian;

namespace Arch.Kafka.Cosumer;

public interface IMessageHandler<TEvent> where TEvent : Event
{
    Task Handle(TEvent @event, Dictionary<string, string> headers);
}
