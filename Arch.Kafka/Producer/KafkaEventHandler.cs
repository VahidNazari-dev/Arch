

using Arch.Domian;
using Arch.Kafka.Attributes;
using Arch.Kafka.Configs;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace Arch.Kafka.Producer;

public class KafkaEventHandler : IEventBus
{
    private readonly ILogger<KafkaEventHandler> _logger;
    private readonly IProducer<Null, string> _producer;
    private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };
    public KafkaEventHandler(KafkaProducerConfig config) : this(config, null)
    {
    }
    public KafkaEventHandler(KafkaProducerConfig config, ILogger<KafkaEventHandler> logger)
    {
        var producerConfig = config;

        _logger = logger;
        _producer = new ProducerBuilder<Null, string>(new ProducerConfig
        {
            BootstrapServers = producerConfig.BootstrapServers,
            MessageMaxBytes = producerConfig.MaxMessageBytes
        }).Build();
    }
    public Task Execute<TEvent>(TEvent @event, Dictionary<string, string> headers, CancellationToken cancellationToken = default) where TEvent : Event
    {
        var eventData = JsonConvert.SerializeObject(@event, _jsonSerializerSettings);
        var message = new Message<Null, string>
        {
            Value = eventData
        };

        AddHeaders(headers, message);


        var topicName = GetTopicName(@event);
        _logger?.LogInformation($"Pushing to ({topicName}): {message.Value}");

        return _producer.ProduceAsync(topicName, message, cancellationToken);
    }
    private static void AddHeaders(Dictionary<string, string> headers, Message<Null, string> message)
    {
        if (headers != null)
        {
            var headerValues = new Headers();

            foreach (var item in headers)
            {
                headerValues.Add(item.Key, Encoding.UTF8.GetBytes(item.Value));
            }

            message.Headers = headerValues;
        }
    }

    private string GetTopicName<TEvent>(TEvent @event) where TEvent : Event
    {
        var topicInfo = @event.GetType()
            .GetCustomAttribute<TopicAttribute>();

        if (topicInfo != null)
            return topicInfo.Name;

        return @event.EventName;
    }
}
