

using Arch.Domian;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Arch.EFCore;

public sealed class UnitOfWork:IUnitOfWork
{
    private readonly ILogger<UnitOfWork> _logger;
    public UnitOfWork(
    DbContext context,
    ILogger<UnitOfWork> logger)
    {
        _logger = logger;
        Context = context;
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

    public async Task<int> Save<T>(AggregateRoot<T> root)         
    {
        int rowCount;

        try
        {
            rowCount = await Context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
        return rowCount;
    }

    internal IGenericRepo<T> GenericRepo<T>() where T : Entity => new GenericRepo<T>(Context);

}
