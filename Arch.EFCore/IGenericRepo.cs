

namespace Arch.EFCore;

public interface IGenericRepo<T> : IQueryable<T>
{
    void Add(T item);
    void AddRange(T[] items);
}
