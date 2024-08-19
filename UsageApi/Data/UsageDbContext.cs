using EntityFrameworkCore.SqlServer.JsonExtention;
using Microsoft.EntityFrameworkCore;
using UsageApi.Domain;

namespace UsageApi.Data;

public class UsageDbContext:DbContext
{
    public DbSet<Usage01> Usage01 { get; set; }
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
