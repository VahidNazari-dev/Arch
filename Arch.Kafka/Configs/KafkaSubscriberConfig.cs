using Arch.Domain;
using Confluent.Kafka;
using System.Reflection;

namespace Arch.Kafka.Configs;
public class KafkaSubscriberConfig 
{
    public KafkaSubscriberConfig()
    {

    }
    public List<string> Topics { get; internal set; } = new List<string>();
    public string BootstrappServers { get; set; }
    public string GroupId { get; set; }
    public AutoOffsetReset OffsetResetType { get; set; } = AutoOffsetReset.Earliest;
    public bool EnableAutoCommit { get; set; } = false;
    public int? SessionTimeoutMs { get; set; }
    public int? MaxPollIntervalMs { get; set; }
    public Assembly[] EventAssemblies { get; set; }
    public Func<IServiceProvider, Event, Dictionary<string, string>, ValueTask> PreMessageHandlingHandler { get; set; } = null;

    public bool IsValid
    {
        get
        {
            return
                this.EventAssemblies.Count() > 0
                &&
                !string.IsNullOrEmpty(this.BootstrappServers)
                &&
                !string.IsNullOrEmpty(this.GroupId);
        }
    }
}
