

using MediatR;

namespace Arch.CQRS.Query;

public interface IQueryHandler<T1,T2>: IRequestHandler<T1, T2> where T1: IQuery<T2>
{
}
