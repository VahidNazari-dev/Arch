using MediatR;

namespace UsageApi.CQRS.Commnand;

public class CreateUsageCommand:IRequest
{
    public string Title { get; set; }
}
