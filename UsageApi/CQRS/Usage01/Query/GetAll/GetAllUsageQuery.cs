using MediatR;

namespace UsageApi.CQRS.Query;

public class GetAllUsageQuery : IRequest<List<GetAllUsageResult>>
{
}
