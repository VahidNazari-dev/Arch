using Arch.BaseApi;
using Arch.EFCore;
using MediatR;
using UsageApi.Domain;

namespace UsageApi.CQRS.Commnand;

public class UpdateUsageCommandHandler : IRequestHandler<UpdateUsageCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUsageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateUsageCommand request, CancellationToken cancellationToken)
    {
        var usage = _unitOfWork.Repo<Usage01>().FirstOrDefault(x=>x.Id==request.Id);
        if (usage==null)
        {
            throw new ValidationException("وجود ندارد");
        }
        usage.UpdateTitle(request.Title);
        await _unitOfWork.Save(usage);
    }
}
