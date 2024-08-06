using Arch.BaseApi;
using Arch.CQRS.Command;
using Arch.EFCore;
using UsageApi.Domain;

namespace UsageApi.CQRS.Commnand;

public class UpdateUsageCommandHandler : ICommandHandler<UpdateUsageCommand,int>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUsageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(UpdateUsageCommand request, CancellationToken cancellationToken)
    {
        var usage = _unitOfWork.Repo<Usage01>().First(x=>x.Id==request.Id);
       
        usage.UpdateTitle(request.Title);
       return await _unitOfWork.Save(usage);
    }
}
