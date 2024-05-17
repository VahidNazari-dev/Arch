

namespace Arch.Domian;

public class Entity
{
    public bool Deleted { get; protected set; }
    public DateTime CreateDate { get; protected set; }
}
public class Entity<T>:Entity
{
    public T Id { get; protected set; }
}