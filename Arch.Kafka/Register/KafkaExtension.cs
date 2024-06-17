

using Arch.Domian;
using Arch.Kafka.Attributes;
using Arch.Kafka.Configs;
using Arch.Kafka.Cosumer;
using Arch.Kafka.Producer;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arch.Kafka.Register;

public static class KafkaExtension
{
    public static void AddKafka(this IServiceCollection services,
Action<KafkaProducerConfig> producerConfig,
Action<KafkaSubscriberConfig> subscriberConfig)
    {
        if (producerConfig != null)
        {
            services.AddKafkaProducer(producerConfig);
        }

        if (subscriberConfig != null)
        {
            services.AddKafkaConsumer(subscriberConfig);
        }
    }

    public static void AddKafkaProducer(this IServiceCollection services,
        Action<KafkaProducerConfig> configurator)
    {
        // Producer
        services.AddScoped<IEventBus, KafkaEventHandler>();
        services.AddSingleton(s =>
        {
            var config = new KafkaProducerConfig();
            configurator(config);
            return config;
        });
    }

    public static void AddKafkaConsumer(this IServiceCollection services, Action<KafkaSubscriberConfig> configurator)
    {
        services.AddSingleton(s =>
        {
            var config = new KafkaSubscriberConfig();
            // Consumer
            configurator(config);
            var events = config.EventAssemblies
           .SelectMany(a => a.GetTypes())
               .Where(t => t.IsSubclassOf(typeof(Event)));
            foreach (var eventType in events)
            {
                if (!eventType.GetConstructors().Any(c => c.GetParameters().Count() == 0))
                {
                    continue;
                }

                var handlerType = typeof(IMessageHandler<>).MakeGenericType(eventType);
                var hasHandler = config
                    .EventAssemblies
                    .Any(ass => ass.GetTypes().Any(ty => handlerType.IsAssignableFrom(ty)));

                if (hasHandler)
                {
                    var topicInfo = eventType.GetCustomAttribute<TopicAttribute>();
                    var topicName = topicInfo?.Name ?? eventType.FullName;
                    if (config.Topics.Contains(topicName))
                        continue;
                    config.Topics.Add(topicName);
                }
            }
            return config;
        });
        services.AddSingleton<MessageHandlerInvoker>();
        services.AddHostedService<KafkaListenerService>();
    }

}
