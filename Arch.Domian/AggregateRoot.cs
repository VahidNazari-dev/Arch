

namespace Arch.Domian;

public abstract class AggregateRoot<T>:Entity<T>
{
    public List<Event> Events { get; private set; } = new List<Event>();
    public int DispatchEventCount { get; private set; }
    public bool IsNew => DispatchEventCount == 0;
    public DateTime? LastDispatchEventDate { get; private set; }
    public string? LastDispatchedEventName { get; private set; }
    public void AddEnvet(Event @event)
    {
        Events.Add(@event);
        if (@event.MustDispatch)
        {
            DispatchEventCount++;
            LastDispatchEventDate = DateTime.Now;
            LastDispatchedEventName = @event.EventName;
        }
    }
    public void AddEnvets(IEnumerable<Event> events)
    {
        foreach (var @event in events)
        {
            Events.Add(@event);
        }
    }
}
public abstract class AggregateRoot : AggregateRoot<Guid>
{
}
