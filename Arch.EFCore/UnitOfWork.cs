

using Arch.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Arch.EFCore;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IEventBus _eventBus;
    public UnitOfWork(
DbContext context,
ILogger<UnitOfWork> logger)
    {
        _logger = logger;
        Context = context;
    }
    public UnitOfWork(
        IEventBus eventBus,
    DbContext context,
    ILogger<UnitOfWork> logger)
    {
        _logger = logger;
        Context = context;
        _eventBus = eventBus;
    }
    public DbContext Context { get; }
    public async Task<int> Delete(AggregateRoot root)
    {
        Context.Remove(root);

        var affectedRows = await Context.SaveChangesAsync();

        return affectedRows;
    }

    public async Task<int> Delete<T>(AggregateRoot<T> root)
    {
        Context.Remove(root);

        var affectedRows = await Context.SaveChangesAsync();

        return affectedRows;
    }

    public async Task<int> Save<T>(AggregateRoot<T> root, bool isTransactionalDispatch = false, CancellationToken cancellationToken = default)
    {
        int rowCount = 0;
        if (!isTransactionalDispatch)
        {
            try
            {
                rowCount = await Context.SaveChangesAsync(cancellationToken);
                await DispatchEvents(root, cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        else
        {
            var executionStrategy = Context.Database.CreateExecutionStrategy();
          await  executionStrategy.Execute(
    async () =>
    {
        using (var transaction = Context.Database.BeginTransaction())
        {
            try
            {
                rowCount = await Context.SaveChangesAsync(cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    await transaction.RollbackAsync();
                }
                await DispatchEvents(root, cancellationToken);
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    });

        }

        return rowCount;
    }
    private async Task DispatchEvents<T>(AggregateRoot<T> root, CancellationToken cancellationToken)
    {
        if (root.Events != null && root.Events.Any(x => x.MustDispatch))
        {
            foreach (var @event in root.Events.Where(x => x.MustDispatch).ToList())
            {
                await _eventBus.Execute<Event>(@event, new Dictionary<string, string>(), cancellationToken);
            }
        }
    }
    internal IGenericRepo<T> GenericRepo<T>() where T : Entity => new GenericRepo<T>(Context);

}
