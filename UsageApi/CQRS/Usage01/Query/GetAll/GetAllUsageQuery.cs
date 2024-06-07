using Arch.CQRS.Query;
using MediatR;

namespace UsageApi.CQRS.Query;

public class GetAllUsageQuery : IQuery<List<GetAllUsageResult>>
{
}
