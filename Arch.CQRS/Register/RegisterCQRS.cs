using Microsoft.Extensions.DependencyInjection;
using Arch.CQRS.Query.Behavior;
using Arch.CQRS.Command.Behavior;
using Arch.CQRS.Command;
using System.Reflection;

namespace Arch.CQRS.Register;

public static class RegisterCQRS
{
    public static void AddCQRS(this IServiceCollection services,bool isCaching,bool isValidating)
    {
        var assembly = Assembly.GetEntryAssembly();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            if (isCaching) 
            cfg.AddOpenBehavior(typeof(QueryCachedBehave<,>));//EnableQueryCaching
            
            //EnableValidate
            if (isValidating)
            {
                cfg.AddOpenBehavior(typeof(CommandValidateBehaveResult<,>));
            }
            
        });
    }
}
