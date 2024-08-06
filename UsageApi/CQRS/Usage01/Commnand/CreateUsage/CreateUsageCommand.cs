using Arch.CQRS.Command;

namespace UsageApi.CQRS.Commnand;

public class CreateUsageCommand:ICommand<int>
{
    public string Title { get; set; }
}
