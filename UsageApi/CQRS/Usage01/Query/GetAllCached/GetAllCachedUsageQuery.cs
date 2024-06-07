using Arch.CQRS.Query;

namespace UsageApi.CQRS.Query;

public class GetAllCachedUsageQuery: QueryCached<List<GetAllUsageResult>>
{
}
