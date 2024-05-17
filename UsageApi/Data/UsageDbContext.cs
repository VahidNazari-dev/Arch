using EntityFrameworkCore.SqlServer.JsonExtention;
using Microsoft.EntityFrameworkCore;

namespace UsageApi.Data;

public class UsageDbContext:DbContext
{
    public UsageDbContext(DbContextOptions<UsageDbContext> options) : base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseJsonFunctions();
        optionsBuilder.UseSqlServer(x => x.UseNetTopologySuite());
        optionsBuilder.EnableSensitiveDataLogging(true);

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
    }
}
