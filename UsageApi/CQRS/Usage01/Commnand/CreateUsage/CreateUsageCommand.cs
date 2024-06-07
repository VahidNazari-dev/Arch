using Arch.CQRS.Command;

namespace UsageApi.CQRS.Commnand;

public class CreateUsageCommand:ICommand
{
    public string Title { get; set; }
}
