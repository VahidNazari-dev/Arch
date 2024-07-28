

namespace Arch.Domain;

public abstract class Entity
{
    public bool Deleted { get; protected set; }
    public DateTime CreateDate { get; protected set; }
}
public abstract class Entity<T>:Entity
{
    public T Id { get; protected set; }
    public override bool Equals(object obj)
    {
        Entity<T> entity = obj as Entity<T>;
        if ((object)entity == null)
        {
            return false;
        }

        if ((object)this == entity)
        {
            return true;
        }

        if (GetType() != entity.GetType())
        {
            return false;
        }

        Type type = Id.GetType();
        if (type == typeof(Guid))
        {
            return Guid.Parse(Id.ToString()) == Guid.Parse(entity.Id.ToString());
        }

        if (type == typeof(long) || Id.GetType() == typeof(int))
        {
            return long.Parse(Id.ToString()) == long.Parse(entity.Id.ToString());
        }

        if (type == typeof(string))
        {
            return Id.ToString() == entity.Id.ToString();
        }

        throw new NotSupportedException("only long and guid id is supported");
    }

    public static bool operator ==(Entity<T> first, Entity<T> second)
    {
        if ((object)first == null && (object)second == null)
        {
            return true;
        }

        if ((object)first == null || (object)second == null)
        {
            return false;
        }

        return first.Equals(second);
    }

    public static bool operator !=(Entity<T> first, Entity<T> second)
    {
        return !(first == second);
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }
}