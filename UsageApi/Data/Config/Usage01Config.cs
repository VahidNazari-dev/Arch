using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsageApi.Domain;
using Arch.EFCore;

namespace UsageApi.Data.Config;

public class Usage01Config : IEntityTypeConfiguration<Domain.Usage01>
{
    public void Configure(EntityTypeBuilder<Usage01> builder)
    {
        builder.Ignore(x=>x.Events);
        builder.Property(x => x.LastDispatchedEventName).HasMaxLength(150);

        builder.Property(x => x.CreateDate).HasDefaultValueSql("GETDATE()");
        builder.Property(x => x.Type).HasConversion(EnumerationEx.IntConverter<Usage01Type>());
    }
}
