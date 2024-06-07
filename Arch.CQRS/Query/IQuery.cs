

using MediatR;

namespace Arch.CQRS.Query;

public interface IQuery<T>:IRequest<T>
{
}
