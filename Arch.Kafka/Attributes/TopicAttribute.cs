

namespace Arch.Kafka.Attributes;

public class TopicAttribute : Attribute
{
    public string Name { get; set; }
    public int PartitionSize { get; set; } = 1;
}
