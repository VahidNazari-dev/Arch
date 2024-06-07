using Arch.CQRS.Command;
using Arch.EFCore;
using UsageApi.Domain;

namespace UsageApi.CQRS.Commnand;

public class CreateUsageCommandHandler : ICommandHandler<CreateUsageCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateUsageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CreateUsageCommand request, CancellationToken cancellationToken)
    {
        var usage = new Usage01(request.Title);
        _unitOfWork.Repo<Usage01>().Add(usage);
       await _unitOfWork.Save(usage);
    }
}
