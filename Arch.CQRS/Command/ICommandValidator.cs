

namespace Arch.CQRS.Command;

public interface ICommandValidator<T,R> where T : ICommand<R>
{
    public Task ValidateAsync(T command);
}
