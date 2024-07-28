

namespace Arch.Domain;

public interface IEventBus
{
    Task Execute<TEvent>(TEvent @event,
            Dictionary<string, string> headers,
            CancellationToken cancellationToken = default) where TEvent : Event;
}
