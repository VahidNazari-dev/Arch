

using Arch.Domain;

namespace Arch.EFCore;

public static class UnitOfWorkEx
{
    public static IGenericRepo<T> Repo<T>(this IUnitOfWork uow) where T : Entity
    {
        if (!(uow is UnitOfWork efUow))
        {
            throw new InvalidOperationException("uow must be EfUnitOfWork");
        }

        return efUow.GenericRepo<T>();
    }
}
