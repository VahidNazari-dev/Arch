

namespace Arch.Domain;

public interface IEventHandler
{
    public interface IEventHandler<T> where T : Event
    {
        Task HandleEvent(T @event);
    }
}
