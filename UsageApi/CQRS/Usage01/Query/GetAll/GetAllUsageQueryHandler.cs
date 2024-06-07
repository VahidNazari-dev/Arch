using Arch.CQRS.Query;
using Arch.EFCore;
using Microsoft.EntityFrameworkCore;
using UsageApi.Domain;

namespace UsageApi.CQRS.Query;

public class GetAllUsageQueryHandler : IQueryHandler<GetAllUsageQuery, List<GetAllUsageResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUsageQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<GetAllUsageResult>> Handle(GetAllUsageQuery request, CancellationToken cancellationToken)
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
