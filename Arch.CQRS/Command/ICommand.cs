

using MediatR;

namespace Arch.CQRS.Command;

public interface ICommand : IRequest
{
}
public interface ICommand<T> : IRequest<T>
{
}