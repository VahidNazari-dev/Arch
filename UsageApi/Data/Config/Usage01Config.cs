using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsageApi.Domain;

namespace UsageApi.Data.Config;

public class Usage01Config : IEntityTypeConfiguration<Domain.Usage01>
{
    public void Configure(EntityTypeBuilder<Usage01> builder)
    {
        builder.Property(x => x.CreateDate).HasDefaultValueSql("GETDATE()");
    }
}
