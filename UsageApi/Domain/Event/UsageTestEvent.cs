using Arch.Domain;

namespace UsageApi.Domain;

public class UsageTestEvent:DomainEvent
{
    public int Id { get; set; }
    public override bool MustDispatch => true;
}
