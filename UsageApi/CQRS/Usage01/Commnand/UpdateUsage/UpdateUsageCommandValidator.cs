using Arch.BaseApi;
using Arch.CQRS.Command;
using Microsoft.EntityFrameworkCore;
using UsageApi.Data;

namespace UsageApi.CQRS.Commnand;

public class UpdateUsageCommandValidator : ICommandValidator<UpdateUsageCommand, int>
{
    private readonly UsageDbContext dbContext;

    public UpdateUsageCommandValidator(UsageDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task ValidateAsync(UpdateUsageCommand command)
    {
        var usage =await dbContext.Usage01.FirstOrDefaultAsync(x => x.Id == command.Id);
        if (usage == null)
        {
            throw new ValidationException("وجود ندارد");
        }
    }
}
