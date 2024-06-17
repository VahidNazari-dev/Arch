using Arch.CQRS.Command;
using Arch.Domian;
using Arch.EFCore;
using UsageApi.Domain;

namespace UsageApi.CQRS.Commnand;

public class CreateUsageCommandHandler : ICommandHandler<CreateUsageCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;

    public CreateUsageCommandHandler(IUnitOfWork unitOfWork, IEventBus eventBus)
    {
        _unitOfWork = unitOfWork;
        _eventBus=eventBus;
    }

    public async Task Handle(CreateUsageCommand request, CancellationToken cancellationToken)
    {
        var usage = new Usage01(request.Title);
        _unitOfWork.Repo<Usage01>().Add(usage);
       await _unitOfWork.Save(usage);
        var @event = new UsageTestEvent()
        {
            Id = usage.Id
        };
      await  _eventBus.Execute(@event, new Dictionary<string, string>(), cancellationToken);
    }
}
