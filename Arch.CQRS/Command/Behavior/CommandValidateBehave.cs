

using Arch.CQRS.Query;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Reflection.PortableExecutable;

namespace Arch.CQRS.Command.Behavior;

public class CommandValidateBehaveResult<TCommand, TResult> : IPipelineBehavior<TCommand, TResult> where TCommand : ICommand<TResult>
{
    private readonly IServiceProvider _serviceProvider;

    public CommandValidateBehaveResult(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> Handle(TCommand command, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var handlerType = typeof(ICommandValidator<TCommand, TResult>);
        if (handlerType!=null)
        {
            var assembly = Assembly.GetEntryAssembly();
            var typesOfCommandValidator = assembly.GetTypes()
        .Where(p => handlerType.IsAssignableFrom(p)).FirstOrDefault();
            if (typesOfCommandValidator != null)
            {
                dynamic handler = ActivatorUtilities.CreateInstance(_serviceProvider, typesOfCommandValidator);
                await handler.ValidateAsync(command);
            }
        }
        return await next();
    }
}
