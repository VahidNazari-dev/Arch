using Arch.Domian;

namespace UsageApi.Domain;

public class UsageTestEvent:DomainEvent
{
    public int Id { get; set; }
    public override bool MustDispatch => true;
}
