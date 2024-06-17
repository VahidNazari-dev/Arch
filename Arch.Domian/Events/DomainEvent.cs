

namespace Arch.Domian;

public class DomainEvent: Event
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public int Version { get; set; }

    public object EventMetadata { get; set; }
    public bool MustPush { get; set; }
}
