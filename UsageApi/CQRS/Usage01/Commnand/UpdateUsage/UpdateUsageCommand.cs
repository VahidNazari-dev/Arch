
using Arch.CQRS.Command;

namespace UsageApi.CQRS.Commnand;

public class UpdateUsageCommand : ICommand
{
    public int Id { get; set; }
    public string Title { get; set; }
}
