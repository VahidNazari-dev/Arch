

using MediatR;

namespace Arch.CQRS.Command;

public interface ICommandHandler<T1,T2>: IRequestHandler<T1, T2> where T1: ICommand<T2>
{
}
public interface ICommandHandler<T1> : IRequestHandler<T1> where T1 : ICommand
{
}
