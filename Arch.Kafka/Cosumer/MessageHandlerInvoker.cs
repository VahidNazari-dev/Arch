

using Arch.Domian;
using Arch.Kafka.Configs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Reflection;

namespace Arch.Kafka.Cosumer;

public class MessageHandlerInvoker
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageHandlerInvoker> _logger;
    private readonly KafkaSubscriberConfig _options;
    private readonly Assembly[] _scanningAssemblies;
    private static JsonSerializerSettings _settings = new JsonSerializerSettings
    {
        Error = (e, args) => args.ErrorContext.Handled = true,
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
    };
    public MessageHandlerInvoker(
       IServiceProvider serviceProvider,
        ILogger<MessageHandlerInvoker> logger,
        KafkaSubscriberConfig options)
    {
        this._serviceProvider = serviceProvider;
        this._logger = logger;
        this._options = options;
        this._scanningAssemblies = options.EventAssemblies;
    }
    public async Task Invoke(string eventName, string eventData, Dictionary<string, string> headers)
    {
        var type = GetType(eventName);

        if (type == null)
        {
            _logger.LogWarning($"Could not find handler for {eventName}");
            return;
        }

        var @event = JsonConvert.DeserializeObject(eventData, type, _settings);

        if (@event == null)
        {
            _logger.LogError($"Could not deserialize to {eventName} payload: {eventData}");

            return;
        }

        var handlerType = typeof(IMessageHandler<>).MakeGenericType(type);

        var typesOfMessageHandler = type.Assembly.GetTypes()
    .Where(p => handlerType.IsAssignableFrom(p)).ToList();

        foreach (var handleritem in typesOfMessageHandler)
        {
            dynamic handler = ActivatorUtilities.CreateInstance(_serviceProvider, handleritem);
            await handler.Handle((dynamic)@event, headers);
        }

    }

    private Type GetType(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type != null)
        {
            return type;
        }

        foreach (var a in _scanningAssemblies)
        {
            type = a.GetType(typeName);

            if (type != null)
            {
                return type;
            }
        }

        return null;
    }
}
