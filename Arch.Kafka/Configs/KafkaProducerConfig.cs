

namespace Arch.Kafka.Configs;

public class KafkaProducerConfig
{
    public string BootstrapServers { get; set; }
    public int? MaxMessageBytes { get; set; }
}
