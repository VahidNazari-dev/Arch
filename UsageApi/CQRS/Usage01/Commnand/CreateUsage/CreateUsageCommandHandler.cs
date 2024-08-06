using Arch.CQRS.Command;
using Arch.Domain;
using Arch.EFCore;
using UsageApi.Domain;

namespace UsageApi.CQRS.Commnand;

public class CreateUsageCommandHandler : ICommandHandler<CreateUsageCommand,int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateUsageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateUsageCommand request, CancellationToken cancellationToken)
    {
        var usage = new Usage01(request.Title, Usage01Type.Type01);
       
        var @event = new UsageTestEvent()
        {
            Id = usage.Id,
        };
//usage.AddEnvet(@event);
        _unitOfWork.Repo<Usage01>().Add(usage);
      return  await _unitOfWork.Save(usage,true,cancellationToken);
    }
}
