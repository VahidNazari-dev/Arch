

using Arch.Kafka.Configs;
using Confluent.Kafka.Admin;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Arch.Domain;
using System.Text;

namespace Arch.Kafka.Cosumer;

public class KafkaListenerService : BackgroundService
{
    private bool _consuming = true;
    private readonly ILogger<KafkaListenerService> _logger;
    private readonly KafkaSubscriberConfig _config;
    private readonly MessageHandlerInvoker _handlerFactory;
    private readonly IServiceProvider _serviceProvider;
    private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
    {
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
    };
    public KafkaListenerService(
    ILogger<KafkaListenerService> logger,
    MessageHandlerInvoker handlerFactory,
    KafkaSubscriberConfig subscriberConfig,
    IServiceProvider serviceProvider)
    {
        if (!subscriberConfig.IsValid)
        {
            throw new ArgumentException(nameof(subscriberConfig));
        }

        this._logger = logger;
        this._config = subscriberConfig;
        this._serviceProvider = serviceProvider;
        this._handlerFactory = handlerFactory;

        if (subscriberConfig.Topics == null || !subscriberConfig.Topics.Any())
        {
            _logger.LogWarning("No topics found to subscribe");
        }
        else
        {
            _logger.LogInformation($"subscribing to {JsonConvert.SerializeObject(subscriberConfig.Topics)}");
        }
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeTopics();

        _ = Task.Run(async () =>
        {
            using var consumer = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build();

            try
            {
                consumer.Subscribe(this._config.Topics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }

            while (_consuming || !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var message = consumer.Consume(TimeSpan.FromMilliseconds(150));

                    if (message is null)
                        continue;

                    var eventData = JsonConvert.DeserializeObject<Event>(message.Message.Value, _serializerSettings);

                    var headers = message.Message.Headers.ToDictionary(
                            k => k.Key,
                            v => Encoding.UTF8.GetString(v.GetValueBytes()))
                        .ToDictionary(d => d.Key, v => v.Value);

                    if (_config.PreMessageHandlingHandler != null)
                        await _config.PreMessageHandlingHandler(_serviceProvider, eventData, headers);

                    await _handlerFactory.Invoke(
                        eventData.EventName,
                        message.Message.Value,
                        headers);

                    consumer.Commit(message);

                    _logger.LogInformation($"Consumed Message {message.Message.Value} from topic: {message.Topic}");
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError(ex.ToString());
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex.ToString());
                    _consuming = false;
                }
            }

            consumer.Close();
        });

    }
    
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _consuming = false;

        return Task.CompletedTask;
    }
    
    private async Task InitializeTopics()
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _config.BootstrappServers }).Build();
        try
        {
            foreach (var topic in _config.Topics)
            {
                await adminClient.CreateTopicsAsync(new TopicSpecification[]
                {
                        new TopicSpecification
                        {
                            Name = topic,
                            ReplicationFactor = 1,
                            NumPartitions = 1
                        }
                });
            }
        }
        catch (CreateTopicsException e)
        {
            _logger.LogError($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
        }
    }

    protected virtual ConsumerConfig ConsumerConfig => new ConsumerConfig
    {
        GroupId = this._config.GroupId,
        BootstrapServers = this._config.BootstrappServers,
        AutoOffsetReset = this._config.OffsetResetType,
        MaxPollIntervalMs = this._config.MaxPollIntervalMs,
        SessionTimeoutMs = this._config.SessionTimeoutMs,
        EnableAutoCommit = this._config.EnableAutoCommit,
        EnableAutoOffsetStore = false,
        AllowAutoCreateTopics = true
    };
}
