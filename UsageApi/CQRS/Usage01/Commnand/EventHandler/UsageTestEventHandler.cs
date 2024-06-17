using Arch.Kafka.Cosumer;

namespace UsageApi.Domain;

public class UsageTestEventHandler : IMessageHandler<UsageTestEvent>
{
    public Task Handle(UsageTestEvent @event, Dictionary<string, string> headers)
    {
        return Task.CompletedTask;
    }
}
