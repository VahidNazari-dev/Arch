using MediatR;

namespace UsageApi.CQRS.Commnand;

public class UpdateUsageCommand : IRequest
{
    public int Id { get; set; }
    public string Title { get; set; }
}
