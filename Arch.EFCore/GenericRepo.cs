using Arch.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq.Expressions;

namespace Arch.EFCore;
public class GenericRepo<T> : IGenericRepo<T> where T : Entity
{
    private readonly DbSet<T> _set;
    private readonly IQueryable<T> _querySet;
    public GenericRepo(DbContext context)
    {
        _set = context.Set<T>();
        _querySet = _set.AsQueryable();
    }
    public Type ElementType => _querySet.ElementType;

    public Expression Expression => _querySet.Expression;

    public IQueryProvider Provider => _querySet.Provider;

    public void Add(T item) => _set.Add(item);
    public void AddRange(T[] items) => _set.AddRange(items);
    public IEnumerator<T> GetEnumerator() => _querySet.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
