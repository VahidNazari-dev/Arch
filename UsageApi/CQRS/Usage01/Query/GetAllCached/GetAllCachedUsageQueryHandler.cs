using Arch.CQRS.Query;
using UsageApi.Domain;
using Microsoft.EntityFrameworkCore;
using Arch.EFCore;

namespace UsageApi.CQRS.Query;

public class GetAllCachedUsageQueryHandler : IQueryHandler<GetAllCachedUsageQuery, List<GetAllUsageResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllCachedUsageQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<GetAllUsageResult>> Handle(GetAllCachedUsageQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Repo<Usage01>().
           Select(x => new GetAllUsageResult
           {
               CreateDate = x.CreateDate,
               Id = x.Id,
               Title = x.Title
           })
           .ToListAsync();
        return result;
    }
}
