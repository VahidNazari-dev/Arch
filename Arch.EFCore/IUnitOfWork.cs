

using Arch.Domain;

namespace Arch.EFCore;

public interface IUnitOfWork
{
    Task<int> Save<T>(AggregateRoot<T> root, bool isTransactionalDispatch = false, CancellationToken cancellationToken = default);
    Task<int> Delete(AggregateRoot root);
    Task<int> Delete<T>(AggregateRoot<T> root);
}
